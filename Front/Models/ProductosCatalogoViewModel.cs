using Minimarket.Negocio.DTOs;

namespace Minimarket.Front.Models;

public class ProductosCatalogoViewModel
{
    public List<ProductoDto> Productos { get; set; } = new();
    public List<CategoriaItemDto> Categorias { get; set; } = new();
    public List<ProveedorItemDto> Proveedores { get; set; } = new();

    // Indicadores KPI del Módulo de Inventario
    public int TotalProductos => Productos.Count;
    public int TotalStockBajo => Productos.Count(p => p.EstaBajoStock);
    public int TotalPorVencer => Productos.Count(p => p.DiasParaVencer.HasValue && p.DiasParaVencer.Value <= 30);
    public decimal ValorTotalInventario => Productos.Sum(p => p.Stock * p.CostoUnitario);
}
