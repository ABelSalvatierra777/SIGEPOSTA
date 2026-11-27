namespace SIGEPOSTA.Entidades.Inventario;

public class DetalleReceta
{
    public int IdDetalleReceta { get; set; }
    public int IdReceta { get; set; }
    public int IdMedicamento { get; set; }
    public int Cantidad { get; set; }
    public string Dosis { get; set; } = "";
    public string Frecuencia { get; set; } = "";
    public string Duracion { get; set; } = "";
    public string Indicaciones { get; set; } = "";
}
