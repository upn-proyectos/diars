using Microsoft.AspNetCore.Mvc;
using Minimarket.Negocio.DTOs;
using Minimarket.Negocio.Interfaces;

namespace Minimarket.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;

    public UsuariosController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var usuarios = await _usuarioService.ListarUsuariosAsync();
        return Ok(usuarios);
    }

    [HttpGet("roles")]
    public async Task<IActionResult> ListarRoles()
    {
        var roles = await _usuarioService.ListarRolesAsync();
        return Ok(roles);
    }

    [HttpPost]
    public async Task<IActionResult> Guardar([FromBody] UsuarioGuardarDto dto)
    {
        var resultado = await _usuarioService.GuardarUsuarioAsync(dto, usuarioResponsableId: 1);
        if (!resultado.Exito)
            return BadRequest(new { mensaje = resultado.Mensaje });

        return Ok(new { mensaje = resultado.Mensaje });
    }
}
