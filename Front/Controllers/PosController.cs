using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Minimarket.Front.Models;

namespace Minimarket.Front.Controllers;

[Authorize(Roles = "Administrador,Cajero")]
public class PosController : Controller
{
    public IActionResult Index()
    {
        return View("~/Views/Shared/EnDesarrollo.cshtml", new ModuloEnDesarrolloViewModel
        {
            Modulo = "Punto de Venta (POS) y Emisión de Boletas",
            Fase = "Fase 6 (Bloque Final)",
            Patron = "Patrón Strategy (Métodos de Pago) + Patrón Singleton (Configuración Tienda)",
            Icono = "shopping-cart",
            Descripcion = "Carrito de compras rápido, cálculo de IGV, cobro por Efectivo, Yape y Tarjeta, y emisión de boletas de venta correlativas."
        });
    }
}
