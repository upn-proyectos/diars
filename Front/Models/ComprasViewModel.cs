using Minimarket.Negocio.DTOs;

namespace Minimarket.Front.Models;

public class ComprasViewModel
{
    public List<OrdenCompraDto> Ordenes { get; set; } = new();
    public List<ProveedorItemDto> Proveedores { get; set; } = new();
    public List<ProductoDto> Productos { get; set; } = new();

    public int TotalPendientes => Ordenes.Count(o => o.Estado.Equals("Pendiente", StringComparison.OrdinalIgnoreCase));
    public int TotalRecibidas => Ordenes.Count(o => o.Estado.Equals("Recibida", StringComparison.OrdinalIgnoreCase));
    public decimal TotalCompradoMesActual => Ordenes
        .Where(o => o.Estado.Equals("Recibida", StringComparison.OrdinalIgnoreCase) &&
                    o.FechaEmision.Month == DateTime.Today.Month &&
                    o.FechaEmision.Year == DateTime.Today.Year)
        .Sum(o => o.Total);
}
