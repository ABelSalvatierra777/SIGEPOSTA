using System.Linq;
using SIGEPOSTA.Entidades.Seguridad;
using SIGEPOSTA.Datos.Interfaces;
using SIGEPOSTA.Negocio.Interfaces;

namespace SIGEPOSTA.Negocio.Servicios;

public class UsuarioServicio : IUsuarioServicio
{
    private readonly IUsuarioRepositorio _repositorio;

    public UsuarioServicio(IUsuarioRepositorio repositorio)
    {
        _repositorio = repositorio
            ?? throw new ArgumentNullException(nameof(repositorio));
    }

    public List<Usuario> ObtenerTodos()
    {
        return _repositorio.ObtenerTodos().ToList();
    }

    public Usuario? ObtenerPorId(int id)
    {
        if (id <= 0)
            throw new ArgumentException("El identificador debe ser mayor que cero.");

        return _repositorio.ObtenerPorId(id);
    }

    public void Registrar(Usuario entidad)
    {
        ArgumentNullException.ThrowIfNull(entidad);
        _repositorio.Agregar(entidad);
    }

    public void Actualizar(Usuario entidad)
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

