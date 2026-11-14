using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Minimarket.Front.Models;
using Minimarket.Negocio.Interfaces;

namespace Minimarket.Front.Controllers;

[Authorize(Roles = "Administrador,Almacenero")]
public class KardexController : Controller
{
    private readonly IKardexService _kardexService;
    private readonly IProductoService _productoService;

    public KardexController(
        IKardexService kardexService,
        IProductoService productoService)
    {
        _kardexService = kardexService;
        _productoService = productoService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int? productoId = null, DateTime? desde = null, DateTime? hasta = null, string? tipo = null)
    {
        var vm = new KardexViewModel
        {
            ProductoIdFiltro = productoId,
            Desde = desde ?? new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1),
            Hasta = hasta ?? DateTime.Today,
            TipoFiltro = string.IsNullOrWhiteSpace(tipo) ? "TODOS" : tipo.Trim().ToUpperInvariant()
        };

        try
        {
            vm.Productos = await _productoService.ListarCatalogoAsync();
            if (vm.ProductoIdFiltro.HasValue && vm.ProductoIdFiltro.Value > 0)
            {
                vm.ProductoSeleccionado = vm.Productos.FirstOrDefault(p => p.Id == vm.ProductoIdFiltro.Value);
            }

            if (vm.Desde.Date > vm.Hasta.Date)
            {
                TempData["ToastError"] = "Intervalo inválido: la fecha inicial no puede ser posterior a la fecha final. Seleccione un rango de fechas válido.";
                return View(vm);
            }

            vm.Movimientos = await _kardexService.ListarMovimientosAsync(
                vm.ProductoIdFiltro,
                vm.Desde,
                vm.Hasta,
                vm.TipoFiltro,
                400);
        }
        catch (Exception ex)
        {
            TempData["ToastError"] = $"Error al cargar el Kardex: {ex.Message}";
        }

        return View(vm);
    }
}
