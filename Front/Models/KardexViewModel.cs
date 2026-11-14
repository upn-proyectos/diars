using Minimarket.Negocio.DTOs;

namespace Minimarket.Front.Models;

public class KardexViewModel
{
    public List<ProductoDto> Productos { get; set; } = new();
    public List<KardexMovimientoDto> Movimientos { get; set; } = new();
    public ProductoDto? ProductoSeleccionado { get; set; }

    public int? ProductoIdFiltro { get; set; }
    public DateTime Desde { get; set; } = new(DateTime.Today.Year, DateTime.Today.Month, 1);
    public DateTime Hasta { get; set; } = DateTime.Today;
    public string TipoFiltro { get; set; } = "TODOS";

    public decimal TotalEntradasPeriodo => Movimientos
        .Where(m => m.TipoMovimiento == "ENTRADA" || (m.TipoMovimiento == "AJUSTE" && m.Cantidad > 0))
        .Sum(m => Math.Abs(m.Cantidad));

    public decimal TotalSalidasPeriodo => Movimientos
        .Where(m => m.TipoMovimiento == "SALIDA" || (m.TipoMovimiento == "AJUSTE" && m.Cantidad < 0))
        .Sum(m => Math.Abs(m.Cantidad));
}
