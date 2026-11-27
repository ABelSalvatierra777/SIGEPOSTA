namespace SIGEPOSTA.Entidades.Personas;

public abstract class Persona
{
    public int IdPersona { get; set; }
    public string TipoDocumento { get; set; } = "";
    public string NumeroDocumento { get; set; } = "";
    public string Nombres { get; set; } = "";
    public string ApellidoPaterno { get; set; } = "";
    public string ApellidoMaterno { get; set; } = "";
    public DateTime? FechaNacimiento { get; set; }
    public string Sexo { get; set; } = "";
    public string Telefono { get; set; } = "";
    public string Direccion { get; set; } = "";
    public string CorreoElectronico { get; set; } = "";
    public bool Estado { get; set; } = true;
    public DateTime FechaRegistro { get; set; } = DateTime.Now;

    public string NombreCompleto =>
        $"{Nombres} {ApellidoPaterno} {ApellidoMaterno}".Trim();
}
