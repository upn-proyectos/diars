using Minimarket.Negocio.DTOs;

namespace Minimarket.Negocio.Interfaces;

public interface IClienteService
{
    Task<List<ClienteDto>> ListarClientesAsync();
    Task<ClienteDto?> ObtenerPorIdAsync(int id);
    Task<(bool Exito, string Mensaje, ClienteDto? Cliente)> GuardarClienteAsync(ClienteGuardarDto dto);
    Task<(bool Exito, string Mensaje)> EliminarClienteAsync(int id);
}
