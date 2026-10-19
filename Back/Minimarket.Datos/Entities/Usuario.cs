namespace Minimarket.Datos.Entities;

public class Usuario
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string UsuarioLogin { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public int RolId { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime FechaCreacion { get; set; } = DateTime.Now;

    public Rol? Rol { get; set; }
    public ICollection<Venta> Ventas { get; set; } = new List<Venta>();
    public ICollection<Kardex> MovimientosKardex { get; set; } = new List<Kardex>();
}
