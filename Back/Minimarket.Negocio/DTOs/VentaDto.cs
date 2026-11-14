namespace Minimarket.Negocio.DTOs;

public class VentaLineaSolicitudDto
{
    public int ProductoId { get; set; }
    public decimal Cantidad { get; set; }
}

public class VentaRegistrarDto
{
    public int ClienteId { get; set; }
    public string MetodoPago { get; set; } = "Efectivo";
    public decimal MontoRecibido { get; set; }
    public string? CodigoReferenciaPago { get; set; }
    public List<VentaLineaSolicitudDto> Lineas { get; set; } = new();
}

public class DetalleVentaDto
{
    public int Id { get; set; }
    public int? ProductoId { get; set; }
    public string NombreProducto { get; set; } = string.Empty;
    public decimal Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal CostoUnitario { get; set; }
    public decimal Subtotal { get; set; }
    public decimal UtilidadLinea => Math.Round(Subtotal - (Cantidad * CostoUnitario), 2);
}

public class VentaDto
{
    public int Id { get; set; }
    public string NumeroBoleta { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public int ClienteId { get; set; }
    public string ClienteNombre { get; set; } = string.Empty;
    public string ClienteTipoDoc { get; set; } = "DNI";
    public string ClienteDocumento { get; set; } = string.Empty;
    public string? ClienteDireccion { get; set; }
    public int UsuarioId { get; set; }
    public string CajeroNombre { get; set; } = string.Empty;
    public string MetodoPago { get; set; } = "Efectivo";
    public decimal Subtotal { get; set; }
    public decimal Igv { get; set; }
    public decimal Total { get; set; }
    public decimal UtilidadEstimada => Detalles.Sum(d => d.UtilidadLinea);
    public List<DetalleVentaDto> Detalles { get; set; } = new();
}

public class ConfiguracionTiendaDto
{
    public string NombreComercio { get; set; } = "Minimarket Chininin SAC";
    public string Ruc { get; set; } = "20601234567";
    public string Direccion { get; set; } = "Av. América Norte 1234 - Trujillo";
    public string SerieBoleta { get; set; } = "B001";
    public int CorrelativoActual { get; set; } = 1;
    public string ProximoComprobante { get; set; } = "B001-00001";
    public decimal TasaIgv { get; set; } = 0.18m;
}
