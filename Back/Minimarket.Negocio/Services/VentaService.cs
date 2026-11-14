using Minimarket.Datos.Entities;
using Minimarket.Datos.Repositories;
using Minimarket.Negocio.DTOs;
using Minimarket.Negocio.Interfaces;
using Minimarket.Negocio.Patterns.Singleton;
using Minimarket.Negocio.Patterns.Strategy;

namespace Minimarket.Negocio.Services;

/// <summary>
/// Capa de Lógica de Negocio para el Punto de Venta (POS) y Emisión de Boletas (CU-03, RF-01, RF-02).
/// Integra el Patrón Singleton (ConfiguracionTiendaSingleton), el Patrón Strategy (PagoStrategyContext)
/// y el Patrón Repository (IVentaRepository + IProductoRepository).
/// </summary>
public class VentaService : IVentaService
{
    private readonly IVentaRepository _ventaRepository;
    private readonly IProductoRepository _productoRepository;
    private readonly PagoStrategyContext _pagoStrategyContext;
    private readonly ConfiguracionTiendaSingleton _configSingleton;

    public VentaService(
        IVentaRepository ventaRepository,
        IProductoRepository productoRepository,
        PagoStrategyContext pagoStrategyContext)
    {
        _ventaRepository = ventaRepository;
        _productoRepository = productoRepository;
        _pagoStrategyContext = pagoStrategyContext;
        _configSingleton = ConfiguracionTiendaSingleton.Instancia;
    }

    public async Task<ConfiguracionTiendaDto> ObtenerConfiguracionSingletonAsync()
    {
        var entidadBd = await _ventaRepository.ObtenerConfiguracionTiendaAsync();
        _configSingleton.SincronizarDesdeEntidad(entidadBd);

        return new ConfiguracionTiendaDto
        {
            NombreComercio = _configSingleton.NombreComercio,
            Ruc = _configSingleton.Ruc,
            Direccion = _configSingleton.Direccion,
            SerieBoleta = _configSingleton.SerieBoleta,
            CorrelativoActual = _configSingleton.CorrelativoActual,
            ProximoComprobante = _configSingleton.ObtenerProximoComprobantePreview(),
            TasaIgv = _configSingleton.TasaIgv
        };
    }

    public async Task<List<VentaDto>> ListarVentasRecientesAsync(int limite = 50, int? usuarioId = null)
    {
        var ventas = await _ventaRepository.ListarRecientesAsync(limite, usuarioId);
        return ventas.Select(MapearADto).ToList();
    }

    public async Task<List<VentaDto>> ListarVentasPorRangoAsync(DateTime desde, DateTime hasta)
    {
        var ventas = await _ventaRepository.ListarPorRangoFechasAsync(desde, hasta);
        return ventas.Select(MapearADto).ToList();
    }

    public async Task<VentaDto?> ObtenerComprobantePorIdAsync(int id)
    {
        var venta = await _ventaRepository.ObtenerPorIdConDetalleAsync(id);
        return venta == null ? null : MapearADto(venta);
    }

    public async Task<(bool Exito, string Mensaje, VentaDto? Venta, ResultadoPagoStrategy? Pago)> RegistrarVentaAsync(
        VentaRegistrarDto solicitud,
        int usuarioId)
    {
        // 1. Validar que el carrito tenga artículos válidos
        if (solicitud.Lineas == null || solicitud.Lineas.Count == 0)
            return (false, "Agregue al menos un producto al carrito antes de confirmar la venta.", null, null);

        var lineasAgrupadas = solicitud.Lineas
            .Where(l => l.ProductoId > 0 && l.Cantidad > 0)
            .GroupBy(l => l.ProductoId)
            .Select(g => (ProductoId: g.Key, Cantidad: Math.Round(g.Sum(x => x.Cantidad), 3)))
            .ToList();

        if (lineasAgrupadas.Count == 0)
            return (false, "Las cantidades de los productos en el carrito deben ser mayores a cero.", null, null);

        // 2. Sincronizar el Singleton de Configuración si aún no se cargó desde BD
        if (!_configSingleton.InicializadoDesdeBd)
        {
            var cfgBd = await _ventaRepository.ObtenerConfiguracionTiendaAsync();
            _configSingleton.SincronizarDesdeEntidad(cfgBd);
        }

        // 3. Calcular el importe real de la venta desde los precios oficiales en BD
        decimal totalCalculado = 0m;
        foreach (var item in lineasAgrupadas)
        {
            var prod = await _productoRepository.ObtenerPorIdAsync(item.ProductoId);
            if (prod == null)
                return (false, $"El producto seleccionado (#{item.ProductoId}) ya no está disponible.", null, null);

            if (prod.Stock < item.Cantidad)
                return (false, $"Stock insuficiente para '{prod.Nombre}'. Disponible: {prod.Stock:0.###} {prod.UnidadMedida}.", null, null);

            totalCalculado += Math.Round(prod.PrecioVenta * item.Cantidad, 2, MidpointRounding.AwayFromZero);
        }

        totalCalculado = Math.Round(totalCalculado, 2, MidpointRounding.AwayFromZero);

        // 4. Ejecutar el Patrón Strategy para validar y procesar el medio de pago (Efectivo, Yape, Plin, Tarjeta)
        ResultadoPagoStrategy resultadoPago;
        try
        {
            var montoRecibido = solicitud.MetodoPago.Equals("Efectivo", StringComparison.OrdinalIgnoreCase) && solicitud.MontoRecibido <= 0
                ? totalCalculado
                : solicitud.MontoRecibido;

            resultadoPago = _pagoStrategyContext.EjecutarCobro(
                solicitud.MetodoPago,
                totalCalculado,
                montoRecibido,
                solicitud.CodigoReferenciaPago);
        }
        catch (InvalidOperationException ex)
        {
            return (false, ex.Message, null, null);
        }

        if (!resultadoPago.EsValido)
            return (false, resultadoPago.MensajeError, null, resultadoPago);

        // 5. Ejecutar la transacción ACID en el repositorio (Venta + DetalleVentas + Descuento Stock + Kardex SALIDA)
        var (exito, mensaje, ventaEntidad, nuevoCorrelativo) = await _ventaRepository.RegistrarVentaTransaccionalAsync(
            solicitud.ClienteId,
            usuarioId,
            resultadoPago.MetodoPagoNormalizado,
            _configSingleton.SerieBoleta,
            _configSingleton.TasaIgv,
            lineasAgrupadas);

        if (!exito || ventaEntidad == null)
            return (false, mensaje, null, resultadoPago);

        // 6. Actualizar el correlativo en la instancia única del Patrón Singleton
        _configSingleton.ActualizarCorrelativo(nuevoCorrelativo);

        return (true, $"{mensaje} · {resultadoPago.MensajeResumen}", MapearADto(ventaEntidad), resultadoPago);
    }

    private static VentaDto MapearADto(Venta v) => new()
    {
        Id = v.Id,
        NumeroBoleta = v.NumeroBoleta,
        Fecha = v.Fecha,
        ClienteId = v.ClienteId,
        ClienteNombre = v.Cliente?.NombreCompleto ?? "Clientes Varios / Público General",
        ClienteTipoDoc = v.Cliente?.TipoDoc ?? "DNI",
        ClienteDocumento = v.Cliente?.Documento ?? "00000000",
        ClienteDireccion = v.Cliente?.Direccion,
        UsuarioId = v.UsuarioId,
        CajeroNombre = v.Usuario?.Nombre ?? "Cajero de Turno",
        MetodoPago = v.MetodoPago,
        Subtotal = v.Subtotal,
        Igv = v.Igv,
        Total = v.Total,
        Detalles = v.Detalles.Select(d => new DetalleVentaDto
        {
            Id = d.Id,
            ProductoId = d.ProductoId,
            NombreProducto = d.NombreProducto,
            Cantidad = d.Cantidad,
            PrecioUnitario = d.PrecioUnitario,
            CostoUnitario = d.CostoUnitario,
            Subtotal = d.Subtotal
        }).ToList()
    };
}
