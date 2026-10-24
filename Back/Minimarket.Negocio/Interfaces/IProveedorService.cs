using Minimarket.Negocio.DTOs;

namespace Minimarket.Negocio.Interfaces;

public interface IProveedorService
{
    Task<List<ProveedorDto>> ListarProveedoresAsync();
    Task<ProveedorDto?> ObtenerPorIdAsync(int id);
    Task<(bool Exito, string Mensaje)> GuardarProveedorAsync(ProveedorGuardarDto dto);
    Task<(bool Exito, string Mensaje)> EliminarProveedorAsync(int id);
}
