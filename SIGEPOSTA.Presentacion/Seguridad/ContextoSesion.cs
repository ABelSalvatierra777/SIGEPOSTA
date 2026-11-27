using SIGEPOSTA.Seguridad.Modelos;

namespace SIGEPOSTA.Presentacion.Seguridad
{
    public static class ContextoSesion
    {
        public static SesionUsuario? Actual { get; private set; }

        public static bool SesionIniciada =>
            Actual != null && Actual.EstaAutenticado;

        public static void Iniciar(SesionUsuario sesion)
        {
            if (sesion == null)
                throw new ArgumentNullException(nameof(sesion));

            if (!sesion.EstaAutenticado)
                throw new InvalidOperationException(
                    "No se puede iniciar una sesión no autenticada."
                );

            Actual = sesion;
        }

        public static void Cerrar()
        {
            Actual = null;
        }
    }
}
