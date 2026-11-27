namespace SIGEPOSTA.Entidades.Inventario;

public class Inventario
{
    public int IdInventario { get; set; }
    public int IdMedicamento { get; set; }
    public string NumeroLote { get; set; } = "";
    public int StockActual { get; set; }
    public DateTime? FechaVencimiento { get; set; }
    public DateTime FechaIngreso { get; set; } = DateTime.Now;
    public string Ubicacion { get; set; } = "";
    public bool Estado { get; set; } = true;
}
