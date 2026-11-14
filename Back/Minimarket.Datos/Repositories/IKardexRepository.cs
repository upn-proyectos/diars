using Minimarket.Datos.Entities;

namespace Minimarket.Datos.Repositories;

public interface IKardexRepository
{
    Task<List<Kardex>> ListarMovimientosAsync(
        int? productoId = null,
        DateTime? desde = null,
        DateTime? hasta = null,
        string? tipoMovimiento = null,
        int limite = 300);
}
