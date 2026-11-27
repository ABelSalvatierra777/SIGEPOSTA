using System.Drawing;
using System.Windows.Forms;

namespace SIGEPOSTA.Presentacion.Estilos;

public static class EstilosBotones
{
    public static void AplicarPrincipal(Button boton)
    {
        boton.BackColor = TemaSIGEPOSTA.Verde;
        boton.ForeColor = Color.White;
        boton.Font = TemaSIGEPOSTA.FuenteNegrita;
        boton.FlatStyle = FlatStyle.Flat;
        boton.FlatAppearance.BorderSize = 0;
        boton.FlatAppearance.MouseOverBackColor =
            TemaSIGEPOSTA.VerdeOscuro;
        boton.Cursor = Cursors.Hand;
        boton.Height = 42;
    }

    public static void AplicarSecundario(Button boton)
    {
        boton.BackColor = Color.White;
        boton.ForeColor = TemaSIGEPOSTA.VerdeOscuro;
        boton.Font = TemaSIGEPOSTA.FuenteNegrita;
        boton.FlatStyle = FlatStyle.Flat;
        boton.FlatAppearance.BorderColor =
            TemaSIGEPOSTA.Borde;
        boton.FlatAppearance.BorderSize = 1;
        boton.Cursor = Cursors.Hand;
        boton.Height = 40;
    }

    public static void AplicarMenu(Button boton)
    {
        boton.BackColor = TemaSIGEPOSTA.VerdeOscuro;
        boton.ForeColor = Color.White;
        boton.Font = TemaSIGEPOSTA.Fuente;
        boton.FlatStyle = FlatStyle.Flat;
        boton.FlatAppearance.BorderSize = 0;
        boton.FlatAppearance.MouseOverBackColor =
            TemaSIGEPOSTA.Verde;
        boton.TextAlign = ContentAlignment.MiddleLeft;
        boton.Padding = new Padding(20, 0, 0, 0);
        boton.Cursor = Cursors.Hand;
        boton.Height = 45;
    }

    public static void AplicarPeligro(Button boton)
    {
        boton.BackColor = TemaSIGEPOSTA.Rojo;
        boton.ForeColor = Color.White;
        boton.Font = TemaSIGEPOSTA.FuenteNegrita;
        boton.FlatStyle = FlatStyle.Flat;
        boton.FlatAppearance.BorderSize = 0;
        boton.Cursor = Cursors.Hand;
        boton.Height = 42;
    }
}
