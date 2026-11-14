using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Minimarket.Front.Models;
using Minimarket.Negocio.DTOs;
using Minimarket.Negocio.Interfaces;

namespace Minimarket.Front.Controllers;

[Authorize(Roles = "Administrador,Cajero")]
public class PosController : Controller
{
    private readonly IVentaService _ventaService;
    private readonly IProductoService _productoService;
    private readonly IClienteService _clienteService;

    public PosController(
        IVentaService ventaService,
        IProductoService productoService,
        IClienteService clienteService)
    {
        _ventaService = ventaService;
        _productoService = productoService;
        _clienteService = clienteService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int? boletaId = null)
    {
        var vm = new PosViewModel();
        try
        {
            vm.Configuracion = await _ventaService.ObtenerConfiguracionSingletonAsync();
            vm.Productos = await _productoService.ListarCatalogoAsync();
            vm.Categorias = await _productoService.ListarCategoriasAsync();
            vm.Clientes = await _clienteService.ListarClientesAsync();

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int? filtroUsuario = User.IsInRole("Administrador")
                ? null
                : (int.TryParse(userIdStr, out var uid) ? uid : null);

            vm.VentasRecientes = await _ventaService.ListarVentasRecientesAsync(15, filtroUsuario);

            if (boletaId.HasValue && boletaId.Value > 0)
            {
                vm.UltimaVentaEmitida = await _ventaService.ObtenerComprobantePorIdAsync(boletaId.Value);
            }
        }
        catch (Exception ex)
        {
            TempData["ToastError"] = $"Advertencia al cargar datos del POS: {ex.Message}";
        }

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Registrar(VentaRegistrarDto solicitud)
    {
        try
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var usuarioId = int.TryParse(userIdStr, out var id) ? id : 1;

            var resultado = await _ventaService.RegistrarVentaAsync(solicitud, usuarioId);
            if (!resultado.Exito || resultado.Venta == null)
            {
                TempData["ToastError"] = resultado.Mensaje;
                return RedirectToAction(nameof(Index));
            }

            TempData["ToastExito"] = resultado.Mensaje;
            TempData["MontoRecibidoBoleta"] = resultado.Pago?.MontoRecibido.ToString("0.00") ?? resultado.Venta.Total.ToString("0.00");
            TempData["VueltoBoleta"] = resultado.Pago?.Vuelto.ToString("0.00") ?? "0.00";
            TempData["ReferenciaBoleta"] = resultado.Pago?.ReferenciaOperacion ?? "";

            return RedirectToAction(nameof(Index), new { boletaId = resultado.Venta.Id });
        }
        catch (Exception ex)
        {
            TempData["ToastError"] = $"Error al procesar la venta: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpGet]
    public async Task<IActionResult> Comprobante(int id)
    {
        var venta = await _ventaService.ObtenerComprobantePorIdAsync(id);
        if (venta == null)
        {
            TempData["ToastError"] = "No se encontró el comprobante solicitado.";
            return RedirectToAction(nameof(Index));
        }

        var cfg = await _ventaService.ObtenerConfiguracionSingletonAsync();
        ViewBag.Configuracion = cfg;
        return View(venta);
    }
}
