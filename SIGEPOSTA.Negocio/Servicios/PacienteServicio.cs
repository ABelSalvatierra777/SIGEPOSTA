using System.Linq;
using SIGEPOSTA.Entidades.Personas;
using SIGEPOSTA.Datos.Interfaces;
using SIGEPOSTA.Negocio.Interfaces;

namespace SIGEPOSTA.Negocio.Servicios;

public class PacienteServicio : IPacienteServicio
{
    private readonly IPacienteRepositorio _repositorio;

    public PacienteServicio(IPacienteRepositorio repositorio)
    {
        _repositorio = repositorio
            ?? throw new ArgumentNullException(nameof(repositorio));
    }

    public List<Paciente> ObtenerTodos()
    {
        return _repositorio.ObtenerTodos().ToList();
    }

    public Paciente? ObtenerPorId(int id)
    {
        if (id <= 0)
            throw new ArgumentException("El identificador debe ser mayor que cero.");

        return _repositorio.ObtenerPorId(id);
    }

    public void Registrar(Paciente entidad)
    {
        ArgumentNullException.ThrowIfNull(entidad);
        _repositorio.Agregar(entidad);
    }

    public void Actualizar(Paciente entidad)
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

