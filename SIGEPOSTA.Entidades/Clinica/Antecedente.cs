namespace SIGEPOSTA.Entidades.Clinica;

public class Antecedente
{
    public int IdAntecedente { get; set; }
    public int IdHistoriaClinica { get; set; }
    public string TipoAntecedente { get; set; } = "";
    public string Descripcion { get; set; } = "";
    public DateTime FechaRegistro { get; set; } = DateTime.Now;
}
