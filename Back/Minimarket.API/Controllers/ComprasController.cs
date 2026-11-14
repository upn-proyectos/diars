using Microsoft.AspNetCore.Mvc;
using Minimarket.Negocio.DTOs;
using Minimarket.Negocio.Interfaces;

namespace Minimarket.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ComprasController : ControllerBase
{
    private readonly IOrdenCompraService _ordenService;

    public ComprasController(IOrdenCompraService ordenService)
    {
        _ordenService = ordenService;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var lista = await _ordenService.ListarOrdenesAsync();
        return Ok(lista);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var orden = await _ordenService.ObtenerPorIdAsync(id);
        return orden == null ? NotFound() : Ok(orden);
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] OrdenCompraCrearDto dto, [FromQuery] int usuarioId = 1)
    {
        var resultado = await _ordenService.CrearOrdenAsync(dto, usuarioId);
        if (!resultado.Exito)
            return BadRequest(new { exito = false, mensaje = resultado.Mensaje });

        return Ok(new { exito = true, mensaje = resultado.Mensaje, orden = resultado.Orden });
    }

    [HttpPost("recepcionar")]
    public async Task<IActionResult> Recepcionar([FromBody] OrdenCompraRecepcionDto dto, [FromQuery] int usuarioId = 1)
    {
        var resultado = await _ordenService.ConfirmarRecepcionAsync(dto, usuarioId);
        if (!resultado.Exito)
            return BadRequest(new { exito = false, mensaje = resultado.Mensaje });

        return Ok(new { exito = true, mensaje = resultado.Mensaje });
    }
}
