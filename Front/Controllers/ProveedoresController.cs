using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Minimarket.Negocio.DTOs;
using Minimarket.Negocio.Interfaces;

namespace Minimarket.Front.Controllers;

[Authorize(Roles = "Administrador,Almacenero")]
public class ProveedoresController : Controller
{
    private readonly IProveedorService _proveedorService;

    public ProveedoresController(IProveedorService proveedorService)
    {
        _proveedorService = proveedorService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        List<ProveedorDto> proveedores;
        try
        {
            proveedores = await _proveedorService.ListarProveedoresAsync();
        }
        catch (Exception)
        {
            proveedores = new List<ProveedorDto>
            {
                new() { Id = 1, Ruc = "20100070970", RazonSocial = "Alicorp S.A.A.", NombreComercial = "Alicorp", Contacto = "Carlos Méndez", Telefono = "946112233", Email = "ventas@alicorp.com", Direccion = "Av. Argentina 4793, Callao", TipoContribuyente = "Persona Jurídica (RUC 20)", TotalProductosAbastecidos = 2 },
                new() { Id = 2, Ruc = "20462509236", RazonSocial = "Leche Gloria S.A.", NombreComercial = "Gloria", Contacto = "Rosa Huamán", Telefono = "951223344", Email = "pedidos@gloria.com", Direccion = "Av. República de Panamá 2461, Lima", TipoContribuyente = "Persona Jurídica (RUC 20)", TotalProductosAbastecidos = 2 },
                new() { Id = 3, Ruc = "20501523837", RazonSocial = "Molitalia S.A.", NombreComercial = "Molitalia", Contacto = "Pedro Castillo", Telefono = "987334455", Email = "compras@molitalia.com", Direccion = "Av. Venezuela 2850, Lima", TipoContribuyente = "Persona Jurídica (RUC 20)", TotalProductosAbastecidos = 0 },
                new() { Id = 4, Ruc = "20100182263", RazonSocial = "Procter & Gamble Perú S.R.L.", NombreComercial = "P&G", Contacto = "Ana Torres", Telefono = "965445566", Email = "distribucion@pg.com", Direccion = "Av. El Derby 254, Surco", TipoContribuyente = "Persona Jurídica (RUC 20)", TotalProductosAbastecidos = 1 },
                new() { Id = 5, Ruc = "20516704517", RazonSocial = "Distribuidora Jiménez S.A.C.", NombreComercial = "Dist. Jiménez", Contacto = "Luis Jiménez", Telefono = "943556677", Email = "pedidos@djimenez.com", Direccion = "Jr. Gamarra 438, Trujillo", TipoContribuyente = "Persona Jurídica (RUC 20)", TotalProductosAbastecidos = 3 }
            };
        }

        return View(proveedores);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Guardar(ProveedorGuardarDto dto)
    {
        try
        {
            var resultado = await _proveedorService.GuardarProveedorAsync(dto);
            if (resultado.Exito)
                TempData["ToastExito"] = resultado.Mensaje;
            else
                TempData["ToastError"] = resultado.Mensaje;
        }
        catch (Exception ex)
        {
            TempData["ToastError"] = $"Error al guardar el proveedor: {ex.Message}";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Eliminar(int id)
    {
        try
        {
            var resultado = await _proveedorService.EliminarProveedorAsync(id);
            if (resultado.Exito)
                TempData["ToastExito"] = resultado.Mensaje;
            else
                TempData["ToastError"] = resultado.Mensaje;
        }
        catch (Exception ex)
        {
            TempData["ToastError"] = $"Error al retirar el proveedor: {ex.Message}";
        }

        return RedirectToAction(nameof(Index));
    }
}
