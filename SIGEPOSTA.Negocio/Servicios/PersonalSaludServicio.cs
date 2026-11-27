using System.Linq;
using SIGEPOSTA.Entidades.Personas;
using SIGEPOSTA.Datos.Interfaces;
using SIGEPOSTA.Negocio.Interfaces;

namespace SIGEPOSTA.Negocio.Servicios;

public class PersonalSaludServicio : IPersonalSaludServicio
{
    private readonly IPersonalSaludRepositorio _repositorio;

    public PersonalSaludServicio(IPersonalSaludRepositorio repositorio)
    {
        _repositorio = repositorio
            ?? throw new ArgumentNullException(nameof(repositorio));
    }

    public List<PersonalSalud> ObtenerTodos()
    {
        return _repositorio.ObtenerTodos().ToList();
    }

    public PersonalSalud? ObtenerPorId(int id)
    {
        if (id <= 0)
            throw new ArgumentException("El identificador debe ser mayor que cero.");

        return _repositorio.ObtenerPorId(id);
    }

    public void Registrar(PersonalSalud entidad)
    {
        ArgumentNullException.ThrowIfNull(entidad);
        _repositorio.Agregar(entidad);
    }

    public void Actualizar(PersonalSalud entidad)
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

