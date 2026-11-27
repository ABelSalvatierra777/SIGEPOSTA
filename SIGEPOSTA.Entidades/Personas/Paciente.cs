namespace SIGEPOSTA.Entidades.Personas;

public class Paciente : Persona
{
    public int IdPaciente { get; set; }
    public string CodigoPaciente { get; set; } = "";
    public string GrupoSanguineo { get; set; } = "";
    public string Alergias { get; set; } = "";
    public string Observaciones { get; set; } = "";
    public DateTime FechaRegistroPaciente { get; set; } = DateTime.Now;
}
