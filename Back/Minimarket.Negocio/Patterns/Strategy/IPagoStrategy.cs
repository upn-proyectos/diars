namespace Minimarket.Negocio.Patterns.Strategy;

/// <summary>
/// Resultado estandarizado del procesamiento de un pago mediante el Patrón Strategy.
/// </summary>
public class ResultadoPagoStrategy
{
    public bool EsValido { get; set; }
    public string MetodoPagoNormalizado { get; set; } = string.Empty;
    public decimal MontoTotal { get; set; }
    public decimal MontoRecibido { get; set; }
    public decimal Vuelto { get; set; }
    public string ReferenciaOperacion { get; set; } = string.Empty;
    public string MensajeResumen { get; set; } = string.Empty;
    public string MensajeError { get; set; } = string.Empty;
}

/// <summary>
/// Patrón de Comportamiento GoF: Strategy.
/// Define el contrato común para los algoritmos intercambiables de cobro en el Punto de Venta (POS).
/// </summary>
public interface IPagoStrategy
{
    string NombreMetodo { get; }
    ResultadoPagoStrategy ProcesarPago(decimal totalVenta, decimal montoRecibido, string? codigoReferencia = null);
}
