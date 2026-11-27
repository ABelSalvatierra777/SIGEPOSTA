using System.Linq;
using SIGEPOSTA.Entidades.Clinica;
using SIGEPOSTA.Datos.Interfaces;
using SIGEPOSTA.Negocio.Interfaces;

namespace SIGEPOSTA.Negocio.Servicios;

public class CitaServicio : ICitaServicio
{
    private readonly ICitaRepositorio _repositorio;

    public CitaServicio(ICitaRepositorio repositorio)
    {
        _repositorio = repositorio
            ?? throw new ArgumentNullException(nameof(repositorio));
    }

    public List<Cita> ObtenerTodos()
    {
        return _repositorio.ObtenerTodos().ToList();
    }

    public Cita? ObtenerPorId(int id)
    {
        if (id <= 0)
            throw new ArgumentException("El identificador debe ser mayor que cero.");

        return _repositorio.ObtenerPorId(id);
    }

    public void Registrar(Cita entidad)
    {
        ArgumentNullException.ThrowIfNull(entidad);
        _repositorio.Agregar(entidad);
    }

    public void Actualizar(Cita entidad)
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

