namespace Minimarket.Datos.Entities;

public class OrdenCompra
{
    public int Id { get; set; }
    public string NumeroOrden { get; set; } = string.Empty;
    public int ProveedorId { get; set; }
    public int UsuarioId { get; set; }
    public DateTime FechaEmision { get; set; } = DateTime.Now;
    public DateTime? FechaRecepcion { get; set; }
    public string Estado { get; set; } = "Pendiente"; // 'Pendiente', 'Recibida', 'Anulada'
    public decimal Total { get; set; }
    public string? Observaciones { get; set; }

    public Proveedor? Proveedor { get; set; }
    public Usuario? Usuario { get; set; }
    public ICollection<DetalleOrden> Detalles { get; set; } = new List<DetalleOrden>();
}
