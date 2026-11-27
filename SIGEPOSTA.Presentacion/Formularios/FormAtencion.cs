using SIGEPOSTA.Negocio.Servicios;
using SIGEPOSTA.Datos.Repositorios;
using SIGEPOSTA.Entidades.Clinica;
using System.Globalization;

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using SIGEPOSTA.Presentacion.Estilos;

namespace SIGEPOSTA.Presentacion.Formularios
{
    internal static class AtencionEstilo
    {
        public static readonly Color Verde = Color.FromArgb(43, 125, 91);
        public static readonly Color VerdeHover = Color.FromArgb(34, 106, 77);
        public static readonly Color VerdeOscuro = Color.FromArgb(27, 65, 54);
        public static readonly Color Fondo = Color.FromArgb(240, 244, 242);
        public static readonly Color Campo = Color.FromArgb(245, 248, 246);
        public static readonly Color Texto = Color.FromArgb(28, 40, 35);
        public static readonly Color Borde = Color.FromArgb(215, 225, 220);
        public static readonly Color Rojo = Color.FromArgb(200, 55, 55);

        public static GraphicsPath Redondear(Rectangle r, int radio)
        {
            GraphicsPath path = new GraphicsPath();

            if (r.Width <= 0 || r.Height <= 0)
                return path;

            int d = Math.Min(radio * 2, Math.Min(r.Width, r.Height));

            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();

            return path;
        }
    }

    internal class CampoAtencion : UserControl
    {
        private readonly TextBox entrada = new();

        public override string Text
        {
            get => entrada.Text;
            set => entrada.Text = value;
        }

        public bool Multiline => entrada.Multiline;

        public CampoAtencion()
        {
            Height = 40;
            MinimumSize = new Size(80, 40);
            BackColor = AtencionEstilo.Campo;
            Padding = new Padding(10, 5, 10, 5);
            Font = new Font("Segoe UI", 10);
            DoubleBuffered = true;

            SetStyle(ControlStyles.ResizeRedraw, true);

            entrada.BorderStyle = BorderStyle.None;
            entrada.Dock = DockStyle.Fill;
            entrada.BackColor = AtencionEstilo.Campo;
            entrada.ForeColor = AtencionEstilo.Texto;
            entrada.Font = new Font("Segoe UI", 10);
            entrada.Margin = new Padding(0);
            entrada.Multiline = false;

            Controls.Add(entrada);

            entrada.GotFocus += (s, e) => Invalidate();
            entrada.LostFocus += (s, e) => Invalidate();
            entrada.TextChanged += (s, e) => OnTextChanged(EventArgs.Empty);
        }

        public void ConfigurarMultilinea(bool multilinea, ScrollBars barras)
        {
            entrada.Multiline = multilinea;
            entrada.ScrollBars = barras;
            entrada.AcceptsReturn = multilinea;
            entrada.AcceptsTab = multilinea;

            Padding = multilinea
                ? new Padding(10, 9, 10, 9)
                : new Padding(10, 5, 10, 5);

            Invalidate();
        }

        public new void Clear()
        {
            entrada.Clear();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle r = new Rectangle(1, 1, Width - 3, Height - 3);

            using GraphicsPath path = AtencionEstilo.Redondear(r, 9);

            Color colorBorde = entrada.Focused
                ? AtencionEstilo.Verde
                : AtencionEstilo.Borde;

            float grosor = entrada.Focused ? 1.8f : 1f;

            using SolidBrush fondo = new SolidBrush(AtencionEstilo.Campo);
            using Pen borde = new Pen(colorBorde, grosor);

            e.Graphics.FillPath(fondo, path);
            e.Graphics.DrawPath(borde, path);
        }
    }

    internal class BotonAtencion : Button
    {
        private readonly TipoBoton tipo;
        private bool encima;
        private bool presionado;

        public BotonAtencion(string texto, TipoBoton tipo)
        {
            Text = texto;
            this.tipo = tipo;

            Size = new Size(155, 46);
            Font = new Font("Segoe UI Semibold", 9.5F);
            Cursor = Cursors.Hand;

            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;

            BackColor = Color.Transparent;
            ForeColor = Color.White;
            UseVisualStyleBackColor = false;

            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw,
                true);

            MouseEnter += (s, e) =>
            {
                encima = true;
                Invalidate();
            };

            MouseLeave += (s, e) =>
            {
                encima = false;
                presionado = false;
                Invalidate();
            };

            MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    presionado = true;
                    Invalidate();
                }
            };

            MouseUp += (s, e) =>
            {
                presionado = false;
                Invalidate();
            };
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            g.Clear(Parent?.BackColor ?? AtencionEstilo.Fondo);

            Rectangle sombra = new(2, 4, Width - 5, Height - 5);
            Rectangle r = new(1, 1, Width - 4, Height - 6);

            Color principal;
            Color secundario;

            if (tipo == TipoBoton.Peligro)
            {
                principal = AtencionEstilo.Rojo;
                secundario = Color.FromArgb(165, 43, 43);
            }
            else if (tipo == TipoBoton.Secundario)
            {
                principal = Color.White;
                secundario = Color.FromArgb(225, 240, 229);
            }
            else
            {
                principal = AtencionEstilo.Verde;
                secundario = AtencionEstilo.VerdeHover;
            }

            if (presionado)
            {
                principal = secundario;
            }
            else if (encima)
            {
                Color temporal = principal;
                principal = secundario;
                secundario = temporal;
            }

            using GraphicsPath sombraPath =
                AtencionEstilo.Redondear(sombra, 10);

            using SolidBrush sombraBrush =
                new SolidBrush(Color.FromArgb(30, 0, 0, 0));

            g.FillPath(sombraBrush, sombraPath);

            using GraphicsPath path =
                AtencionEstilo.Redondear(r, 10);

            using LinearGradientBrush degradado = new(
                r, principal, secundario, LinearGradientMode.Vertical);

            g.FillPath(degradado, path);

            Color colorTexto =
                tipo == TipoBoton.Secundario
                    ? AtencionEstilo.VerdeOscuro
                    : Color.White;

            if (!Enabled)
                colorTexto = Color.Gray;

            TextRenderer.DrawText(
                g,
                Text,
                Font,
                r,
                colorTexto,
                TextFormatFlags.HorizontalCenter |
                TextFormatFlags.VerticalCenter);
        }
    }
    internal class TarjetaAtencion : Panel
    {
        public TarjetaAtencion()
        {
            BackColor = AtencionEstilo.Fondo;
            Padding = new Padding(22, 12, 22, 15);
            Margin = new Padding(0, 0, 0, 18);
            DoubleBuffered = true;

            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw,
                true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle sombra = new Rectangle(3, 4, Width - 7, Height - 7);
            Rectangle r = new Rectangle(1, 1, Width - 4, Height - 5);

            using GraphicsPath sombraPath = AtencionEstilo.Redondear(sombra, 15);
            using GraphicsPath path = AtencionEstilo.Redondear(r, 15);

            using SolidBrush sombraColor =
                new SolidBrush(Color.FromArgb(17, 30, 50, 40));

            using SolidBrush fondo = new SolidBrush(Color.White);
            using Pen borde = new Pen(AtencionEstilo.Borde, 1);

            e.Graphics.FillPath(sombraColor, sombraPath);
            e.Graphics.FillPath(fondo, path);
            e.Graphics.DrawPath(borde, path);
        }
    }

    public class FormAtencion : Form
    {
        private readonly BindingList<AtencionVista> atenciones = new();

        private readonly AtencionServicio servicioAtencion =
            new AtencionServicio(new AtencionRepositorio());

        private readonly PacienteRepositorio repositorioPacientes =
            new PacienteRepositorio();

        private readonly PersonalSaludRepositorio repositorioPersonal =
            new PersonalSaludRepositorio();


        private CampoAtencion txtDniPaciente = new();
        private CampoAtencion txtPaciente = new();
        private CampoAtencion txtDniProfesional = new();
        private CampoAtencion txtProfesional = new();
        private CampoAtencion txtTemperatura = new();
        private CampoAtencion txtPresion = new();
        private CampoAtencion txtPeso = new();
        private CampoAtencion txtTalla = new();
        private CampoAtencion txtSintomas = new();
        private CampoAtencion txtEvaluacion = new();
        private CampoAtencion txtTratamiento = new();
        private CampoAtencion txtObservaciones = new();

        private DateTimePicker dtpFecha = new();
        private ComboBox cmbEstado = new();
        private DataGridView dgvAtenciones = new();

        private int indiceSeleccionado = -1;
        private FlowLayoutPanel columna = null!;
        private Panel cuerpo = null!;

        public FormAtencion()
        {
            ConstruirFormulario();
            ConfigurarTabla();
            AplicarEstilos();
        
            Load += (s, e) =>
            {
                try { CargarAtenciones(); }
                catch (Exception ex)
                {
                    MessageBox.Show("No se pudieron cargar las atenciones:" + ex.Message);
                }
            };
}

        // ===================== INTERFAZ =====================

        private void ConstruirFormulario()
        {
            Text = "SIGEPOSTA - Atención médica";
            Size = new Size(1250, 950);
            MinimumSize = new Size(950, 750);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = AtencionEstilo.Fondo;
            Font = new Font("Segoe UI", 10);

            Panel encabezado = new Panel
            {
                Dock = DockStyle.Top,
                Height = 105,
                BackColor = AtencionEstilo.VerdeOscuro,
                Padding = new Padding(30, 10, 30, 10)
            };

            TableLayoutPanel contenidoEncabezado = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                BackColor = Color.Transparent
            };

            contenidoEncabezado.RowStyles.Add(
                new RowStyle(SizeType.Percent, 60));

            contenidoEncabezado.RowStyles.Add(
                new RowStyle(SizeType.Percent, 40));

            contenidoEncabezado.Controls.Add(new Label
            {
                Text = "REGISTRO DE ATENCIÓN MÉDICA",
                Dock = DockStyle.Fill,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 21, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft
            }, 0, 0);

            contenidoEncabezado.Controls.Add(new Label
            {
                Text = "Registro de consultas, evaluación clínica y tratamiento del paciente",
                Dock = DockStyle.Fill,
                ForeColor = Color.FromArgb(200, 225, 210),
                Font = new Font("Segoe UI", 10),
                TextAlign = ContentAlignment.MiddleLeft
            }, 0, 1);

            encabezado.Controls.Add(contenidoEncabezado);

            cuerpo = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(28, 25, 28, 25),
                BackColor = AtencionEstilo.Fondo
            };

            columna = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Location = new Point(28, 25),
                BackColor = AtencionEstilo.Fondo,
                Margin = new Padding(0),
                Padding = new Padding(0)
            };

            columna.Controls.Add(CrearGrupoPaciente());
            columna.Controls.Add(CrearGrupoSignos());
            columna.Controls.Add(CrearGrupoClinico());
            columna.Controls.Add(CrearGrupoEstado());
            columna.Controls.Add(CrearBotonera());

            cuerpo.Controls.Add(columna);

            void AjustarDistribucion()
            {
                int ancho = Math.Max(
                    650,
                    cuerpo.ClientSize.Width - cuerpo.Padding.Horizontal - 5);

                columna.Location = new Point(
                    cuerpo.Padding.Left,
                    cuerpo.Padding.Top);

                columna.Width = ancho;

                foreach (Control control in columna.Controls)
                    control.Width = ancho;

                columna.PerformLayout();
            }

            cuerpo.Resize += (s, e) => AjustarDistribucion();

            Controls.Add(cuerpo);
            Controls.Add(encabezado);

            Shown += (s, e) => AjustarDistribucion();
        }

        // ===================== TARJETAS =====================

        private TarjetaAtencion CrearGrupo(string titulo, int alto)
        {
            TarjetaAtencion tarjeta = new TarjetaAtencion
            {
                Height = alto,
                Width = 1100,
                Padding = new Padding(22, 12, 22, 15),
                Margin = new Padding(0, 0, 0, 18)
            };

            TableLayoutPanel estructura = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                BackColor = Color.White,
                Margin = new Padding(0),
                Padding = new Padding(0)
            };

            estructura.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 42));

            estructura.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100));

            Label etiquetaTitulo = new Label
            {
                Text = titulo,
                Dock = DockStyle.Fill,
                ForeColor = AtencionEstilo.VerdeOscuro,
                Font = new Font("Segoe UI", 15, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(0, 0, 0, 5)
            };

            Panel contenido = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Margin = new Padding(0)
            };

            estructura.Controls.Add(etiquetaTitulo, 0, 0);
            estructura.Controls.Add(contenido, 0, 1);

            tarjeta.Controls.Add(estructura);
            tarjeta.Tag = contenido;

            return tarjeta;
        }

        private Panel ObtenerContenido(TarjetaAtencion tarjeta)
        {
            return (Panel)tarjeta.Tag!;
        }

        // ===================== PACIENTE =====================

        private TarjetaAtencion CrearGrupoPaciente()
        {
            TarjetaAtencion tarjeta = CrearGrupo(
                "Información del paciente y profesional", 230);

            Panel contenido = ObtenerContenido(tarjeta);

            TableLayoutPanel rejilla = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 2,
                BackColor = Color.White,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };

            for (int i = 0; i < 3; i++)
            {
                rejilla.ColumnStyles.Add(
                    new ColumnStyle(SizeType.Percent, 33.33f));
            }

            rejilla.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            rejilla.RowStyles.Add(new RowStyle(SizeType.Percent, 50));

            txtDniPaciente = NuevoCampo();
            txtPaciente = NuevoCampo();
            txtDniProfesional = NuevoCampo();
            txtProfesional = NuevoCampo();

            dtpFecha = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                MaxDate = DateTime.Today,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10),
                Margin = new Padding(0, 4, 15, 8)
            };

            AgregarCampoVertical(rejilla, "DNI del paciente", txtDniPaciente, 0, 0);
            AgregarCampoVertical(rejilla, "Nombre del paciente", txtPaciente, 1, 0);
            AgregarCampoVertical(rejilla, "DNI del profesional", txtDniProfesional, 2, 0);

            AgregarCampoVertical(rejilla, "Profesional responsable", txtProfesional, 0, 1);
            AgregarCampoVertical(rejilla, "Fecha de atención", dtpFecha, 1, 1);

            contenido.Controls.Add(rejilla);

            return tarjeta;
        }

        // ===================== SIGNOS VITALES =====================

        private TarjetaAtencion CrearGrupoSignos()
        {
            TarjetaAtencion tarjeta = CrearGrupo(
                "Signos vitales y medidas", 170);

            Panel contenido = ObtenerContenido(tarjeta);

            TableLayoutPanel rejilla = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 1,
                BackColor = Color.White,
                Padding = new Padding(0)
            };

            for (int i = 0; i < 4; i++)
            {
                rejilla.ColumnStyles.Add(
                    new ColumnStyle(SizeType.Percent, 25));
            }

            txtTemperatura = NuevoCampo();
            txtPresion = NuevoCampo();
            txtPeso = NuevoCampo();
            txtTalla = NuevoCampo();

            AgregarCampoVertical(rejilla, "Temperatura (°C)", txtTemperatura, 0, 0);
            AgregarCampoVertical(rejilla, "Presión arterial", txtPresion, 1, 0);
            AgregarCampoVertical(rejilla, "Peso (kg)", txtPeso, 2, 0);
            AgregarCampoVertical(rejilla, "Talla (m)", txtTalla, 3, 0);

            contenido.Controls.Add(rejilla);

            return tarjeta;
        }

        // ===================== EVALUACIÓN CLÍNICA =====================

        private TarjetaAtencion CrearGrupoClinico()
        {
            TarjetaAtencion tarjeta = CrearGrupo(
                "Evaluación clínica", 350);

            Panel contenido = ObtenerContenido(tarjeta);

            TableLayoutPanel rejilla = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2,
                BackColor = Color.White,
                Padding = new Padding(0)
            };

            rejilla.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 50));

            rejilla.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 50));

            rejilla.RowStyles.Add(
                new RowStyle(SizeType.Percent, 50));

            rejilla.RowStyles.Add(
                new RowStyle(SizeType.Percent, 50));

            txtSintomas = NuevaArea();
            txtEvaluacion = NuevaArea();
            txtTratamiento = NuevaArea();
            txtObservaciones = NuevaArea();

            rejilla.Controls.Add(
                BloqueArea("Síntomas", txtSintomas), 0, 0);

            rejilla.Controls.Add(
                BloqueArea("Evaluación clínica", txtEvaluacion), 1, 0);

            rejilla.Controls.Add(
                BloqueArea("Tratamiento indicado", txtTratamiento), 0, 1);

            rejilla.Controls.Add(
                BloqueArea("Observaciones", txtObservaciones), 1, 1);

            contenido.Controls.Add(rejilla);

            return tarjeta;
        }

        // ===================== ESTADO =====================

        private TarjetaAtencion CrearGrupoEstado()
        {
            TarjetaAtencion tarjeta = CrearGrupo(
                "Estado de la atención", 105);

            Panel contenido = ObtenerContenido(tarjeta);

            TableLayoutPanel rejilla = new TableLayoutPanel
            {
                Dock = DockStyle.Left,
                Width = 430,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.White
            };

            rejilla.ColumnStyles.Add(
                new ColumnStyle(SizeType.Absolute, 100));

            rejilla.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100));

            cmbEstado = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10),
                Margin = new Padding(0, 6, 10, 6)
            };

            cmbEstado.Items.AddRange(new object[]
            {
                "En proceso",
                "Finalizada",
                "Derivada"
            });

            cmbEstado.SelectedIndex = 0;

            AgregarPar(rejilla, "Estado:", cmbEstado, 0, 0);

            contenido.Controls.Add(rejilla);

            return tarjeta;
        }

        // ===================== BOTONES =====================

        private Control CrearBotonera()
        {
            Panel panelBotones = new()
            {
                Height = 90,
                Width = 1100,
                BackColor = AtencionEstilo.Fondo,
                Padding = new Padding(0, 8, 0, 8),
                Margin = new Padding(0)
            };

            FlowLayoutPanel botones = new()
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Anchor = AnchorStyles.None,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Padding = new Padding(10, 10, 10, 10),
                Margin = new Padding(0),
                BackColor = AtencionEstilo.Fondo
            };

            BotonAtencion btnRegistrar =
                NuevoBoton("REGISTRAR", TipoBoton.Primario);

            btnRegistrar.Click += (s, e) => Registrar();

            BotonAtencion btnActualizar =
                NuevoBoton("ACTUALIZAR", TipoBoton.Secundario);

            btnActualizar.Click += (s, e) => Actualizar();

            BotonAtencion btnEliminar =
                NuevoBoton("ELIMINAR", TipoBoton.Peligro);

            btnEliminar.Click += (s, e) => Eliminar();

            BotonAtencion btnLimpiar =
                NuevoBoton("LIMPIAR", TipoBoton.Secundario);

            btnLimpiar.Click += (s, e) => Limpiar();

            BotonAtencion btnConsultar =
                NuevoBoton("CONSULTAR ATENCIONES", TipoBoton.Secundario);

            btnConsultar.Width = 230;
            btnConsultar.Click += (s, e) => MostrarListado();

            botones.Controls.AddRange(new Control[]
            {
                btnRegistrar,
                btnActualizar,
                btnEliminar,
                btnLimpiar,
                btnConsultar
            });

            panelBotones.Controls.Add(botones);

            void CentrarBotones()
            {
                botones.Location = new Point(
                    Math.Max(
                        0,
                        (panelBotones.ClientSize.Width - botones.Width) / 2),
                    Math.Max(
                        0,
                        (panelBotones.ClientSize.Height - botones.Height) / 2));
            }

            panelBotones.Resize += (s, e) => CentrarBotones();
            botones.SizeChanged += (s, e) => CentrarBotones();

            CentrarBotones();

            return panelBotones;
        }
        // ===================== AYUDAS DE DISEÑO =====================

        private TableLayoutPanel CrearRejilla(int columnas, int filas)
        {
            TableLayoutPanel rejilla = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = columnas,
                RowCount = filas,
                BackColor = Color.White,
                Padding = new Padding(0)
            };

            for (int i = 0; i < columnas; i++)
            {
                rejilla.ColumnStyles.Add(
                    new ColumnStyle(
                        SizeType.Percent,
                        100f / columnas));
            }

            for (int i = 0; i < filas; i++)
            {
                rejilla.RowStyles.Add(
                    new RowStyle(SizeType.Absolute, 46));
            }

            return rejilla;
        }

        private void AgregarPar(
            TableLayoutPanel rejilla,
            string texto,
            Control campo,
            int columna,
            int fila)
        {
            Label etiqueta = new Label
            {
                Text = texto,
                Dock = DockStyle.Fill,
                ForeColor = AtencionEstilo.Texto,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(0, 4, 5, 4),
                AutoEllipsis = true
            };

            campo.Dock = DockStyle.Fill;
            campo.Margin = new Padding(0, 5, 12, 5);

            rejilla.Controls.Add(etiqueta, columna, fila);
            rejilla.Controls.Add(campo, columna + 1, fila);
        }

        private void AgregarCampoVertical(
            TableLayoutPanel rejilla,
            string titulo,
            Control campo,
            int columna,
            int fila)
        {
            Panel bloque = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(0, 2, 15, 5),
                Margin = new Padding(0)
            };

            TableLayoutPanel distribucion = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                BackColor = Color.White,
                Margin = new Padding(0),
                Padding = new Padding(0)
            };

            distribucion.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 27));

            distribucion.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100));

            Label etiqueta = new Label
            {
                Text = titulo,
                Dock = DockStyle.Fill,
                ForeColor = AtencionEstilo.Texto,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(0, 0, 0, 2),
                AutoEllipsis = true
            };

            campo.Dock = DockStyle.Fill;
            campo.Margin = new Padding(0, 3, 0, 5);

            distribucion.Controls.Add(etiqueta, 0, 0);
            distribucion.Controls.Add(campo, 0, 1);

            bloque.Controls.Add(distribucion);

            rejilla.Controls.Add(bloque, columna, fila);
        }

        private Control BloqueArea(string titulo, CampoAtencion area)
        {
            TableLayoutPanel bloque = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                BackColor = Color.White,
                Margin = new Padding(0, 0, 16, 12),
                Padding = new Padding(0)
            };

            bloque.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 29));

            bloque.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100));

            bloque.Controls.Add(new Label
            {
                Text = titulo,
                Dock = DockStyle.Fill,
                ForeColor = AtencionEstilo.Texto,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(0, 0, 0, 4)
            }, 0, 0);

            area.Dock = DockStyle.Fill;
            area.Margin = new Padding(0, 0, 0, 5);

            bloque.Controls.Add(area, 0, 1);

            return bloque;
        }

        private CampoAtencion NuevoCampo()
        {
            return new CampoAtencion();
        }

        private CampoAtencion NuevaArea()
        {
            CampoAtencion campo = new CampoAtencion
            {
                Height = 100,
                MinimumSize = new Size(100, 80)
            };

            campo.ConfigurarMultilinea(true, ScrollBars.Vertical);

            return campo;
        }

        private BotonAtencion NuevoBoton(string texto, TipoBoton tipo)
        {
            return new BotonAtencion(texto, tipo);
        }

        // ===================== TABLA =====================

        private void ConfigurarTabla()
        {
            dgvAtenciones = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                EnableHeadersVisualStyles = false,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                GridColor = AtencionEstilo.Borde,
                ColumnHeadersHeight = 44,
                RowTemplate = { Height = 38 },
                Font = new Font("Segoe UI", 9.5f),
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
            };

            dgvAtenciones.ColumnHeadersDefaultCellStyle.BackColor =
                AtencionEstilo.VerdeOscuro;

            dgvAtenciones.ColumnHeadersDefaultCellStyle.ForeColor =
                Color.White;

            dgvAtenciones.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 10, FontStyle.Bold);

            dgvAtenciones.ColumnHeadersDefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleLeft;

            dgvAtenciones.DefaultCellStyle.BackColor = Color.White;
            dgvAtenciones.DefaultCellStyle.ForeColor = AtencionEstilo.Texto;

            dgvAtenciones.DefaultCellStyle.SelectionBackColor =
                Color.FromArgb(225, 240, 229);

            dgvAtenciones.DefaultCellStyle.SelectionForeColor =
                AtencionEstilo.VerdeOscuro;

            dgvAtenciones.AlternatingRowsDefaultCellStyle.BackColor =
                Color.FromArgb(247, 250, 248);

            Columna("DniPaciente", "DNI paciente");
            Columna("Paciente", "Paciente");
            Columna("Profesional", "Profesional");
            Columna("Fecha", "Fecha");
            Columna("Temperatura", "Temperatura");
            Columna("Presion", "Presión");
            Columna("Estado", "Estado");

            dgvAtenciones.Columns["Fecha"]!.DefaultCellStyle.Format =
                "dd/MM/yyyy";

            dgvAtenciones.DataSource = atenciones;
            dgvAtenciones.SelectionChanged += (s, e) => Seleccionar();
        }

        private void Columna(string propiedad, string titulo)
        {
            dgvAtenciones.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = propiedad,
                DataPropertyName = propiedad,
                HeaderText = titulo
            });
        }

        private void MostrarListado()
        {
            Form ventana = new Form
            {
                Text = "Listado de atenciones médicas",
                Size = new Size(1100, 520),
                MinimumSize = new Size(850, 400),
                StartPosition = FormStartPosition.CenterParent,
                BackColor = AtencionEstilo.Fondo,
                Padding = new Padding(20)
            };

            ventana.Controls.Add(dgvAtenciones);
            ventana.ShowDialog(this);

            ventana.Controls.Remove(dgvAtenciones);
        }

        // ===================== ESTILOS =====================

        private void AplicarEstilos()
        {
            BackColor = AtencionEstilo.Fondo;
            Font = new Font("Segoe UI", 10);

            dtpFecha.CalendarMonthBackground = Color.White;
            dtpFecha.CalendarForeColor = AtencionEstilo.Texto;

            cmbEstado.BackColor = AtencionEstilo.Campo;
            cmbEstado.ForeColor = AtencionEstilo.Texto;
            cmbEstado.FlatStyle = FlatStyle.Flat;
        }

        // ===================== LÓGICA =====================

        private Atencion ConvertirEntidad(AtencionVista vista)
        {
            var paciente = repositorioPacientes.ObtenerTodos()
                .FirstOrDefault(p =>
                    p.NumeroDocumento.Trim().Equals(
                        vista.DniPaciente.Trim(),
                        StringComparison.OrdinalIgnoreCase));

            if (paciente == null)
                throw new InvalidOperationException(
                    "No se encontró un paciente con el DNI ingresado.");

            var personal = repositorioPersonal.ObtenerTodos()
                .FirstOrDefault(p =>
                    p.NumeroDocumento.Trim().Equals(
                        vista.DniProfesional.Trim(),
                        StringComparison.OrdinalIgnoreCase));

            if (personal == null)
                throw new InvalidOperationException(
                    "No se encontró un profesional de salud con ese DNI.");

            return new Atencion
            {
                IdAtencion = vista.IdAtencion,
                IdPaciente = paciente.IdPaciente,
                IdPersonalSalud = personal.IdPersonalSalud,
                FechaAtencion = vista.Fecha,
                MotivoConsulta = vista.Sintomas,
                Sintomas = vista.Sintomas,
                ExamenFisico = vista.Evaluacion,
                Tratamiento = vista.Tratamiento,
                Observaciones = vista.Observaciones,
                Temperatura = ConvertirDecimal(vista.Temperatura),
                PresionArterial = vista.Presion,
                Peso = ConvertirDecimal(vista.Peso),
                Talla = ConvertirDecimal(vista.Talla),
                Estado = !vista.Estado.Equals(
                    "Inactivo", StringComparison.OrdinalIgnoreCase)
                    && !vista.Estado.Equals(
                    "Anulado", StringComparison.OrdinalIgnoreCase)
            };
        }

        private static decimal? ConvertirDecimal(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return null;

            if (decimal.TryParse(
                valor,
                NumberStyles.Number,
                CultureInfo.CurrentCulture,
                out decimal resultado))
                return resultado;

            if (decimal.TryParse(
                valor.Replace(',', '.'),
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out resultado))
                return resultado;

            throw new InvalidOperationException(
                "Uno de los signos vitales contiene un número inválido: " + valor);
        }

        private void CargarAtenciones()
        {
            atenciones.Clear();

            foreach (Atencion a in servicioAtencion.ObtenerTodos())
            {
                atenciones.Add(new AtencionVista
                {
                    IdAtencion = a.IdAtencion,
                    IdPaciente = a.IdPaciente,
                    IdPersonalSalud = a.IdPersonalSalud,
                    DniPaciente = a.DniPaciente,
                    Paciente = a.NombrePaciente,
                    DniProfesional = a.DniPersonalSalud,
                    Profesional = a.NombrePersonalSalud,
                    Fecha = a.FechaAtencion,
                    Temperatura = a.Temperatura?.ToString(CultureInfo.CurrentCulture) ?? "",
                    Presion = a.PresionArterial,
                    Peso = a.Peso?.ToString(CultureInfo.CurrentCulture) ?? "",
                    Talla = a.Talla?.ToString(CultureInfo.CurrentCulture) ?? "",
                    Sintomas = a.MotivoConsulta,
                    Evaluacion = a.ExamenFisico,
                    Tratamiento = a.Tratamiento,
                    Observaciones = a.Observaciones,
                    Estado = a.Estado ? "Activo" : "Inactivo"
                });
            }

            dgvAtenciones.Refresh();
        }
        private void Registrar()
        {
            if (!Validar())
                return;

            try
            {
                AtencionVista vista = ObtenerDatos();
                Atencion entidad = ConvertirEntidad(vista);

                servicioAtencion.Registrar(entidad);

                MessageBox.Show(
                    "Atención médica guardada correctamente.",
                    "SIGEPOSTA",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                CargarAtenciones();
                Limpiar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo registrar la atención:\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void Actualizar()
        {
            if (indiceSeleccionado < 0 || indiceSeleccionado >= atenciones.Count)
            {
                MessageBox.Show("Seleccione una atención del listado.");
                return;
            }

            if (!Validar())
                return;

            try
            {
                AtencionVista vista = ObtenerDatos();
                vista.IdAtencion = atenciones[indiceSeleccionado].IdAtencion;

                Atencion entidad = ConvertirEntidad(vista);

                servicioAtencion.Actualizar(entidad);

                MessageBox.Show(
                    "Atención actualizada correctamente.",
                    "SIGEPOSTA",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                CargarAtenciones();
                Limpiar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo actualizar la atención:\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void Eliminar()
        {
            if (indiceSeleccionado < 0 || indiceSeleccionado >= atenciones.Count)
            {
                MessageBox.Show("Seleccione una atención.");
                return;
            }

            DialogResult respuesta = MessageBox.Show(
                "¿Desea eliminar este registro de atención?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (respuesta != DialogResult.Yes)
                return;

            try
            {
                int id = atenciones[indiceSeleccionado].IdAtencion;

                servicioAtencion.Eliminar(id);

                MessageBox.Show("Registro eliminado correctamente.");

                CargarAtenciones();
                Limpiar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo eliminar el registro:\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void Seleccionar()
        {
            if (dgvAtenciones.CurrentRow?.DataBoundItem is not AtencionVista a)
                return;

            indiceSeleccionado = atenciones.IndexOf(a);

            txtDniPaciente.Text = a.DniPaciente;
            txtPaciente.Text = a.Paciente;
            txtDniProfesional.Text = a.DniProfesional;
            txtProfesional.Text = a.Profesional;
            dtpFecha.Value = a.Fecha;

            txtTemperatura.Text = a.Temperatura;
            txtPresion.Text = a.Presion;
            txtPeso.Text = a.Peso;
            txtTalla.Text = a.Talla;
            txtSintomas.Text = a.Sintomas;
            txtEvaluacion.Text = a.Evaluacion;
            txtTratamiento.Text = a.Tratamiento;
            txtObservaciones.Text = a.Observaciones;
            cmbEstado.SelectedItem = a.Estado;
        }

        private bool Validar()
        {
            if (string.IsNullOrWhiteSpace(txtDniPaciente.Text) ||
                string.IsNullOrWhiteSpace(txtPaciente.Text) ||
                string.IsNullOrWhiteSpace(txtDniProfesional.Text) ||
                string.IsNullOrWhiteSpace(txtProfesional.Text) ||
                string.IsNullOrWhiteSpace(txtSintomas.Text) ||
                string.IsNullOrWhiteSpace(txtEvaluacion.Text) ||
                string.IsNullOrWhiteSpace(txtTratamiento.Text))
            {
                MessageBox.Show("Complete los datos obligatorios de la atención.");
                return false;
            }

            if (!DniValido(txtDniPaciente.Text) ||
                !DniValido(txtDniProfesional.Text))
            {
                MessageBox.Show("Los DNI deben tener exactamente 8 dígitos.");
                return false;
            }

            if (!NumeroValido(txtTemperatura.Text, 25, 45) ||
                !NumeroValido(txtPeso.Text, 0.1, 500) ||
                !NumeroValido(txtTalla.Text, 0.1, 2.8))
            {
                MessageBox.Show("Revise la temperatura, el peso y la talla.");
                return false;
            }

            if (cmbEstado.SelectedItem == null)
            {
                MessageBox.Show("Seleccione el estado de la atención.");
                return false;
            }

            return true;
        }

        private bool DniValido(string dni)
        {
            return dni.Trim().Length == 8 &&
                   dni.Trim().All(char.IsDigit);
        }

        private bool NumeroValido(
            string valor,
            double minimo,
            double maximo)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return true;

            string normalizado = valor.Trim().Replace(',', '.');

            return double.TryParse(
                normalizado,
                System.Globalization.NumberStyles.AllowDecimalPoint,
                System.Globalization.CultureInfo.InvariantCulture,
                out double numero
            ) && numero >= minimo && numero <= maximo;
        }

        private AtencionVista ObtenerDatos()
        {
            return new AtencionVista
            {
                DniPaciente = txtDniPaciente.Text.Trim(),
                Paciente = txtPaciente.Text.Trim(),
                DniProfesional = txtDniProfesional.Text.Trim(),
                Profesional = txtProfesional.Text.Trim(),
                Fecha = dtpFecha.Value.Date,
                Temperatura = txtTemperatura.Text.Trim(),
                Presion = txtPresion.Text.Trim(),
                Peso = txtPeso.Text.Trim(),
                Talla = txtTalla.Text.Trim(),
                Sintomas = txtSintomas.Text.Trim(),
                Evaluacion = txtEvaluacion.Text.Trim(),
                Tratamiento = txtTratamiento.Text.Trim(),
                Observaciones = txtObservaciones.Text.Trim(),
                Estado = cmbEstado.Text
            };
        }

        private void Limpiar()
        {
            indiceSeleccionado = -1;

            txtDniPaciente.Clear();
            txtPaciente.Clear();
            txtDniProfesional.Clear();
            txtProfesional.Clear();
            txtTemperatura.Clear();
            txtPresion.Clear();
            txtPeso.Clear();
            txtTalla.Clear();
            txtSintomas.Clear();
            txtEvaluacion.Clear();
            txtTratamiento.Clear();
            txtObservaciones.Clear();

            dtpFecha.Value = DateTime.Today;
            cmbEstado.SelectedIndex = 0;

            dgvAtenciones.ClearSelection();
        }

        private class AtencionVista
        {
            public int IdAtencion { get; set; }
            public int IdPaciente { get; set; }
            public int IdPersonalSalud { get; set; }
            public string DniPaciente { get; set; } = "";
            public string Paciente { get; set; } = "";
            public string DniProfesional { get; set; } = "";
            public string Profesional { get; set; } = "";
            public DateTime Fecha { get; set; }
            public string Temperatura { get; set; } = "";
            public string Presion { get; set; } = "";
            public string Peso { get; set; } = "";
            public string Talla { get; set; } = "";
            public string Sintomas { get; set; } = "";
            public string Evaluacion { get; set; } = "";
            public string Tratamiento { get; set; } = "";
            public string Observaciones { get; set; } = "";
            public string Estado { get; set; } = "";
        }
    }
}





