namespace Minimarket.Datos.Entities;

public class DetalleOrden
{
    public int Id { get; set; }
    public int OrdenCompraId { get; set; }
    public int ProductoId { get; set; }
    public string NombreProducto { get; set; } = string.Empty;
    public decimal Cantidad { get; set; }
    public decimal CantidadRecibida { get; set; }
    public decimal CostoUnitario { get; set; }
    public decimal Subtotal { get; set; }

    public OrdenCompra? OrdenCompra { get; set; }
    public Producto? Producto { get; set; }
}
