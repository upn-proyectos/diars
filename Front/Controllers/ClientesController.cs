using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Minimarket.Negocio.DTOs;
using Minimarket.Negocio.Interfaces;

namespace Minimarket.Front.Controllers;

[Authorize(Roles = "Administrador,Cajero")]
public class ClientesController : Controller
{
    private readonly IClienteService _clienteService;

    public ClientesController(IClienteService clienteService)
    {
        _clienteService = clienteService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        List<ClienteDto> clientes;
        try
        {
            clientes = await _clienteService.ListarClientesAsync();
        }
        catch (Exception)
        {
            clientes = new List<ClienteDto>
            {
                new() { Id = 1, TipoDoc = "DNI", Documento = "00000000", NombreCompleto = "Clientes Varios / Público General", Telefono = "-", Direccion = "-", TipoContribuyente = "Comprobante Rápido" }
            };
        }

        return View(clientes);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Guardar(ClienteGuardarDto dto)
    {
        try
        {
            var resultado = await _clienteService.GuardarClienteAsync(dto);
            if (resultado.Exito)
                TempData["ToastExito"] = resultado.Mensaje;
            else
                TempData["ToastError"] = resultado.Mensaje;
        }
        catch (Exception ex)
        {
            TempData["ToastError"] = $"Error al guardar el cliente: {ex.Message}";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CrearRapidoJson([FromBody] ClienteGuardarDto dto)
    {
        try
        {
            dto.Id = null;
            var resultado = await _clienteService.GuardarClienteAsync(dto);
            if (!resultado.Exito || resultado.Cliente == null)
                return BadRequest(new { exito = false, mensaje = resultado.Mensaje });

            return Ok(new
            {
                exito = true,
                mensaje = resultado.Mensaje,
                cliente = resultado.Cliente
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { exito = false, mensaje = $"Error al registrar cliente: {ex.Message}" });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Eliminar(int id)
    {
        try
        {
            var resultado = await _clienteService.EliminarClienteAsync(id);
            if (resultado.Exito)
                TempData["ToastExito"] = resultado.Mensaje;
            else
                TempData["ToastError"] = resultado.Mensaje;
        }
        catch (Exception ex)
        {
            TempData["ToastError"] = $"Error al retirar el cliente: {ex.Message}";
        }

        return RedirectToAction(nameof(Index));
    }
}
