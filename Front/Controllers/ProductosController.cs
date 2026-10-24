using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Minimarket.Front.Models;
using Minimarket.Negocio.DTOs;
using Minimarket.Negocio.Interfaces;

namespace Minimarket.Front.Controllers;

[Authorize(Roles = "Administrador,Almacenero")]
public class ProductosController : Controller
{
    private readonly IProductoService _productoService;

    public ProductosController(IProductoService productoService)
    {
        _productoService = productoService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        ProductosCatalogoViewModel viewModel;

        try
        {
            var productos = await _productoService.ListarCatalogoAsync();
            var categorias = await _productoService.ListarCategoriasAsync();
            var proveedores = await _productoService.ListarProveedoresAsync();

            viewModel = new ProductosCatalogoViewModel
            {
                Productos = productos,
                Categorias = categorias,
                Proveedores = proveedores
            };
        }
        catch (Exception)
        {
            // Respaldo de contingencia en caso de que el motor SQL Server local esté detenido
            viewModel = ConstruirRespaldoDemo();
        }

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Guardar(ProductoGuardarDto dto)
    {
        try
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int? usuarioId = int.TryParse(userIdClaim, out var uid) ? uid : null;

            var resultado = await _productoService.GuardarProductoAsync(dto, usuarioId);
            if (resultado.Exito)
                TempData["ToastExito"] = resultado.Mensaje;
            else
                TempData["ToastError"] = resultado.Mensaje;
        }
        catch (Exception ex)
        {
            TempData["ToastError"] = $"Error al guardar en base de datos: {ex.Message}";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AjustarStock(AjusteStockDto dto)
    {
        try
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var usuarioId = int.TryParse(userIdClaim, out var uid) ? uid : 1;

            var resultado = await _productoService.AjustarStockAsync(dto, usuarioId);
            if (resultado.Exito)
                TempData["ToastExito"] = resultado.Mensaje;
            else
                TempData["ToastError"] = resultado.Mensaje;
        }
        catch (Exception ex)
        {
            TempData["ToastError"] = $"Error al ajustar el stock: {ex.Message}";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Eliminar(int id)
    {
        try
        {
            var resultado = await _productoService.EliminarProductoAsync(id);
            if (resultado.Exito)
                TempData["ToastExito"] = resultado.Mensaje;
            else
                TempData["ToastError"] = resultado.Mensaje;
        }
        catch (Exception ex)
        {
            TempData["ToastError"] = $"Error al retirar el producto: {ex.Message}";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> CrearCategoriaRapida([FromBody] CategoriaGuardarDto dto)
    {
        try
        {
            dto.Id = null;
            var resultado = await _productoService.GuardarCategoriaAsync(dto);
            if (!resultado.Exito)
                return BadRequest(new { exito = false, mensaje = resultado.Mensaje });

            var categorias = await _productoService.ListarCategoriasAsync();
            var creada = categorias.FirstOrDefault(c =>
                string.Equals(c.Nombre, dto.Nombre.Trim(), StringComparison.OrdinalIgnoreCase));

            return Ok(new
            {
                exito = true,
                mensaje = resultado.Mensaje,
                id = creada?.Id ?? 0,
                nombre = creada?.Nombre ?? dto.Nombre.Trim()
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { exito = false, mensaje = $"Error al crear categoría: {ex.Message}" });
        }
    }

    private static ProductosCatalogoViewModel ConstruirRespaldoDemo()
    {
        return new ProductosCatalogoViewModel
        {
            Categorias = new List<CategoriaItemDto>
            {
                new() { Id = 1, Nombre = "Abarrotes" },
                new() { Id = 2, Nombre = "Bebidas y Licores" },
                new() { Id = 3, Nombre = "Lácteos y Huevos" },
                new() { Id = 5, Nombre = "Frutas y Verduras" },
                new() { Id = 6, Nombre = "Limpieza y Aseo" }
            },
            Proveedores = new List<ProveedorItemDto>
            {
                new() { Id = 1, Ruc = "20100070970", RazonSocial = "Alicorp S.A.A.", NombreMostrar = "Alicorp" },
                new() { Id = 2, Ruc = "20462509236", RazonSocial = "Leche Gloria S.A.", NombreMostrar = "Gloria" },
                new() { Id = 5, Ruc = "20516704517", RazonSocial = "Distribuidora Jiménez S.A.C.", NombreMostrar = "Dist. Jiménez" }
            },
            Productos = new List<ProductoDto>
            {
                new() { Id = 1, CodigoBarras = "7750123456789", Nombre = "Arroz Costeño Extra 1kg", CategoriaId = 1, CategoriaNombre = "Abarrotes", ProveedorId = 1, ProveedorNombre = "Alicorp", PrecioVenta = 4.80m, CostoUnitario = 3.80m, Stock = 50, StockMinimo = 10, UnidadMedida = "UNIDAD", FechaVencimiento = DateTime.Today.AddMonths(6) },
                new() { Id = 2, CodigoBarras = "7750123456791", Nombre = "Aceite Primor Premium 900ml", CategoriaId = 1, CategoriaNombre = "Abarrotes", ProveedorId = 1, ProveedorNombre = "Alicorp", PrecioVenta = 8.50m, CostoUnitario = 7.00m, Stock = 5, StockMinimo = 8, UnidadMedida = "UNIDAD", FechaVencimiento = DateTime.Today.AddMonths(10) },
                new() { Id = 3, CodigoBarras = "7750123456795", Nombre = "Yogurt Gloria Fresa 1L", CategoriaId = 3, CategoriaNombre = "Lácteos y Huevos", ProveedorId = 2, ProveedorNombre = "Gloria", PrecioVenta = 6.80m, CostoUnitario = 5.20m, Stock = 18, StockMinimo = 5, UnidadMedida = "UNIDAD", FechaVencimiento = DateTime.Today.AddDays(20) },
                new() { Id = 4, CodigoBarras = "7750123456796", Nombre = "Tomate Italiano (Granel)", CategoriaId = 5, CategoriaNombre = "Frutas y Verduras", ProveedorId = null, ProveedorNombre = "—", PrecioVenta = 3.50m, CostoUnitario = 2.30m, Stock = 4.500m, StockMinimo = 5.000m, UnidadMedida = "KG", FechaVencimiento = DateTime.Today.AddDays(6) }
            }
        };
    }
}
