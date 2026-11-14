using Minimarket.Negocio.DTOs;

namespace Minimarket.Front.Models;

public class ReportesViewModel
{
    public DateTime Desde { get; set; } = new(DateTime.Today.Year, DateTime.Today.Month, 1);
    public DateTime Hasta { get; set; } = DateTime.Today;
    public bool IntervaloInvalido { get; set; }

    public List<VentaDto> Ventas { get; set; } = new();
    public List<ReporteMetodoPagoItem> VentasPorMetodo { get; set; } = new();
    public List<ReporteProductoRentabilidadItem> VentasPorProducto { get; set; } = new();
    public List<ReporteDiaItem> VentasPorDia { get; set; } = new();

    public int TotalTransacciones => Ventas.Count;
    public decimal TotalIngresos => Ventas.Sum(v => v.Total);
    public decimal TotalSubtotalNeto => Ventas.Sum(v => v.Subtotal);
    public decimal TotalIgv => Ventas.Sum(v => v.Igv);
    public decimal TicketPromedio => TotalTransacciones > 0
        ? Math.Round(TotalIngresos / TotalTransacciones, 2)
        : 0m;

    public decimal UtilidadBrutaEstimada => VentasPorProducto.Sum(p => p.UtilidadBruta);
    public decimal MargenPorcentajeGlobal => TotalIngresos > 0
        ? Math.Round((UtilidadBrutaEstimada / TotalIngresos) * 100m, 1)
        : 0m;
}

public class ReporteMetodoPagoItem
{
    public string MetodoPago { get; set; } = string.Empty;
    public int CantidadVentas { get; set; }
    public decimal TotalSoles { get; set; }
    public decimal Porcentaje { get; set; }
}

public class ReporteProductoRentabilidadItem
{
    public int ProductoId { get; set; }
    public string NombreProducto { get; set; } = string.Empty;
    public decimal CantidadVendida { get; set; }
    public decimal IngresosTotales { get; set; }
    public decimal CostoTotal { get; set; }
    public decimal UtilidadBruta => Math.Round(IngresosTotales - CostoTotal, 2);
    public decimal MargenPorcentaje => IngresosTotales > 0
        ? Math.Round((UtilidadBruta / IngresosTotales) * 100m, 1)
        : 0m;
}

public class ReporteDiaItem
{
    public string EtiquetaFecha { get; set; } = string.Empty;
    public string FechaIso { get; set; } = string.Empty;
    public int CantidadVentas { get; set; }
    public decimal TotalSoles { get; set; }
}
