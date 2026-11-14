using Minimarket.Negocio.DTOs;

namespace Minimarket.Front.Models;

public class UsuariosViewModel
{
    public List<UsuarioDto> Usuarios { get; set; } = new();
    public List<RolDto> Roles { get; set; } = new();
}
