namespace Minimarket.Negocio.Patterns.Adapter;

public class ResultadoValidacionDocumento
{
    public bool EsValido { get; set; }
    public string DocumentoNormalizado { get; set; } = string.Empty;
    public string TipoContribuyente { get; set; } = string.Empty;
    public string MensajeError { get; set; } = string.Empty;
}

/// <summary>
/// Patrón Adapter (GoF Estructural) — Target Interface:
/// Define la interfaz estándar que los servicios de negocio del Minimarket utilizan
/// para validar documentos oficiales (RUC 11 dígitos y DNI 8 dígitos).
/// </summary>
public interface IValidadorDocumentoAdapter
{
    ResultadoValidacionDocumento ValidarRuc(string ruc);
    ResultadoValidacionDocumento ValidarDni(string dni);
}
