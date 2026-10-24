namespace Minimarket.Negocio.Patterns.Adapter;

/// <summary>
/// Patrón Adapter (GoF Estructural) — Adapter Concreto:
/// Envuelve (wraps) a SunatReniecMotorExterno y adapta sus códigos técnicos de respuesta
/// a la interfaz limpia IValidadorDocumentoAdapter esperada por la capa de negocio.
/// </summary>
public class ValidadorDocumentoPeruAdapter : IValidadorDocumentoAdapter
{
    private readonly SunatReniecMotorExterno _motorExterno;

    public ValidadorDocumentoPeruAdapter(SunatReniecMotorExterno motorExterno)
    {
        _motorExterno = motorExterno;
    }

    public ResultadoValidacionDocumento ValidarRuc(string ruc)
    {
        var codigo = _motorExterno.VerificarPadronSunatRuc(ruc, out var clasificacion);

        return codigo switch
        {
            0 => new ResultadoValidacionDocumento
            {
                EsValido = true,
                DocumentoNormalizado = ruc.Trim(),
                TipoContribuyente = clasificacion
            },
            1 => new ResultadoValidacionDocumento
            {
                EsValido = false,
                MensajeError = "El número de RUC es obligatorio."
            },
            2 => new ResultadoValidacionDocumento
            {
                EsValido = false,
                MensajeError = "El RUC solo debe contener dígitos numéricos."
            },
            3 => new ResultadoValidacionDocumento
            {
                EsValido = false,
                MensajeError = "El RUC debe tener exactamente 11 dígitos oficiales."
            },
            4 => new ResultadoValidacionDocumento
            {
                EsValido = false,
                MensajeError = "El RUC debe iniciar con un prefijo válido de SUNAT (10 o 20)."
            },
            _ => new ResultadoValidacionDocumento
            {
                EsValido = false,
                MensajeError = "Formato de RUC inválido."
            }
        };
    }

    public ResultadoValidacionDocumento ValidarDni(string dni)
    {
        var codigo = _motorExterno.VerificarFormatoReniecDni(dni);

        return codigo switch
        {
            0 => new ResultadoValidacionDocumento
            {
                EsValido = true,
                DocumentoNormalizado = dni.Trim(),
                TipoContribuyente = "Ciudadano Identificado (DNI 8 dígitos)"
            },
            1 => new ResultadoValidacionDocumento
            {
                EsValido = false,
                MensajeError = "El número de DNI es obligatorio."
            },
            2 => new ResultadoValidacionDocumento
            {
                EsValido = false,
                MensajeError = "El DNI solo debe contener dígitos numéricos."
            },
            3 => new ResultadoValidacionDocumento
            {
                EsValido = false,
                MensajeError = "El DNI debe tener exactamente 8 dígitos."
            },
            _ => new ResultadoValidacionDocumento
            {
                EsValido = false,
                MensajeError = "Formato de DNI inválido."
            }
        };
    }
}
