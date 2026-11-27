namespace SIGEPOSTA.Seguridad.Modelos;

public class SesionUsuario
{
    public int UsuarioId { get; set; }

    public string NombreUsuario { get; set; } = string.Empty;

    public string NombreCompleto { get; set; } = string.Empty;

    public string Rol { get; set; } = string.Empty;

    public DateTime FechaInicio { get; set; } = DateTime.Now;

    public bool EstaAutenticado { get; set; }
}
