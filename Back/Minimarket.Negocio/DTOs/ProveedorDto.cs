namespace Minimarket.Negocio.DTOs;

public class ProveedorDto
{
    public int Id { get; set; }
    public string Ruc { get; set; } = string.Empty;
    public string RazonSocial { get; set; } = string.Empty;
    public string? NombreComercial { get; set; }
    public string? Contacto { get; set; }
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public string? Direccion { get; set; }
    public string TipoContribuyente { get; set; } = string.Empty;
    public int TotalProductosAbastecidos { get; set; }

    public string NombreMostrar => !string.IsNullOrWhiteSpace(NombreComercial) ? NombreComercial : RazonSocial;
}

public class ProveedorGuardarDto
{
    public int? Id { get; set; }
    public string Ruc { get; set; } = string.Empty;
    public string RazonSocial { get; set; } = string.Empty;
    public string? NombreComercial { get; set; }
    public string? Contacto { get; set; }
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public string? Direccion { get; set; }
}
