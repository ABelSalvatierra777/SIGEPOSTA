namespace SIGEPOSTA.Entidades.Seguridad;

public class Permiso
{
    public int IdPermiso { get; set; }
    public string Codigo { get; set; } = "";
    public string Nombre { get; set; } = "";
    public string Descripcion { get; set; } = "";
    public bool Estado { get; set; } = true;
}
