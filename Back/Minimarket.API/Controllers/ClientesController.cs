using Microsoft.AspNetCore.Mvc;
using Minimarket.Negocio.DTOs;
using Minimarket.Negocio.Interfaces;

namespace Minimarket.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientesController : ControllerBase
{
    private readonly IClienteService _clienteService;

    public ClientesController(IClienteService clienteService)
    {
        _clienteService = clienteService;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var clientes = await _clienteService.ListarClientesAsync();
        return Ok(clientes);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var cliente = await _clienteService.ObtenerPorIdAsync(id);
        if (cliente == null)
            return NotFound(new { mensaje = "Cliente no encontrado" });

        return Ok(cliente);
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] ClienteGuardarDto dto)
    {
        dto.Id = null;
        var resultado = await _clienteService.GuardarClienteAsync(dto);
        if (!resultado.Exito)
            return BadRequest(new { mensaje = resultado.Mensaje });

        return Ok(new { mensaje = resultado.Mensaje, cliente = resultado.Cliente });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] ClienteGuardarDto dto)
    {
        dto.Id = id;
        var resultado = await _clienteService.GuardarClienteAsync(dto);
        if (!resultado.Exito)
            return BadRequest(new { mensaje = resultado.Mensaje });

        return Ok(new { mensaje = resultado.Mensaje, cliente = resultado.Cliente });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var resultado = await _clienteService.EliminarClienteAsync(id);
        if (!resultado.Exito)
            return BadRequest(new { mensaje = resultado.Mensaje });

        return Ok(new { mensaje = resultado.Mensaje });
    }
}
