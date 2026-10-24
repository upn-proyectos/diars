using Microsoft.AspNetCore.Mvc;
using Minimarket.Negocio.DTOs;
using Minimarket.Negocio.Interfaces;

namespace Minimarket.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProveedoresController : ControllerBase
{
    private readonly IProveedorService _proveedorService;

    public ProveedoresController(IProveedorService proveedorService)
    {
        _proveedorService = proveedorService;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var proveedores = await _proveedorService.ListarProveedoresAsync();
        return Ok(proveedores);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var proveedor = await _proveedorService.ObtenerPorIdAsync(id);
        if (proveedor == null)
            return NotFound(new { mensaje = "Proveedor no encontrado" });

        return Ok(proveedor);
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] ProveedorGuardarDto dto)
    {
        dto.Id = null;
        var resultado = await _proveedorService.GuardarProveedorAsync(dto);
        if (!resultado.Exito)
            return BadRequest(new { mensaje = resultado.Mensaje });

        return Ok(new { mensaje = resultado.Mensaje });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] ProveedorGuardarDto dto)
    {
        dto.Id = id;
        var resultado = await _proveedorService.GuardarProveedorAsync(dto);
        if (!resultado.Exito)
            return BadRequest(new { mensaje = resultado.Mensaje });

        return Ok(new { mensaje = resultado.Mensaje });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var resultado = await _proveedorService.EliminarProveedorAsync(id);
        if (!resultado.Exito)
            return NotFound(new { mensaje = resultado.Mensaje });

        return Ok(new { mensaje = resultado.Mensaje });
    }
}
