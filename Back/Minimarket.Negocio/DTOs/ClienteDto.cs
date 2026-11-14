namespace Minimarket.Negocio.DTOs;

public class ClienteDto
{
    public int Id { get; set; }
    public string TipoDoc { get; set; } = "DNI";
    public string Documento { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string? Direccion { get; set; }
    public string TipoContribuyente { get; set; } = string.Empty;
    public int TotalComprasRealizadas { get; set; }
    public decimal MontoTotalHistorico { get; set; }
    public bool EsPublicoGeneral => Documento == "00000000";
}

public class ClienteGuardarDto
{
    public int? Id { get; set; }
    public string TipoDoc { get; set; } = "DNI";
    public string Documento { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string? Direccion { get; set; }
}
