namespace SIGEPOSTA.Entidades.Personas;

public class PersonalSalud : Persona
{
    public int IdPersonalSalud { get; set; }
    public string CodigoTrabajador { get; set; } = "";
    public string Profesion { get; set; } = "";
    public string Especialidad { get; set; } = "";
    public string NumeroColegiatura { get; set; } = "";
    public string Cargo { get; set; } = "";
    public DateTime? FechaContratacion { get; set; }
    public bool EsActivo { get; set; } = true;
}
