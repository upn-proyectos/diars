using Microsoft.EntityFrameworkCore;
using Minimarket.Datos.Context;
using Minimarket.Datos.Entities;

namespace Minimarket.Datos.Repositories;

/// <summary>
/// Implementación concreta del Patrón Repository utilizando Entity Framework Core y SQL Server.
/// </summary>
public class ProductoRepository : IProductoRepository
{
    private readonly MinimarketDbContext _context;

    public ProductoRepository(MinimarketDbContext context)
    {
        _context = context;
    }

    public async Task<List<Producto>> ListarActivosAsync()
    {
        return await _context.Productos
            .Include(p => p.Categoria)
            .Include(p => p.Proveedor)
            .Where(p => p.Activo)
            .OrderBy(p => p.Nombre)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Producto?> ObtenerPorIdAsync(int id)
    {
        return await _context.Productos
            .Include(p => p.Categoria)
            .Include(p => p.Proveedor)
            .FirstOrDefaultAsync(p => p.Id == id && p.Activo);
    }

    public async Task<bool> ExisteCodigoBarrasAsync(string codigoBarras, int? excluirId = null)
    {
        if (string.IsNullOrWhiteSpace(codigoBarras))
            return false;

        var codigoNormalizado = codigoBarras.Trim();
        return await _context.Productos.AnyAsync(p =>
            p.Activo &&
            p.CodigoBarras == codigoNormalizado &&
            (!excluirId.HasValue || p.Id != excluirId.Value));
    }

    public async Task<Producto> CrearAsync(Producto producto, int? usuarioId = null)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            // Si el producto nace con stock inicial > 0, registramos su entrada inicial en Kardex
            if (producto.Stock > 0)
            {
                var uid = usuarioId ?? await ObtenerUsuarioPorDefectoAsync();
                if (uid > 0)
                {
                    _context.Kardex.Add(new Kardex
                    {
                        ProductoId = producto.Id,
                        Fecha = DateTime.Now,
                        TipoMovimiento = "ENTRADA",
                        Cantidad = producto.Stock,
                        Saldo = producto.Stock,
                        Motivo = "Inventario inicial al registrar producto",
                        UsuarioId = uid
                    });
                    await _context.SaveChangesAsync();
                }
            }

            await transaction.CommitAsync();
            return producto;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task ActualizarAsync(Producto producto)
    {
        _context.Productos.Update(producto);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> AjustarStockAsync(int productoId, decimal nuevoStock, string motivo, int usuarioId)
    {
        var producto = await _context.Productos.FirstOrDefaultAsync(p => p.Id == productoId && p.Activo);
        if (producto == null)
            return false;

        var diferencia = nuevoStock - producto.Stock;
        if (diferencia == 0)
            return true;

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var cantidadAbsoluta = Math.Abs(diferencia);

            producto.Stock = nuevoStock;
            _context.Productos.Update(producto);

            var uid = usuarioId > 0 ? usuarioId : await ObtenerUsuarioPorDefectoAsync();
            if (uid > 0)
            {
                _context.Kardex.Add(new Kardex
                {
                    ProductoId = producto.Id,
                    Fecha = DateTime.Now,
                    TipoMovimiento = "AJUSTE",
                    Cantidad = cantidadAbsoluta,
                    Saldo = nuevoStock,
                    Motivo = $"{motivo} ({(diferencia > 0 ? "+" : "-")}{cantidadAbsoluta:0.##} {producto.UnidadMedida})",
                    UsuarioId = uid
                });
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<bool> EliminarLogicoAsync(int id)
    {
        var producto = await _context.Productos.FirstOrDefaultAsync(p => p.Id == id && p.Activo);
        if (producto == null)
            return false;

        producto.Activo = false;
        _context.Productos.Update(producto);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<Categoria>> ListarCategoriasActivasAsync()
    {
        return await _context.Categorias
            .Include(c => c.Productos.Where(p => p.Activo))
            .Where(c => c.Activo)
            .OrderBy(c => c.Nombre)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Categoria?> ObtenerCategoriaPorIdAsync(int id)
    {
        return await _context.Categorias
            .Include(c => c.Productos.Where(p => p.Activo))
            .FirstOrDefaultAsync(c => c.Id == id && c.Activo);
    }

    public async Task<bool> ExisteNombreCategoriaAsync(string nombre, int? excluirId = null)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            return false;

        var nombreLimpio = nombre.Trim().ToLower();
        return await _context.Categorias.AnyAsync(c =>
            c.Activo &&
            c.Nombre.ToLower() == nombreLimpio &&
            (!excluirId.HasValue || c.Id != excluirId.Value));
    }

    public async Task<Categoria> CrearCategoriaAsync(Categoria categoria)
    {
        // Si existía una categoría inactiva con el mismo nombre, la reactivamos y actualizamos
        var inactiva = await _context.Categorias.FirstOrDefaultAsync(c =>
            !c.Activo && c.Nombre.ToLower() == categoria.Nombre.Trim().ToLower());

        if (inactiva != null)
        {
            inactiva.Activo = true;
            inactiva.Descripcion = categoria.Descripcion;
            _context.Categorias.Update(inactiva);
            await _context.SaveChangesAsync();
            return inactiva;
        }

        _context.Categorias.Add(categoria);
        await _context.SaveChangesAsync();
        return categoria;
    }

    public async Task ActualizarCategoriaAsync(Categoria categoria)
    {
        _context.Categorias.Update(categoria);
        await _context.SaveChangesAsync();
    }

    public async Task<(bool Exito, string Mensaje)> EliminarCategoriaLogicoAsync(int id)
    {
        var categoria = await _context.Categorias
            .Include(c => c.Productos.Where(p => p.Activo))
            .FirstOrDefaultAsync(c => c.Id == id && c.Activo);

        if (categoria == null)
            return (false, "No se encontró la categoría solicitada.");

        if (categoria.Productos.Any())
            return (false, $"No se puede eliminar la categoría '{categoria.Nombre}' porque tiene {categoria.Productos.Count} producto(s) activo(s) asignado(s).");

        categoria.Activo = false;
        _context.Categorias.Update(categoria);
        await _context.SaveChangesAsync();
        return (true, $"Categoría '{categoria.Nombre}' eliminada correctamente.");
    }

    public async Task<List<Proveedor>> ListarProveedoresActivosAsync()
    {
        return await _context.Proveedores
            .Where(pr => pr.Activo)
            .OrderBy(pr => pr.RazonSocial)
            .AsNoTracking()
            .ToListAsync();
    }

    private async Task<int> ObtenerUsuarioPorDefectoAsync()
    {
        var admin = await _context.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.Activo);
        return admin?.Id ?? 0;
    }
}
