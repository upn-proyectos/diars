using Minimarket.Datos.Entities;

namespace Minimarket.Datos.Repositories;

/// <summary>
/// Patrón Repository para la gestión de Usuarios y Roles del sistema (CU-02 / RF-05).
/// </summary>
public interface IUsuarioRepository
{
    Task<List<Usuario>> ListarTodosAsync();
    Task<List<Rol>> ListarRolesAsync();
    Task<Usuario?> ObtenerPorIdAsync(int id);
    Task<Rol?> ObtenerRolPorIdAsync(int rolId);
    Task<bool> ExisteLoginAsync(string usuarioLogin, int? excluirId = null);
    Task<int> ContarAdministradoresActivosAsync(int? excluirUsuarioId = null);
    Task<Usuario> CrearAsync(Usuario usuario);
    Task ActualizarAsync(Usuario usuario);
}
