namespace Minimarket.Negocio.DTOs;

public class OrdenCompraDto
{
    public int Id { get; set; }
    public string NumeroOrden { get; set; } = string.Empty;
    public int ProveedorId { get; set; }
    public string ProveedorNombre { get; set; } = string.Empty;
    public string ProveedorRuc { get; set; } = string.Empty;
    public int UsuarioId { get; set; }
    public string UsuarioNombre { get; set; } = string.Empty;
    public DateTime FechaEmision { get; set; }
    public DateTime? FechaRecepcion { get; set; }
    public string Estado { get; set; } = "Pendiente";
    public decimal Total { get; set; }
    public string? Observaciones { get; set; }
    public List<DetalleOrdenDto> Detalles { get; set; } = new();
    public int TotalItems => Detalles.Count;
}

public class DetalleOrdenDto
{
    public int Id { get; set; }
    public int ProductoId { get; set; }
    public string NombreProducto { get; set; } = string.Empty;
    public decimal Cantidad { get; set; }
    public decimal CantidadRecibida { get; set; }
    public decimal CostoUnitario { get; set; }
    public decimal Subtotal { get; set; }
}

public class OrdenCompraCrearDto
{
    public int ProveedorId { get; set; }
    public string? Observaciones { get; set; }
    public List<OrdenCompraItemCrearDto> Items { get; set; } = new();
}

public class OrdenCompraItemCrearDto
{
    public int ProductoId { get; set; }
    public decimal Cantidad { get; set; }
    public decimal CostoUnitario { get; set; }
}

public class OrdenCompraRecepcionDto
{
    public int OrdenId { get; set; }
    public List<OrdenCompraItemRecepcionDto> Items { get; set; } = new();
}

public class OrdenCompraItemRecepcionDto
{
    public int ProductoId { get; set; }
    public decimal CantidadRecibida { get; set; }
}
