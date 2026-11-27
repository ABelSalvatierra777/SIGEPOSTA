using System;
using System.Drawing;
using System.Windows.Forms;
using SIGEPOSTA.Datos.Repositorios;
using SIGEPOSTA.Negocio.Servicios;
using SIGEPOSTA.Seguridad.Servicios;
using SIGEPOSTA.Seguridad.Modelos;

namespace SIGEPOSTA.Presentacion.Formularios;

public class FormInicioSesion : Form
{
    private TextBox txtUsuario = null!;
    private TextBox txtContrasena = null!;
    private Label lblMensaje = null!;
    private Button btnIngresar = null!;

    public static SesionUsuario? UsuarioAutenticado { get; private set; }

    public FormInicioSesion()
    {
        Text = "SIGEPOSTA - Inicio de sesión";
        Size = new Size(950, 580);
        MinimumSize = new Size(850, 520);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        BackColor = Color.White;

        ConstruirInterfaz();
    }

    private void ConstruirInterfaz()
    {
        Panel panelIzquierdo = new Panel
        {
            Dock = DockStyle.Left,
            Width = 390,
            BackColor = Color.FromArgb(27, 65, 54)
        };

        panelIzquierdo.Controls.Add(new Label
        {
            Text = "SIGEPOSTA",
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 30, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(45, 155)
        });

        panelIzquierdo.Controls.Add(new Label
        {
            Text = "SISTEMA DE GESTIÓN\nDE LA POSTA DE SALUD",
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            Location = new Point(50, 220),
            Size = new Size(300, 80)
        });

        panelIzquierdo.Controls.Add(new Label
        {
            Text = "MOLLEPATA",
            ForeColor = Color.FromArgb(190, 220, 205),
            Font = new Font("Segoe UI", 12),
            Location = new Point(50, 315),
            AutoSize = true
        });

        Panel panelDerecho = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White
        };

        panelDerecho.Controls.Add(new Label
        {
            Text = "Bienvenido",
            Font = new Font("Segoe UI", 25, FontStyle.Bold),
            ForeColor = Color.FromArgb(35, 70, 55),
            Location = new Point(85, 85),
            AutoSize = true
        });

        panelDerecho.Controls.Add(new Label
        {
            Text = "Ingresa tus credenciales para continuar",
            Font = new Font("Segoe UI", 10),
            ForeColor = Color.Gray,
            Location = new Point(88, 135),
            AutoSize = true
        });

        panelDerecho.Controls.Add(new Label
        {
            Text = "Usuario",
            Location = new Point(90, 200),
            AutoSize = true,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        });

        txtUsuario = new TextBox
        {
            Location = new Point(90, 230),
            Size = new Size(360, 38),
            Font = new Font("Segoe UI", 12)
        };

        panelDerecho.Controls.Add(txtUsuario);

        panelDerecho.Controls.Add(new Label
        {
            Text = "Contraseña",
            Location = new Point(90, 290),
            AutoSize = true,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        });

        txtContrasena = new TextBox
        {
            Location = new Point(90, 320),
            Size = new Size(360, 38),
            Font = new Font("Segoe UI", 12),
            UseSystemPasswordChar = true
        };

        panelDerecho.Controls.Add(txtContrasena);

        btnIngresar = new Button
        {
            Text = "INICIAR SESIÓN",
            Location = new Point(90, 390),
            Size = new Size(360, 45),
            BackColor = Color.FromArgb(43, 125, 91),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            Cursor = Cursors.Hand
        };

        btnIngresar.Click += BtnIngresar_Click;
        panelDerecho.Controls.Add(btnIngresar);

        lblMensaje = new Label
        {
            Text = "",
            Location = new Point(90, 450),
            Size = new Size(360, 45),
            ForeColor = Color.Firebrick,
            Font = new Font("Segoe UI", 9)
        };

        panelDerecho.Controls.Add(lblMensaje);

        Controls.Add(panelDerecho);
        Controls.Add(panelIzquierdo);

        AcceptButton = btnIngresar;
    }

    private void BtnIngresar_Click(object? sender, EventArgs e)
    {
        string usuario = txtUsuario.Text.Trim();
        string contrasena = txtContrasena.Text;

        if (string.IsNullOrWhiteSpace(usuario))
        {
            lblMensaje.Text = "Ingrese su nombre de usuario.";
            txtUsuario.Focus();
            return;
        }

        if (string.IsNullOrWhiteSpace(contrasena))
        {
            lblMensaje.Text = "Ingrese su contraseña.";
            txtContrasena.Focus();
            return;
        }

        try
        {
            btnIngresar.Enabled = false;
            lblMensaje.ForeColor = Color.Gray;
            lblMensaje.Text = "Verificando credenciales...";

            var repositorio = new UsuarioRepositorio();
            var encriptador = new EncriptadorContrasenas();

            var autenticacion = new AutenticacionServicio(
                repositorio,
                encriptador
            );

            var resultado = autenticacion.Autenticar(usuario, contrasena);

            if (!resultado.Exitoso)
            {
                lblMensaje.ForeColor = Color.Firebrick;
                lblMensaje.Text = resultado.Mensaje;
                txtContrasena.Clear();
                txtContrasena.Focus();
                return;
            }

            UsuarioAutenticado = resultado.Sesion;
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            lblMensaje.ForeColor = Color.Firebrick;
            lblMensaje.Text = "No se pudo iniciar sesión.";

            MessageBox.Show(
                "Ocurrió un error al autenticar:\n\n" + ex.Message,
                "Error de autenticación",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }
        finally
        {
            btnIngresar.Enabled = true;
        }
    }
}

