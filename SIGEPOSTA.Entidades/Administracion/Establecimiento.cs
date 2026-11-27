namespace SIGEPOSTA.Entidades.Administracion;

public class Establecimiento
{
    public int IdEstablecimiento { get; set; }
    public string CodigoRenipress { get; set; } = "";
    public string Nombre { get; set; } = "Posta de Salud de Mollepata";
    public string Direccion { get; set; } = "";
    public string Distrito { get; set; } = "Mollepata";
    public string Provincia { get; set; } = "";
    public string Departamento { get; set; } = "";
    public string Telefono { get; set; } = "";
    public string Categoria { get; set; } = "";
    public bool Estado { get; set; } = true;
}
