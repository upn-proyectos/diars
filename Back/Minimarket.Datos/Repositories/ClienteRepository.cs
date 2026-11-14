using Microsoft.EntityFrameworkCore;
using Minimarket.Datos.Context;
using Minimarket.Datos.Entities;

namespace Minimarket.Datos.Repositories;

public class ClienteRepository : IClienteRepository
{
    private readonly MinimarketDbContext _context;

    public ClienteRepository(MinimarketDbContext context)
    {
        _context = context;
    }

    public async Task<List<Cliente>> ListarActivosAsync()
    {
        return await _context.Clientes
            .Include(c => c.Ventas)
            .Where(c => c.Activo)
            .OrderBy(c => c.Documento == "00000000" ? 0 : 1)
            .ThenBy(c => c.NombreCompleto)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Cliente?> ObtenerPorIdAsync(int id)
    {
        return await _context.Clientes
            .Include(c => c.Ventas)
            .FirstOrDefaultAsync(c => c.Id == id && c.Activo);
    }

    public async Task<Cliente?> ObtenerPorDocumentoAsync(string documento)
    {
        if (string.IsNullOrWhiteSpace(documento))
            return null;

        var doc = documento.Trim();
        return await _context.Clientes
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Activo && c.Documento == doc);
    }

    public async Task<Cliente> ObtenerClientePublicoGeneralAsync()
    {
        var publico = await _context.Clientes
            .FirstOrDefaultAsync(c => c.Documento == "00000000");

        if (publico != null)
        {
            if (!publico.Activo)
            {
                publico.Activo = true;
                await _context.SaveChangesAsync();
            }
            return publico;
        }

        var nuevo = new Cliente
        {
            TipoDoc = "DNI",
            Documento = "00000000",
            NombreCompleto = "Clientes Varios / Público General",
            Telefono = "-",
            Direccion = "-",
            Activo = true
        };
        _context.Clientes.Add(nuevo);
        await _context.SaveChangesAsync();
        return nuevo;
    }

    public async Task<bool> ExisteDocumentoAsync(string documento, int? excluirId = null)
    {
        if (string.IsNullOrWhiteSpace(documento))
            return false;

        var doc = documento.Trim();
        return await _context.Clientes.AnyAsync(c =>
            c.Activo &&
            c.Documento == doc &&
            (!excluirId.HasValue || c.Id != excluirId.Value));
    }

    public async Task<Cliente> CrearAsync(Cliente cliente)
    {
        var inactivo = await _context.Clientes.FirstOrDefaultAsync(c =>
            !c.Activo && c.Documento == cliente.Documento.Trim());

        if (inactivo != null)
        {
            inactivo.Activo = true;
            inactivo.TipoDoc = cliente.TipoDoc;
            inactivo.NombreCompleto = cliente.NombreCompleto;
            inactivo.Telefono = cliente.Telefono;
            inactivo.Direccion = cliente.Direccion;
            _context.Clientes.Update(inactivo);
            await _context.SaveChangesAsync();
            return inactivo;
        }

        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();
        return cliente;
    }

    public async Task ActualizarAsync(Cliente cliente)
    {
        _context.Clientes.Update(cliente);
        await _context.SaveChangesAsync();
    }

    public async Task<(bool Exito, string Mensaje)> EliminarLogicoAsync(int id)
    {
        var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Id == id && c.Activo);
        if (cliente == null)
            return (false, "No se encontró el cliente solicitado.");

        if (cliente.Documento == "00000000")
            return (false, "El registro 'Clientes Varios / Público General' es del sistema y no puede desactivarse.");

        cliente.Activo = false;
        _context.Clientes.Update(cliente);
        await _context.SaveChangesAsync();
        return (true, $"Cliente '{cliente.NombreCompleto}' retirado del directorio activo.");
    }
}
