using Minimarket.Negocio.DTOs;

namespace Minimarket.Negocio.Interfaces;

public interface IProductoService
{
    Task<List<ProductoDto>> ListarCatalogoAsync();
    Task<ProductoDto?> ObtenerPorIdAsync(int id);
    Task<(bool Exito, string Mensaje)> GuardarProductoAsync(ProductoGuardarDto dto, int? usuarioId = null);
    Task<(bool Exito, string Mensaje)> AjustarStockAsync(AjusteStockDto dto, int usuarioId);
    Task<(bool Exito, string Mensaje)> EliminarProductoAsync(int id);

    // Gestión de Categorías
    Task<List<CategoriaItemDto>> ListarCategoriasAsync();
    Task<(bool Exito, string Mensaje)> GuardarCategoriaAsync(CategoriaGuardarDto dto);
    Task<(bool Exito, string Mensaje)> EliminarCategoriaAsync(int id);

    // Proveedores de apoyo
    Task<List<ProveedorItemDto>> ListarProveedoresAsync();
}
