using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using SIGEPOSTA.Presentacion.Estilos;

namespace SIGEPOSTA.Presentacion.Formularios
{
    public class FormPrincipal : Form
    {
        private sealed record Modulo(
            string Icono, string Titulo, string Descripcion, string Grupo, Action Abrir);

        private readonly Panel panelMenu = new();
        private readonly Panel panelPrincipal = new();
        private readonly Panel panelTrabajo = new();
        private readonly Label lblSeccion = new();
        private readonly Label lblRuta = new();
        private readonly List<BotonMenu> botonesMenu = new();
        private readonly List<Modulo> modulos;
        private Form? formularioActual;

        private readonly Color verdeOscuro = TemaSIGEPOSTA.VerdeOscuro;
        private readonly Color verde = TemaSIGEPOSTA.Verde;
        private readonly Color fondo = TemaSIGEPOSTA.Fondo;

        public FormPrincipal()
        {
            modulos = CrearModulos();

            ConfigurarVentana();
            CrearEstructura();
            CrearMenu();
            CrearBarraSuperior();
            MostrarInicio();
        }

        // ───────────────────────── Definición de módulos ─────────────────────────

        private List<Modulo> CrearModulos() => new()
        {
            new("\uE77B", "Pacientes", "Registro y gestión de pacientes",
                "GESTIÓN ASISTENCIAL", () => AbrirFormulario<FormPacientes>("Pacientes")),
            new("\uE716", "Personal de salud", "Gestión del personal sanitario",
                "GESTIÓN ASISTENCIAL", () => AbrirFormulario<FormPersonalSalud>("Personal de salud")),
            new("\uE787", "Citas médicas", "Programación y seguimiento de citas",
                "GESTIÓN ASISTENCIAL", () => AbrirFormulario<FormCitas>("Citas médicas")),
            new("\uE8D4", "Atención médica", "Registro de atenciones médicas",
                "GESTIÓN ASISTENCIAL", () => AbrirFormulario<FormAtencion>("Atención médica")),
            new("\uE8A5", "Historias clínicas", "Consulta de información clínica",
                "GESTIÓN ASISTENCIAL", () => AbrirFormulario<FormHistoriaClinica>("Historias clínicas")),
            new("\uE8EC", "Diagnósticos", "Registro de diagnósticos",
                "GESTIÓN ASISTENCIAL", () => AbrirFormulario<FormDiagnosticos>("Diagnósticos")),

            new("\uE8D7", "Farmacia", "Administración de medicamentos",
                "GESTIÓN Y CONTROL", () => AbrirFormulario<FormFarmacia>("Farmacia")),
            new("\uE8B7", "Inventario", "Control de existencias",
                "GESTIÓN Y CONTROL", () => AbrirFormulario<FormInventario>("Inventario")),
            new("\uE9D2", "Reportes", "Informes y consultas del sistema",
                "GESTIÓN Y CONTROL", () => AbrirFormulario<FormReportes>("Reportes")),
            new("\uE9D5", "Respaldos", "Copias de seguridad",
                "GESTIÓN Y CONTROL", () => AbrirFormulario<FormRespaldos>("Respaldos")),

            new("\uE72E", "Usuarios", "Cuentas y accesos",
                "ADMINISTRACIÓN", () => AbrirFormulario<FormUsuarios>("Usuarios")),
            new("\uE713", "Configuración", "Parámetros del sistema",
                "ADMINISTRACIÓN", () => AbrirFormulario<FormConfiguracion>("Configuración")),
        };

        // ───────────────────────── Ventana y estructura ─────────────────────────

        private void ConfigurarVentana()
        {
            Text = "SIGEPOSTA | Gestión de la Posta de Salud";
            Size = new Size(1400, 850);
            MinimumSize = new Size(1150, 720);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = fondo;
            Font = TemaSIGEPOSTA.Fuente;
            DoubleBuffered = true;
        }

        private void CrearEstructura()
        {
            panelMenu.Dock = DockStyle.Left;
            panelMenu.Width = 260;
            panelMenu.BackColor = verdeOscuro;

            panelPrincipal.Dock = DockStyle.Fill;
            panelPrincipal.BackColor = fondo;

            Controls.Add(panelPrincipal);
            Controls.Add(panelMenu);
        }

        // ───────────────────────── Menú lateral ─────────────────────────

        private void CrearMenu()
        {
            // Logo
            var logo = new LogoMenu
            {
                Dock = DockStyle.Top,
                Height = 135,
                BackColor = verdeOscuro
            };

            var tituloLogo = new Label
            {
                Text = "SIGEPOSTA",
                Font = new Font("Segoe UI", 17, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Bottom,
                Height = 32,
                TextAlign = ContentAlignment.MiddleCenter
            };

            var subtituloLogo = new Label
            {
                Text = "POSTA DE SALUD · MOLLEPATA",
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(180, 210, 190),
                Dock = DockStyle.Bottom,
                Height = 26,
                TextAlign = ContentAlignment.TopCenter
            };

            logo.Controls.Add(tituloLogo);
            logo.Controls.Add(subtituloLogo);

            // Lista de opciones
            var menu = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                BackColor = verdeOscuro,
                Padding = new Padding(0, 4, 0, 10)
            };

            AgregarSeccion(menu, "GENERAL");
            AgregarOpcion(menu, "\uE80F", "Panel principal", MostrarInicio);

            foreach (var grupo in modulos.GroupBy(m => m.Grupo))
            {
                AgregarSeccion(menu, grupo.Key);
                foreach (var m in grupo)
                    AgregarOpcion(menu, m.Icono, m.Titulo, m.Abrir);
            }

            // Mantiene el ancho de cada elemento ajustado al panel (con o sin scroll)
            menu.Layout += (s, e) =>
            {
                int ancho = menu.ClientSize.Width;
                foreach (Control c in menu.Controls)
                    if (c.Width != ancho) c.Width = ancho;
            };

            // Botón cerrar
            var cerrar = new BotonMenu
            {
                Icono = "\uE7E8",
                Text = "Cerrar sistema",
                Tag = "__cerrar",
                Dock = DockStyle.Bottom,
                Height = 54,
                BackColor = Color.FromArgb(22, 60, 43)
            };
            cerrar.Click += (s, e) => CerrarSistema();

            panelMenu.Controls.Add(menu);
            panelMenu.Controls.Add(logo);
            panelMenu.Controls.Add(cerrar);
        }

        private void AgregarSeccion(FlowLayoutPanel menu, string texto)
        {
            var seccion = new Label
            {
                Text = texto,
                Width = menu.Width,
                Height = 34,
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(150, 185, 163),
                TextAlign = ContentAlignment.BottomLeft,
                Padding = new Padding(22, 0, 0, 5),
                Margin = new Padding(0)
            };
            menu.Controls.Add(seccion);
        }

        private void AgregarOpcion(FlowLayoutPanel menu, string icono, string texto, Action accion)
        {
            var boton = new BotonMenu
            {
                Icono = icono,
                Text = texto,
                Tag = texto,
                Width = menu.Width,
                Height = 40,
                BackColor = verdeOscuro,
                Margin = new Padding(0)
            };
            boton.Click += (s, e) => accion();

            botonesMenu.Add(boton);
            menu.Controls.Add(boton);
        }

        // ───────────────────────── Barra superior ─────────────────────────

        private void CrearBarraSuperior()
        {
            var barra = new Panel
            {
                Dock = DockStyle.Top,
                Height = 72,
                BackColor = Color.White
            };

            // Línea inferior sutil
            barra.Paint += (s, e) =>
            {
                using var pen = new Pen(Color.FromArgb(222, 230, 225));
                e.Graphics.DrawLine(pen, 0, barra.Height - 1, barra.Width, barra.Height - 1);
            };

            lblSeccion.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblSeccion.ForeColor = verdeOscuro;
            lblSeccion.AutoSize = true;
            lblSeccion.Location = new Point(28, 12);

            lblRuta.Font = new Font("Segoe UI", 8.5F);
            lblRuta.ForeColor = TemaSIGEPOSTA.TextoSecundario;
            lblRuta.AutoSize = true;
            lblRuta.Location = new Point(30, 44);

            string nombreUsuario =
                FormInicioSesion.UsuarioAutenticado?.NombreCompleto
                ?? FormInicioSesion.UsuarioAutenticado?.NombreUsuario
                ?? "Usuario";

            string rol = FormInicioSesion.UsuarioAutenticado?.Rol ?? "Sin rol";

            var zonaUsuario = new Panel
            {
                Dock = DockStyle.Right,
                Width = 290,
                BackColor = Color.White
            };

            var avatar = new AvatarUsuario
            {
                Iniciales = ObtenerIniciales(nombreUsuario),
                Size = new Size(42, 42),
                Location = new Point(18, 15),
                BackColor = Color.White
            };

            var lblNombre = new Label
            {
                Text = nombreUsuario,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = TemaSIGEPOSTA.Texto,
                AutoEllipsis = true,
                Size = new Size(205, 20),
                Location = new Point(70, 16)
            };

            var lblRol = new Label
            {
                Text = rol,
                Font = new Font("Segoe UI", 8.5F),
                ForeColor = TemaSIGEPOSTA.TextoSecundario,
                AutoEllipsis = true,
                Size = new Size(205, 18),
                Location = new Point(70, 37)
            };

            zonaUsuario.Controls.Add(avatar);
            zonaUsuario.Controls.Add(lblNombre);
            zonaUsuario.Controls.Add(lblRol);

            barra.Controls.Add(lblSeccion);
            barra.Controls.Add(lblRuta);
            barra.Controls.Add(zonaUsuario);

            panelTrabajo.Dock = DockStyle.Fill;
            panelTrabajo.BackColor = fondo;
            panelTrabajo.Padding = new Padding(28, 24, 28, 18);

            panelPrincipal.Controls.Add(panelTrabajo);
            panelPrincipal.Controls.Add(barra);
        }

        private static string ObtenerIniciales(string nombre)
        {
            var partes = nombre.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (partes.Length == 0) return "U";
            if (partes.Length == 1) return partes[0][..1].ToUpper();
            return (partes[0][..1] + partes[1][..1]).ToUpper();
        }

        private void ActualizarEncabezado(string titulo)
        {
            lblSeccion.Text = titulo;
            lblRuta.Text = "SIGEPOSTA   ›   " + titulo;

            foreach (var b in botonesMenu)
                b.Activo = (b.Tag as string) == titulo;
        }

        // ───────────────────────── Panel de inicio ─────────────────────────

        private void MostrarInicio()
        {
            CerrarModuloActual();
            LimpiarPanelTrabajo();
            ActualizarEncabezado("Panel principal");

            var tarjetas = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                WrapContents = true,
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = fondo,
                Padding = new Padding(0, 6, 0, 10)
            };

            foreach (var m in modulos)
            {
                var tarjeta = new TarjetaModulo
                {
                    Icono = m.Icono,
                    Titulo = m.Titulo,
                    Descripcion = m.Descripcion,
                    Size = new Size(264, 150),
                    Margin = new Padding(0, 0, 14, 10),
                    BackColor = fondo
                };
                tarjeta.Click += (s, e) => m.Abrir();
                tarjetas.Controls.Add(tarjeta);
            }

            var tituloModulos = new Label
            {
                Text = "Módulos del sistema",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = verdeOscuro,
                Dock = DockStyle.Top,
                Height = 54,
                TextAlign = ContentAlignment.BottomLeft,
                Padding = new Padding(0, 0, 0, 6)
            };

            string fecha = DateTime.Now.ToString("dddd, d 'de' MMMM 'de' yyyy", new CultureInfo("es-PE"));
            fecha = char.ToUpper(fecha[0]) + fecha[1..];

            var banner = new BannerBienvenida
            {
                Dock = DockStyle.Top,
                Height = 150,
                Fecha = fecha,
                Titulo = "Bienvenido a SIGEPOSTA",
                Descripcion = "Gestiona los servicios y procesos de la posta de salud desde un solo lugar.",
                BackColor = fondo
            };

            // El control Fill primero; los Top después (el último agregado queda arriba)
            panelTrabajo.Controls.Add(tarjetas);
            panelTrabajo.Controls.Add(tituloModulos);
            panelTrabajo.Controls.Add(banner);
        }

        // ───────────────────────── Navegación entre módulos ─────────────────────────

        private void AbrirFormulario<T>(string titulo) where T : Form, new()
        {
            try
            {
                CerrarModuloActual();
                LimpiarPanelTrabajo();
                ActualizarEncabezado(titulo);

                formularioActual = new T
                {
                    TopLevel = false,
                    FormBorderStyle = FormBorderStyle.None,
                    Dock = DockStyle.Fill,
                    BackColor = fondo
                };

                panelTrabajo.Controls.Add(formularioActual);
                formularioActual.Show();
                formularioActual.BringToFront();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo abrir el módulo \"" + titulo + "\".\n\n" + ex.Message,
                    "SIGEPOSTA",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                MostrarInicio();
            }
        }

        private void LimpiarPanelTrabajo()
        {
            foreach (Control c in panelTrabajo.Controls.Cast<Control>().ToList())
            {
                panelTrabajo.Controls.Remove(c);
                c.Dispose();
            }
        }

        private void CerrarModuloActual()
        {
            if (formularioActual != null)
            {
                formularioActual.Close();
                formularioActual.Dispose();
                formularioActual = null;
            }
        }

        private void CerrarSistema()
        {
            DialogResult resultado = MessageBox.Show(
                "¿Está seguro de que desea cerrar SIGEPOSTA?",
                "Cerrar sistema",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (resultado == DialogResult.Yes)
                Close();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            CerrarModuloActual();
            base.OnFormClosed(e);
        }
    }

    // ═════════════════════════ Utilidades de dibujo ═════════════════════════

    internal static class Dibujo
    {
        public static GraphicsPath Redondeado(Rectangle r, int radio)
        {
            int d = radio * 2;
            var p = new GraphicsPath();
            p.AddArc(r.X, r.Y, d, d, 180, 90);
            p.AddArc(r.Right - d - 1, r.Y, d, d, 270, 90);
            p.AddArc(r.Right - d - 1, r.Bottom - d - 1, d, d, 0, 90);
            p.AddArc(r.X, r.Bottom - d - 1, d, d, 90, 90);
            p.CloseFigure();
            return p;
        }

        // Cruz médica centrada en (cx, cy)
        public static void CruzMedica(Graphics g, Brush brush, float cx, float cy, float largo, float grosor)
        {
            using var h = Redondeado(new Rectangle(
                (int)(cx - largo / 2), (int)(cy - grosor / 2), (int)largo, (int)grosor), (int)(grosor / 3));
            using var v = Redondeado(new Rectangle(
                (int)(cx - grosor / 2), (int)(cy - largo / 2), (int)grosor, (int)largo), (int)(grosor / 3));
            g.FillPath(brush, h);
            g.FillPath(brush, v);
        }
    }

    internal static class Iconos
    {
        // Windows 11 usa "Segoe Fluent Icons"; Windows 10 usa "Segoe MDL2 Assets"
        public static readonly string Familia =
            FontFamily.Families.Any(f => f.Name == "Segoe Fluent Icons")
                ? "Segoe Fluent Icons"
                : "Segoe MDL2 Assets";

        public static Font Crear(float tamano) => new(Familia, tamano, FontStyle.Regular, GraphicsUnit.Point);
    }

    // ═════════════════════════ Controles personalizados ═════════════════════════

    internal sealed class LogoMenu : Panel
    {
        public LogoMenu() => DoubleBuffered = true;

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int d = 58;
            var circulo = new Rectangle((Width - d) / 2, 18, d, d);

            using (var b = new SolidBrush(Color.FromArgb(32, Color.White)))
                g.FillEllipse(b, circulo);
            using (var pen = new Pen(Color.FromArgb(70, Color.White), 1.5F))
                g.DrawEllipse(pen, circulo);
            using (var b = new SolidBrush(Color.White))
                Dibujo.CruzMedica(g, b, Width / 2f, 18 + d / 2f, 28, 9);

            using var linea = new Pen(Color.FromArgb(40, Color.White));
            g.DrawLine(linea, 24, Height - 1, Width - 24, Height - 1);
        }
    }

    internal sealed class BotonMenu : Control
    {
        private static readonly Font FuenteIcono = Iconos.Crear(12.5F);
        private static readonly Font FuenteTexto = new("Segoe UI", 9.5F);
        private static readonly Font FuenteActiva = new("Segoe UI", 9.5F, FontStyle.Bold);

        private bool hover;
        private bool activo;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Icono { get; set; } = "";

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Activo
        {
            get => activo;
            set { if (activo != value) { activo = value; Invalidate(); } }
        }

        public BotonMenu()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            Cursor = Cursors.Hand;
        }

        protected override void OnMouseEnter(EventArgs e) { hover = true; Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { hover = false; Invalidate(); base.OnMouseLeave(e); }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(BackColor);
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var area = new Rectangle(12, 2, Width - 24, Height - 4);

            if (Activo)
            {
                using var path = Dibujo.Redondeado(area, 9);
                using var fondo = new SolidBrush(TemaSIGEPOSTA.Verde);
                g.FillPath(fondo, path);

                using var marca = Dibujo.Redondeado(new Rectangle(area.X + 2, area.Y + 9, 4, area.Height - 18), 2);
                using var blanco = new SolidBrush(Color.White);
                g.FillPath(blanco, marca);
            }
            else if (hover)
            {
                using var path = Dibujo.Redondeado(area, 9);
                using var fondo = new SolidBrush(Color.FromArgb(28, Color.White));
                g.FillPath(fondo, path);
            }

            Color colorTexto = Activo ? Color.White : Color.FromArgb(222, 236, 227);

            TextRenderer.DrawText(g, Icono, FuenteIcono,
                new Rectangle(24, 0, 30, Height), colorTexto,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);

            TextRenderer.DrawText(g, Text, Activo ? FuenteActiva : FuenteTexto,
                new Rectangle(62, 0, Width - 74, Height), colorTexto,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter |
                TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
        }
    }

    internal sealed class TarjetaModulo : Control
    {
        private static readonly Font FuenteIcono = Iconos.Crear(19F);
        private static readonly Font FuenteFlecha = Iconos.Crear(8.5F);
        private static readonly Font FuenteTitulo = new("Segoe UI", 11, FontStyle.Bold);
        private static readonly Font FuenteDetalle = new("Segoe UI", 8.5F);
        private static readonly Font FuentePie = new("Segoe UI", 8, FontStyle.Bold);

        private bool hover;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Icono { get; set; } = "";
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Titulo { get; set; } = "";
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Descripcion { get; set; } = "";

        public TarjetaModulo()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            Cursor = Cursors.Hand;
        }

        protected override void OnMouseEnter(EventArgs e) { hover = true; Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { hover = false; Invalidate(); base.OnMouseLeave(e); }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(BackColor);
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var tarjeta = new Rectangle(3, 2, Width - 9, Height - 10);

            // Sombra suave
            for (int i = 3; i >= 1; i--)
            {
                var sombra = new Rectangle(tarjeta.X - i, tarjeta.Y - i + 3, tarjeta.Width + i * 2, tarjeta.Height + i * 2);
                using var sp = Dibujo.Redondeado(sombra, 14 + i);
                using var sb = new SolidBrush(Color.FromArgb(hover ? 14 : 7, 20, 60, 40));
                g.FillPath(sb, sp);
            }

            // Fondo y borde
            using (var path = Dibujo.Redondeado(tarjeta, 14))
            {
                using var fondo = new SolidBrush(Color.White);
                g.FillPath(fondo, path);
                using var borde = new Pen(hover ? TemaSIGEPOSTA.Verde : Color.FromArgb(225, 232, 228), hover ? 1.6F : 1F);
                g.DrawPath(borde, path);
            }

            int x = tarjeta.X + 20;
            int ancho = tarjeta.Width - 40;

            // Círculo con ícono
            var circulo = new Rectangle(x, tarjeta.Y + 18, 46, 46);
            using (var b = new SolidBrush(hover ? TemaSIGEPOSTA.Verde : TemaSIGEPOSTA.VerdeClaro))
                g.FillEllipse(b, circulo);

            TextRenderer.DrawText(g, Icono, FuenteIcono, circulo,
                hover ? Color.White : TemaSIGEPOSTA.Verde,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);

            // Textos
            var flags = TextFormatFlags.Left | TextFormatFlags.VerticalCenter |
                        TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding;

            TextRenderer.DrawText(g, Titulo, FuenteTitulo,
                new Rectangle(x, tarjeta.Y + 72, ancho, 24), TemaSIGEPOSTA.VerdeOscuro, flags);

            TextRenderer.DrawText(g, Descripcion, FuenteDetalle,
                new Rectangle(x, tarjeta.Y + 96, ancho, 18), TemaSIGEPOSTA.TextoSecundario, flags);

            // Pie: "Abrir módulo →"
            const string pie = "ABRIR MÓDULO";
            var tam = TextRenderer.MeasureText(g, pie, FuentePie, Size.Empty, TextFormatFlags.NoPadding);
            TextRenderer.DrawText(g, pie, FuentePie,
                new Rectangle(x, tarjeta.Y + 116, tam.Width + 4, 18), TemaSIGEPOSTA.Verde, flags);

            TextRenderer.DrawText(g, "\uE72A", FuenteFlecha,
                new Rectangle(x + tam.Width + 6 + (hover ? 4 : 0), tarjeta.Y + 116, 18, 18),
                TemaSIGEPOSTA.Verde,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
        }
    }

    internal sealed class AvatarUsuario : Control
    {
        private static readonly Font Fuente = new("Segoe UI", 10.5F, FontStyle.Bold);

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Iniciales { get; set; } = "U";

        public AvatarUsuario()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(BackColor);
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var r = new Rectangle(0, 0, Width - 1, Height - 1);
            using (var b = new LinearGradientBrush(r, TemaSIGEPOSTA.Verde, TemaSIGEPOSTA.VerdeOscuro, 45f))
                g.FillEllipse(b, r);

            TextRenderer.DrawText(g, Iniciales, Fuente, r, Color.White,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
        }
    }

    internal sealed class BannerBienvenida : Panel
    {
        private static readonly Font FuenteFecha = new("Segoe UI", 8.5F, FontStyle.Bold);
        private static readonly Font FuenteTitulo = new("Segoe UI", 22, FontStyle.Bold);
        private static readonly Font FuenteDescripcion = new("Segoe UI", 10.5F);

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Fecha { get; set; } = "";
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Titulo { get; set; } = "";
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Descripcion { get; set; } = "";

        public BannerBienvenida() => DoubleBuffered = true;

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(BackColor);
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var r = new Rectangle(0, 0, Width, Height);
            using var path = Dibujo.Redondeado(r, 18);

            using (var degradado = new LinearGradientBrush(r, TemaSIGEPOSTA.VerdeOscuro, TemaSIGEPOSTA.Verde, 0f))
                g.FillPath(degradado, path);

            // Decoración recortada a los bordes redondeados
            g.SetClip(path);
            using (var b = new SolidBrush(Color.FromArgb(16, Color.White)))
            {
                g.FillEllipse(b, Width - 260, -70, 300, 300);
                g.FillEllipse(b, Width - 400, Height - 80, 220, 220);
            }
            using (var b = new SolidBrush(Color.FromArgb(38, Color.White)))
                Dibujo.CruzMedica(g, b, Width - 120, Height / 2f, 84, 26);
            g.ResetClip();

            var flags = TextFormatFlags.Left | TextFormatFlags.VerticalCenter |
                        TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding;

            TextRenderer.DrawText(g, Fecha.ToUpper(), FuenteFecha,
                new Rectangle(34, 22, Width - 300, 20), Color.FromArgb(190, 222, 202), flags);

            TextRenderer.DrawText(g, Titulo, FuenteTitulo,
                new Rectangle(34, 46, Width - 300, 44), Color.White, flags);

            TextRenderer.DrawText(g, Descripcion, FuenteDescripcion,
                new Rectangle(34, 96, Width - 300, 24), Color.FromArgb(222, 237, 227), flags);
        }
    }
}
