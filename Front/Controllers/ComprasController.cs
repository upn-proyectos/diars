using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Minimarket.Front.Models;
using Minimarket.Negocio.DTOs;
using Minimarket.Negocio.Interfaces;

namespace Minimarket.Front.Controllers;

[Authorize(Roles = "Administrador,Almacenero")]
public class ComprasController : Controller
{
    private readonly IOrdenCompraService _ordenService;
    private readonly IProductoService _productoService;

    public ComprasController(
        IOrdenCompraService ordenService,
        IProductoService productoService)
    {
        _ordenService = ordenService;
        _productoService = productoService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var vm = new ComprasViewModel();
        try
        {
            vm.Ordenes = await _ordenService.ListarOrdenesAsync();
            vm.Proveedores = await _productoService.ListarProveedoresAsync();
            vm.Productos = await _productoService.ListarCatalogoAsync();
        }
        catch (Exception ex)
        {
            TempData["ToastError"] = $"Error al cargar órdenes de compra: {ex.Message}";
        }

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(OrdenCompraCrearDto dto)
    {
        try
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var usuarioId = int.TryParse(userIdStr, out var id) ? id : 1;

            var resultado = await _ordenService.CrearOrdenAsync(dto, usuarioId);
            if (resultado.Exito)
                TempData["ToastExito"] = resultado.Mensaje;
            else
                TempData["ToastError"] = resultado.Mensaje;
        }
        catch (Exception ex)
        {
            TempData["ToastError"] = $"Error al generar la orden: {ex.Message}";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmarRecepcion(OrdenCompraRecepcionDto dto)
    {
        try
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var usuarioId = int.TryParse(userIdStr, out var id) ? id : 1;

            var resultado = await _ordenService.ConfirmarRecepcionAsync(dto, usuarioId);
            if (resultado.Exito)
                TempData["ToastExito"] = resultado.Mensaje;
            else
                TempData["ToastError"] = resultado.Mensaje;
        }
        catch (Exception ex)
        {
            TempData["ToastError"] = $"Error al confirmar recepción: {ex.Message}";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Anular(int ordenId)
    {
        try
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var usuarioId = int.TryParse(userIdStr, out var id) ? id : 1;

            var resultado = await _ordenService.AnularOrdenAsync(ordenId, usuarioId);
            if (resultado.Exito)
                TempData["ToastExito"] = resultado.Mensaje;
            else
                TempData["ToastError"] = resultado.Mensaje;
        }
        catch (Exception ex)
        {
            TempData["ToastError"] = $"Error al anular orden: {ex.Message}";
        }

        return RedirectToAction(nameof(Index));
    }
}
