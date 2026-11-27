namespace SIGEPOSTA.Datos.Interfaces;

public interface IRepositorio<T> where T : class
{
    IReadOnlyList<T> ObtenerTodos();
    T? ObtenerPorId(int id);
    void Agregar(T entidad);
    void Actualizar(T entidad);
    void Eliminar(int id);
}
