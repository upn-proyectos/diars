namespace Minimarket.Negocio.Patterns.Adapter;

/// <summary>
/// Patrón Adapter (GoF Estructural) — Adaptee (Servicio Externo / Legado):
/// Simula el motor tributario de verificación de padrón SUNAT / RENIEC con una interfaz
/// técnica propia basada en códigos de retorno enteros y parámetros de salida.
/// </summary>
public class SunatReniecMotorExterno
{
    /// <summary>
    /// Códigos de retorno del motor externo:
    /// 0 = Válido
    /// 1 = Vacío
    /// 2 = Contiene caracteres no numéricos
    /// 3 = Longitud distinta de 11 dígitos
    /// 4 = Prefijo tributario no reconocido por SUNAT (debe iniciar en 10, 15, 17 o 20)
    /// </summary>
    public int VerificarPadronSunatRuc(string? rawRuc, out string clasificacionFiscal)
    {
        clasificacionFiscal = "No identificado";

        if (string.IsNullOrWhiteSpace(rawRuc))
            return 1;

        var limpio = rawRuc.Trim();
        if (!limpio.All(char.IsDigit))
            return 2;

        if (limpio.Length != 11)
            return 3;

        var prefijo = limpio.Substring(0, 2);
        switch (prefijo)
        {
            case "20":
                clasificacionFiscal = "Persona Jurídica (RUC 20)";
                return 0;
            case "10":
                clasificacionFiscal = "Persona Natural con Negocio (RUC 10)";
                return 0;
            case "15":
            case "17":
                clasificacionFiscal = "Régimen Especial / Otros (SUNAT)";
                return 0;
            default:
                return 4;
        }
    }

    /// <summary>
    /// Códigos de retorno RENIEC:
    /// 0 = Válido, 1 = Vacío, 2 = No numérico, 3 = Longitud distinta de 8 dígitos
    /// </summary>
    public int VerificarFormatoReniecDni(string? rawDni)
    {
        if (string.IsNullOrWhiteSpace(rawDni))
            return 1;

        var limpio = rawDni.Trim();
        if (!limpio.All(char.IsDigit))
            return 2;

        if (limpio.Length != 8)
            return 3;

        return 0;
    }
}
