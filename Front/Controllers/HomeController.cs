using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Minimarket.Datos.Context;
using Minimarket.Front.Models;

namespace Minimarket.Front.Controllers;

public class HomeController : Controller
{
    private readonly MinimarketDbContext _context;

    public HomeController(MinimarketDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
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

        var model = new DashboardViewModel
        {
            TotalProductos = totalProductos,
            TotalProveedores = totalProveedores,
            TotalCategorias = totalCategorias,
            ProductosStockBajo = alertasStock.Count,
            ValorInventario = valorInventario,
            AlertasStock = alertasStock
        };

        return View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
