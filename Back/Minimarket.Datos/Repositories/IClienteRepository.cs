using Minimarket.Datos.Entities;

namespace Minimarket.Datos.Repositories;

/// <summary>
/// Patrón Repository para la gestión de Clientes en SQL Server.
/// </summary>
public interface IClienteRepository
{
    Task<List<Cliente>> ListarActivosAsync();
    Task<Cliente?> ObtenerPorIdAsync(int id);
    Task<Cliente?> ObtenerPorDocumentoAsync(string documento);
    Task<Cliente> ObtenerClientePublicoGeneralAsync();
    Task<bool> ExisteDocumentoAsync(string documento, int? excluirId = null);
    Task<Cliente> CrearAsync(Cliente cliente);
    Task ActualizarAsync(Cliente cliente);
    Task<(bool Exito, string Mensaje)> EliminarLogicoAsync(int id);
}
