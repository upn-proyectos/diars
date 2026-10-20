using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Minimarket.Front.Models;

namespace Minimarket.Front.Controllers;

[Authorize(Roles = "Administrador,Cajero")]
public class ClientesController : Controller
{
    public IActionResult Index()
    {
        return View("~/Views/Shared/EnDesarrollo.cshtml", new ModuloEnDesarrolloViewModel
        {
            Modulo = "Directorio de Clientes",
            Fase = "Fase 6 (Bloque Final)",
            Patron = "Patrón Adapter (Validación DNI / RUC)",
            Icono = "users-2",
            Descripcion = "Búsqueda y registro de clientes con DNI o RUC para emisión personalizada de comprobantes de pago."
        });
    }
}
