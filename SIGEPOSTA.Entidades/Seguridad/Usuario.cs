using SIGEPOSTA.Entidades.Personas;

namespace SIGEPOSTA.Entidades.Seguridad;

public class Usuario
{
    public int IdUsuario { get; set; }
    public int? IdPersonalSalud { get; set; }
    public string NombreUsuario { get; set; } = "";
    public string HashContrasena { get; set; } = "";
    public bool Estado { get; set; } = true;
    public DateTime FechaCreacion { get; set; } = DateTime.Now;
    public DateTime? UltimoAcceso { get; set; }

    public PersonalSalud? PersonalSalud { get; set; }
    public List<Rol> Roles { get; set; } = new();
}
