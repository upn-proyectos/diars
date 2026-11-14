using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Minimarket.Front.Models;
using Minimarket.Negocio.DTOs;
using Minimarket.Negocio.Interfaces;

namespace Minimarket.Front.Controllers;

[Authorize(Roles = "Administrador")]
public class UsuariosController : Controller
{
    private readonly IUsuarioService _usuarioService;

    public UsuariosController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var vm = new UsuariosViewModel();
        try
        {
            vm.Usuarios = await _usuarioService.ListarUsuariosAsync();
            vm.Roles = await _usuarioService.ListarRolesAsync();
        }
        catch (Exception)
        {
            vm.Roles = new List<RolDto>
            {
                new() { Id = 1, Nombre = "Administrador", Descripcion = "Acceso total al sistema" },
                new() { Id = 2, Nombre = "Almacenero", Descripcion = "Gestión de catálogo, stock y proveedores" },
                new() { Id = 3, Nombre = "Cajero", Descripcion = "Punto de venta y clientes" }
            };
        }

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Guardar(UsuarioGuardarDto dto)
    {
        try
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var responsableId = int.TryParse(userIdStr, out var id) ? id : 1;

            var resultado = await _usuarioService.GuardarUsuarioAsync(dto, responsableId);
            if (resultado.Exito)
                TempData["ToastExito"] = resultado.Mensaje;
            else
                TempData["ToastError"] = resultado.Mensaje;
        }
        catch (Exception ex)
        {
            TempData["ToastError"] = $"Error al guardar la cuenta de usuario: {ex.Message}";
        }

        return RedirectToAction(nameof(Index));
    }
}
