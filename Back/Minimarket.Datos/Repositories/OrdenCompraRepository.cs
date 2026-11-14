using System.Data;
using Microsoft.EntityFrameworkCore;
using Minimarket.Datos.Context;
using Minimarket.Datos.Entities;

namespace Minimarket.Datos.Repositories;

public class OrdenCompraRepository : IOrdenCompraRepository
{
    private readonly MinimarketDbContext _context;
    private static bool _esquemaVerificado;

    public OrdenCompraRepository(MinimarketDbContext context)
    {
        _context = context;
    }

    private async Task AsegurarEsquemaYSemillaAsync()
    {
        if (_esquemaVerificado)
            return;

        await _context.Database.ExecuteSqlRawAsync(@"
IF OBJECT_ID('dbo.OrdenesCompra', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.OrdenesCompra (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        NumeroOrden NVARCHAR(30) NOT NULL UNIQUE,
        ProveedorId INT NOT NULL FOREIGN KEY REFERENCES dbo.Proveedores(Id),
        UsuarioId INT NOT NULL FOREIGN KEY REFERENCES dbo.Usuarios(Id),
        FechaEmision DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
        FechaRecepcion DATETIME2 NULL,
        Estado NVARCHAR(20) NOT NULL DEFAULT N'Pendiente',
        Total DECIMAL(18,2) NOT NULL DEFAULT 0,
        Observaciones NVARCHAR(250) NULL
    );
END;

IF OBJECT_ID('dbo.DetallesOrden', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.DetallesOrden (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        OrdenCompraId INT NOT NULL FOREIGN KEY REFERENCES dbo.OrdenesCompra(Id) ON DELETE CASCADE,
        ProductoId INT NOT NULL FOREIGN KEY REFERENCES dbo.Productos(Id),
        NombreProducto NVARCHAR(150) NOT NULL,
        Cantidad DECIMAL(10,3) NOT NULL,
        CantidadRecibida DECIMAL(10,3) NOT NULL DEFAULT 0,
        CostoUnitario DECIMAL(18,2) NOT NULL,
        Subtotal DECIMAL(18,2) NOT NULL
    );
END;");

        if (!await _context.OrdenesCompra.AnyAsync())
        {
            var proveedores = await _context.Proveedores.Where(p => p.Activo).OrderBy(p => p.Id).Take(3).ToListAsync();
            var productos = await _context.Productos.Where(p => p.Activo).OrderBy(p => p.Id).Take(6).ToListAsync();
            var usuario = await _context.Usuarios.OrderBy(u => u.Id).FirstOrDefaultAsync();

            if (proveedores.Count >= 1 && productos.Count >= 2 && usuario != null)
            {
                var prov1 = proveedores[0];
                var prov2 = proveedores.Count > 1 ? proveedores[1] : proveedores[0];
                var p1 = productos[0];
                var p2 = productos[1];
                var p3 = productos.Count > 2 ? productos[2] : productos[0];
                var p4 = productos.Count > 3 ? productos[3] : productos[1];

                var orden1 = new OrdenCompra
                {
                    NumeroOrden = "OC-00001",
                    ProveedorId = prov1.Id,
                    UsuarioId = usuario.Id,
                    FechaEmision = DateTime.Now.AddDays(-5),
                    FechaRecepcion = DateTime.Now.AddDays(-4),
                    Estado = "Recibida",
                    Observaciones = "Reposición semanal de abarrotes principales",
                    Detalles = new List<DetalleOrden>
                    {
                        new()
                        {
                            ProductoId = p1.Id,
                            NombreProducto = p1.Nombre,
                            Cantidad = 20,
                            CantidadRecibida = 20,
                            CostoUnitario = p1.CostoUnitario,
                            Subtotal = Math.Round(20 * p1.CostoUnitario, 2)
                        },
                        new()
                        {
                            ProductoId = p2.Id,
                            NombreProducto = p2.Nombre,
                            Cantidad = 15,
                            CantidadRecibida = 15,
                            CostoUnitario = p2.CostoUnitario,
                            Subtotal = Math.Round(15 * p2.CostoUnitario, 2)
                        }
                    }
                };
                orden1.Total = orden1.Detalles.Sum(d => d.Subtotal);

                var orden2 = new OrdenCompra
                {
                    NumeroOrden = "OC-00002",
                    ProveedorId = prov2.Id,
                    UsuarioId = usuario.Id,
                    FechaEmision = DateTime.Now.AddDays(-1),
                    FechaRecepcion = null,
                    Estado = "Pendiente",
                    Observaciones = "Pedido programado pendiente de recepción en almacén",
                    Detalles = new List<DetalleOrden>
                    {
                        new()
                        {
                            ProductoId = p3.Id,
                            NombreProducto = p3.Nombre,
                            Cantidad = 24,
                            CantidadRecibida = 0,
                            CostoUnitario = p3.CostoUnitario,
                            Subtotal = Math.Round(24 * p3.CostoUnitario, 2)
                        },
                        new()
                        {
                            ProductoId = p4.Id,
                            NombreProducto = p4.Nombre,
                            Cantidad = 12,
                            CantidadRecibida = 0,
                            CostoUnitario = p4.CostoUnitario,
                            Subtotal = Math.Round(12 * p4.CostoUnitario, 2)
                        }
                    }
                };
                orden2.Total = orden2.Detalles.Sum(d => d.Subtotal);

                _context.OrdenesCompra.AddRange(orden1, orden2);

                if (!await _context.Kardex.AnyAsync())
                {
                    _context.Kardex.AddRange(
                        new Kardex
                        {
                            ProductoId = p1.Id,
                            Fecha = DateTime.Now.AddDays(-4),
                            TipoMovimiento = "ENTRADA",
                            Cantidad = 20,
                            Saldo = p1.Stock,
                            Motivo = $"Recepción Orden OC-00001 ({prov1.RazonSocial})",
                            UsuarioId = usuario.Id
                        },
                        new Kardex
                        {
                            ProductoId = p2.Id,
                            Fecha = DateTime.Now.AddDays(-4),
                            TipoMovimiento = "ENTRADA",
                            Cantidad = 15,
                            Saldo = p2.Stock,
                            Motivo = $"Recepción Orden OC-00001 ({prov1.RazonSocial})",
                            UsuarioId = usuario.Id
                        });
                }

                await _context.SaveChangesAsync();
            }
        }

        _esquemaVerificado = true;
    }

    public async Task<List<OrdenCompra>> ListarOrdenesAsync()
    {
        await AsegurarEsquemaYSemillaAsync();

        return await _context.OrdenesCompra
            .Include(o => o.Proveedor)
            .Include(o => o.Usuario)
            .Include(o => o.Detalles)
            .OrderByDescending(o => o.FechaEmision)
            .ThenByDescending(o => o.Id)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<OrdenCompra?> ObtenerPorIdAsync(int id)
    {
        await AsegurarEsquemaYSemillaAsync();

        return await _context.OrdenesCompra
            .Include(o => o.Proveedor)
            .Include(o => o.Usuario)
            .Include(o => o.Detalles)
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<(bool Exito, string Mensaje, OrdenCompra? OrdenCreada)> CrearOrdenAsync(
        int proveedorId,
        int usuarioId,
        string? observaciones,
        List<(int ProductoId, decimal Cantidad, decimal CostoUnitario)> items)
    {
        await AsegurarEsquemaYSemillaAsync();

        var proveedor = await _context.Proveedores.FirstOrDefaultAsync(p => p.Id == proveedorId && p.Activo);
        if (proveedor == null)
            return (false, "El proveedor seleccionado no existe o se encuentra inactivo.", null);

        var usuarioExiste = await _context.Usuarios.AnyAsync(u => u.Id == usuarioId);
        if (!usuarioExiste)
        {
            var primerUsuario = await _context.Usuarios.OrderBy(u => u.Id).FirstOrDefaultAsync();
            if (primerUsuario == null)
                return (false, "No existe un usuario activo para registrar la orden.", null);
            usuarioId = primerUsuario.Id;
        }

        var ultimoId = await _context.OrdenesCompra.MaxAsync(o => (int?)o.Id) ?? 0;
        var correlativo = ultimoId + 1;
        string numeroOrden;
        do
        {
            numeroOrden = $"OC-{correlativo:D5}";
            correlativo++;
        }
        while (await _context.OrdenesCompra.AnyAsync(o => o.NumeroOrden == numeroOrden));

        var orden = new OrdenCompra
        {
            NumeroOrden = numeroOrden,
            ProveedorId = proveedor.Id,
            UsuarioId = usuarioId,
            FechaEmision = DateTime.Now,
            Estado = "Pendiente",
            Observaciones = string.IsNullOrWhiteSpace(observaciones) ? null : observaciones.Trim()
        };

        foreach (var item in items)
        {
            var prod = await _context.Productos.FirstOrDefaultAsync(p => p.Id == item.ProductoId && p.Activo);
            if (prod == null)
                return (false, $"El producto con ID #{item.ProductoId} no existe en el catálogo.", null);

            var costo = item.CostoUnitario > 0 ? item.CostoUnitario : prod.CostoUnitario;
            var subtotal = Math.Round(item.Cantidad * costo, 2, MidpointRounding.AwayFromZero);

            orden.Detalles.Add(new DetalleOrden
            {
                ProductoId = prod.Id,
                NombreProducto = prod.Nombre,
                Cantidad = item.Cantidad,
                CantidadRecibida = 0,
                CostoUnitario = costo,
                Subtotal = subtotal
            });

            orden.Total += subtotal;
        }

        orden.Total = Math.Round(orden.Total, 2, MidpointRounding.AwayFromZero);
        _context.OrdenesCompra.Add(orden);
        await _context.SaveChangesAsync();

        var creada = await ObtenerPorIdAsync(orden.Id);
        return (true, $"Orden de compra {orden.NumeroOrden} generada exitosamente por S/ {orden.Total:0.00}.", creada);
    }

    public async Task<(bool Exito, string Mensaje)> ConfirmarRecepcionTransaccionalAsync(
        int ordenId,
        int usuarioId,
        Dictionary<int, decimal>? cantidadesRecibidas = null)
    {
        await AsegurarEsquemaYSemillaAsync();
        await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);

        try
        {
            var orden = await _context.OrdenesCompra
                .Include(o => o.Proveedor)
                .Include(o => o.Detalles)
                .FirstOrDefaultAsync(o => o.Id == ordenId);

            if (orden == null)
                return (false, "No se encontró la orden de compra solicitada.");

            if (!orden.Estado.Equals("Pendiente", StringComparison.OrdinalIgnoreCase))
                return (false, $"La orden {orden.NumeroOrden} ya fue procesada (Estado actual: {orden.Estado}).");

            var usuarioExiste = await _context.Usuarios.AnyAsync(u => u.Id == usuarioId);
            if (!usuarioExiste)
            {
                var primerUsuario = await _context.Usuarios.OrderBy(u => u.Id).FirstOrDefaultAsync();
                if (primerUsuario != null)
                    usuarioId = primerUsuario.Id;
            }

            decimal nuevoTotal = 0m;
            int productosIngresados = 0;

            foreach (var detalle in orden.Detalles.OrderBy(d => d.ProductoId))
            {
                var cantRecibida = detalle.Cantidad;
                if (cantidadesRecibidas != null && cantidadesRecibidas.TryGetValue(detalle.ProductoId, out var cantAjustada))
                {
                    cantRecibida = Math.Max(0, cantAjustada);
                }

                detalle.CantidadRecibida = cantRecibida;
                detalle.Subtotal = Math.Round(cantRecibida * detalle.CostoUnitario, 2, MidpointRounding.AwayFromZero);
                nuevoTotal += detalle.Subtotal;

                if (cantRecibida <= 0)
                    continue;

                var producto = await _context.Productos.FirstOrDefaultAsync(p => p.Id == detalle.ProductoId);
                if (producto == null)
                    continue;

                producto.Stock += cantRecibida;
                if (detalle.CostoUnitario > 0)
                {
                    producto.CostoUnitario = detalle.CostoUnitario;
                }

                _context.Kardex.Add(new Kardex
                {
                    ProductoId = producto.Id,
                    Fecha = DateTime.Now,
                    TipoMovimiento = "ENTRADA",
                    Cantidad = cantRecibida,
                    Saldo = producto.Stock,
                    Motivo = $"Recepción Orden {orden.NumeroOrden} ({orden.Proveedor?.RazonSocial ?? "Proveedor"})",
                    UsuarioId = usuarioId
                });

                productosIngresados++;
            }

            if (productosIngresados == 0)
            {
                await transaction.RollbackAsync();
                return (false, "Debe ingresar una cantidad recibida mayor a cero en al menos un producto.");
            }

            orden.Total = Math.Round(nuevoTotal, 2, MidpointRounding.AwayFromZero);
            orden.Estado = "Recibida";
            orden.FechaRecepcion = DateTime.Now;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return (true, $"Recepción de la orden {orden.NumeroOrden} confirmada. Stock y Kardex actualizados en SQL Server.");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return (false, $"Error transaccional al confirmar la recepción: {ex.Message}");
        }
    }

    public async Task<(bool Exito, string Mensaje)> AnularOrdenAsync(int ordenId, int usuarioId)
    {
        await AsegurarEsquemaYSemillaAsync();

        var orden = await _context.OrdenesCompra.FirstOrDefaultAsync(o => o.Id == ordenId);
        if (orden == null)
            return (false, "No se encontró la orden de compra solicitada.");

        if (!orden.Estado.Equals("Pendiente", StringComparison.OrdinalIgnoreCase))
            return (false, $"Solo se pueden anular órdenes en estado Pendiente (Estado actual: {orden.Estado}).");

        orden.Estado = "Anulada";
        await _context.SaveChangesAsync();

        return (true, $"Orden de compra {orden.NumeroOrden} anulada correctamente.");
    }
}
