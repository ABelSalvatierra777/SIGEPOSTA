using System.Data;
using SIGEPOSTA.Datos.Repositorios;

namespace SIGEPOSTA.Negocio.Servicios;

public class ReporteServicio
{
    private readonly ReporteRepositorio _repositorio;

    public ReporteServicio(ReporteRepositorio repositorio)
    {
        _repositorio = repositorio
            ?? throw new ArgumentNullException(nameof(repositorio));
    }

    public DataTable ObtenerReporte(
        string tipo,
        DateTime desde,
        DateTime hasta)
    {
        if (string.IsNullOrWhiteSpace(tipo))
            throw new ArgumentException("Seleccione un tipo de reporte.");

        if (desde.Date > hasta.Date)
            throw new ArgumentException("La fecha inicial no puede ser posterior a la fecha final.");

        return _repositorio.ObtenerReporte(tipo, desde.Date, hasta.Date);
    }
}
