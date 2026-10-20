using Microsoft.AspNetCore.Mvc;
using Minimarket.Front.Models;

namespace Minimarket.Front.Controllers;

public class ReportesController : Controller
{
    public IActionResult Index()
    {
        return View("~/Views/Shared/EnDesarrollo.cshtml", new ModuloEnDesarrolloViewModel
        {
            Modulo = "Reportes Comerciales y Exportación",
            Fase = "Fase 8 (Bloque Final)",
            Patron = "Reportes Financieros y Métricas de Rendimiento",
            Icono = "bar-chart-3",
            Descripcion = "Consolidado de ventas diarias, productos más vendidos, rotación de mercadería y exportación de resúmenes en CSV / Excel."
        });
    }
}
