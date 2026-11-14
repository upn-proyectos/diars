using System.Text.RegularExpressions;
using Minimarket.Datos.Repositories;
using Minimarket.Negocio.DTOs;
using Minimarket.Negocio.Interfaces;

namespace Minimarket.Negocio.Services;

public class KardexService : IKardexService
{
    private readonly IKardexRepository _kardexRepository;
    private static readonly Regex RegexDocumento = new(@"(B\d{3}-\d+|OC-\d+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public KardexService(IKardexRepository kardexRepository)
    {
        _kardexRepository = kardexRepository;
    }

    public async Task<List<KardexMovimientoDto>> ListarMovimientosAsync(
        int? productoId = null,
        DateTime? desde = null,
        DateTime? hasta = null,
        string? tipoMovimiento = null,
        int limite = 300)
    {
        var lista = await _kardexRepository.ListarMovimientosAsync(productoId, desde, hasta, tipoMovimiento, limite);

        return lista.Select(k =>
        {
            var match = RegexDocumento.Match(k.Motivo ?? string.Empty);
            var doc = match.Success
                ? match.Value.ToUpperInvariant()
                : (k.TipoMovimiento.Equals("AJUSTE", StringComparison.OrdinalIgnoreCase) ? "AJUSTE-INV" : "MOV-ALM");

            return new KardexMovimientoDto
            {
                Id = k.Id,
                ProductoId = k.ProductoId,
                ProductoNombre = k.Producto?.Nombre ?? $"Producto #{k.ProductoId}",
                ProductoCodigo = k.Producto?.CodigoBarras ?? "",
                UnidadMedida = k.Producto?.UnidadMedida ?? "UNIDAD",
                Fecha = k.Fecha,
                TipoMovimiento = k.TipoMovimiento.ToUpperInvariant(),
                Cantidad = k.Cantidad,
                Saldo = k.Saldo,
                Motivo = k.Motivo ?? string.Empty,
                DocumentoReferencia = doc,
                UsuarioId = k.UsuarioId,
                UsuarioNombre = k.Usuario?.Nombre ?? "Sistema"
            };
        }).ToList();
    }
}
