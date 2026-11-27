namespace SIGEPOSTA.Respaldos.Modelos;

public class RegistroRespaldo
{
    public string NombreArchivo { get; set; } = string.Empty;

    public string RutaArchivo { get; set; } = string.Empty;

    public DateTime FechaCreacion { get; set; }

    public long TamanoBytes { get; set; }

    public string TamanoMB =>
        (TamanoBytes / 1024d / 1024d).ToString("N2") + " MB";
}
