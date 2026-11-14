using Minimarket.Negocio.DTOs;

namespace Minimarket.Front.Models;

public class PosViewModel
{
    public ConfiguracionTiendaDto Configuracion { get; set; } = new();
    public List<ProductoDto> Productos { get; set; } = new();
    public List<CategoriaItemDto> Categorias { get; set; } = new();
    public List<ClienteDto> Clientes { get; set; } = new();
    public List<VentaDto> VentasRecientes { get; set; } = new();
    public VentaDto? UltimaVentaEmitida { get; set; }
}
