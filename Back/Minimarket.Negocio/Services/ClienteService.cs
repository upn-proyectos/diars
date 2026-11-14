using Minimarket.Datos.Entities;
using Minimarket.Datos.Repositories;
using Minimarket.Negocio.DTOs;
using Minimarket.Negocio.Interfaces;
using Minimarket.Negocio.Patterns.Adapter;

namespace Minimarket.Negocio.Services;

/// <summary>
/// Capa de Lógica de Negocio para Clientes.
/// Reutiliza el Patrón Adapter (IValidadorDocumentoAdapter) para validar DNI (8 dígitos) y RUC (11 dígitos).
/// </summary>
public class ClienteService : IClienteService
{
    private readonly IClienteRepository _clienteRepository;
    private readonly IValidadorDocumentoAdapter _validadorAdapter;

    public ClienteService(
        IClienteRepository clienteRepository,
        IValidadorDocumentoAdapter validadorAdapter)
    {
        _clienteRepository = clienteRepository;
        _validadorAdapter = validadorAdapter;
    }

    public async Task<List<ClienteDto>> ListarClientesAsync()
    {
        await _clienteRepository.ObtenerClientePublicoGeneralAsync();
        var entidades = await _clienteRepository.ListarActivosAsync();
        return entidades.Select(MapearADto).ToList();
    }

    public async Task<ClienteDto?> ObtenerPorIdAsync(int id)
    {
        var entidad = await _clienteRepository.ObtenerPorIdAsync(id);
        return entidad == null ? null : MapearADto(entidad);
    }

    public async Task<(bool Exito, string Mensaje, ClienteDto? Cliente)> GuardarClienteAsync(ClienteGuardarDto dto)
    {
        var tipoDoc = (dto.TipoDoc ?? "DNI").Trim().ToUpperInvariant();
        if (tipoDoc != "DNI" && tipoDoc != "RUC")
            return (false, "El tipo de documento debe ser DNI (8 dígitos) o RUC (11 dígitos).", null);

        var validacion = tipoDoc == "DNI"
            ? _validadorAdapter.ValidarDni(dto.Documento)
            : _validadorAdapter.ValidarRuc(dto.Documento);

        if (!validacion.EsValido)
            return (false, validacion.MensajeError, null);

        if (string.IsNullOrWhiteSpace(dto.NombreCompleto) || dto.NombreCompleto.Trim().Length < 3)
            return (false, "El nombre completo o razón social del cliente debe tener al menos 3 caracteres.", null);

        if (await _clienteRepository.ExisteDocumentoAsync(validacion.DocumentoNormalizado, dto.Id))
            return (false, $"Ya existe un cliente activo registrado con el {tipoDoc} '{validacion.DocumentoNormalizado}'.", null);

        var nombreLimpio = dto.NombreCompleto.Trim();
        var telefonoLimpio = string.IsNullOrWhiteSpace(dto.Telefono) ? null : dto.Telefono.Trim();
        var direccionLimpia = string.IsNullOrWhiteSpace(dto.Direccion) ? null : dto.Direccion.Trim();

        if (dto.Id.HasValue && dto.Id.Value > 0)
        {
            var existente = await _clienteRepository.ObtenerPorIdAsync(dto.Id.Value);
            if (existente == null)
                return (false, "No se encontró el cliente que intenta editar.", null);

            if (existente.Documento == "00000000")
                return (false, "El cliente genérico 'Público General' no puede ser modificado.", null);

            existente.TipoDoc = tipoDoc;
            existente.Documento = validacion.DocumentoNormalizado;
            existente.NombreCompleto = nombreLimpio;
            existente.Telefono = telefonoLimpio;
            existente.Direccion = direccionLimpia;

            await _clienteRepository.ActualizarAsync(existente);
            return (true, $"Cliente '{nombreLimpio}' actualizado correctamente.", MapearADto(existente));
        }
        else
        {
            var nuevo = new Cliente
            {
                TipoDoc = tipoDoc,
                Documento = validacion.DocumentoNormalizado,
                NombreCompleto = nombreLimpio,
                Telefono = telefonoLimpio,
                Direccion = direccionLimpia,
                Activo = true
            };

            var creado = await _clienteRepository.CrearAsync(nuevo);
            return (true, $"Cliente '{nombreLimpio}' registrado y validado ({validacion.TipoContribuyente}).", MapearADto(creado));
        }
    }

    public async Task<(bool Exito, string Mensaje)> EliminarClienteAsync(int id)
    {
        return await _clienteRepository.EliminarLogicoAsync(id);
    }

    private ClienteDto MapearADto(Cliente c)
    {
        string tipoContribuyente = "Cliente General";
        if (c.Documento == "00000000")
        {
            tipoContribuyente = "Comprobante Rápido";
        }
        else if (c.TipoDoc.Equals("RUC", StringComparison.OrdinalIgnoreCase))
        {
            var val = _validadorAdapter.ValidarRuc(c.Documento);
            tipoContribuyente = val.EsValido ? val.TipoContribuyente : "Contribuyente RUC";
        }
        else
        {
            var val = _validadorAdapter.ValidarDni(c.Documento);
            tipoContribuyente = val.EsValido ? val.TipoContribuyente : "Persona Natural (DNI)";
        }

        return new ClienteDto
        {
            Id = c.Id,
            TipoDoc = c.TipoDoc,
            Documento = c.Documento,
            NombreCompleto = c.NombreCompleto,
            Telefono = c.Telefono,
            Direccion = c.Direccion,
            TipoContribuyente = tipoContribuyente,
            TotalComprasRealizadas = c.Ventas?.Count ?? 0,
            MontoTotalHistorico = c.Ventas?.Sum(v => v.Total) ?? 0m
        };
    }
}
