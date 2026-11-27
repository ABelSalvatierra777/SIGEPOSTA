namespace SIGEPOSTA.Entidades.Inventario;

public class Medicamento
{
    public int IdMedicamento { get; set; }

    public string Codigo { get; set; } = "";

    public string Nombre { get; set; } = "";

    public string PrincipioActivo { get; set; } = "";

    public string Concentracion { get; set; } = "";

    public string FormaFarmaceutica { get; set; } = "";

    public string Presentacion { get; set; } = "";

    public string UnidadMedida { get; set; } = "";

    public string Laboratorio { get; set; } = "";

    public string Categoria { get; set; } = "";

    public int Stock { get; set; }

    public int StockMinimo { get; set; }

    public decimal Precio { get; set; }

    public DateTime? FechaVencimiento { get; set; }

    public string Descripcion { get; set; } = "";
    public string Observacion { get; set; } = "";

    public string EstadoDescripcion { get; set; } = "Disponible";

    public bool Estado { get; set; } = true;

    public string Lote { get; set; } = "";
}

