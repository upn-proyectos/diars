using System.Text.RegularExpressions;
using Minimarket.Datos.Entities;
using Minimarket.Datos.Repositories;
using Minimarket.Negocio.DTOs;
using Minimarket.Negocio.Interfaces;
using Minimarket.Negocio.Patterns.Adapter;

namespace Minimarket.Negocio.Services;

/// <summary>
/// Capa de Lógica de Negocio para el Módulo de Proveedores y Abastecimiento.
/// Integra el Patrón Adapter (IValidadorDocumentoAdapter) y el Patrón Repository (IProveedorRepository).
/// </summary>
public class ProveedorService : IProveedorService
{
    private readonly IProveedorRepository _proveedorRepository;
    private readonly IValidadorDocumentoAdapter _validadorDocumentoAdapter;

    public ProveedorService(
        IProveedorRepository proveedorRepository,
        IValidadorDocumentoAdapter validadorDocumentoAdapter)
    {
        _proveedorRepository = proveedorRepository;
        _validadorDocumentoAdapter = validadorDocumentoAdapter;
    }

    public async Task<List<ProveedorDto>> ListarProveedoresAsync()
    {
        var entidades = await _proveedorRepository.ListarActivosAsync();
        return entidades.Select(MapearADto).ToList();
    }

    public async Task<ProveedorDto?> ObtenerPorIdAsync(int id)
    {
        var entidad = await _proveedorRepository.ObtenerPorIdAsync(id);
        return entidad == null ? null : MapearADto(entidad);
    }

    public async Task<(bool Exito, string Mensaje)> GuardarProveedorAsync(ProveedorGuardarDto dto)
    {
        // 1. Validar el RUC mediante el Patrón Adapter (IValidadorDocumentoAdapter)
        var validacionRuc = _validadorDocumentoAdapter.ValidarRuc(dto.Ruc);
        if (!validacionRuc.EsValido)
            return (false, validacionRuc.MensajeError);

        // 2. Validar Razón Social
        if (string.IsNullOrWhiteSpace(dto.RazonSocial) || dto.RazonSocial.Trim().Length < 3)
            return (false, "La razón social es obligatoria y debe tener al menos 3 caracteres.");

        // 3. Validar formato de correo electrónico si fue ingresado
        if (!string.IsNullOrWhiteSpace(dto.Email))
        {
            var emailLimpio = dto.Email.Trim();
            if (!Regex.IsMatch(emailLimpio, @"^[^\s@]+@[^\s@]+\.[^\s@]+$"))
                return (false, "El correo electrónico ingresado no tiene un formato válido.");
        }

        // 4. Validar unicidad del RUC en la base de datos SQL Server
        if (await _proveedorRepository.ExisteRucAsync(validacionRuc.DocumentoNormalizado, dto.Id))
            return (false, $"Ya existe un proveedor registrado con el RUC '{validacionRuc.DocumentoNormalizado}'.");

        var razonSocialLimpia = dto.RazonSocial.Trim();
        var nombreComercialLimpio = string.IsNullOrWhiteSpace(dto.NombreComercial) ? null : dto.NombreComercial.Trim();
        var contactoLimpio = string.IsNullOrWhiteSpace(dto.Contacto) ? null : dto.Contacto.Trim();
        var telefonoLimpio = string.IsNullOrWhiteSpace(dto.Telefono) ? null : dto.Telefono.Trim();
        var emailFinal = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim();
        var direccionLimpia = string.IsNullOrWhiteSpace(dto.Direccion) ? null : dto.Direccion.Trim();

        if (dto.Id.HasValue && dto.Id.Value > 0)
        {
            var existente = await _proveedorRepository.ObtenerPorIdAsync(dto.Id.Value);
            if (existente == null)
                return (false, "No se encontró el proveedor que intenta editar.");

            existente.Ruc = validacionRuc.DocumentoNormalizado;
            existente.RazonSocial = razonSocialLimpia;
            existente.NombreComercial = nombreComercialLimpio;
            existente.Contacto = contactoLimpio;
            existente.Telefono = telefonoLimpio;
            existente.Email = emailFinal;
            existente.Direccion = direccionLimpia;

            await _proveedorRepository.ActualizarAsync(existente);
            return (true, $"Proveedor '{nombreComercialLimpio ?? razonSocialLimpia}' actualizado ({validacionRuc.TipoContribuyente}).");
        }
        else
        {
            var nuevo = new Proveedor
            {
                Ruc = validacionRuc.DocumentoNormalizado,
                RazonSocial = razonSocialLimpia,
                NombreComercial = nombreComercialLimpio,
                Contacto = contactoLimpio,
                Telefono = telefonoLimpio,
                Email = emailFinal,
                Direccion = direccionLimpia,
                Activo = true
            };

            await _proveedorRepository.CrearAsync(nuevo);
            return (true, $"Proveedor '{nombreComercialLimpio ?? razonSocialLimpia}' registrado con éxito ({validacionRuc.TipoContribuyente}).");
        }
    }

    public async Task<(bool Exito, string Mensaje)> EliminarProveedorAsync(int id)
    {
        return await _proveedorRepository.EliminarLogicoAsync(id);
    }

    private ProveedorDto MapearADto(Proveedor p)
    {
        var validacion = _validadorDocumentoAdapter.ValidarRuc(p.Ruc);
        return new ProveedorDto
        {
            Id = p.Id,
            Ruc = p.Ruc,
            RazonSocial = p.RazonSocial,
            NombreComercial = p.NombreComercial,
            Contacto = p.Contacto,
            Telefono = p.Telefono,
            Email = p.Email,
            Direccion = p.Direccion,
            TipoContribuyente = validacion.EsValido ? validacion.TipoContribuyente : "RUC Registrado",
            TotalProductosAbastecidos = p.Productos?.Count ?? 0
        };
    }
}
