using SIGEPOSTA.Entidades.Personas;

namespace SIGEPOSTA.Negocio.Interfaces;

public interface IPersonalSaludServicio
{
    List<PersonalSalud> ObtenerTodos();

    PersonalSalud? ObtenerPorId(int id);

    void Registrar(PersonalSalud entidad);

    void Actualizar(PersonalSalud entidad);

    void Eliminar(int id);
}
