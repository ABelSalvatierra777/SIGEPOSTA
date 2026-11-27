namespace SIGEPOSTA.Entidades.Inventario;

public class Receta
{
    public int IdReceta { get; set; }
    public int IdAtencion { get; set; }
    public int IdPaciente { get; set; }
    public int IdPersonalSalud { get; set; }
    public DateTime FechaEmision { get; set; } = DateTime.Now;
    public string IndicacionesGenerales { get; set; } = "";
    public string Estado { get; set; } = "Emitida";

    public List<DetalleReceta> Detalles { get; set; } = new();
}
