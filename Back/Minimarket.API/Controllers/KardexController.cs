using Microsoft.AspNetCore.Mvc;
using Minimarket.Negocio.Interfaces;

namespace Minimarket.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class KardexController : ControllerBase
{
    private readonly IKardexService _kardexService;

    public KardexController(IKardexService kardexService)
    {
        _kardexService = kardexService;
    }

    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] int? productoId = null,
        [FromQuery] DateTime? desde = null,
        [FromQuery] DateTime? hasta = null,
        [FromQuery] string? tipo = null)
    {
        var lista = await _kardexService.ListarMovimientosAsync(productoId, desde, hasta, tipo);
        return Ok(lista);
    }
}
