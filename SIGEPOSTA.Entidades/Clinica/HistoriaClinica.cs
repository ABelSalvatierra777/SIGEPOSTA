namespace SIGEPOSTA.Entidades.Clinica;

public class HistoriaClinica
{
    public int IdHistoriaClinica { get; set; }
    public int IdPaciente { get; set; }

    public string NumeroHistoria { get; set; } = "";

    public string GrupoSanguineo { get; set; } = "";
    public string Alergias { get; set; } = "";
    public string AntecedentesPersonales { get; set; } = "";
    public string AntecedentesFamiliares { get; set; } = "";

    public DateTime FechaApertura { get; set; } = DateTime.Now;
    public string Observaciones { get; set; } = "";
    public bool Estado { get; set; } = true;

    public List<Antecedente> Antecedentes { get; set; } = new();
}
