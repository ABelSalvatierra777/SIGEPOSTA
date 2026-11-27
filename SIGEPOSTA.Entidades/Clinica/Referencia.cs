namespace SIGEPOSTA.Entidades.Clinica;

public class Referencia
{
    public int IdReferencia { get; set; }
    public int IdPaciente { get; set; }
    public int? IdAtencion { get; set; }
    public string EstablecimientoDestino { get; set; } = "";
    public string MotivoReferencia { get; set; } = "";
    public string DiagnosticoPresuntivo { get; set; } = "";
    public DateTime FechaReferencia { get; set; } = DateTime.Now;
    public string Estado { get; set; } = "Pendiente";
}
