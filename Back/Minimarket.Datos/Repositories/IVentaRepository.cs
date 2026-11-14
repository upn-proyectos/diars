using Minimarket.Datos.Entities;

namespace Minimarket.Datos.Repositories;

/// <summary>
/// Patrón Repository para el procesamiento transaccional de Ventas (POS), Boletas y descuento en Kardex.
/// </summary>
public interface IVentaRepository
{
    Task<ConfiguracionTienda> ObtenerConfiguracionTiendaAsync();
    Task<List<Venta>> ListarRecientesAsync(int limite = 50, int? usuarioId = null);
    Task<List<Venta>> ListarPorRangoFechasAsync(DateTime desde, DateTime hasta);
    Task<Venta?> ObtenerPorIdConDetalleAsync(int id);
    Task<(bool Exito, string Mensaje, Venta? VentaRegistrada, int NuevoCorrelativo)> RegistrarVentaTransaccionalAsync(
        int clienteId,
        int usuarioId,
        string metodoPago,
        string serieBoleta,
        decimal tasaIgv,
        List<(int ProductoId, decimal Cantidad)> lineas);
}
