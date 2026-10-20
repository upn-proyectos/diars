namespace Minimarket.Front.Models;

public class DashboardViewModel
{
    public int TotalProductos { get; set; }
    public int TotalProveedores { get; set; }
    public int TotalCategorias { get; set; }
    public int ProductosStockBajo { get; set; }
    public decimal ValorInventario { get; set; }
    public List<ProductoAlertaDto> AlertasStock { get; set; } = new();
}

public class ProductoAlertaDto
{
    public int Id { get; set; }
    public string CodigoBarras { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public decimal Stock { get; set; }
    public decimal StockMinimo { get; set; }
    public string UnidadMedida { get; set; } = string.Empty;
    public decimal PrecioVenta { get; set; }
}
