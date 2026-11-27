using System.Drawing;
using System.Windows.Forms;

namespace SIGEPOSTA.Presentacion.Estilos;

public static class EstilosFormularios
{
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
        panel.BackColor = Color.White;
        panel.BorderStyle = BorderStyle.FixedSingle;
    }

    public static void AplicarCampo(TextBox campo)
    {
        campo.Font = TemaSIGEPOSTA.Fuente;
        campo.ForeColor = TemaSIGEPOSTA.Texto;
        campo.BackColor = Color.White;
    }
}
