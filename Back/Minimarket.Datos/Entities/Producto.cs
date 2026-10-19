namespace Minimarket.Datos.Entities;

public class Producto
{
    public int Id { get; set; }
    public string? CodigoBarras { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int CategoriaId { get; set; }
    public int? ProveedorId { get; set; }
    public decimal PrecioVenta { get; set; }
    public decimal CostoUnitario { get; set; }
    public decimal Stock { get; set; }
    public decimal StockMinimo { get; set; } = 5;
    public string UnidadMedida { get; set; } = "UNIDAD"; // "UNIDAD" o "KG"
    public DateTime? FechaVencimiento { get; set; }
    public bool Activo { get; set; } = true;

    public Categoria? Categoria { get; set; }
    public Proveedor? Proveedor { get; set; }
    public ICollection<DetalleVenta> DetallesVenta { get; set; } = new List<DetalleVenta>();
    public ICollection<Kardex> MovimientosKardex { get; set; } = new List<Kardex>();

    public bool EstaBajoStock => Stock <= StockMinimo;
}
