using Microsoft.EntityFrameworkCore;
using Minimarket.Datos.Context;
using Minimarket.Datos.Entities;

namespace Minimarket.Datos.Repositories;

public class ProveedorRepository : IProveedorRepository
{
    private readonly MinimarketDbContext _context;

    public ProveedorRepository(MinimarketDbContext context)
    {
        _context = context;
    }

    public async Task<List<Proveedor>> ListarActivosAsync()
    {
        return await _context.Proveedores
            .Include(p => p.Productos.Where(pr => pr.Activo))
            .Where(p => p.Activo)
            .OrderBy(p => p.RazonSocial)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Proveedor?> ObtenerPorIdAsync(int id)
    {
        return await _context.Proveedores
            .Include(p => p.Productos.Where(pr => pr.Activo))
            .FirstOrDefaultAsync(p => p.Id == id && p.Activo);
    }

    public async Task<bool> ExisteRucAsync(string ruc, int? excluirId = null)
    {
        if (string.IsNullOrWhiteSpace(ruc))
            return false;

        var rucNormalizado = ruc.Trim();
        return await _context.Proveedores.AnyAsync(p =>
            p.Activo &&
            p.Ruc == rucNormalizado &&
            (!excluirId.HasValue || p.Id != excluirId.Value));
    }

    public async Task<Proveedor> CrearAsync(Proveedor proveedor)
    {
        // Si existía un proveedor inactivo con el mismo RUC, lo reactivamos y actualizamos
        var inactivo = await _context.Proveedores.FirstOrDefaultAsync(p =>
            !p.Activo && p.Ruc == proveedor.Ruc.Trim());

        if (inactivo != null)
        {
            inactivo.Activo = true;
            inactivo.RazonSocial = proveedor.RazonSocial;
            inactivo.NombreComercial = proveedor.NombreComercial;
            inactivo.Contacto = proveedor.Contacto;
            inactivo.Telefono = proveedor.Telefono;
            inactivo.Email = proveedor.Email;
            inactivo.Direccion = proveedor.Direccion;
            _context.Proveedores.Update(inactivo);
            await _context.SaveChangesAsync();
            return inactivo;
        }

        _context.Proveedores.Add(proveedor);
        await _context.SaveChangesAsync();
        return proveedor;
    }

    public async Task ActualizarAsync(Proveedor proveedor)
    {
        _context.Proveedores.Update(proveedor);
        await _context.SaveChangesAsync();
    }

    public async Task<(bool Exito, string Mensaje)> EliminarLogicoAsync(int id)
    {
        var proveedor = await _context.Proveedores.FirstOrDefaultAsync(p => p.Id == id && p.Activo);
        if (proveedor == null)
            return (false, "No se encontró el proveedor solicitado.");

        proveedor.Activo = false;
        _context.Proveedores.Update(proveedor);
        await _context.SaveChangesAsync();
        return (true, $"Proveedor '{proveedor.NombreComercial ?? proveedor.RazonSocial}' retirado del directorio activo.");
    }
}
