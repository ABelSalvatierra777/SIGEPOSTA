namespace SIGEPOSTA.Entidades.Clinica;

public class Diagnostico
{
    public int IdDiagnostico { get; set; }
    public int IdAtencion { get; set; }
    public string CodigoCIE10 { get; set; } = "";
    public string Descripcion { get; set; } = "";
    public string TipoDiagnostico { get; set; } = "";
    public DateTime FechaRegistro { get; set; } = DateTime.Now;
    public string Observaciones { get; set; } = "";
    public string Estado { get; set; } = "Activo";
}
