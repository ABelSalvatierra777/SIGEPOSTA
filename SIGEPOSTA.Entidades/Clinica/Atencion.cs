namespace SIGEPOSTA.Entidades.Clinica;

public class Atencion
{
    public int IdAtencion { get; set; }
    public int? IdCita { get; set; }
    public int IdPaciente { get; set; }
    public int IdPersonalSalud { get; set; }

    public DateTime FechaAtencion { get; set; } = DateTime.Now;

    public string MotivoConsulta { get; set; } = "";
    public string Sintomas { get; set; } = "";
    public string ExamenFisico { get; set; } = "";
    public string Tratamiento { get; set; } = "";
    public string Observaciones { get; set; } = "";

    // Signos vitales almacenados en la tabla SignosVitales.
    public decimal? Temperatura { get; set; }
    public string PresionArterial { get; set; } = "";
    public decimal? Peso { get; set; }
    public decimal? Talla { get; set; }

    // Estado real de la tabla Atenciones: bit.
    public bool Estado { get; set; } = true;

    // Datos relacionados para mostrar en el listado.
    public string DniPaciente { get; set; } = "";
    public string NombrePaciente { get; set; } = "";
    public string DniPersonalSalud { get; set; } = "";
    public string NombrePersonalSalud { get; set; } = "";
}
