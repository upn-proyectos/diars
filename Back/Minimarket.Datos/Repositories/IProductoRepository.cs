using Minimarket.Datos.Entities;

namespace Minimarket.Datos.Repositories;

/// <summary>
/// Patrón Repository: Abstrae el acceso a datos de la entidad Producto, Categoría y sus relaciones
/// para desacoplar la capa de Lógica de Negocio de Entity Framework Core.
/// </summary>
public interface IProductoRepository
{
    Task<List<Producto>> ListarActivosAsync();
    Task<Producto?> ObtenerPorIdAsync(int id);
    Task<bool> ExisteCodigoBarrasAsync(string codigoBarras, int? excluirId = null);
    Task<Producto> CrearAsync(Producto producto, int? usuarioId = null);
    Task ActualizarAsync(Producto producto);
    Task<bool> AjustarStockAsync(int productoId, decimal nuevoStock, string motivo, int usuarioId);
    Task<bool> EliminarLogicoAsync(int id);

    // Operaciones de Categorías de Inventario
    Task<List<Categoria>> ListarCategoriasActivasAsync();
    Task<Categoria?> ObtenerCategoriaPorIdAsync(int id);
    Task<bool> ExisteNombreCategoriaAsync(string nombre, int? excluirId = null);
    Task<Categoria> CrearCategoriaAsync(Categoria categoria);
    Task ActualizarCategoriaAsync(Categoria categoria);
    Task<(bool Exito, string Mensaje)> EliminarCategoriaLogicoAsync(int id);

    // Proveedores de apoyo para el catálogo
    Task<List<Proveedor>> ListarProveedoresActivosAsync();
}
