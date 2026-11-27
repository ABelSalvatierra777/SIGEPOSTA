using System.Collections.Generic;
using SIGEPOSTA.Datos.Repositorios;

namespace SIGEPOSTA.Negocio.Servicios
{
    public class ConfiguracionServicio
    {
        private readonly ConfiguracionRepositorio repositorio;

        public ConfiguracionServicio()
        {
            repositorio = new ConfiguracionRepositorio();
        }

        public Dictionary<string, string> ObtenerConfiguracion()
        {
            return repositorio.ObtenerConfiguracion();
        }

        public void GuardarConfiguracion(
            Dictionary<string, string> datos)
        {
            repositorio.GuardarConfiguracion(datos);
        }
    }
}
