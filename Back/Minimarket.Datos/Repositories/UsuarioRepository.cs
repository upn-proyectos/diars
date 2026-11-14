using Microsoft.EntityFrameworkCore;
using Minimarket.Datos.Context;
using Minimarket.Datos.Entities;

namespace Minimarket.Datos.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly MinimarketDbContext _context;

    public UsuarioRepository(MinimarketDbContext context)
    {
        _context = context;
    }

    public async Task<List<Usuario>> ListarTodosAsync()
    {
        return await _context.Usuarios
            .Include(u => u.Rol)
            .Include(u => u.Ventas)
            .OrderByDescending(u => u.Activo)
            .ThenBy(u => u.RolId)
            .ThenBy(u => u.Nombre)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<Rol>> ListarRolesAsync()
    {
        return await _context.Roles
            .OrderBy(r => r.Id)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Usuario?> ObtenerPorIdAsync(int id)
    {
        return await _context.Usuarios
            .Include(u => u.Rol)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<Rol?> ObtenerRolPorIdAsync(int rolId)
    {
        return await _context.Roles
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == rolId);
    }

    public async Task<bool> ExisteLoginAsync(string usuarioLogin, int? excluirId = null)
    {
        if (string.IsNullOrWhiteSpace(usuarioLogin))
            return false;

        var login = usuarioLogin.Trim().ToLower();
        return await _context.Usuarios.AnyAsync(u =>
            u.UsuarioLogin.ToLower() == login &&
            (!excluirId.HasValue || u.Id != excluirId.Value));
    }

    public async Task<int> ContarAdministradoresActivosAsync(int? excluirUsuarioId = null)
    {
        return await _context.Usuarios
            .Include(u => u.Rol)
            .CountAsync(u =>
                u.Activo &&
                u.Rol != null &&
                u.Rol.Nombre == "Administrador" &&
                (!excluirUsuarioId.HasValue || u.Id != excluirUsuarioId.Value));
    }

    public async Task<Usuario> CrearAsync(Usuario usuario)
    {
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();
        return usuario;
    }

    public async Task ActualizarAsync(Usuario usuario)
    {
        _context.Usuarios.Update(usuario);
        await _context.SaveChangesAsync();
    }
}
