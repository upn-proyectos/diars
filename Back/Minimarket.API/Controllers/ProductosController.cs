using Microsoft.AspNetCore.Mvc;
using Minimarket.Negocio.DTOs;
using Minimarket.Negocio.Interfaces;

namespace Minimarket.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductosController : ControllerBase
{
    private readonly IProductoService _productoService;

    public ProductosController(IProductoService productoService)
    {
        _productoService = productoService;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var productos = await _productoService.ListarCatalogoAsync();
        return Ok(productos);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var producto = await _productoService.ObtenerPorIdAsync(id);
        if (producto == null)
            return NotFound(new { mensaje = "Producto no encontrado" });

        return Ok(producto);
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] ProductoGuardarDto dto)
    {
        dto.Id = null;
        var resultado = await _productoService.GuardarProductoAsync(dto);
        if (!resultado.Exito)
            return BadRequest(new { mensaje = resultado.Mensaje });

        return Ok(new { mensaje = resultado.Mensaje });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] ProductoGuardarDto dto)
    {
        dto.Id = id;
        var resultado = await _productoService.GuardarProductoAsync(dto);
        if (!resultado.Exito)
            return BadRequest(new { mensaje = resultado.Mensaje });

        return Ok(new { mensaje = resultado.Mensaje });
    }

    [HttpPost("{id:int}/ajustar-stock")]
    public async Task<IActionResult> AjustarStock(int id, [FromBody] AjusteStockDto dto)
    {
        dto.ProductoId = id;
        var resultado = await _productoService.AjustarStockAsync(dto, usuarioId: 1);
        if (!resultado.Exito)
            return BadRequest(new { mensaje = resultado.Mensaje });

        return Ok(new { mensaje = resultado.Mensaje });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var resultado = await _productoService.EliminarProductoAsync(id);
        if (!resultado.Exito)
            return NotFound(new { mensaje = resultado.Mensaje });

        return Ok(new { mensaje = resultado.Mensaje });
    }
}
