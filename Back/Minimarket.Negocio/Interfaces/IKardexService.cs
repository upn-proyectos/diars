using Minimarket.Negocio.DTOs;

namespace Minimarket.Negocio.Interfaces;

public interface IKardexService
{
    Task<List<KardexMovimientoDto>> ListarMovimientosAsync(
        int? productoId = null,
        DateTime? desde = null,
        DateTime? hasta = null,
        string? tipoMovimiento = null,
        int limite = 300);
}
