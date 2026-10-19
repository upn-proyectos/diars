namespace Minimarket.Datos.Entities;

public class Venta
{
    public int Id { get; set; }
    public string NumeroBoleta { get; set; } = string.Empty;
    public DateTime Fecha { get; set; } = DateTime.Now;
    public int ClienteId { get; set; }
    public int UsuarioId { get; set; }
    public string MetodoPago { get; set; } = "Efectivo";
    public decimal Subtotal { get; set; }
    public decimal Igv { get; set; }
    public decimal Total { get; set; }

    public Cliente? Cliente { get; set; }
    public Usuario? Usuario { get; set; }
    public ICollection<DetalleVenta> Detalles { get; set; } = new List<DetalleVenta>();
}
