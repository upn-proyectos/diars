using System.Data;
using Microsoft.EntityFrameworkCore;
using Minimarket.Datos.Context;
using Minimarket.Datos.Entities;

namespace Minimarket.Datos.Repositories;

public class VentaRepository : IVentaRepository
{
    private readonly MinimarketDbContext _context;

    public VentaRepository(MinimarketDbContext context)
    {
        _context = context;
    }

    public async Task<ConfiguracionTienda> ObtenerConfiguracionTiendaAsync()
    {
        var cfg = await _context.Configuracion
            .OrderBy(c => c.Id)
            .FirstOrDefaultAsync();

        if (cfg != null)
            return cfg;

        cfg = new ConfiguracionTienda
        {
            NombreComercio = "Minimarket Chininin SAC",
            Ruc = "20601234567",
            Direccion = "Av. América Norte 1234 - Trujillo",
            SerieBoleta = "B001",
            CorrelativoBoleta = 1,
            TasaIgv = 0.18m
        };
        _context.Configuracion.Add(cfg);
        await _context.SaveChangesAsync();
        return cfg;
    }

    public async Task<List<Venta>> ListarRecientesAsync(int limite = 50, int? usuarioId = null)
    {
        var query = _context.Ventas
            .Include(v => v.Cliente)
            .Include(v => v.Usuario)
            .Include(v => v.Detalles)
            .AsNoTracking()
            .AsQueryable();

        if (usuarioId.HasValue && usuarioId.Value > 0)
        {
            query = query.Where(v => v.UsuarioId == usuarioId.Value);
        }

        return await query
            .OrderByDescending(v => v.Fecha)
            .ThenByDescending(v => v.Id)
            .Take(limite)
            .ToListAsync();
    }

    public async Task<List<Venta>> ListarPorRangoFechasAsync(DateTime desde, DateTime hasta)
    {
        var inicio = desde.Date;
        var finExclusivo = hasta.Date.AddDays(1);

        return await _context.Ventas
            .Include(v => v.Cliente)
            .Include(v => v.Usuario)
            .Include(v => v.Detalles)
            .Where(v => v.Fecha >= inicio && v.Fecha < finExclusivo)
            .OrderByDescending(v => v.Fecha)
            .ThenByDescending(v => v.Id)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Venta?> ObtenerPorIdConDetalleAsync(int id)
    {
        return await _context.Ventas
            .Include(v => v.Cliente)
            .Include(v => v.Usuario)
            .Include(v => v.Detalles)
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == id);
    }

    public async Task<(bool Exito, string Mensaje, Venta? VentaRegistrada, int NuevoCorrelativo)> RegistrarVentaTransaccionalAsync(
        int clienteId,
        int usuarioId,
        string metodoPago,
        string serieBoleta,
        decimal tasaIgv,
        List<(int ProductoId, decimal Cantidad)> lineas)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);

        try
        {
            // 1. Obtener o crear configuración bloqueando fila para correlativo único
            var cfg = await _context.Configuracion
                .FromSqlRaw("SELECT * FROM dbo.Configuracion WITH (UPDLOCK, HOLDLOCK)")
                .OrderBy(c => c.Id)
                .FirstOrDefaultAsync();

            if (cfg == null)
            {
                cfg = new ConfiguracionTienda
                {
                    NombreComercio = "Minimarket Chininin SAC",
                    Ruc = "20601234567",
                    Direccion = "Av. América Norte 1234 - Trujillo",
                    SerieBoleta = string.IsNullOrWhiteSpace(serieBoleta) ? "B001" : serieBoleta,
                    CorrelativoBoleta = 1,
                    TasaIgv = tasaIgv is > 0m and < 1m ? tasaIgv : 0.18m
                };
                _context.Configuracion.Add(cfg);
                await _context.SaveChangesAsync();
            }

            // 2. Verificar cliente (si es 0 o no existe, asignar Público General)
            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Id == clienteId && c.Activo);
            if (cliente == null)
            {
                cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Documento == "00000000");
                if (cliente == null)
                {
                    cliente = new Cliente
                    {
                        TipoDoc = "DNI",
                        Documento = "00000000",
                        NombreCompleto = "Clientes Varios / Público General",
                        Telefono = "-",
                        Direccion = "-",
                        Activo = true
                    };
                    _context.Clientes.Add(cliente);
                    await _context.SaveChangesAsync();
                }
            }

            // 3. Verificar usuario cajero/admin
            var usuarioExiste = await _context.Usuarios.AnyAsync(u => u.Id == usuarioId && u.Activo);
            if (!usuarioExiste)
            {
                var primerUsuario = await _context.Usuarios.OrderBy(u => u.Id).FirstOrDefaultAsync();
                if (primerUsuario == null)
                    return (false, "No existe un usuario activo en el sistema para registrar la venta.", null, cfg.CorrelativoBoleta);
                usuarioId = primerUsuario.Id;
            }

            // 4. Procesar cada línea del carrito en orden de ID para evitar deadlocks
            var venta = new Venta
            {
                Fecha = DateTime.Now,
                ClienteId = cliente.Id,
                UsuarioId = usuarioId,
                MetodoPago = metodoPago
            };

            var movimientosPendientes = new List<(Producto Producto, decimal CantidadVendida, decimal SaldoResultante)>();

            foreach (var item in lineas.OrderBy(l => l.ProductoId))
            {
                var producto = await _context.Productos
                    .FromSqlInterpolated($"SELECT * FROM dbo.Productos WITH (UPDLOCK, HOLDLOCK) WHERE Id = {item.ProductoId}")
                    .FirstOrDefaultAsync();

                if (producto == null || !producto.Activo)
                {
                    await transaction.RollbackAsync();
                    return (false, $"El producto con ID #{item.ProductoId} no se encuentra activo en el catálogo.", null, cfg.CorrelativoBoleta);
                }

                if (producto.UnidadMedida.Equals("UNIDAD", StringComparison.OrdinalIgnoreCase) &&
                    decimal.Truncate(item.Cantidad) != item.Cantidad)
                {
                    await transaction.RollbackAsync();
                    return (false, $"El producto '{producto.Nombre}' se comercializa por UNIDAD entera (no admite decimales).", null, cfg.CorrelativoBoleta);
                }

                if (producto.FechaVencimiento.HasValue && producto.FechaVencimiento.Value.Date < DateTime.Today)
                {
                    await transaction.RollbackAsync();
                    return (false, $"El producto '{producto.Nombre}' se encuentra vencido ({producto.FechaVencimiento:yyyy-MM-dd}) y no puede venderse.", null, cfg.CorrelativoBoleta);
                }

                if (producto.Stock < item.Cantidad)
                {
                    await transaction.RollbackAsync();
                    return (false, $"Stock insuficiente para '{producto.Nombre}'. Disponible: {producto.Stock:0.###} {producto.UnidadMedida}, solicitado: {item.Cantidad:0.###}.", null, cfg.CorrelativoBoleta);
                }

                var subtotalLinea = Math.Round(producto.PrecioVenta * item.Cantidad, 2, MidpointRounding.AwayFromZero);

                venta.Detalles.Add(new DetalleVenta
                {
                    ProductoId = producto.Id,
                    NombreProducto = producto.Nombre,
                    Cantidad = item.Cantidad,
                    PrecioUnitario = producto.PrecioVenta,
                    CostoUnitario = producto.CostoUnitario,
                    Subtotal = subtotalLinea
                });

                venta.Total += subtotalLinea;
                producto.Stock -= item.Cantidad;

                movimientosPendientes.Add((producto, item.Cantidad, producto.Stock));
            }

            // 5. Calcular desglose tributario (IGV incluido en precios de lista)
            var igvRate = cfg.TasaIgv is > 0m and < 1m ? cfg.TasaIgv : 0.18m;
            venta.Total = Math.Round(venta.Total, 2, MidpointRounding.AwayFromZero);
            venta.Subtotal = Math.Round(venta.Total / (1m + igvRate), 2, MidpointRounding.AwayFromZero);
            venta.Igv = Math.Round(venta.Total - venta.Subtotal, 2, MidpointRounding.AwayFromZero);

            // 6. Generar correlativo único de boleta
            var serie = string.IsNullOrWhiteSpace(cfg.SerieBoleta) ? "B001" : cfg.SerieBoleta.Trim().ToUpperInvariant();
            string numeroBoleta;
            do
            {
                numeroBoleta = $"{serie}-{cfg.CorrelativoBoleta:D5}";
                cfg.CorrelativoBoleta++;
            }
            while (await _context.Ventas.AnyAsync(v => v.NumeroBoleta == numeroBoleta));

            venta.NumeroBoleta = numeroBoleta;
            _context.Ventas.Add(venta);
            await _context.SaveChangesAsync();

            // 7. Registrar movimientos automáticos de SALIDA en Kardex (RF-02)
            foreach (var mov in movimientosPendientes)
            {
                _context.Kardex.Add(new Kardex
                {
                    ProductoId = mov.Producto.Id,
                    Fecha = venta.Fecha,
                    TipoMovimiento = "SALIDA",
                    Cantidad = mov.CantidadVendida,
                    Saldo = mov.SaldoResultante,
                    Motivo = $"Venta {venta.NumeroBoleta} ({venta.MetodoPago})",
                    UsuarioId = usuarioId
                });
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            var ventaCompleta = await ObtenerPorIdConDetalleAsync(venta.Id);
            return (true, $"Venta {venta.NumeroBoleta} registrada exitosamente por S/ {venta.Total:0.00}.", ventaCompleta, cfg.CorrelativoBoleta);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return (false, $"Error transaccional al procesar la venta: {ex.Message}", null, 1);
        }
    }
}
