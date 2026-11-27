using SIGEPOSTA.Entidades.Personas;

namespace SIGEPOSTA.Negocio.Interfaces;

public interface IPacienteServicio
{
    List<Paciente> ObtenerTodos();

    Paciente? ObtenerPorId(int id);

    void Registrar(Paciente entidad);

    void Actualizar(Paciente entidad);

    void Eliminar(int id);
}
