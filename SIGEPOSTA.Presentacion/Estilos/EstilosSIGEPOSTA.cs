using System.Drawing;
using System.Windows.Forms;

namespace SIGEPOSTA.Presentacion.Estilos;

public static class EstilosBotones
{
    // ───────────── Métodos originales (se mantienen) ─────────────
    public static void AplicarPrincipal(Button boton)
    {
        boton.UseVisualStyleBackColor = false;
        boton.BackColor = TemaSIGEPOSTA.Verde;
        boton.ForeColor = Color.White;
        boton.Font = TemaSIGEPOSTA.FuenteBoton;
        boton.FlatStyle = FlatStyle.Flat;
        boton.FlatAppearance.BorderSize = 1;
        boton.FlatAppearance.BorderColor = TemaSIGEPOSTA.Verde;
        boton.FlatAppearance.MouseOverBackColor = TemaSIGEPOSTA.VerdeHover;
        boton.FlatAppearance.MouseDownBackColor = TemaSIGEPOSTA.VerdeActivo;
        boton.TextAlign = ContentAlignment.MiddleCenter;
        boton.Cursor = Cursors.Hand;
        boton.Height = TemaSIGEPOSTA.AlturaBoton;
    }

    public static void AplicarSecundario(Button boton)
    {
        boton.UseVisualStyleBackColor = false;
        boton.BackColor = Color.White;
        boton.ForeColor = TemaSIGEPOSTA.VerdeOscuro;
        boton.Font = TemaSIGEPOSTA.FuenteBoton;
        boton.FlatStyle = FlatStyle.Flat;
        boton.FlatAppearance.BorderSize = 1;
        boton.FlatAppearance.BorderColor = TemaSIGEPOSTA.BordeCampo;
        boton.FlatAppearance.MouseOverBackColor = TemaSIGEPOSTA.VerdeClaro;
        boton.FlatAppearance.MouseDownBackColor = Color.FromArgb(205, 228, 212);
        boton.TextAlign = ContentAlignment.MiddleCenter;
        boton.Cursor = Cursors.Hand;
        boton.Height = TemaSIGEPOSTA.AlturaBoton;
    }

    public static void AplicarMenu(Button boton)
    {
        boton.UseVisualStyleBackColor = false;
        boton.BackColor = TemaSIGEPOSTA.VerdeOscuro;
        boton.ForeColor = Color.White;
        boton.Font = TemaSIGEPOSTA.Fuente;
        boton.FlatStyle = FlatStyle.Flat;
        boton.FlatAppearance.BorderSize = 0;
        boton.FlatAppearance.MouseOverBackColor = Color.FromArgb(40, 98, 68);
        boton.FlatAppearance.MouseDownBackColor = TemaSIGEPOSTA.Verde;
        boton.TextAlign = ContentAlignment.MiddleLeft;
        boton.Padding = new Padding(20, 0, 0, 0);
        boton.Cursor = Cursors.Hand;
        boton.Height = 45;
    }

    public static void AplicarPeligro(Button boton)
    {
        boton.UseVisualStyleBackColor = false;
        boton.BackColor = TemaSIGEPOSTA.Rojo;
        boton.ForeColor = Color.White;
        boton.Font = TemaSIGEPOSTA.FuenteBoton;
        boton.FlatStyle = FlatStyle.Flat;
        boton.FlatAppearance.BorderSize = 1;
        boton.FlatAppearance.BorderColor = TemaSIGEPOSTA.Rojo;
        boton.FlatAppearance.MouseOverBackColor = TemaSIGEPOSTA.RojoHover;
        boton.FlatAppearance.MouseDownBackColor = TemaSIGEPOSTA.RojoActivo;
        boton.TextAlign = ContentAlignment.MiddleCenter;
        boton.Cursor = Cursors.Hand;
        boton.Height = TemaSIGEPOSTA.AlturaBoton;
    }

    // ───────────── Nuevos ─────────────

    // Botón del menú lateral que corresponde a la pantalla actual.
    public static void AplicarMenuActivo(Button boton)
    {
        AplicarMenu(boton);
        boton.BackColor = TemaSIGEPOSTA.Verde;
        boton.Font = TemaSIGEPOSTA.FuenteNegrita;
        boton.FlatAppearance.MouseOverBackColor = TemaSIGEPOSTA.Verde;
    }

    // Quita el estado activo (vuelve al estilo normal del menú).
    public static void QuitarMenuActivo(Button boton)
    {
        AplicarMenu(boton);
    }

    // Mismo ancho para un grupo de botones (Registrar, Actualizar, Eliminar, Limpiar).
    public static void IgualarAncho(int ancho, params Button[] botones)
    {
        foreach (Button b in botones)
        {
            b.Width = ancho;
        }
    }
}