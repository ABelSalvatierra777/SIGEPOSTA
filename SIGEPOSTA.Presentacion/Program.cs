using System;
using System.Windows.Forms;
using SIGEPOSTA.Presentacion.Formularios;

namespace SIGEPOSTA.Presentacion;

internal static class Program
{
    [STAThread]
    static void Main()
    {
ApplicationConfiguration.Initialize();

using (var login = new FormInicioSesion())
{
    if (login.ShowDialog() == DialogResult.OK)
    {
        Application.Run(new FormPrincipal());
    }
}
    }
}
