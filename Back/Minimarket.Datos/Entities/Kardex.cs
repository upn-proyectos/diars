namespace Minimarket.Datos.Entities;

public class Kardex
{
    public int Id { get; set; }
    public int ProductoId { get; set; }
    public DateTime Fecha { get; set; } = DateTime.Now;
    public string TipoMovimiento { get; set; } = "SALIDA"; // 'ENTRADA', 'SALIDA', 'AJUSTE'
    public decimal Cantidad { get; set; }
    public decimal Saldo { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public int UsuarioId { get; set; }

    public Producto? Producto { get; set; }
    public Usuario? Usuario { get; set; }
}
