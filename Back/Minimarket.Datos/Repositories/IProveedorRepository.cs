using Minimarket.Datos.Entities;

namespace Minimarket.Datos.Repositories;

/// <summary>
/// Patrón Repository para el acceso a datos de la entidad Proveedor en SQL Server.
/// </summary>
public interface IProveedorRepository
{
    Task<List<Proveedor>> ListarActivosAsync();
    Task<Proveedor?> ObtenerPorIdAsync(int id);
    Task<bool> ExisteRucAsync(string ruc, int? excluirId = null);
    Task<Proveedor> CrearAsync(Proveedor proveedor);
    Task ActualizarAsync(Proveedor proveedor);
    Task<(bool Exito, string Mensaje)> EliminarLogicoAsync(int id);
}
