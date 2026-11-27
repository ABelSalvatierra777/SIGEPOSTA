namespace SIGEPOSTA.Negocio.Interfaces
{
    public interface IAutenticacionServicio
    {
        SIGEPOSTA.Seguridad.Modelos.ResultadoAutenticacion Autenticar(
            string nombreUsuario,
            string contrasena
        );
    }
}
