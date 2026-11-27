namespace SIGEPOSTA.Respaldos.Modelos;

public class ResultadoRespaldo
{
    public bool Exitoso { get; set; }

    public string Mensaje { get; set; } = string.Empty;

    public string? RutaArchivo { get; set; }

    public DateTime FechaOperacion { get; set; } = DateTime.Now;
}
