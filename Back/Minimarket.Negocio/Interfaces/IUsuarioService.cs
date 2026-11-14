using Minimarket.Negocio.DTOs;

namespace Minimarket.Negocio.Interfaces;

public interface IUsuarioService
{
    Task<List<UsuarioDto>> ListarUsuariosAsync();
    Task<List<RolDto>> ListarRolesAsync();
    Task<UsuarioDto?> ObtenerPorIdAsync(int id);
    Task<(bool Exito, string Mensaje)> GuardarUsuarioAsync(UsuarioGuardarDto dto, int usuarioResponsableId);
}
