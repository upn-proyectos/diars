using Minimarket.Negocio.DTOs;

namespace Minimarket.Negocio.Interfaces;

public interface IOrdenCompraService
{
    Task<List<OrdenCompraDto>> ListarOrdenesAsync();
    Task<OrdenCompraDto?> ObtenerPorIdAsync(int id);
    Task<(bool Exito, string Mensaje, OrdenCompraDto? Orden)> CrearOrdenAsync(OrdenCompraCrearDto dto, int usuarioId);
    Task<(bool Exito, string Mensaje)> ConfirmarRecepcionAsync(OrdenCompraRecepcionDto dto, int usuarioId);
    Task<(bool Exito, string Mensaje)> AnularOrdenAsync(int ordenId, int usuarioId);
}
