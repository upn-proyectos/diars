using Microsoft.AspNetCore.Mvc;
using Minimarket.Negocio.DTOs;
using Minimarket.Negocio.Interfaces;

namespace Minimarket.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VentasController : ControllerBase
{
    private readonly IVentaService _ventaService;

    public VentasController(IVentaService ventaService)
    {
        _ventaService = ventaService;
    }

    [HttpGet]
    public async Task<IActionResult> ListarRecientes([FromQuery] int limite = 50)
    {
        var ventas = await _ventaService.ListarVentasRecientesAsync(limite);
        return Ok(ventas);
    }

    [HttpGet("configuracion")]
    public async Task<IActionResult> ObtenerConfiguracionSingleton()
    {
        var cfg = await _ventaService.ObtenerConfiguracionSingletonAsync();
        return Ok(cfg);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var venta = await _ventaService.ObtenerComprobantePorIdAsync(id);
        if (venta == null)
            return NotFound(new { mensaje = "Comprobante de venta no encontrado" });

        return Ok(venta);
    }

    [HttpPost]
    public async Task<IActionResult> Registrar([FromBody] VentaRegistrarDto solicitud, [FromQuery] int usuarioId = 1)
    {
        var resultado = await _ventaService.RegistrarVentaAsync(solicitud, usuarioId);
        if (!resultado.Exito)
            return BadRequest(new { mensaje = resultado.Mensaje });

        return Ok(new
        {
            mensaje = resultado.Mensaje,
            venta = resultado.Venta,
            pago = resultado.Pago
        });
    }
}
