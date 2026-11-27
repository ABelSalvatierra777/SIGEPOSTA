#nullable enable

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Windows.Forms;
using SIGEPOSTA.Datos.Repositorios;
using SIGEPOSTA.Negocio.Servicios;
using SIGEPOSTA.Seguridad.Servicios;
using SIGEPOSTA.Seguridad.Modelos;

namespace SIGEPOSTA.Presentacion.Formularios
{
    internal static class LoginPaleta
    {
        public static readonly Color Primario = Color.FromArgb(43, 125, 91);
        public static readonly Color PrimarioHover = Color.FromArgb(34, 106, 77);
        public static readonly Color PrimarioActivo = Color.FromArgb(27, 85, 62);
        public static readonly Color Oscuro = Color.FromArgb(27, 65, 54);
        public static readonly Color Texto = Color.FromArgb(28, 40, 35);
        public static readonly Color TextoSuave = Color.FromArgb(110, 125, 119);
        public static readonly Color Borde = Color.FromArgb(215, 225, 220);
        public static readonly Color CampoFondo = Color.FromArgb(245, 248, 246);
        public static readonly Color FondoPagina = Color.FromArgb(240, 244, 242);
        public static readonly Color Error = Color.FromArgb(200, 55, 55);
        public static readonly Color Exito = Color.FromArgb(43, 125, 91);
    }

    internal static class LoginDibujo
    {
        public static GraphicsPath Redondeado(Rectangle rectangulo, int radio)
        {
            GraphicsPath path = new GraphicsPath();

            if (rectangulo.Width <= 0 || rectangulo.Height <= 0)
                return path;

            int d = Math.Min(radio * 2,
                Math.Min(rectangulo.Width, rectangulo.Height));

            path.AddArc(rectangulo.X, rectangulo.Y, d, d, 180, 90);
            path.AddArc(rectangulo.Right - d, rectangulo.Y, d, d, 270, 90);
            path.AddArc(rectangulo.Right - d, rectangulo.Bottom - d, d, d, 0, 90);
            path.AddArc(rectangulo.X, rectangulo.Bottom - d, d, d, 90, 90);
            path.CloseFigure();

            return path;
        }
    }

    // ───────────────────────── PANEL IZQUIERDO ─────────────────────────
    internal class LoginPanelHero : Panel
    {
        private Image? _imagen;

        [System.ComponentModel.DesignerSerializationVisibility(
            System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public Image? Imagen
        {
            get => _imagen;
            set
            {
                if (ReferenceEquals(_imagen, value))
                    return;

                _imagen?.Dispose();
                _imagen = value;
                Invalidate();
            }
        }

        public LoginPanelHero()
        {
            DoubleBuffered = true;
            ResizeRedraw = true;
            BackColor = LoginPaleta.Oscuro;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (Width <= 0 || Height <= 0)
                return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            g.Clear(LoginPaleta.Oscuro);

            if (_imagen != null)
            {
                float escala = Math.Max(
                    (float)Width / _imagen.Width,
                    (float)Height / _imagen.Height);

                float srcW = Width / escala;
                float srcH = Height / escala;

                RectangleF origen = new RectangleF(
                    (_imagen.Width - srcW) / 2f,
                    (_imagen.Height - srcH) / 2f,
                    srcW,
                    srcH);

                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(_imagen,
                    new RectangleF(0, 0, Width, Height),
                    origen, GraphicsUnit.Pixel);
            }

            using (SolidBrush tinte = new SolidBrush(
                Color.FromArgb(75, 27, 65, 54)))
                g.FillRectangle(tinte, ClientRectangle);

            using (LinearGradientBrush gradiente =
                new LinearGradientBrush(
                    ClientRectangle,
                    Color.FromArgb(40, 10, 35, 28),
                    Color.FromArgb(240, 12, 40, 32), 90f))
                g.FillRectangle(gradiente, ClientRectangle);

            using (SolidBrush circulo = new SolidBrush(
                Color.FromArgb(18, 255, 255, 255)))
            {
                g.FillEllipse(circulo, Width - 220, -120, 340, 340);
                g.FillEllipse(circulo, -90, Height / 2 - 60, 220, 220);
            }

            Rectangle badge = new Rectangle(48, 48, 215, 36);

            using (GraphicsPath path = LoginDibujo.Redondeado(badge, 18))
            using (SolidBrush fondo = new SolidBrush(
                Color.FromArgb(45, 255, 255, 255)))
            using (Pen borde = new Pen(
                Color.FromArgb(70, 255, 255, 255)))
            {
                g.FillPath(fondo, path);
                g.DrawPath(borde, path);
            }

            using (SolidBrush punto = new SolidBrush(
                Color.FromArgb(120, 230, 175)))
                g.FillEllipse(punto, 64, 61, 9, 9);

            using (Font fuente = new Font("Segoe UI", 9, FontStyle.Bold))
                TextRenderer.DrawText(g, "SALUD · MOLLEPATA",
                    fuente, new Point(80, 56), Color.White);

            int baseY = Math.Max(120, Height - 250);

            using (Font fuente = new Font("Segoe UI", 40, FontStyle.Bold))
                TextRenderer.DrawText(g, "SIGEPOSTA",
                    fuente, new Point(42, baseY), Color.White);

            using (SolidBrush barra = new SolidBrush(
                Color.FromArgb(120, 230, 175)))
                g.FillRectangle(barra, 50, baseY + 82, 56, 4);

            using (Font fuente = new Font("Segoe UI", 15))
            using (SolidBrush pincel = new SolidBrush(
                Color.FromArgb(235, 255, 255, 255)))
                g.DrawString(
                    "Sistema de gestión de la posta de salud",
                    fuente, pincel,
                    new RectangleF(50, baseY + 102,
                        Math.Max(100, Width - 100), 70));

            using (Font fuente = new Font("Segoe UI", 10))
                TextRenderer.DrawText(g,
                    "Atención · Citas · Historias clínicas · Farmacia",
                    fuente, new Point(50, baseY + 160),
                    Color.FromArgb(190, 220, 205));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Image? imagen = _imagen;
                _imagen = null;
                imagen?.Dispose();
            }

            base.Dispose(disposing);
        }
    }

    // ───────────────────────── TARJETA CON SOMBRA ─────────────────────────
    internal class LoginTarjeta : Panel
    {
        public LoginTarjeta()
        {
            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw,
                true);

            BackColor = LoginPaleta.FondoPagina;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(BackColor);

            Rectangle tarjeta = new Rectangle(16, 10, Width - 32, Height - 30);

            // Sombra suave en capas
            for (int i = 10; i >= 1; i--)
            {
                Rectangle s = new Rectangle(
                    tarjeta.X - i,
                    tarjeta.Y - i + 8,
                    tarjeta.Width + i * 2,
                    tarjeta.Height + i * 2);

                using (GraphicsPath p = LoginDibujo.Redondeado(s, 22 + i))
                using (SolidBrush b = new SolidBrush(
                    Color.FromArgb(5, 15, 50, 35)))
                    g.FillPath(b, p);
            }

            using (GraphicsPath p = LoginDibujo.Redondeado(tarjeta, 22))
            using (SolidBrush b = new SolidBrush(Color.White))
            using (Pen borde = new Pen(Color.FromArgb(228, 235, 231), 1f))
            {
                g.FillPath(b, p);
                g.DrawPath(borde, p);
            }

            // Franja de color en la parte superior de la tarjeta
            Rectangle franja = new Rectangle(tarjeta.X + 40, tarjeta.Y, tarjeta.Width - 80, 4);
            using (LinearGradientBrush grad = new LinearGradientBrush(
                franja,
                Color.FromArgb(70, 170, 125),
                LoginPaleta.Primario,
                0f))
                g.FillRectangle(grad, franja);
        }
    }

    // ───────────────────────── LOGO (CRUZ MÉDICA) ─────────────────────────
    internal class LoginLogo : Control
    {
        public LoginLogo()
        {
            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer,
                true);

            BackColor = Color.White;
            Size = new Size(56, 56);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.White);

            Rectangle r = new Rectangle(0, 0, Width - 1, Height - 1);

            using (GraphicsPath p = LoginDibujo.Redondeado(r, 16))
            using (LinearGradientBrush grad = new LinearGradientBrush(
                r,
                Color.FromArgb(62, 155, 115),
                LoginPaleta.PrimarioHover,
                45f))
                g.FillPath(grad, p);

            int cx = Width / 2;
            int cy = Height / 2;

            using (SolidBrush blanco = new SolidBrush(Color.White))
            {
                g.FillRectangle(blanco, cx - 4, cy - 15, 8, 30);
                g.FillRectangle(blanco, cx - 15, cy - 4, 30, 8);
            }
        }
    }

    // ───────────────────────── CAMPO DE TEXTO ─────────────────────────
    internal class LoginCampo : Panel
    {
        public TextBox Texto { get; private set; } = null!;

        private Label _icono = null!;
        private Label? _ojo;
        private bool _enfocado;
        private bool _hover;

        public LoginCampo(string icono, string placeholder, bool esContrasena)
        {
            DoubleBuffered = true;
            Height = 56;
            BackColor = Color.White;

            _icono = new Label
            {
                Text = icono,
                Font = new Font("Segoe MDL2 Assets", 14),
                ForeColor = LoginPaleta.TextoSuave,
                BackColor = LoginPaleta.CampoFondo,
                AutoSize = false,
                Size = new Size(34, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Texto = new TextBox
            {
                BorderStyle = BorderStyle.None,
                BackColor = LoginPaleta.CampoFondo,
                ForeColor = LoginPaleta.Texto,
                Font = new Font("Segoe UI", 11.5f),
                PlaceholderText = placeholder,
                UseSystemPasswordChar = esContrasena
            };

            Texto.Enter += AlEntrar;
            Texto.Leave += AlSalir;
            Texto.MouseEnter += AlEntrarMouse;
            Texto.MouseLeave += AlSalirMouse;

            Controls.Add(_icono);
            Controls.Add(Texto);

            if (esContrasena)
            {
                _ojo = new Label
                {
                    Text = "\uE7B3",
                    Font = new Font("Segoe MDL2 Assets", 13),
                    ForeColor = LoginPaleta.TextoSuave,
                    BackColor = LoginPaleta.CampoFondo,
                    AutoSize = false,
                    Size = new Size(34, 30),
                    Cursor = Cursors.Hand,
                    TextAlign = ContentAlignment.MiddleCenter
                };

                _ojo.Click += AlTocarOjo;
                Controls.Add(_ojo);
            }

            Click += AlHacerClic;
            _icono.Click += AlHacerClic;
            MouseEnter += AlEntrarMouse;
            MouseLeave += AlSalirMouse;
            Resize += AlCambiarTamano;

            Acomodar();
        }

        private Color ColorFondo()
        {
            return _enfocado ? Color.White : LoginPaleta.CampoFondo;
        }

        private void AplicarFondo()
        {
            Color fondo = ColorFondo();
            _icono.BackColor = fondo;
            Texto.BackColor = fondo;

            if (_ojo != null)
                _ojo.BackColor = fondo;
        }

        private void AlEntrar(object? sender, EventArgs e)
        {
            _enfocado = true;
            _icono.ForeColor = LoginPaleta.Primario;
            AplicarFondo();
            Invalidate();
        }

        private void AlSalir(object? sender, EventArgs e)
        {
            _enfocado = false;
            _icono.ForeColor = LoginPaleta.TextoSuave;
            AplicarFondo();
            Invalidate();
        }

        private void AlEntrarMouse(object? sender, EventArgs e)
        {
            _hover = true;
            Invalidate();
        }

        private void AlSalirMouse(object? sender, EventArgs e)
        {
            _hover = false;
            Invalidate();
        }

        private void AlTocarOjo(object? sender, EventArgs e)
        {
            Texto.UseSystemPasswordChar = !Texto.UseSystemPasswordChar;

            if (_ojo != null)
            {
                _ojo.Text = Texto.UseSystemPasswordChar ? "\uE7B3" : "\uED1A";
                _ojo.ForeColor = Texto.UseSystemPasswordChar
                    ? LoginPaleta.TextoSuave
                    : LoginPaleta.Primario;
            }

            Texto.Focus();
        }

        private void AlHacerClic(object? sender, EventArgs e)
        {
            Texto.Focus();
        }

        private void AlCambiarTamano(object? sender, EventArgs e)
        {
            Acomodar();
        }

        private void Acomodar()
        {
            int derecha = _ojo != null ? 50 : 18;

            _icono.Location = new Point(14, (Height - _icono.Height) / 2);

            Texto.Width = Math.Max(1, Width - 56 - derecha);
            Texto.Location = new Point(56, (Height - Texto.Height) / 2);

            if (_ojo != null)
                _ojo.Location = new Point(
                    Math.Max(0, Width - 46),
                    (Height - _ojo.Height) / 2);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (Width < 10 || Height < 10)
                return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle rect = new Rectangle(3, 3, Width - 7, Height - 7);

            using (GraphicsPath path = LoginDibujo.Redondeado(rect, 13))
            {
                using (SolidBrush fondo = new SolidBrush(ColorFondo()))
                    g.FillPath(fondo, path);

                if (_enfocado)
                {
                    using (Pen halo = new Pen(
                        Color.FromArgb(40, LoginPaleta.Primario), 5f))
                        g.DrawPath(halo, path);
                }

                Color colorBorde = _enfocado
                    ? LoginPaleta.Primario
                    : _hover
                        ? Color.FromArgb(170, 195, 183)
                        : LoginPaleta.Borde;

                using (Pen borde = new Pen(colorBorde, _enfocado ? 1.8f : 1.2f))
                    g.DrawPath(borde, path);
            }
        }
    }

    // ───────────────────────── BOTÓN ─────────────────────────
    internal class LoginBoton : Button
    {
        private bool _hover;
        private bool _presionado;

        public LoginBoton()
        {
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            Cursor = Cursors.Hand;
            ForeColor = Color.White;
            Font = new Font("Segoe UI", 11, FontStyle.Bold);

            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw,
                true);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            _hover = true;
            Invalidate();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            _hover = false;
            _presionado = false;
            Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                _presionado = true;

            Invalidate();
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            _presionado = false;
            Invalidate();
            base.OnMouseUp(e);
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            _presionado = false;
            Invalidate();
            base.OnEnabledChanged(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Width <= 0 || Height <= 0)
                return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Parent != null ? Parent.BackColor : Color.White);

            Color c1;
            Color c2;

            if (!Enabled)
            {
                c1 = Color.FromArgb(165, 200, 186);
                c2 = Color.FromArgb(150, 188, 172);
            }
            else if (_presionado)
            {
                c1 = LoginPaleta.PrimarioActivo;
                c2 = LoginPaleta.PrimarioActivo;
            }
            else if (_hover)
            {
                c1 = Color.FromArgb(40, 118, 85);
                c2 = Color.FromArgb(30, 98, 71);
            }
            else
            {
                c1 = Color.FromArgb(54, 145, 106);
                c2 = Color.FromArgb(38, 112, 82);
            }

            Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);

            // Sombra del botón
            if (Enabled && !_presionado)
            {
                Rectangle sombra = new Rectangle(4, 5, Width - 9, Height - 4);
                using (GraphicsPath ps = LoginDibujo.Redondeado(sombra, 12))
                using (SolidBrush bs = new SolidBrush(
                    Color.FromArgb(_hover ? 55 : 35, 43, 125, 91)))
                    g.FillPath(bs, ps);
            }

            Rectangle cuerpo = _presionado
                ? new Rectangle(0, 1, Width - 1, Height - 3)
                : new Rectangle(0, 0, Width - 1, Height - 5);

            using (GraphicsPath path = LoginDibujo.Redondeado(cuerpo, 12))
            using (LinearGradientBrush brush = new LinearGradientBrush(
                cuerpo, c1, c2, 90f))
                g.FillPath(brush, path);

            TextRenderer.DrawText(
                g, Text, Font, cuerpo, Color.White,
                TextFormatFlags.HorizontalCenter |
                TextFormatFlags.VerticalCenter |
                TextFormatFlags.SingleLine);
        }
    }

    // ───────────────────────── FORMULARIO ─────────────────────────
    public class FormInicioSesion : Form
    {
        private const int MargenX = 56;
        private const int AnchoContenido = 388;

        private TextBox txtUsuario = null!;
        private TextBox txtContrasena = null!;
        private Label lblMensaje = null!;
        private LoginBoton btnIngresar = null!;
        private LoginPanelHero panelHero = null!;
        private Panel panelDerecho = null!;
        private LoginTarjeta contenedor = null!;

        public static SesionUsuario? UsuarioAutenticado
        {
            get;
            private set;
        }

        public FormInicioSesion()
        {
            Text = "SIGEPOSTA - Inicio de sesión";
            WindowState = FormWindowState.Maximized;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;
            MinimumSize = new Size(950, 680);
            DoubleBuffered = true;

            UsuarioAutenticado = null;

            ConstruirInterfaz();

            Shown += AlMostrar;
            Resize += AlCambiarTamano;
        }

        private static string? BuscarImagenLogin()
        {
            string[] nombres =
            {
                "login.jpeg",
                "login.jpg",
                "login.png"
            };

            DirectoryInfo? dir = new DirectoryInfo(Application.StartupPath);

            while (dir != null)
            {
                foreach (string nombre in nombres)
                {
                    string ruta = Path.Combine(dir.FullName, "Recursos", nombre);

                    if (File.Exists(ruta))
                        return ruta;
                }

                dir = dir.Parent;
            }

            return null;
        }

        private Label CrearEtiqueta(
            string texto, Font fuente, Color color, int x, int y)
        {
            return new Label
            {
                Text = texto,
                Font = fuente,
                ForeColor = color,
                BackColor = Color.White,
                AutoSize = true,
                Location = new Point(x, y)
            };
        }

        private void ConstruirInterfaz()
        {
            panelHero = new LoginPanelHero
            {
                Dock = DockStyle.Left,
                Width = 550
            };

            string? ruta = BuscarImagenLogin();

            if (!string.IsNullOrEmpty(ruta))
            {
                try
                {
                    using (Image imagen = Image.FromFile(ruta))
                        panelHero.Imagen = new Bitmap(imagen);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }

            panelDerecho = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = LoginPaleta.FondoPagina
            };

            contenedor = new LoginTarjeta
            {
                Size = new Size(500, 620),
                BackColor = LoginPaleta.FondoPagina
            };

            // Logo + nombre
            contenedor.Controls.Add(new LoginLogo
            {
                Location = new Point(MargenX, 46)
            });

            contenedor.Controls.Add(CrearEtiqueta(
                "SIGEPOSTA",
                new Font("Segoe UI", 14, FontStyle.Bold),
                LoginPaleta.Texto,
                MargenX + 68, 50));

            contenedor.Controls.Add(CrearEtiqueta(
                "Posta de Salud Mollepata",
                new Font("Segoe UI", 9),
                LoginPaleta.TextoSuave,
                MargenX + 70, 76));

            // Título
            contenedor.Controls.Add(CrearEtiqueta(
                "Bienvenido de nuevo",
                new Font("Segoe UI", 22, FontStyle.Bold),
                LoginPaleta.Texto,
                MargenX - 2, 130));

            contenedor.Controls.Add(CrearEtiqueta(
                "Ingresa tus credenciales para continuar",
                new Font("Segoe UI", 10),
                LoginPaleta.TextoSuave,
                MargenX, 176));

            // Usuario
            contenedor.Controls.Add(CrearEtiqueta(
                "Usuario",
                new Font("Segoe UI Semibold", 10),
                LoginPaleta.Texto,
                MargenX, 222));

            LoginCampo campoUsuario = new LoginCampo(
                "\uE77B", "Ingrese su nombre de usuario", false)
            {
                Location = new Point(MargenX, 248),
                Size = new Size(AnchoContenido, 56)
            };

            txtUsuario = campoUsuario.Texto;
            contenedor.Controls.Add(campoUsuario);

            // Contraseña
            contenedor.Controls.Add(CrearEtiqueta(
                "Contraseña",
                new Font("Segoe UI Semibold", 10),
                LoginPaleta.Texto,
                MargenX, 322));

            LoginCampo campoClave = new LoginCampo(
                "\uE72E", "Ingrese su contraseña", true)
            {
                Location = new Point(MargenX, 348),
                Size = new Size(AnchoContenido, 56)
            };

            txtContrasena = campoClave.Texto;
            contenedor.Controls.Add(campoClave);

            // Botón
            btnIngresar = new LoginBoton
            {
                Text = "INICIAR SESIÓN",
                Location = new Point(MargenX, 432),
                Size = new Size(AnchoContenido, 58),
                BackColor = Color.White
            };

            btnIngresar.Click += BtnIngresar_Click;
            contenedor.Controls.Add(btnIngresar);

            // Mensaje
            lblMensaje = new Label
            {
                Text = "",
                Location = new Point(MargenX, 500),
                Size = new Size(AnchoContenido, 26),
                ForeColor = LoginPaleta.Error,
                BackColor = Color.White,
                Font = new Font("Segoe UI", 9.5f),
                TextAlign = ContentAlignment.MiddleCenter
            };

            contenedor.Controls.Add(lblMensaje);

            // Separador
            contenedor.Controls.Add(new Panel
            {
                BackColor = Color.FromArgb(234, 239, 236),
                Location = new Point(MargenX, 540),
                Size = new Size(AnchoContenido, 1)
            });

            // Ayuda
            contenedor.Controls.Add(new Label
            {
                Text = "¿Problemas para ingresar? Contacte al administrador",
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = LoginPaleta.TextoSuave,
                BackColor = Color.White,
                AutoSize = false,
                Location = new Point(MargenX, 550),
                Size = new Size(AnchoContenido, 22),
                TextAlign = ContentAlignment.MiddleCenter
            });

            panelDerecho.Controls.Add(contenedor);

            Label pie = new Label
            {
                Text = "© 2026 SIGEPOSTA · Posta de Salud Mollepata",
                Dock = DockStyle.Bottom,
                Height = 48,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = LoginPaleta.TextoSuave,
                BackColor = LoginPaleta.FondoPagina
            };

            panelDerecho.Controls.Add(pie);

            Controls.Add(panelDerecho);
            Controls.Add(panelHero);

            AcceptButton = btnIngresar;

            panelDerecho.Resize += AlCambiarTamanoDerecho;

            Distribuir();
        }

        // ── EVENTOS ──
        private void AlMostrar(object? sender, EventArgs e)
        {
            Distribuir();
            txtUsuario.Focus();
        }

        private void AlCambiarTamano(object? sender, EventArgs e)
        {
            Distribuir();
        }

        private void AlCambiarTamanoDerecho(object? sender, EventArgs e)
        {
            CentrarContenedor();
        }

        // ── RESPONSIVE ──
        private void Distribuir()
        {
            if (panelHero == null || panelDerecho == null || contenedor == null)
                return;

            int anchoFormulario = ClientSize.Width;
            int anchoDisponibleHero = Math.Max(350, anchoFormulario - 520);
            int anchoHero = Math.Min((int)(anchoFormulario * 0.52), anchoDisponibleHero);

            panelHero.Width = anchoHero;

            CentrarContenedor();
        }

        private void CentrarContenedor()
        {
            if (panelDerecho == null || contenedor == null)
                return;

            int x = (panelDerecho.ClientSize.Width - contenedor.Width) / 2;
            int espacioVertical = panelDerecho.ClientSize.Height - 48;
            int y = (espacioVertical - contenedor.Height) / 2;

            contenedor.Location = new Point(Math.Max(5, x), Math.Max(5, y));
        }

        // ── AUTENTICACIÓN ──
        private void BtnIngresar_Click(object? sender, EventArgs e)
        {
            string usuario = txtUsuario.Text.Trim();
            string contrasena = txtContrasena.Text;

            if (string.IsNullOrWhiteSpace(usuario))
            {
                MostrarError("Ingrese su nombre de usuario.");
                txtUsuario.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(contrasena))
            {
                MostrarError("Ingrese su contraseña.");
                txtContrasena.Focus();
                return;
            }

            try
            {
                btnIngresar.Enabled = false;

                lblMensaje.ForeColor = LoginPaleta.TextoSuave;
                lblMensaje.Text = "Verificando credenciales...";
                lblMensaje.Refresh();

                var repositorio = new UsuarioRepositorio();
                var encriptador = new EncriptadorContrasenas();

                var autenticacion = new AutenticacionServicio(
                    repositorio, encriptador);

                var resultado = autenticacion.Autenticar(usuario, contrasena);

                if (!resultado.Exitoso)
                {
                    MostrarError(resultado.Mensaje);
                    txtContrasena.Clear();
                    txtContrasena.Focus();
                    return;
                }

                UsuarioAutenticado = resultado.Sesion;

                lblMensaje.ForeColor = LoginPaleta.Exito;
                lblMensaje.Text = "Acceso autorizado.";

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MostrarError("No se pudo iniciar sesión.");

                System.Diagnostics.Debug.WriteLine(
                    "Error de autenticación: " + ex);

                MessageBox.Show(
                    "Ocurrió un error al autenticar.\n\n" + ex.Message,
                    "Error de autenticación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                if (!IsDisposed && DialogResult != DialogResult.OK)
                    btnIngresar.Enabled = true;
            }
        }

        private void MostrarError(string mensaje)
        {
            lblMensaje.ForeColor = LoginPaleta.Error;
            lblMensaje.Text = mensaje;
        }
    }
}