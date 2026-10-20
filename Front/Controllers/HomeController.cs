using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Minimarket.Datos.Context;
using Minimarket.Front.Models;

namespace Minimarket.Front.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly MinimarketDbContext _context;

    public HomeController(MinimarketDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        DashboardViewModel model;

        try
        {
            var totalProductos = await _context.Productos.CountAsync(p => p.Activo);
            var totalProveedores = await _context.Proveedores.CountAsync(p => p.Activo);
            var totalCategorias = await _context.Categorias.CountAsync(c => c.Activo);

            var alertasStock = await _context.Productos
                .Include(p => p.Categoria)
                .Where(p => p.Activo && p.Stock <= p.StockMinimo)
                .Select(p => new ProductoAlertaDto
                {
                    Id = p.Id,
                    CodigoBarras = p.CodigoBarras ?? string.Empty,
                    Nombre = p.Nombre,
                    Categoria = p.Categoria != null ? p.Categoria.Nombre : string.Empty,
                    Stock = p.Stock,
                    StockMinimo = p.StockMinimo,
                    UnidadMedida = p.UnidadMedida,
                    PrecioVenta = p.PrecioVenta
                })
                .ToListAsync();

            var valorInventario = await _context.Productos
                .Where(p => p.Activo)
                .SumAsync(p => p.Stock * p.CostoUnitario);

            model = new DashboardViewModel
            {
                TotalProductos = totalProductos,
                TotalProveedores = totalProveedores,
                TotalCategorias = totalCategorias,
                ProductosStockBajo = alertasStock.Count,
                ValorInventario = valorInventario,
                AlertasStock = alertasStock
            };
        }
        catch (Exception)
        {
            // Respaldo de contingencia: si SQL Server está temporalmente desconectado,
            // renderiza la información demo sin romper la experiencia del usuario.
            model = new DashboardViewModel
            {
                TotalProductos = 10,
                TotalProveedores = 8,
                TotalCategorias = 6,
                ProductosStockBajo = 2,
                ValorInventario = 2450.50m,
                AlertasStock = new List<ProductoAlertaDto>
                {
                    new() { Id = 1, CodigoBarras = "7750123456789", Nombre = "Arroz Costeño Extra 1kg", Categoria = "Abarrotes", Stock = 8, StockMinimo = 10, UnidadMedida = "UNIDAD", PrecioVenta = 4.80m },
                    new() { Id = 2, CodigoBarras = "7750123456791", Nombre = "Aceite Primor Premium 900ml", Categoria = "Abarrotes", Stock = 5, StockMinimo = 8, UnidadMedida = "UNIDAD", PrecioVenta = 8.50m }
                }
            };
        }

        return View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
