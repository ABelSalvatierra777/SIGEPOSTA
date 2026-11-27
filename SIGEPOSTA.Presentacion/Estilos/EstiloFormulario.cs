using System.Drawing;
using System.Windows.Forms;

namespace SIGEPOSTA.Presentacion.Estilos;

public static class EstiloFormulario
{
    public static void Aplicar(Form formulario)
    {
        formulario.BackColor = TemaSIGEPOSTA.Fondo;
        formulario.Font = TemaSIGEPOSTA.Fuente;
        formulario.ForeColor = TemaSIGEPOSTA.Texto;

        AplicarControles(formulario.Controls);
    }

    private static void AplicarControles(Control.ControlCollection controles)
    {
        foreach (Control control in controles)
        {
            // Etiquetas
            if (control is Label etiqueta)
            {
                etiqueta.ForeColor = TemaSIGEPOSTA.Texto;
                etiqueta.Font = TemaSIGEPOSTA.Fuente;
            }

            // Cuadros de texto
            else if (control is TextBox texto)
            {
                texto.Font = TemaSIGEPOSTA.Fuente;
                texto.BackColor = Color.White;
                texto.ForeColor = TemaSIGEPOSTA.Texto;
            }

            // Listas desplegables
            else if (control is ComboBox combo)
            {
                combo.Font = TemaSIGEPOSTA.Fuente;
                combo.BackColor = Color.White;
                combo.ForeColor = TemaSIGEPOSTA.Texto;
                combo.FlatStyle = FlatStyle.Flat;
            }

            // Botones
            else if (control is Button boton)
            {
                boton.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
                boton.FlatStyle = FlatStyle.Flat;
                boton.FlatAppearance.BorderSize = 0;
                boton.BackColor = TemaSIGEPOSTA.VerdeOscuro;
                boton.ForeColor = Color.White;
                boton.Cursor = Cursors.Hand;
            }

            // Aplicar también a los controles internos
            if (control.HasChildren)
            {
                AplicarControles(control.Controls);
            }
        }
    }
}