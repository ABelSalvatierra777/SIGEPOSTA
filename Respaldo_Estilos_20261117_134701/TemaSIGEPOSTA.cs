using System.Drawing;

namespace SIGEPOSTA.Presentacion.Estilos;

public static class TemaSIGEPOSTA
{
    // Colores institucionales
    public static readonly Color VerdeOscuro =
        Color.FromArgb(31, 77, 54);

    public static readonly Color Verde =
        Color.FromArgb(47, 125, 69);

    public static readonly Color VerdeClaro =
        Color.FromArgb(225, 240, 229);

    public static readonly Color Fondo =
        Color.FromArgb(245, 247, 246);

    public static readonly Color Blanco =
        Color.White;

    public static readonly Color Texto =
        Color.FromArgb(45, 55, 50);

    public static readonly Color TextoSecundario =
        Color.FromArgb(110, 120, 115);

    public static readonly Color Borde =
        Color.FromArgb(225, 230, 226);

    public static readonly Color Rojo =
        Color.FromArgb(190, 55, 55);

    // Tipografía
    public static readonly Font Fuente =
        new Font("Segoe UI", 10F);

    public static readonly Font FuenteNegrita =
        new Font("Segoe UI", 10F, FontStyle.Bold);

    public static readonly Font Titulo =
        new Font("Segoe UI", 22F, FontStyle.Bold);

    public static readonly Font Subtitulo =
        new Font("Segoe UI", 12F, FontStyle.Regular);

    public static readonly Font TituloModulo =
        new Font("Segoe UI", 18F, FontStyle.Bold);
}
