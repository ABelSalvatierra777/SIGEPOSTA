using System.Linq;
using SIGEPOSTA.Entidades.Inventario;
using SIGEPOSTA.Datos.Interfaces;
using SIGEPOSTA.Negocio.Interfaces;

namespace SIGEPOSTA.Negocio.Servicios;

public class MedicamentoServicio : IMedicamentoServicio
{
    private readonly IMedicamentoRepositorio _repositorio;

    public MedicamentoServicio(IMedicamentoRepositorio repositorio)
    {
        _repositorio = repositorio
            ?? throw new ArgumentNullException(nameof(repositorio));
    }

    public List<Medicamento> ObtenerTodos()
    {
        return _repositorio.ObtenerTodos().ToList();
    }

    public Medicamento? ObtenerPorId(int id)
    {
        if (id <= 0)
            throw new ArgumentException("El identificador debe ser mayor que cero.");

        return _repositorio.ObtenerPorId(id);
    }

    public void Registrar(Medicamento entidad)
    {
        ArgumentNullException.ThrowIfNull(entidad);
        _repositorio.Agregar(entidad);
    }

    public void Actualizar(Medicamento entidad)
    {
        ArgumentNullException.ThrowIfNull(entidad);
        _repositorio.Actualizar(entidad);
    }

    public void Eliminar(int id)
    {
        if (id <= 0)
            throw new ArgumentException("El identificador debe ser mayor que cero.");

        _repositorio.Eliminar(id);
    }
}

