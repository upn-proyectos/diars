namespace Minimarket.Negocio.DTOs;

public class KardexMovimientoDto
{
    public int Id { get; set; }
    public int ProductoId { get; set; }
    public string ProductoNombre { get; set; } = string.Empty;
    public string ProductoCodigo { get; set; } = string.Empty;
    public string UnidadMedida { get; set; } = "UNIDAD";
    public DateTime Fecha { get; set; }
    public string TipoMovimiento { get; set; } = "SALIDA"; // ENTRADA, SALIDA, AJUSTE
    public decimal Cantidad { get; set; }
    public decimal Saldo { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public string DocumentoReferencia { get; set; } = "—";
    public int UsuarioId { get; set; }
    public string UsuarioNombre { get; set; } = string.Empty;
}
