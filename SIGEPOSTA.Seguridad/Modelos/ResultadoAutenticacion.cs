namespace SIGEPOSTA.Seguridad.Modelos;

public class ResultadoAutenticacion
{
    public bool Exitoso { get; set; }

    public string Mensaje { get; set; } = string.Empty;

    public SesionUsuario? Sesion { get; set; }

    public static ResultadoAutenticacion Correcto(SesionUsuario sesion)
    {
        return new ResultadoAutenticacion
        {
            Exitoso = true,
            Mensaje = "Autenticación correcta.",
            Sesion = sesion
        };
    }

    public static ResultadoAutenticacion Rechazado(string mensaje)
    {
        return new ResultadoAutenticacion
        {
            Exitoso = false,
            Mensaje = mensaje,
            Sesion = null
        };
    }
}
