using Minimarket.Negocio.DTOs;
using Minimarket.Negocio.Patterns.Strategy;

namespace Minimarket.Negocio.Interfaces;

public interface IVentaService
{
    Task<ConfiguracionTiendaDto> ObtenerConfiguracionSingletonAsync();
    Task<List<VentaDto>> ListarVentasRecientesAsync(int limite = 50, int? usuarioId = null);
    Task<List<VentaDto>> ListarVentasPorRangoAsync(DateTime desde, DateTime hasta);
    Task<VentaDto?> ObtenerComprobantePorIdAsync(int id);
    Task<(bool Exito, string Mensaje, VentaDto? Venta, ResultadoPagoStrategy? Pago)> RegistrarVentaAsync(VentaRegistrarDto solicitud, int usuarioId);
}
