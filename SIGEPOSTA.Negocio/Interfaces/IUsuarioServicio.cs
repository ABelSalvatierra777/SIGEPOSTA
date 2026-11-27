using SIGEPOSTA.Entidades.Seguridad;

namespace SIGEPOSTA.Negocio.Interfaces;

public interface IUsuarioServicio
{
    List<Usuario> ObtenerTodos();

    Usuario? ObtenerPorId(int id);

    void Registrar(Usuario entidad);

    void Actualizar(Usuario entidad);

    void Eliminar(int id);
}
