using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Minimarket.Negocio.DTOs;
using Minimarket.Negocio.Interfaces;

namespace Minimarket.Front.Controllers;

[Authorize(Roles = "Administrador,Almacenero")]
public class CategoriasController : Controller
{
    private readonly IProductoService _productoService;

    public CategoriasController(IProductoService productoService)
    {
        _productoService = productoService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        List<CategoriaItemDto> categorias;
        try
        {
            categorias = await _productoService.ListarCategoriasAsync();
        }
        catch (Exception)
        {
            categorias = new List<CategoriaItemDto>
            {
                new() { Id = 1, Nombre = "Abarrotes", Descripcion = "Arroz, azúcar, fideos y granos", TotalProductos = 3 },
                new() { Id = 2, Nombre = "Bebidas y Licores", Descripcion = "Gaseosas, aguas, jugos y cervezas", TotalProductos = 2 },
                new() { Id = 3, Nombre = "Lácteos y Huevos", Descripcion = "Leche, yogures, quesos y mantequilla", TotalProductos = 2 },
                new() { Id = 4, Nombre = "Carnes y Embutidos", Descripcion = "Pollo, jamonada, salchichas", TotalProductos = 0 },
                new() { Id = 5, Nombre = "Frutas y Verduras", Descripcion = "Productos frescos pesados por kilogramo", TotalProductos = 2 },
                new() { Id = 6, Nombre = "Limpieza y Aseo", Descripcion = "Detergentes, jabones y desinfectantes", TotalProductos = 1 }
            };
        }

        return View(categorias);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Guardar(CategoriaGuardarDto dto, string? returnUrl = null)
    {
        try
        {
            var resultado = await _productoService.GuardarCategoriaAsync(dto);
            if (resultado.Exito)
                TempData["ToastExito"] = resultado.Mensaje;
            else
                TempData["ToastError"] = resultado.Mensaje;
        }
        catch (Exception ex)
        {
            TempData["ToastError"] = $"Error al guardar categoría: {ex.Message}";
        }

        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Eliminar(int id)
    {
        try
        {
            var resultado = await _productoService.EliminarCategoriaAsync(id);
            if (resultado.Exito)
                TempData["ToastExito"] = resultado.Mensaje;
            else
                TempData["ToastError"] = resultado.Mensaje;
        }
        catch (Exception ex)
        {
            TempData["ToastError"] = $"Error al eliminar categoría: {ex.Message}";
        }

        return RedirectToAction(nameof(Index));
    }
}
