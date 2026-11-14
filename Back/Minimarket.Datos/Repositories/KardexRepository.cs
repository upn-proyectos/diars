using Microsoft.EntityFrameworkCore;
using Minimarket.Datos.Context;
using Minimarket.Datos.Entities;

namespace Minimarket.Datos.Repositories;

public class KardexRepository : IKardexRepository
{
    private readonly MinimarketDbContext _context;

    public KardexRepository(MinimarketDbContext context)
    {
        _context = context;
    }

    public async Task<List<Kardex>> ListarMovimientosAsync(
        int? productoId = null,
        DateTime? desde = null,
        DateTime? hasta = null,
        string? tipoMovimiento = null,
        int limite = 300)
    {
        var query = _context.Kardex
            .Include(k => k.Producto)
            .Include(k => k.Usuario)
            .AsNoTracking()
            .AsQueryable();

        if (productoId.HasValue && productoId.Value > 0)
        {
            query = query.Where(k => k.ProductoId == productoId.Value);
        }

        if (desde.HasValue)
        {
            var inicio = desde.Value.Date;
            query = query.Where(k => k.Fecha >= inicio);
        }

        if (hasta.HasValue)
        {
            var finExclusivo = hasta.Value.Date.AddDays(1);
            query = query.Where(k => k.Fecha < finExclusivo);
        }

        if (!string.IsNullOrWhiteSpace(tipoMovimiento) &&
            !tipoMovimiento.Equals("TODOS", StringComparison.OrdinalIgnoreCase))
        {
            var tipoNormalizado = tipoMovimiento.Trim().ToUpperInvariant();
            query = query.Where(k => k.TipoMovimiento.ToUpper() == tipoNormalizado);
        }

        return await query
            .OrderByDescending(k => k.Fecha)
            .ThenByDescending(k => k.Id)
            .Take(limite)
            .ToListAsync();
    }
}
