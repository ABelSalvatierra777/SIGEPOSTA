using System;
using System.Drawing;
using System.Windows.Forms;

namespace SIGEPOSTA.Presentacion.Estilos;

public static class EstilosFormularios
{
    // ───────────── Métodos originales (se mantienen) ─────────────
    public static void Aplicar(Form formulario)
    {
        formulario.BackColor = TemaSIGEPOSTA.Fondo;
        formulario.Font = TemaSIGEPOSTA.Fuente;
        formulario.ForeColor = TemaSIGEPOSTA.Texto;
        formulario.StartPosition = FormStartPosition.CenterScreen;
    }

    public static void AplicarTitulo(Label etiqueta)
    {
        etiqueta.Font = TemaSIGEPOSTA.TituloModulo;
        etiqueta.ForeColor = TemaSIGEPOSTA.VerdeOscuro;
        etiqueta.AutoSize = true;
    }

    public static void AplicarSubtitulo(Label etiqueta)
    {
        etiqueta.Font = TemaSIGEPOSTA.Subtitulo;
        etiqueta.ForeColor = TemaSIGEPOSTA.TextoSecundario;
        etiqueta.AutoSize = true;
    }

    public static void AplicarPanel(Panel panel)
    {
        // Ahora la tarjeta tiene borde fino dibujado, sin el marco 3D de FixedSingle.
        AplicarTarjeta(panel);
    }

    public static void AplicarCampo(TextBox campo)
    {
        TemaSIGEPOSTA.EstilizarTextBox(campo);
    }

    // ───────────── Nuevos ─────────────

    // Título para usar SOBRE un fondo verde oscuro (banner). Texto blanco.
    public static void AplicarTituloBanner(Label etiqueta)
    {
        etiqueta.Font = TemaSIGEPOSTA.TituloModulo;
        etiqueta.ForeColor = Color.White;
        etiqueta.BackColor = Color.Transparent;
        etiqueta.AutoSize = true;
    }

    public static void AplicarSubtituloBanner(Label etiqueta)
    {
        etiqueta.Font = TemaSIGEPOSTA.Subtitulo;
        etiqueta.ForeColor = Color.FromArgb(200, 225, 210);
        etiqueta.BackColor = Color.Transparent;
        etiqueta.AutoSize = true;
    }

    // Banner de módulo: fondo verde oscuro con título y subtítulo en blanco.
    public static void AplicarBanner(Panel banner, Label titulo, Label subtitulo = null)
    {
        banner.BackColor = TemaSIGEPOSTA.VerdeOscuro;
        banner.BorderStyle = BorderStyle.None;
        banner.Padding = new Padding(24, 0, 24, 0);

        AplicarTituloBanner(titulo);
        titulo.Location = new Point(24, subtitulo == null ? (banner.Height - titulo.Height) / 2 : 14);

        if (subtitulo != null)
        {
            AplicarSubtituloBanner(subtitulo);
            subtitulo.Location = new Point(26, titulo.Bottom + 2);
        }
    }

    // Tarjeta blanca con borde fino (para agrupar campos o tablas).
    public static void AplicarTarjeta(Panel panel)
    {
        panel.BackColor = Color.White;
        panel.BorderStyle = BorderStyle.None;
        panel.Padding = new Padding(16);

        panel.Paint -= PintarBordeTarjeta;
        panel.Paint += PintarBordeTarjeta;
        panel.Invalidate();
    }

    private static void PintarBordeTarjeta(object sender, PaintEventArgs e)
    {
        Control c = (Control)sender;

        using (Pen lapiz = new Pen(TemaSIGEPOSTA.Borde, 1f))
        {
            e.Graphics.DrawRectangle(lapiz, 0, 0, c.Width - 1, c.Height - 1);
        }
    }

    // Título de sección dentro de una tarjeta (ej. "Datos del paciente").
    public static void AplicarTituloSeccion(Label etiqueta)
    {
        etiqueta.Font = new Font("Segoe UI Semibold", 11.5F);
        etiqueta.ForeColor = TemaSIGEPOSTA.VerdeOscuro;
        etiqueta.AutoSize = true;
    }

    // Etiqueta de campo (DNI, Nombres, ...).
    public static void AplicarEtiquetaCampo(Label etiqueta)
    {
        TemaSIGEPOSTA.EstilizarEtiqueta(etiqueta);
        etiqueta.AutoSize = true;
    }

    // Aplica Aplicar() + el tema completo a todos los controles del formulario.
    public static void AplicarCompleto(Form formulario)
    {
        Aplicar(formulario);
        TemaSIGEPOSTA.AplicarTema(formulario);
    }
}