using Minimarket.Negocio.DTOs;

namespace Minimarket.Negocio.Interfaces;

public interface IAuthService
{
    Task<UsuarioSessionDto?> ValidarCredencialesAsync(string usuario, string password);
}
