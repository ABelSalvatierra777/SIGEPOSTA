using SIGEPOSTA.Entidades.Clinica;

namespace SIGEPOSTA.Negocio.Interfaces;

public interface IAtencionServicio
{
    List<Atencion> ObtenerTodos();

    Atencion? ObtenerPorId(int id);

    void Registrar(Atencion entidad);

    void Actualizar(Atencion entidad);

    void Eliminar(int id);
}
