
namespace SIGEPOSTA.Entidades.Seguridad;

public class Rol
{
    public int IdRol { get; set; }
    public string Nombre { get; set; } = "";
    public string Descripcion { get; set; } = "";
    public bool Estado { get; set; } = true;

    public List<Permiso> Permisos { get; set; } = new();
}
