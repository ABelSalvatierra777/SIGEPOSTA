using SIGEPOSTA.Entidades.Clinica;
using SIGEPOSTA.Datos.Interfaces;
using SIGEPOSTA.Negocio.Interfaces;

namespace SIGEPOSTA.Negocio.Servicios;

public class HistoriaClinicaServicio : IHistoriaClinicaServicio
{
    private readonly IHistoriaClinicaRepositorio repositorio;

    public HistoriaClinicaServicio(IHistoriaClinicaRepositorio repositorio)
    {
        this.repositorio = repositorio
            ?? throw new ArgumentNullException(nameof(repositorio));
    }

    public List<HistoriaClinica> ObtenerTodos()
        => repositorio.ObtenerTodos().ToList();

    public HistoriaClinica? ObtenerPorId(int id)
        => repositorio.ObtenerPorId(id);

    public void Registrar(HistoriaClinica entidad)
        => repositorio.Agregar(entidad);

    public void Actualizar(HistoriaClinica entidad)
        => repositorio.Actualizar(entidad);

    public void Eliminar(int id)
        => repositorio.Eliminar(id);
}
