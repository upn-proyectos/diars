namespace Minimarket.Datos.Entities;

public class DetalleVenta
{
    public int Id { get; set; }
    public int VentaId { get; set; }
    public int? ProductoId { get; set; }
    public string NombreProducto { get; set; } = string.Empty;
    public decimal Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal CostoUnitario { get; set; }
    public decimal Subtotal { get; set; }

    public Venta? Venta { get; set; }
    public Producto? Producto { get; set; }
}
