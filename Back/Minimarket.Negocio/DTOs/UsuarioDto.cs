namespace Minimarket.Negocio.DTOs;

public class UsuarioDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string UsuarioLogin { get; set; } = string.Empty;
    public int RolId { get; set; }
    public string RolNombre { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;
    public DateTime FechaCreacion { get; set; }
    public bool UsaHashSeguro { get; set; }
    public int TotalVentasRegistradas { get; set; }
}

public class RolDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
}

public class UsuarioGuardarDto
{
    public int? Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string UsuarioLogin { get; set; } = string.Empty;
    public int RolId { get; set; }
    public bool Activo { get; set; } = true;
    public string? Password { get; set; }
}
