namespace Minimarket.Datos.Entities;

public class ConfiguracionTienda
{
    public int Id { get; set; }
    public string NombreComercio { get; set; } = "Minimarket Chininin SAC";
    public string Ruc { get; set; } = "20601234567";
    public string Direccion { get; set; } = "Av. América Norte 1234 - Trujillo";
    public string SerieBoleta { get; set; } = "B001";
    public int CorrelativoBoleta { get; set; } = 1;
    public decimal TasaIgv { get; set; } = 0.18m;
}
