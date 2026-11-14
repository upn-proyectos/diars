namespace Minimarket.Negocio.Patterns.Strategy;

/// <summary>
/// Estrategia Concreta 1: Pago en Efectivo (valida monto recibido >= total y calcula vuelto).
/// </summary>
public class PagoEfectivoStrategy : IPagoStrategy
{
    public string NombreMetodo => "Efectivo";

    public ResultadoPagoStrategy ProcesarPago(decimal totalVenta, decimal montoRecibido, string? codigoReferencia = null)
    {
        var total = Math.Round(totalVenta, 2, MidpointRounding.AwayFromZero);
        if (total <= 0)
        {
            return new ResultadoPagoStrategy
            {
                EsValido = false,
                MetodoPagoNormalizado = NombreMetodo,
                MensajeError = "El importe total de la venta debe ser mayor a S/ 0.00."
            };
        }

        // Si no se especificó monto recibido o es 0, sólo es inválido si se exige monto; si es >= total calcula vuelto
        var recibido = Math.Round(montoRecibido, 2, MidpointRounding.AwayFromZero);
        if (recibido < total)
        {
            return new ResultadoPagoStrategy
            {
                EsValido = false,
                MetodoPagoNormalizado = NombreMetodo,
                MontoTotal = total,
                MontoRecibido = recibido,
                MensajeError = $"El efectivo recibido (S/ {recibido:0.00}) es menor al total a pagar (S/ {total:0.00})."
            };
        }

        var vuelto = Math.Round(recibido - total, 2, MidpointRounding.AwayFromZero);
        return new ResultadoPagoStrategy
        {
            EsValido = true,
            MetodoPagoNormalizado = NombreMetodo,
            MontoTotal = total,
            MontoRecibido = recibido,
            Vuelto = vuelto,
            ReferenciaOperacion = "CAJA-EFECTIVO",
            MensajeResumen = vuelto > 0
                ? $"Cobro en Efectivo (Recibido: S/ {recibido:0.00} · Vuelto: S/ {vuelto:0.00})"
                : $"Cobro exacto en Efectivo (S/ {total:0.00})"
        };
    }
}

/// <summary>
/// Estrategia Concreta 2: Pago por Billetera Digital (Yape / Plin).
/// </summary>
public class PagoYapePlinStrategy : IPagoStrategy
{
    private readonly string _billetera;

    public PagoYapePlinStrategy(string billetera = "Yape")
    {
        _billetera = billetera.Equals("Plin", StringComparison.OrdinalIgnoreCase) ? "Plin" : "Yape";
    }

    public string NombreMetodo => _billetera;

    public ResultadoPagoStrategy ProcesarPago(decimal totalVenta, decimal montoRecibido, string? codigoReferencia = null)
    {
        var total = Math.Round(totalVenta, 2, MidpointRounding.AwayFromZero);
        if (total <= 0)
        {
            return new ResultadoPagoStrategy
            {
                EsValido = false,
                MetodoPagoNormalizado = NombreMetodo,
                MensajeError = "El importe total de la venta debe ser mayor a S/ 0.00."
            };
        }

        var refLimpia = string.IsNullOrWhiteSpace(codigoReferencia)
            ? $"{NombreMetodo.ToUpperInvariant()}-{DateTime.Now:HHmmss}"
            : codigoReferencia.Trim().ToUpperInvariant();

        return new ResultadoPagoStrategy
        {
            EsValido = true,
            MetodoPagoNormalizado = NombreMetodo,
            MontoTotal = total,
            MontoRecibido = total,
            Vuelto = 0m,
            ReferenciaOperacion = refLimpia,
            MensajeResumen = $"Cobro confirmado vía {NombreMetodo} por S/ {total:0.00} (Op: {refLimpia})"
        };
    }
}

/// <summary>
/// Estrategia Concreta 3: Pago con Tarjeta de Débito / Crédito en POS.
/// </summary>
public class PagoTarjetaStrategy : IPagoStrategy
{
    public string NombreMetodo => "Tarjeta";

    public ResultadoPagoStrategy ProcesarPago(decimal totalVenta, decimal montoRecibido, string? codigoReferencia = null)
    {
        var total = Math.Round(totalVenta, 2, MidpointRounding.AwayFromZero);
        if (total <= 0)
        {
            return new ResultadoPagoStrategy
            {
                EsValido = false,
                MetodoPagoNormalizado = NombreMetodo,
                MensajeError = "El importe total de la venta debe ser mayor a S/ 0.00."
            };
        }

        var refLimpia = string.IsNullOrWhiteSpace(codigoReferencia)
            ? $"POS-{DateTime.Now:HHmmss}"
            : codigoReferencia.Trim().ToUpperInvariant();

        return new ResultadoPagoStrategy
        {
            EsValido = true,
            MetodoPagoNormalizado = NombreMetodo,
            MontoTotal = total,
            MontoRecibido = total,
            Vuelto = 0m,
            ReferenciaOperacion = refLimpia,
            MensajeResumen = $"Cobro con Tarjeta aprobado por S/ {total:0.00} (Ref: {refLimpia})"
        };
    }
}

/// <summary>
/// Contexto del Patrón Strategy: selecciona y ejecuta en tiempo de ejecución la estrategia de pago adecuada.
/// Cumple con el Principio Abierto/Cerrado (OCP).
/// </summary>
public class PagoStrategyContext
{
    public IPagoStrategy ResolverEstrategia(string? metodoPago)
    {
        var normalizado = (metodoPago ?? "Efectivo").Trim();

        return normalizado.ToUpperInvariant() switch
        {
            "EFECTIVO" => new PagoEfectivoStrategy(),
            "YAPE" => new PagoYapePlinStrategy("Yape"),
            "PLIN" => new PagoYapePlinStrategy("Plin"),
            "YAPE/PLIN" => new PagoYapePlinStrategy("Yape"),
            "TARJETA" => new PagoTarjetaStrategy(),
            _ => throw new InvalidOperationException($"El método de pago '{metodoPago}' no está soportado por el sistema.")
        };
    }

    public ResultadoPagoStrategy EjecutarCobro(string? metodoPago, decimal totalVenta, decimal montoRecibido, string? codigoReferencia = null)
    {
        var estrategia = ResolverEstrategia(metodoPago);
        return estrategia.ProcesarPago(totalVenta, montoRecibido, codigoReferencia);
    }
}
