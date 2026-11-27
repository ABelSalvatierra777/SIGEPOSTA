namespace SIGEPOSTA.Entidades.Clinica;

public class Cita
{
    public int IdCita { get; set; }
    public int IdPaciente { get; set; }
    public int? IdPersonalSalud { get; set; }
    public DateTime FechaHora { get; set; }
    public string Motivo { get; set; } = "";
    public string Estado { get; set; } = "Pendiente";
    public string Observaciones { get; set; } = "";
    public DateTime FechaRegistro { get; set; } = DateTime.Now;
}
