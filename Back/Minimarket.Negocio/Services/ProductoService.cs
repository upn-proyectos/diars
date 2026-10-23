using Minimarket.Datos.Entities;
using Minimarket.Datos.Repositories;
using Minimarket.Negocio.DTOs;
using Minimarket.Negocio.Interfaces;

namespace Minimarket.Negocio.Services;

/// <summary>
/// Capa de Lógica de Negocio para la gestión de Inventario, Catálogo y Categorías (CU-04 y CU-05).
/// Consume IProductoRepository (Patrón Repository) mediante Inyección de Dependencias.
/// </summary>
public class ProductoService : IProductoService
{
    private readonly IProductoRepository _productoRepository;

    public ProductoService(IProductoRepository productoRepository)
    {
        _productoRepository = productoRepository;
    }

    public async Task<List<ProductoDto>> ListarCatalogoAsync()
    {
        var entidades = await _productoRepository.ListarActivosAsync();
        return entidades.Select(MapearADto).ToList();
    }

    public async Task<ProductoDto?> ObtenerPorIdAsync(int id)
    {
        var entidad = await _productoRepository.ObtenerPorIdAsync(id);
        return entidad == null ? null : MapearADto(entidad);
    }

    public async Task<(bool Exito, string Mensaje)> GuardarProductoAsync(ProductoGuardarDto dto, int? usuarioId = null)
    {
        if (string.IsNullOrWhiteSpace(dto.Nombre))
            return (false, "El nombre del producto es obligatorio.");

        if (string.IsNullOrWhiteSpace(dto.CodigoBarras))
            return (false, "El código de barras es obligatorio.");

        if (dto.CategoriaId <= 0)
            return (false, "Debe seleccionar una categoría válida.");

        if (dto.PrecioVenta < 0 || dto.CostoUnitario < 0)
            return (false, "El precio de venta y el costo unitario no pueden ser negativos.");

        if (dto.Stock < 0 || dto.StockMinimo < 0)
            return (false, "El stock actual y el stock mínimo no pueden ser negativos.");

        var codigoLimpio = dto.CodigoBarras.Trim();
        var nombreLimpio = dto.Nombre.Trim();
        var unidadLimpia = string.IsNullOrWhiteSpace(dto.UnidadMedida) ? "UNIDAD" : dto.UnidadMedida.Trim().ToUpperInvariant();

        if (await _productoRepository.ExisteCodigoBarrasAsync(codigoLimpio, dto.Id))
            return (false, $"Ya existe un producto activo con el código de barras '{codigoLimpio}'.");

        if (dto.Id.HasValue && dto.Id.Value > 0)
        {
            var existente = await _productoRepository.ObtenerPorIdAsync(dto.Id.Value);
            if (existente == null)
                return (false, "El producto que intenta editar no existe.");

            existente.CodigoBarras = codigoLimpio;
            existente.Nombre = nombreLimpio;
            existente.CategoriaId = dto.CategoriaId;
            existente.ProveedorId = dto.ProveedorId > 0 ? dto.ProveedorId : null;
            existente.PrecioVenta = dto.PrecioVenta;
            existente.CostoUnitario = dto.CostoUnitario;
            existente.Stock = dto.Stock;
            existente.StockMinimo = dto.StockMinimo;
            existente.UnidadMedida = unidadLimpia;
            existente.FechaVencimiento = dto.FechaVencimiento;

            await _productoRepository.ActualizarAsync(existente);
            return (true, $"Producto '{nombreLimpio}' actualizado correctamente.");
        }
        else
        {
            var nuevo = new Producto
            {
                CodigoBarras = codigoLimpio,
                Nombre = nombreLimpio,
                CategoriaId = dto.CategoriaId,
                ProveedorId = dto.ProveedorId > 0 ? dto.ProveedorId : null,
                PrecioVenta = dto.PrecioVenta,
                CostoUnitario = dto.CostoUnitario,
                Stock = dto.Stock,
                StockMinimo = dto.StockMinimo,
                UnidadMedida = unidadLimpia,
                FechaVencimiento = dto.FechaVencimiento,
                Activo = true
            };

            await _productoRepository.CrearAsync(nuevo, usuarioId);
            return (true, $"Producto '{nombreLimpio}' registrado en el inventario.");
        }
    }

    public async Task<(bool Exito, string Mensaje)> AjustarStockAsync(AjusteStockDto dto, int usuarioId)
    {
        if (dto.NuevoStock < 0)
            return (false, "El nuevo stock no puede ser negativo.");

        var motivoFinal = dto.Motivo == "Otro"
            ? (string.IsNullOrWhiteSpace(dto.MotivoOtro) ? "Ajuste manual" : dto.MotivoOtro.Trim())
            : (string.IsNullOrWhiteSpace(dto.Motivo) ? "Inventario físico" : dto.Motivo.Trim());

        var exito = await _productoRepository.AjustarStockAsync(dto.ProductoId, dto.NuevoStock, motivoFinal, usuarioId);
        if (!exito)
            return (false, "No se encontró el producto para realizar el ajuste de stock.");

        return (true, "Stock ajustado y movimiento registrado en Kardex.");
    }

    public async Task<(bool Exito, string Mensaje)> EliminarProductoAsync(int id)
    {
        var exito = await _productoRepository.EliminarLogicoAsync(id);
        return exito
            ? (true, "Producto retirado del catálogo activo.")
            : (false, "No se encontró el producto solicitado.");
    }

    public async Task<List<CategoriaItemDto>> ListarCategoriasAsync()
    {
        var categorias = await _productoRepository.ListarCategoriasActivasAsync();
        return categorias.Select(c => new CategoriaItemDto
        {
            Id = c.Id,
            Nombre = c.Nombre,
            Descripcion = c.Descripcion,
            TotalProductos = c.Productos?.Count ?? 0
        }).ToList();
    }

    public async Task<(bool Exito, string Mensaje)> GuardarCategoriaAsync(CategoriaGuardarDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nombre) || dto.Nombre.Trim().Length < 3)
            return (false, "El nombre de la categoría debe tener al menos 3 caracteres.");

        var nombreLimpio = dto.Nombre.Trim();
        var descLimpia = string.IsNullOrWhiteSpace(dto.Descripcion) ? null : dto.Descripcion.Trim();

        if (await _productoRepository.ExisteNombreCategoriaAsync(nombreLimpio, dto.Id))
            return (false, $"Ya existe una categoría activa llamada '{nombreLimpio}'.");

        if (dto.Id.HasValue && dto.Id.Value > 0)
        {
            var existente = await _productoRepository.ObtenerCategoriaPorIdAsync(dto.Id.Value);
            if (existente == null)
                return (false, "No se encontró la categoría que intenta editar.");

            existente.Nombre = nombreLimpio;
            existente.Descripcion = descLimpia;
            await _productoRepository.ActualizarCategoriaAsync(existente);
            return (true, $"Categoría '{nombreLimpio}' actualizada correctamente.");
        }
        else
        {
            var nueva = new Categoria
            {
                Nombre = nombreLimpio,
                Descripcion = descLimpia,
                Activo = true
            };

            await _productoRepository.CrearCategoriaAsync(nueva);
            return (true, $"Categoría '{nombreLimpio}' creada correctamente.");
        }
    }

    public async Task<(bool Exito, string Mensaje)> EliminarCategoriaAsync(int id)
    {
        return await _productoRepository.EliminarCategoriaLogicoAsync(id);
    }

    public async Task<List<ProveedorItemDto>> ListarProveedoresAsync()
    {
        var proveedores = await _productoRepository.ListarProveedoresActivosAsync();
        return proveedores.Select(p => new ProveedorItemDto
        {
            Id = p.Id,
            Ruc = p.Ruc,
            RazonSocial = p.RazonSocial,
            NombreMostrar = !string.IsNullOrWhiteSpace(p.NombreComercial) ? p.NombreComercial : p.RazonSocial
        }).ToList();
    }

    private static ProductoDto MapearADto(Producto p)
    {
        return new ProductoDto
        {
            Id = p.Id,
            CodigoBarras = p.CodigoBarras ?? string.Empty,
            Nombre = p.Nombre,
            CategoriaId = p.CategoriaId,
            CategoriaNombre = p.Categoria?.Nombre ?? "Sin categoría",
            ProveedorId = p.ProveedorId,
            ProveedorNombre = p.Proveedor != null
                ? (!string.IsNullOrWhiteSpace(p.Proveedor.NombreComercial) ? p.Proveedor.NombreComercial : p.Proveedor.RazonSocial)
                : "—",
            PrecioVenta = p.PrecioVenta,
            CostoUnitario = p.CostoUnitario,
            Stock = p.Stock,
            StockMinimo = p.StockMinimo,
            UnidadMedida = p.UnidadMedida,
            FechaVencimiento = p.FechaVencimiento
        };
    }
}
