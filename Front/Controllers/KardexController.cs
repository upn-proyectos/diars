using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Minimarket.Front.Models;

namespace Minimarket.Front.Controllers;

[Authorize(Roles = "Administrador")]
public class KardexController : Controller
{
    public IActionResult Index()
    {
        return View("~/Views/Shared/EnDesarrollo.cshtml", new ModuloEnDesarrolloViewModel
        {
            Modulo = "Auditoría de Movimientos y Kardex Físico",
            Fase = "Fase 7 (Bloque Final)",
            Patron = "Patrón Observer / Eventos de Dominio para Movimientos de Stock",
            Icono = "book-open",
            Descripcion = "Registro transaccional de ingresos por compra, salidas por venta, ajustes de inventario y saldos acumulados de existencias."
        });
    }
}
