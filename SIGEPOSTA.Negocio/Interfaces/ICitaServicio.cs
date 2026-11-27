using SIGEPOSTA.Entidades.Clinica;

namespace SIGEPOSTA.Negocio.Interfaces;

public interface ICitaServicio
{
    List<Cita> ObtenerTodos();

    Cita? ObtenerPorId(int id);

    void Registrar(Cita entidad);

    void Actualizar(Cita entidad);

    void Eliminar(int id);
}
