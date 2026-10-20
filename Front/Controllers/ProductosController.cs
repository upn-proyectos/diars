using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Minimarket.Front.Models;

namespace Minimarket.Front.Controllers;

[Authorize(Roles = "Administrador,Almacenero")]
public class ProductosController : Controller
{
    public IActionResult Index()
    {
        return View("~/Views/Shared/EnDesarrollo.cshtml", new ModuloEnDesarrolloViewModel
        {
            Modulo = "Catálogo de Productos e Inventario",
            Fase = "Fase 4 (Primer Avance T2)",
            Patron = "Patrón Repository (IProductoRepository) + Dependency Injection",
            Icono = "package",
            Descripcion = "Gestión integral de productos, stock en tiempo real, alertas de stock mínimo, fechas de caducidad y filtros por categoría."
        });
    }
}
