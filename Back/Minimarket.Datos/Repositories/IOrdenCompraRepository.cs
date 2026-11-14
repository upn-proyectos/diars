using Minimarket.Datos.Entities;

namespace Minimarket.Datos.Repositories;

public interface IOrdenCompraRepository
{
    Task<List<OrdenCompra>> ListarOrdenesAsync();
    Task<OrdenCompra?> ObtenerPorIdAsync(int id);
    Task<(bool Exito, string Mensaje, OrdenCompra? OrdenCreada)> CrearOrdenAsync(
        int proveedorId,
        int usuarioId,
        string? observaciones,
        List<(int ProductoId, decimal Cantidad, decimal CostoUnitario)> items);
    Task<(bool Exito, string Mensaje)> ConfirmarRecepcionTransaccionalAsync(
        int ordenId,
        int usuarioId,
        Dictionary<int, decimal>? cantidadesRecibidas = null);
    Task<(bool Exito, string Mensaje)> AnularOrdenAsync(int ordenId, int usuarioId);
}
