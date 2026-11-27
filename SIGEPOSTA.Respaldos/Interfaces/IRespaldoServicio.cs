using SIGEPOSTA.Respaldos.Modelos;

namespace SIGEPOSTA.Respaldos.Interfaces;

public interface IRespaldoServicio
{
    ResultadoRespaldo CrearRespaldo(string carpetaDestino);

    ResultadoRespaldo RestaurarRespaldo(string archivoRespaldo);

    List<RegistroRespaldo> ListarRespaldos(string carpetaDestino);
}
