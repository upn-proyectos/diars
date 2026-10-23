namespace Minimarket.Negocio.DTOs;

public class ProductoDto
{
    public int Id { get; set; }
    public string CodigoBarras { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public int CategoriaId { get; set; }
    public string CategoriaNombre { get; set; } = string.Empty;
    public int? ProveedorId { get; set; }
    public string ProveedorNombre { get; set; } = string.Empty;
    public decimal PrecioVenta { get; set; }
    public decimal CostoUnitario { get; set; }
    public decimal Stock { get; set; }
    public decimal StockMinimo { get; set; }
    public string UnidadMedida { get; set; } = "UNIDAD";
    public DateTime? FechaVencimiento { get; set; }

    // Reglas de Negocio Calculadas
    public bool EstaBajoStock => Stock <= StockMinimo;
    public decimal MargenGanancia => PrecioVenta - CostoUnitario;
    public decimal MargenPorcentaje => CostoUnitario > 0 ? Math.Round(((PrecioVenta - CostoUnitario) / CostoUnitario) * 100, 1) : 0;

    public int? DiasParaVencer => FechaVencimiento.HasValue
        ? (int)(FechaVencimiento.Value.Date - DateTime.Today).TotalDays
        : null;

    public string EstadoVencimiento
    {
        get
        {
            if (!DiasParaVencer.HasValue) return "SinFecha";
            if (DiasParaVencer.Value < 0) return "Vencido";
            if (DiasParaVencer.Value <= 7) return "Critico";
            if (DiasParaVencer.Value <= 30) return "Proximo";
            return "Vigente";
        }
    }
}

public class ProductoGuardarDto
{
    public int? Id { get; set; }
    public string CodigoBarras { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public int CategoriaId { get; set; }
    public int? ProveedorId { get; set; }
    public decimal PrecioVenta { get; set; }
    public decimal CostoUnitario { get; set; }
    public decimal Stock { get; set; }
    public decimal StockMinimo { get; set; } = 5;
    public string UnidadMedida { get; set; } = "UNIDAD";
    public DateTime? FechaVencimiento { get; set; }
}

public class AjusteStockDto
{
    public int ProductoId { get; set; }
    public decimal NuevoStock { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public string? MotivoOtro { get; set; }
}

public class CategoriaItemDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public int TotalProductos { get; set; }
}

public class CategoriaGuardarDto
{
    public int? Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
}

public class ProveedorItemDto
{
    public int Id { get; set; }
    public string Ruc { get; set; } = string.Empty;
    public string RazonSocial { get; set; } = string.Empty;
    public string NombreMostrar { get; set; } = string.Empty;
}
