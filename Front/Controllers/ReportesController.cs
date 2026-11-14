using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Minimarket.Front.Models;
using Minimarket.Negocio.Interfaces;

namespace Minimarket.Front.Controllers;

[Authorize(Roles = "Administrador")]
public class ReportesController : Controller
{
    private readonly IVentaService _ventaService;

    public ReportesController(IVentaService ventaService)
    {
        _ventaService = ventaService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(DateTime? desde = null, DateTime? hasta = null, string? periodo = null)
    {
        DateTime fechaDesde;
        DateTime fechaHasta = (hasta ?? DateTime.Today).Date;

        if (string.Equals(periodo, "hoy", StringComparison.OrdinalIgnoreCase))
        {
            fechaDesde = DateTime.Today;
            fechaHasta = DateTime.Today;
        }
        else if (string.Equals(periodo, "semana", StringComparison.OrdinalIgnoreCase))
        {
            fechaDesde = DateTime.Today.AddDays(-6);
            fechaHasta = DateTime.Today;
        }
        else if (string.Equals(periodo, "mes", StringComparison.OrdinalIgnoreCase))
        {
            fechaDesde = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            fechaHasta = DateTime.Today;
        }
        else
        {
            fechaDesde = (desde ?? new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1)).Date;
        }

        var vm = new ReportesViewModel
        {
            Desde = fechaDesde,
            Hasta = fechaHasta
        };

        // Validación explícita PF-21 (CU-07 / RF-06): rechazar cuando la fecha inicial es posterior a la fecha final
        if (vm.Desde > vm.Hasta)
        {
            vm.IntervaloInvalido = true;
            TempData["ToastError"] = "Intervalo de fechas inválido: la fecha inicial no puede ser posterior a la fecha final. Por favor, seleccione un rango de fechas válido.";
            return View(vm);
        }

        try
        {
            var ventas = await _ventaService.ListarVentasPorRangoAsync(vm.Desde, vm.Hasta);
            vm.Ventas = ventas;

            var totalGlobal = ventas.Sum(v => v.Total);

            // 1. Agrupación por Método de Pago (Patrón Strategy)
            vm.VentasPorMetodo = ventas
                .GroupBy(v => string.IsNullOrWhiteSpace(v.MetodoPago) ? "Efectivo" : v.MetodoPago)
                .Select(g => new ReporteMetodoPagoItem
                {
                    MetodoPago = g.Key,
                    CantidadVentas = g.Count(),
                    TotalSoles = Math.Round(g.Sum(x => x.Total), 2),
                    Porcentaje = totalGlobal > 0
                        ? Math.Round((g.Sum(x => x.Total) / totalGlobal) * 100m, 1)
                        : 0m
                })
                .OrderByDescending(m => m.TotalSoles)
                .ToList();

            // 2. Agrupación por Producto con cálculo de Costo y Margen Bruto Estimado (Sección 5.7.1 y PF-20)
            vm.VentasPorProducto = ventas
                .SelectMany(v => v.Detalles)
                .GroupBy(d => new { ProductoId = d.ProductoId ?? 0, d.NombreProducto })
                .Select(g => new ReporteProductoRentabilidadItem
                {
                    ProductoId = g.Key.ProductoId,
                    NombreProducto = g.Key.NombreProducto,
                    CantidadVendida = Math.Round(g.Sum(x => x.Cantidad), 3),
                    IngresosTotales = Math.Round(g.Sum(x => x.Subtotal), 2),
                    CostoTotal = Math.Round(g.Sum(x => x.Cantidad * x.CostoUnitario), 2)
                })
                .OrderByDescending(p => p.IngresosTotales)
                .ToList();

            // 3. Serie diaria dentro del rango consultado
            vm.VentasPorDia = ventas
                .GroupBy(v => v.Fecha.Date)
                .OrderBy(g => g.Key)
                .Select(g => new ReporteDiaItem
                {
                    EtiquetaFecha = g.Key.ToString("dd/MM"),
                    FechaIso = g.Key.ToString("yyyy-MM-dd"),
                    CantidadVentas = g.Count(),
                    TotalSoles = Math.Round(g.Sum(x => x.Total), 2)
                })
                .ToList();
        }
        catch (Exception ex)
        {
            TempData["ToastError"] = $"Error al generar el reporte comercial: {ex.Message}";
        }

        return View(vm);
    }
}
