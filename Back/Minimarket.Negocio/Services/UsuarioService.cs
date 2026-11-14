using Minimarket.Datos.Entities;
using Minimarket.Datos.Repositories;
using Minimarket.Negocio.DTOs;
using Minimarket.Negocio.Interfaces;

namespace Minimarket.Negocio.Services;

/// <summary>
/// Capa de Lógica de Negocio para la gestión de Usuarios y Roles (CU-02 / RF-05)
/// con hash seguro PBKDF2-SHA256 con sal (RNF-05 / CAR-15).
/// </summary>
public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _usuarioRepository;

    public UsuarioService(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    public async Task<List<UsuarioDto>> ListarUsuariosAsync()
    {
        var usuarios = await _usuarioRepository.ListarTodosAsync();
        return usuarios.Select(MapearADto).ToList();
    }

    public async Task<List<RolDto>> ListarRolesAsync()
    {
        var roles = await _usuarioRepository.ListarRolesAsync();
        return roles.Select(r => new RolDto
        {
            Id = r.Id,
            Nombre = r.Nombre,
            Descripcion = r.Descripcion
        }).ToList();
    }

    public async Task<UsuarioDto?> ObtenerPorIdAsync(int id)
    {
        var u = await _usuarioRepository.ObtenerPorIdAsync(id);
        return u == null ? null : MapearADto(u);
    }

    public async Task<(bool Exito, string Mensaje)> GuardarUsuarioAsync(UsuarioGuardarDto dto, int usuarioResponsableId)
    {
        if (string.IsNullOrWhiteSpace(dto.Nombre) || dto.Nombre.Trim().Length < 3)
            return (false, "El nombre completo del trabajador debe tener al menos 3 caracteres.");

        if (string.IsNullOrWhiteSpace(dto.UsuarioLogin) || dto.UsuarioLogin.Trim().Length < 3)
            return (false, "El nombre de usuario (login) debe tener al menos 3 caracteres.");

        var loginLimpio = dto.UsuarioLogin.Trim();
        if (loginLimpio.Contains(' '))
            return (false, "El nombre de usuario no debe contener espacios en blanco.");

        var rol = await _usuarioRepository.ObtenerRolPorIdAsync(dto.RolId);
        if (rol == null || !new[] { "Administrador", "Almacenero", "Cajero" }.Contains(rol.Nombre))
            return (false, "Seleccione uno de los 3 roles oficiales (Administrador, Almacenero o Cajero).");

        if (await _usuarioRepository.ExisteLoginAsync(loginLimpio, dto.Id))
            return (false, $"El nombre de usuario '{loginLimpio}' ya se encuentra registrado.");

        if (dto.Id.HasValue && dto.Id.Value > 0)
        {
            var existente = await _usuarioRepository.ObtenerPorIdAsync(dto.Id.Value);
            if (existente == null)
                return (false, "No se encontró la cuenta de usuario que intenta modificar.");

            // Impedir que el administrador en sesión se desactive o se quite el rol de Administrador a sí mismo
            if (existente.Id == usuarioResponsableId && (!dto.Activo || rol.Nombre != "Administrador"))
                return (false, "Por seguridad, no puede desactivar ni quitar el rol de Administrador a su propia cuenta en sesión.");

            // Garantizar que siempre quede al menos 1 Administrador activo
            if (existente.Rol?.Nombre == "Administrador" && (!dto.Activo || rol.Nombre != "Administrador"))
            {
                var otrosAdmins = await _usuarioRepository.ContarAdministradoresActivosAsync(existente.Id);
                if (otrosAdmins == 0)
                    return (false, "El sistema debe conservar al menos un Administrador activo.");
            }

            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                var pass = dto.Password.Trim();
                if (pass.Length < 6 || pass.Length > 128)
                    return (false, "La nueva contraseña debe tener entre 6 y 128 caracteres.");

                // Si es una de las 3 cuentas demo y ponen 'admin123', permitimos conservarla o hashearla;
                // para cualquier otra clave aplicamos PBKDF2-SHA256 con sal (RNF-05)
                existente.PasswordHash = PasswordHasher.Hashear(pass);
            }

            existente.Nombre = dto.Nombre.Trim();
            existente.UsuarioLogin = loginLimpio;
            existente.RolId = rol.Id;
            existente.Activo = dto.Activo;

            await _usuarioRepository.ActualizarAsync(existente);
            return (true, $"Cuenta de '{existente.Nombre}' ({rol.Nombre}) actualizada correctamente.");
        }
        else
        {
            if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Trim().Length < 6)
                return (false, "Para crear un nuevo trabajador debe ingresar una contraseña de al menos 6 caracteres.");

            var nuevo = new Usuario
            {
                Nombre = dto.Nombre.Trim(),
                UsuarioLogin = loginLimpio,
                PasswordHash = PasswordHasher.Hashear(dto.Password.Trim()),
                RolId = rol.Id,
                Activo = dto.Activo,
                FechaCreacion = DateTime.Now
            };

            await _usuarioRepository.CrearAsync(nuevo);
            return (true, $"Trabajador '{nuevo.Nombre}' registrado con rol '{rol.Nombre}' y contraseña protegida con PBKDF2-SHA256.");
        }
    }

    private static UsuarioDto MapearADto(Usuario u) => new()
    {
        Id = u.Id,
        Nombre = u.Nombre,
        UsuarioLogin = u.UsuarioLogin,
        RolId = u.RolId,
        RolNombre = u.Rol?.Nombre ?? "Cajero",
        Activo = u.Activo,
        FechaCreacion = u.FechaCreacion,
        UsaHashSeguro = PasswordHasher.EsHashSeguro(u.PasswordHash),
        TotalVentasRegistradas = u.Ventas?.Count ?? 0
    };
}
