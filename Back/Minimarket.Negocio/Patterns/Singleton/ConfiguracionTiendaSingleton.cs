using Minimarket.Datos.Entities;

namespace Minimarket.Negocio.Patterns.Singleton;

/// <summary>
/// Patrón Creacional GoF: Singleton (Thread-Safe).
/// Centraliza en una única instancia en memoria los parámetros globales de Minimarket Chininin SAC:
/// Tasa de IGV (18%), Serie de Boleta (B001), Correlativo actual y datos fiscales del emisor.
/// </summary>
public sealed class ConfiguracionTiendaSingleton
{
    private static readonly Lazy<ConfiguracionTiendaSingleton> _instancia =
        new(() => new ConfiguracionTiendaSingleton(), isThreadSafe: true);

    private readonly object _lock = new();

    public static ConfiguracionTiendaSingleton Instancia => _instancia.Value;

    public string NombreComercio { get; private set; } = "Minimarket Chininin SAC";
    public string Ruc { get; private set; } = "20601234567";
    public string Direccion { get; private set; } = "Av. América Norte 1234 - Trujillo";
    public string SerieBoleta { get; private set; } = "B001";
    public int CorrelativoActual { get; private set; } = 1;
    public decimal TasaIgv { get; private set; } = 0.18m;
    public bool InicializadoDesdeBd { get; private set; }

    private ConfiguracionTiendaSingleton()
    {
    }

    /// <summary>
    /// Sincroniza el estado en memoria del Singleton con el registro almacenado en SQL Server.
    /// </summary>
    public void SincronizarDesdeEntidad(ConfiguracionTienda? entidad)
    {
        if (entidad == null)
            return;

        lock (_lock)
        {
            NombreComercio = string.IsNullOrWhiteSpace(entidad.NombreComercio)
                ? "Minimarket Chininin SAC"
                : entidad.NombreComercio.Trim();
            Ruc = string.IsNullOrWhiteSpace(entidad.Ruc)
                ? "20601234567"
                : entidad.Ruc.Trim();
            Direccion = string.IsNullOrWhiteSpace(entidad.Direccion)
                ? "Av. América Norte 1234 - Trujillo"
                : entidad.Direccion.Trim();
            SerieBoleta = string.IsNullOrWhiteSpace(entidad.SerieBoleta)
                ? "B001"
                : entidad.SerieBoleta.Trim().ToUpperInvariant();
            CorrelativoActual = Math.Max(1, entidad.CorrelativoBoleta);
            TasaIgv = entidad.TasaIgv is > 0m and < 1m ? entidad.TasaIgv : 0.18m;
            InicializadoDesdeBd = true;
        }
    }

    /// <summary>
    /// Calcula el Subtotal (base imponible) y el IGV a partir del Total (los precios de lista incluyen IGV).
    /// </summary>
    public (decimal Subtotal, decimal Igv, decimal Total) CalcularDesgloseTributario(decimal totalBruto)
    {
        lock (_lock)
        {
            var total = Math.Round(totalBruto, 2, MidpointRounding.AwayFromZero);
            var subtotal = Math.Round(total / (1m + TasaIgv), 2, MidpointRounding.AwayFromZero);
            var igv = Math.Round(total - subtotal, 2, MidpointRounding.AwayFromZero);
            return (subtotal, igv, total);
        }
    }

    /// <summary>
    /// Formatea un número de boleta siguiendo el estándar de la tienda (ej. B001-00001).
    /// </summary>
    public string FormatearNumeroBoleta(int correlativo)
    {
        lock (_lock)
        {
            return $"{SerieBoleta}-{Math.Max(1, correlativo):D5}";
        }
    }

    /// <summary>
    /// Obtiene una vista previa del próximo comprobante a emitir en el POS.
    /// </summary>
    public string ObtenerProximoComprobantePreview()
    {
        lock (_lock)
        {
            return $"{SerieBoleta}-{CorrelativoActual:D5}";
        }
    }

    /// <summary>
    /// Actualiza el correlativo en memoria después de confirmar una venta en SQL Server.
    /// </summary>
    public void ActualizarCorrelativo(int nuevoCorrelativo)
    {
        lock (_lock)
        {
            if (nuevoCorrelativo > CorrelativoActual)
            {
                CorrelativoActual = nuevoCorrelativo;
            }
        }
    }
}
