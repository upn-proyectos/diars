namespace Minimarket.Datos.Entities;

public class Cliente
{
    public int Id { get; set; }
    public string TipoDoc { get; set; } = "DNI";
    public string Documento { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string? Direccion { get; set; }
    public bool Activo { get; set; } = true;

    public ICollection<Venta> Ventas { get; set; } = new List<Venta>();
}
