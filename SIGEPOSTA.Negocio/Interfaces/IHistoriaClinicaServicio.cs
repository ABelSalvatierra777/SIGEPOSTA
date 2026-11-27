using SIGEPOSTA.Entidades.Clinica;

namespace SIGEPOSTA.Negocio.Interfaces;

public interface IHistoriaClinicaServicio
{
    List<HistoriaClinica> ObtenerTodos();
    HistoriaClinica? ObtenerPorId(int id);
    void Registrar(HistoriaClinica entidad);
    void Actualizar(HistoriaClinica entidad);
    void Eliminar(int id);
}
