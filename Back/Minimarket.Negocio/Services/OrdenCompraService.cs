using Minimarket.Datos.Entities;
using Minimarket.Datos.Repositories;
using Minimarket.Negocio.DTOs;
using Minimarket.Negocio.Interfaces;

namespace Minimarket.Negocio.Services;

public class OrdenCompraService : IOrdenCompraService
{
    private readonly IOrdenCompraRepository _ordenRepository;

    public OrdenCompraService(IOrdenCompraRepository ordenRepository)
    {
        _ordenRepository = ordenRepository;
    }

    public async Task<List<OrdenCompraDto>> ListarOrdenesAsync()
    {
        var ordenes = await _ordenRepository.ListarOrdenesAsync();
        return ordenes.Select(MapearDto).ToList();
    }

    public async Task<OrdenCompraDto?> ObtenerPorIdAsync(int id)
    {
        var orden = await _ordenRepository.ObtenerPorIdAsync(id);
        return orden == null ? null : MapearDto(orden);
    }

    public async Task<(bool Exito, string Mensaje, OrdenCompraDto? Orden)> CrearOrdenAsync(OrdenCompraCrearDto dto, int usuarioId)
    {
        if (dto == null || dto.ProveedorId <= 0)
            return (false, "Debe seleccionar un proveedor válido para la orden de compra.", null);

        var itemsValidos = dto.Items
            .Where(i => i.ProductoId > 0 && i.Cantidad > 0)
            .GroupBy(i => i.ProductoId)
            .Select(g => (
                ProductoId: g.Key,
                Cantidad: Math.Round(g.Sum(x => x.Cantidad), 3),
                CostoUnitario: Math.Round(g.Last().CostoUnitario, 2)))
            .ToList();

        if (itemsValidos.Count == 0)
            return (false, "Debe agregar al menos un producto con cantidad mayor a cero.", null);

        var resultado = await _ordenRepository.CrearOrdenAsync(dto.ProveedorId, usuarioId, dto.Observaciones, itemsValidos);
        if (!resultado.Exito || resultado.OrdenCreada == null)
            return (false, resultado.Mensaje, null);

        return (true, resultado.Mensaje, MapearDto(resultado.OrdenCreada));
    }

    public async Task<(bool Exito, string Mensaje)> ConfirmarRecepcionAsync(OrdenCompraRecepcionDto dto, int usuarioId)
    {
        if (dto == null || dto.OrdenId <= 0)
            return (false, "Identificador de orden de compra inválido.");

        var mapaCantidades = dto.Items?
            .Where(i => i.ProductoId > 0)
            .GroupBy(i => i.ProductoId)
            .ToDictionary(g => g.Key, g => g.Last().CantidadRecibida);

        return await _ordenRepository.ConfirmarRecepcionTransaccionalAsync(dto.OrdenId, usuarioId, mapaCantidades);
    }

    public async Task<(bool Exito, string Mensaje)> AnularOrdenAsync(int ordenId, int usuarioId)
    {
        if (ordenId <= 0)
            return (false, "Identificador de orden de compra inválido.");

        return await _ordenRepository.AnularOrdenAsync(ordenId, usuarioId);
    }

    private static OrdenCompraDto MapearDto(OrdenCompra o) => new()
    {
        Id = o.Id,
        NumeroOrden = o.NumeroOrden,
        ProveedorId = o.ProveedorId,
        ProveedorNombre = !string.IsNullOrWhiteSpace(o.Proveedor?.NombreComercial)
            ? o.Proveedor.NombreComercial
            : (o.Proveedor?.RazonSocial ?? "Proveedor"),
        ProveedorRuc = o.Proveedor?.Ruc ?? "",
        UsuarioId = o.UsuarioId,
        UsuarioNombre = o.Usuario?.Nombre ?? "Almacén",
        FechaEmision = o.FechaEmision,
        FechaRecepcion = o.FechaRecepcion,
        Estado = o.Estado,
        Total = o.Total,
        Observaciones = o.Observaciones,
        Detalles = o.Detalles.Select(d => new DetalleOrdenDto
        {
            Id = d.Id,
            ProductoId = d.ProductoId,
            NombreProducto = d.NombreProducto,
            Cantidad = d.Cantidad,
            CantidadRecibida = d.CantidadRecibida,
            CostoUnitario = d.CostoUnitario,
            Subtotal = d.Subtotal
        }).ToList()
    };
}
