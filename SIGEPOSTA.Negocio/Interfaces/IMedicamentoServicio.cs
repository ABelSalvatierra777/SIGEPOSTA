using SIGEPOSTA.Entidades.Inventario;

namespace SIGEPOSTA.Negocio.Interfaces;

public interface IMedicamentoServicio
{
    List<Medicamento> ObtenerTodos();

    Medicamento? ObtenerPorId(int id);

    void Registrar(Medicamento entidad);

    void Actualizar(Medicamento entidad);

    void Eliminar(int id);
}
