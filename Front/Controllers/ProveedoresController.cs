using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Minimarket.Front.Models;

namespace Minimarket.Front.Controllers;

[Authorize(Roles = "Administrador,Almacenero")]
public class ProveedoresController : Controller
{
    public IActionResult Index()
    {
        return View("~/Views/Shared/EnDesarrollo.cshtml", new ModuloEnDesarrolloViewModel
        {
            Modulo = "Gestión de Proveedores y Abastecimiento",
            Fase = "Fase 5 (Primer Avance T2)",
            Patron = "Patrón Adapter (Validador de RUC / DNI SUNAT)",
            Icono = "truck",
            Descripcion = "Registro y administración de empresas proveedoras con validación adaptativa de RUC de 11 dígitos y contactos comerciales."
        });
    }
}
