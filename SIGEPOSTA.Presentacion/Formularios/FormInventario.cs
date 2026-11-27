
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using SIGEPOSTA.Presentacion.Estilos;
using SIGEPOSTA.Datos.Repositorios;

namespace SIGEPOSTA.Presentacion.Formularios
{
    internal static class InventarioEstilo
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
            GraphicsPath p = new();

            if (r.Width <= 0 || r.Height <= 0)
                return p;

            int d = Math.Min(radio * 2, Math.Min(r.Width, r.Height));

            p.AddArc(r.X, r.Y, d, d, 180, 90);
            p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            p.CloseFigure();

            return p;
        }
    }

    // =========================================================
    // CAMPO PERSONALIZADO
    // =========================================================

    internal class CampoInventario : UserControl
    {
        private readonly TextBox entrada = new();

        public override string Text
        {
            get => entrada.Text;
            set => entrada.Text = value;
        }

        public void ConfigurarMultilinea(
            bool multilinea,
            ScrollBars barras = ScrollBars.None)
        {
            entrada.Multiline = multilinea;
            entrada.ScrollBars = barras;
            AjustarEntrada();
        }

        public void Clear()
        {
            entrada.Clear();
        }

        public CampoInventario()
        {
            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw,
                true);

            BackColor = InventarioEstilo.Campo;
            Size = new Size(200, 38);
            MinimumSize = new Size(60, 38);
            Padding = new Padding(10, 3, 10, 3);

            entrada.BorderStyle = BorderStyle.None;
            entrada.BackColor = InventarioEstilo.Campo;
            entrada.ForeColor = InventarioEstilo.Texto;
            entrada.Font = new Font("Segoe UI", 10F);
            entrada.Dock = DockStyle.Fill;

            entrada.GotFocus += (s, e) => Invalidate();
            entrada.LostFocus += (s, e) => Invalidate();

            entrada.TextChanged += (s, e) =>
            {
                OnTextChanged(EventArgs.Empty);
            };

            Controls.Add(entrada);
            Resize += (s, e) => AjustarEntrada();
            AjustarEntrada();
        }

        private void AjustarEntrada()
        {
            if (entrada.Multiline)
            {
                Padding = new Padding(10, 7, 10, 7);
                MinimumSize = new Size(60, 70);

                entrada.Dock = DockStyle.Fill;
            }
            else
            {
                Padding = new Padding(10, 3, 10, 3);
                MinimumSize = new Size(60, 38);

                entrada.Dock = DockStyle.None;

                int alto = entrada.PreferredHeight;
                int ancho = Math.Max(0, ClientSize.Width - Padding.Horizontal);
                int posicionY = Math.Max(0, (ClientSize.Height - alto) / 2);

                entrada.SetBounds(
                    Padding.Left,
                    posicionY,
                    ancho,
                    alto
                );
            }

            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle r = new(1, 1, Width - 3, Height - 3);

            using GraphicsPath path =
                InventarioEstilo.Redondear(r, 9);

            using SolidBrush fondo =
                new(InventarioEstilo.Campo);

            using Pen borde = new(
                entrada.Focused
                    ? InventarioEstilo.Verde
                    : InventarioEstilo.Borde,
                entrada.Focused ? 1.8F : 1F);

            e.Graphics.FillPath(fondo, path);
            e.Graphics.DrawPath(borde, path);
        }
    }

    // =========================================================
    // BOTÓN PERSONALIZADO
    // =========================================================

    internal class BotonInventario : Button
    {
        private readonly TipoBoton tipo;
        private bool encima;
        private bool presionado;

        public BotonInventario(string texto, TipoBoton tipo)
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

            g.Clear(Parent?.BackColor ?? InventarioEstilo.Fondo);

            Rectangle sombra = new(2, 4, Width - 5, Height - 5);
            Rectangle r = new(1, 1, Width - 4, Height - 6);

            Color principal;
            Color secundario;

            if (tipo == TipoBoton.Peligro)
            {
                principal = InventarioEstilo.Rojo;
                secundario = Color.FromArgb(165, 43, 43);
            }
            else if (tipo == TipoBoton.Secundario)
            {
                principal = Color.White;
                secundario = Color.FromArgb(225, 240, 229);
            }
            else
            {
                principal = InventarioEstilo.Verde;
                secundario = InventarioEstilo.VerdeHover;
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
                InventarioEstilo.Redondear(sombra, 10);

            using SolidBrush sombraBrush =
                new(Color.FromArgb(30, 0, 0, 0));

            g.FillPath(sombraBrush, sombraPath);

            using GraphicsPath path =
                InventarioEstilo.Redondear(r, 10);

            using LinearGradientBrush degradado = new(
                r, principal, secundario, LinearGradientMode.Vertical);

            g.FillPath(degradado, path);

            Color colorTexto =
                tipo == TipoBoton.Secundario
                    ? InventarioEstilo.VerdeOscuro
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

    // =========================================================
    // TARJETA PERSONALIZADA
    // =========================================================

    internal class TarjetaInventario : Panel
    {
        public TarjetaInventario()
        {
            BackColor = Color.Transparent;
            Padding = new Padding(15);

            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw,
                true);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            e.Graphics.Clear(
                Parent?.BackColor ?? InventarioEstilo.Fondo);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle sombra = new(3, 4, Width - 8, Height - 8);
            Rectangle r = new(1, 1, Width - 4, Height - 5);

            using GraphicsPath sombraPath =
                InventarioEstilo.Redondear(sombra, 15);

            using SolidBrush sombraBrush =
                new(Color.FromArgb(18, 0, 0, 0));

            g.FillPath(sombraBrush, sombraPath);

            using GraphicsPath path =
                InventarioEstilo.Redondear(r, 15);

            using SolidBrush fondo = new(Color.White);
            using Pen borde = new(InventarioEstilo.Borde);

            g.FillPath(fondo, path);
            g.DrawPath(borde, path);
        }
    }

    // =========================================================
    // FORMULARIO INVENTARIO
    // =========================================================

    public class FormInventario : Form
    {
        private readonly BindingList<MovimientoVista> movimientos = new();
        private readonly InventarioRepositorio repositorio = new();
        private SIGEPOSTA.Seguridad.Modelos.SesionUsuario? sesion;

        private CampoInventario txtCodigoMedicamento = new();
        private CampoInventario txtMedicamento = new();
        private CampoInventario txtCantidad = new();
        private CampoInventario txtResponsable = new();
        private CampoInventario txtDocumento = new();
        private CampoInventario txtObservacion = new();

        private ComboBox cmbTipoMovimiento = new();
        private ComboBox cmbMotivo = new();

        private DateTimePicker dtpFecha = new();
        private DataGridView dgvMovimientos = new();

        private int indiceSeleccionado = -1;

        public FormInventario()
        {
            ConstruirFormulario();
            ConfigurarTabla();
            AplicarEstilos();

            sesion = FormInicioSesion.UsuarioAutenticado;

            txtCodigoMedicamento.Leave += (s, e) =>
                BuscarMedicamento();

            if (sesion != null && sesion.EstaAutenticado)
                txtResponsable.Text = sesion.NombreCompleto;

            CargarMovimientos();
        }

        // =====================================================
        // CONSTRUCCIÓN
        // =====================================================

        private void ConstruirFormulario()
        {
            Text = "SIGEPOSTA - Inventario";
            Size = new Size(1200, 850);
            MinimumSize = new Size(1000, 700);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = InventarioEstilo.Fondo;
            Font = new Font("Segoe UI", 10F);
            DoubleBuffered = true;

            // ENCABEZADO SUPERIOR

            Panel encabezado = new()
            {
                Dock = DockStyle.Top,
                Height = 110,
                BackColor = InventarioEstilo.VerdeOscuro,
                Padding = new Padding(30, 12, 30, 12)
            };

            TableLayoutPanel contenidoEncabezado = new()
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                BackColor = InventarioEstilo.VerdeOscuro,
                Margin = new Padding(0),
                Padding = new Padding(0)
            };

            contenidoEncabezado.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100));

            contenidoEncabezado.RowStyles.Add(
                new RowStyle(SizeType.Percent, 58));

            contenidoEncabezado.RowStyles.Add(
                new RowStyle(SizeType.Percent, 42));

            Label tituloEncabezado = new()
            {
                Text = "CONTROL DE INVENTARIO",
                Dock = DockStyle.Fill,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 21F, FontStyle.Bold),
                TextAlign = ContentAlignment.BottomLeft,
                BackColor = Color.Transparent,
                Margin = new Padding(0)
            };

            Label subtituloEncabezado = new()
            {
                Text = "Registro y seguimiento de movimientos de existencias",
                Dock = DockStyle.Fill,
                ForeColor = Color.FromArgb(220, 235, 225),
                Font = new Font("Segoe UI", 10F),
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent,
                Margin = new Padding(2, 0, 0, 0)
            };

            contenidoEncabezado.Controls.Add(tituloEncabezado, 0, 0);
            contenidoEncabezado.Controls.Add(subtituloEncabezado, 0, 1);

            encabezado.Controls.Add(contenidoEncabezado);

            // CONTENIDO PRINCIPAL

            Panel contenido = new()
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,

                // Separación superior de 40 píxeles
                Padding = new Padding(28, 40, 28, 20),

                BackColor = InventarioEstilo.Fondo
            };

            Controls.Add(contenido);
            Controls.Add(encabezado);

            TableLayoutPanel columna = new()
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 1,
                RowCount = 3,
                BackColor = InventarioEstilo.Fondo,
                Padding = new Padding(0)
            };

            columna.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100));

            for (int i = 0; i < 3; i++)
                columna.RowStyles.Add(
                    new RowStyle(SizeType.AutoSize));

            contenido.Controls.Add(columna);

            // =================================================
            // TARJETA DEL FORMULARIO
            // =================================================

            TarjetaInventario tarjetaFormulario = new()
            {
                Dock = DockStyle.Top,
                Height = 375,
                Margin = new Padding(0, 0, 0, 18)
            };

            columna.Controls.Add(tarjetaFormulario, 0, 0);

            TableLayoutPanel distribucionFormulario = new()
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(20, 12, 20, 15),
                BackColor = Color.White
            };

            distribucionFormulario.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100));

            distribucionFormulario.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 48));

            distribucionFormulario.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100));

            tarjetaFormulario.Controls.Add(distribucionFormulario);

            Label tituloFormulario = new()
            {
                Text = "Información del movimiento",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = InventarioEstilo.VerdeOscuro,
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.White,
                Margin = new Padding(0)
            };

            distribucionFormulario.Controls.Add(
                tituloFormulario, 0, 0);

            TableLayoutPanel rejilla = CrearRejilla();

            distribucionFormulario.Controls.Add(rejilla, 0, 1);

            // CAMPOS

            txtCodigoMedicamento = NuevoCampo();
            txtMedicamento = NuevoCampo();
            txtCantidad = NuevoCampo();
            txtResponsable = NuevoCampo();
            txtDocumento = NuevoCampo();
            txtObservacion = NuevaArea();

            cmbTipoMovimiento = NuevoCombo(
                "Entrada",
                "Salida",
                "Ajuste positivo",
                "Ajuste negativo");

            cmbMotivo = NuevoCombo(
                "Compra",
                "Reposición",
                "Consumo médico",
                "Entrega a paciente",
                "Vencimiento",
                "Deterioro",
                "Corrección de inventario",
                "Otro");

            dtpFecha = NuevoFecha();

            AgregarPar(rejilla, 0, 0,
                "ID medicamento:", txtCodigoMedicamento);

            AgregarPar(rejilla, 0, 2,
                "Medicamento:", txtMedicamento);

            AgregarPar(rejilla, 0, 4,
                "Tipo movimiento:", cmbTipoMovimiento);

            AgregarPar(rejilla, 1, 0,
                "Cantidad:", txtCantidad);

            AgregarPar(rejilla, 1, 2,
                "Fecha:", dtpFecha);

            AgregarPar(rejilla, 1, 4,
                "Motivo:", cmbMotivo);

            AgregarPar(rejilla, 2, 0,
                "Responsable:", txtResponsable);

            AgregarPar(rejilla, 2, 2,
                "Documento:", txtDocumento);

            rejilla.Controls.Add(
                NuevaEtiqueta("Observación:"), 0, 3);

            txtObservacion.Dock = DockStyle.Fill;
            txtObservacion.Margin = new Padding(3, 5, 12, 5);

            rejilla.Controls.Add(txtObservacion, 1, 3);
            rejilla.SetColumnSpan(txtObservacion, 5);

            // =================================================
            // BOTONES CENTRADOS
            // =================================================

            Panel panelBotones = new()
            {
                Dock = DockStyle.Fill,
                BackColor = InventarioEstilo.Fondo,
                Padding = new Padding(0, 8, 0, 8)
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
                BackColor = InventarioEstilo.Fondo
            };

            BotonInventario btnRegistrar =
                NuevoBoton("REGISTRAR", TipoBoton.Primario);

            btnRegistrar.Click += (s, e) => Registrar();

            BotonInventario btnActualizar =
                NuevoBoton("ACTUALIZAR", TipoBoton.Secundario);

            btnActualizar.Click += (s, e) => Actualizar();

            BotonInventario btnEliminar =
                NuevoBoton("ELIMINAR", TipoBoton.Peligro);

            btnEliminar.Click += (s, e) => Eliminar();

            BotonInventario btnLimpiar =
                NuevoBoton("LIMPIAR", TipoBoton.Secundario);

            btnLimpiar.Click += (s, e) => Limpiar();

            botones.Controls.AddRange(new Control[]
            {
                btnRegistrar,
                btnActualizar,
                btnEliminar,
                btnLimpiar
            });

            panelBotones.Controls.Add(botones);

            panelBotones.Resize += (s, e) =>
            {
                botones.Location = new Point(
                    Math.Max(0, (panelBotones.ClientSize.Width - botones.Width) / 2),
                    Math.Max(0, (panelBotones.ClientSize.Height - botones.Height) / 2));
            };

            columna.Controls.Add(panelBotones, 0, 1);

            // =================================================
            // TARJETA DEL HISTORIAL
            // =================================================

            TarjetaInventario tarjetaTabla = new()
            {
                Dock = DockStyle.Top,
                Height = 370,
                Margin = new Padding(0, 0, 0, 15)
            };

            columna.Controls.Add(tarjetaTabla, 0, 2);

            TableLayoutPanel distribucionTabla = new()
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(20, 12, 20, 15),
                BackColor = Color.White
            };

            distribucionTabla.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100));

            distribucionTabla.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 48));

            distribucionTabla.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100));

            tarjetaTabla.Controls.Add(distribucionTabla);

            Label tituloTabla = new()
            {
                Text = "Historial de movimientos",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = InventarioEstilo.VerdeOscuro,
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.White,
                Margin = new Padding(0)
            };

            distribucionTabla.Controls.Add(tituloTabla, 0, 0);

            Panel contenedorTabla = new()
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0),
                BackColor = Color.White,
                Margin = new Padding(0)
            };

            dgvMovimientos = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode =
                    DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode =
                    DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White,
                RowHeadersVisible = false,
                BorderStyle = BorderStyle.None,
                Margin = new Padding(0)
            };

            dgvMovimientos.SelectionChanged +=
                (s, e) => Seleccionar();

            contenedorTabla.Controls.Add(dgvMovimientos);
            distribucionTabla.Controls.Add(contenedorTabla, 0, 1);
        }

        // =====================================================
        // REJILLA
        // =====================================================

        private TableLayoutPanel CrearRejilla()
        {
            TableLayoutPanel tabla = new()
            {
                Dock = DockStyle.Fill,
                ColumnCount = 6,
                RowCount = 4,
                BackColor = Color.White,
                Padding = new Padding(5, 5, 5, 0),
                GrowStyle = TableLayoutPanelGrowStyle.FixedSize
            };

            tabla.ColumnStyles.Add(
                new ColumnStyle(SizeType.Absolute, 135));

            tabla.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 33.33F));

            tabla.ColumnStyles.Add(
                new ColumnStyle(SizeType.Absolute, 110));

            tabla.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 33.33F));

            tabla.ColumnStyles.Add(
                new ColumnStyle(SizeType.Absolute, 125));

            tabla.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 33.33F));

            tabla.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 48));

            tabla.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 48));

            tabla.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 48));

            tabla.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100));

            return tabla;
        }

        private void AgregarPar(
            TableLayoutPanel tabla,
            int fila,
            int columna,
            string texto,
            Control campo)
        {
            tabla.Controls.Add(
                NuevaEtiqueta(texto), columna, fila);

            campo.Dock = DockStyle.Fill;
            campo.Margin = new Padding(3, 5, 12, 5);

            tabla.Controls.Add(campo, columna + 1, fila);
        }

        // =====================================================
        // CONTROLES
        // =====================================================

        private Label NuevaEtiqueta(string texto)
        {
            return new Label
            {
                Text = texto,
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                ForeColor = InventarioEstilo.Texto,
                Font = new Font("Segoe UI Semibold", 9.5F),
                BackColor = Color.Transparent,
                Margin = new Padding(3, 10, 3, 3)
            };
        }

        private CampoInventario NuevoCampo()
        {
            return new CampoInventario();
        }

        private CampoInventario NuevaArea()
        {
            CampoInventario campo = new()
            {
                Dock = DockStyle.Fill,
                BackColor = InventarioEstilo.Campo,
                ForeColor = InventarioEstilo.Texto,
                Font = new Font("Segoe UI", 10F),
                Margin = new Padding(5, 8, 5, 8),
                MinimumSize = new Size(0, 70),
                Padding = new Padding(12)
            };

            campo.ConfigurarMultilinea(
                true, ScrollBars.Vertical);

            return campo;
        }

        private ComboBox NuevoCombo(params string[] opciones)
        {
            ComboBox combo = new()
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10F),
                BackColor = InventarioEstilo.Campo,
                ForeColor = InventarioEstilo.Texto,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(3, 5, 12, 5)
            };

            combo.Items.AddRange(opciones);

            if (combo.Items.Count > 0)
                combo.SelectedIndex = 0;

            return combo;
        }

        private DateTimePicker NuevoFecha()
        {
            return new DateTimePicker
            {
                Dock = DockStyle.Fill,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy",
                Value = DateTime.Today,
                Font = new Font("Segoe UI", 10F),
                Margin = new Padding(3, 5, 12, 5),
                CalendarMonthBackground = Color.White,
                CalendarTitleBackColor = InventarioEstilo.VerdeOscuro,
                CalendarTitleForeColor = Color.White
            };
        }

        private BotonInventario NuevoBoton(
            string texto,
            TipoBoton tipo)
        {
            return new BotonInventario(texto, tipo)
            {
                Size = new Size(155, 46),
                Margin = new Padding(8, 0, 8, 5)
            };
        }

        // =====================================================
        // ESTILOS DE TABLA
        // =====================================================

        private void AplicarEstilos()
        {
            dgvMovimientos.EnableHeadersVisualStyles = false;
            dgvMovimientos.BackgroundColor = Color.White;
            dgvMovimientos.BorderStyle = BorderStyle.None;
            dgvMovimientos.GridColor = InventarioEstilo.Borde;
            dgvMovimientos.RowHeadersVisible = false;

            dgvMovimientos.ColumnHeadersHeight = 44;

            dgvMovimientos.ColumnHeadersHeightSizeMode =
                DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            dgvMovimientos.ColumnHeadersDefaultCellStyle =
                new DataGridViewCellStyle
                {
                    BackColor = InventarioEstilo.VerdeOscuro,
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                    SelectionBackColor = InventarioEstilo.VerdeOscuro,
                    SelectionForeColor = Color.White,
                    Padding = new Padding(8, 0, 0, 0)
                };

            dgvMovimientos.DefaultCellStyle =
                new DataGridViewCellStyle
                {
                    BackColor = Color.White,
                    ForeColor = InventarioEstilo.Texto,
                    Font = new Font("Segoe UI", 9.5F),
                    SelectionBackColor = Color.FromArgb(225, 240, 229),
                    SelectionForeColor = InventarioEstilo.Texto,
                    Padding = new Padding(8, 0, 0, 0)
                };

            dgvMovimientos.AlternatingRowsDefaultCellStyle =
                new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(247, 250, 248),
                    ForeColor = InventarioEstilo.Texto,
                    SelectionBackColor = Color.FromArgb(225, 240, 229),
                    SelectionForeColor = InventarioEstilo.Texto,
                    Font = new Font("Segoe UI", 9.5F)
                };

            dgvMovimientos.RowTemplate.Height = 38;
        }

        // =====================================================
        // CONFIGURAR TABLA
        // =====================================================

        private void ConfigurarTabla()
        {
            dgvMovimientos.AutoGenerateColumns = false;

            Columna("CodigoMedicamento", "Código");
            Columna("Medicamento", "Medicamento");
            Columna("Tipo", "Movimiento");
            Columna("Cantidad", "Cantidad");
            Columna("Fecha", "Fecha");
            Columna("Motivo", "Motivo");
            Columna("Responsable", "Responsable");

            dgvMovimientos.DataSource = movimientos;
        }

        private void Columna(string propiedad, string titulo)
        {
            dgvMovimientos.Columns.Add(
                new DataGridViewTextBoxColumn
                {
                    DataPropertyName = propiedad,
                    HeaderText = titulo
                });
        }

        // =====================================================
        // =====================================================
        // REGISTRAR MOVIMIENTO EN SQL SERVER
        // =====================================================

        private void Registrar()
        {
            if (!Validar())
                return;

            if (sesion == null || !sesion.EstaAutenticado)
            {
                MessageBox.Show("No existe una sesión autenticada.");
                return;
            }

            try
            {
                string motivo = cmbMotivo.Text;

                if (!string.IsNullOrWhiteSpace(txtDocumento.Text))
                    motivo += " | Documento: " + txtDocumento.Text.Trim();

                if (!string.IsNullOrWhiteSpace(txtObservacion.Text))
                    motivo += " | Observación: " + txtObservacion.Text.Trim();

                repositorio.RegistrarMovimiento(
                    int.Parse(txtCodigoMedicamento.Text),
                    sesion.UsuarioId,
                    cmbTipoMovimiento.Text,
                    int.Parse(txtCantidad.Text),
                    motivo,
                    dtpFecha.Value
                );

                CargarMovimientos();

                MessageBox.Show(
                    "Movimiento registrado correctamente en la base de datos.",
                    "Inventario",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                Limpiar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "No se pudo registrar el movimiento",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        // =====================================================
        // ACTUALIZAR
        // =====================================================

        private void Actualizar()
        {
            if (!Validar())
                return;

            if (dgvMovimientos.CurrentRow?.DataBoundItem
                is not MovimientoVista seleccionado ||
                seleccionado.IdMovimiento <= 0)
            {
                MessageBox.Show(
                    "Seleccione un movimiento del historial para actualizar.",
                    "Inventario",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            try
            {
                string motivo = cmbMotivo.Text.Trim();

                if (!string.IsNullOrWhiteSpace(txtDocumento.Text))
                    motivo += " | Documento: " + txtDocumento.Text.Trim();

                if (!string.IsNullOrWhiteSpace(txtObservacion.Text))
                    motivo += " | Observación: " + txtObservacion.Text.Trim();

                repositorio.ActualizarMovimiento(
                    seleccionado.IdMovimiento,
                    int.Parse(txtCodigoMedicamento.Text),
                    sesion!.UsuarioId,
                    cmbTipoMovimiento.Text,
                    int.Parse(txtCantidad.Text),
                    motivo,
                    dtpFecha.Value
                );

                CargarMovimientos();

                MessageBox.Show(
                    "Movimiento actualizado correctamente.",
                    "Inventario",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                Limpiar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "No se pudo actualizar el movimiento",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        // =====================================================
        // ELIMINAR
        // =====================================================

        private void Eliminar()
        {
            if (dgvMovimientos.CurrentRow?.DataBoundItem
                is not MovimientoVista seleccionado ||
                seleccionado.IdMovimiento <= 0)
            {
                MessageBox.Show(
                    "Seleccione un movimiento del historial para eliminar.",
                    "Inventario",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            DialogResult respuesta = MessageBox.Show(
                "¿Está seguro de eliminar este movimiento? " +
                "También se revertirá su efecto sobre el stock.",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (respuesta != DialogResult.Yes)
                return;

            try
            {
                repositorio.EliminarMovimiento(seleccionado.IdMovimiento);

                CargarMovimientos();

                MessageBox.Show(
                    "Movimiento eliminado y stock revertido correctamente.",
                    "Inventario",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                Limpiar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "No se pudo eliminar el movimiento",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        // =====================================================
        // SELECCIONAR
        // =====================================================

        private void Seleccionar()
        {
            if (dgvMovimientos.CurrentRow?.DataBoundItem
                is not MovimientoVista m)
                return;

            indiceSeleccionado = movimientos.IndexOf(m);

            txtCodigoMedicamento.Text = m.CodigoMedicamento;
            txtMedicamento.Text = m.Medicamento;

            if (cmbTipoMovimiento.Items.Contains(m.Tipo))
                cmbTipoMovimiento.SelectedItem = m.Tipo;

            txtCantidad.Text = m.Cantidad.ToString();
            dtpFecha.Value = m.Fecha;
            txtResponsable.Text = m.Responsable;

            txtDocumento.Text = ExtraerDatoMotivo(
                m.Motivo, "Documento:");

            txtObservacion.Text = ExtraerDatoMotivo(
                m.Motivo, "Observación:");

            string motivoBase = m.Motivo;

            int posicionDocumento = motivoBase.IndexOf(
                " | Documento:", StringComparison.OrdinalIgnoreCase);

            if (posicionDocumento >= 0)
                motivoBase = motivoBase.Substring(0, posicionDocumento);

            int posicionObservacion = motivoBase.IndexOf(
                " | Observación:", StringComparison.OrdinalIgnoreCase);

            if (posicionObservacion >= 0)
                motivoBase = motivoBase.Substring(0, posicionObservacion);

            motivoBase = motivoBase.Trim();

            if (cmbMotivo.Items.Contains(motivoBase))
                cmbMotivo.SelectedItem = motivoBase;
            else
                cmbMotivo.SelectedIndex = -1;
        }

        private string ExtraerDatoMotivo(string motivo, string etiqueta)
        {
            int inicio = motivo.IndexOf(
                etiqueta, StringComparison.OrdinalIgnoreCase);

            if (inicio < 0)
                return string.Empty;

            inicio += etiqueta.Length;

            int siguienteDocumento = motivo.IndexOf(
                " | Documento:", inicio, StringComparison.OrdinalIgnoreCase);

            int siguienteObservacion = motivo.IndexOf(
                " | Observación:", inicio, StringComparison.OrdinalIgnoreCase);

            int fin = motivo.Length;

            if (siguienteDocumento >= 0 && siguienteDocumento < fin)
                fin = siguienteDocumento;

            if (siguienteObservacion >= 0 && siguienteObservacion < fin)
                fin = siguienteObservacion;

            return motivo.Substring(inicio, fin - inicio).Trim();
        }

        // =====================================================
        // VALIDAR
        // =====================================================

        private bool Validar()
        {
            if (!int.TryParse(
                txtCodigoMedicamento.Text,
                out int idMedicamento) || idMedicamento <= 0)
            {
                MessageBox.Show("Ingrese un ID de medicamento válido.");
                txtCodigoMedicamento.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtMedicamento.Text))
            {
                MessageBox.Show("Busque un medicamento registrado.");
                txtCodigoMedicamento.Focus();
                return false;
            }

            if (sesion == null || !sesion.EstaAutenticado)
            {
                MessageBox.Show("No se encontró el usuario autenticado.");
                return false;
            }

            if (!int.TryParse(txtCantidad.Text, out int cantidad) ||
                cantidad <= 0)
            {
                MessageBox.Show("Ingrese una cantidad mayor que cero.");
                txtCantidad.Focus();
                return false;
            }

            if (cmbTipoMovimiento.SelectedItem == null ||
                cmbMotivo.SelectedItem == null)
            {
                MessageBox.Show("Seleccione el tipo y el motivo.");
                return false;
            }

            return true;
        }

        // =====================================================
        // OBTENER DATOS
        // =====================================================

        private MovimientoVista ObtenerDatos()
        {
            return new MovimientoVista
            {
                CodigoMedicamento = txtCodigoMedicamento.Text.Trim(),
                Medicamento = txtMedicamento.Text.Trim(),
                Tipo = cmbTipoMovimiento.Text,
                Cantidad = int.Parse(txtCantidad.Text),
                Fecha = dtpFecha.Value,
                Motivo = cmbMotivo.Text,
                Responsable = txtResponsable.Text.Trim(),
                Documento = txtDocumento.Text.Trim(),
                Observacion = txtObservacion.Text.Trim()
            };
        }

        // =====================================================
        // LIMPIAR
        // =====================================================

        private void Limpiar()
        {
            indiceSeleccionado = -1;

            txtCodigoMedicamento.Clear();
            txtMedicamento.Clear();
            txtCantidad.Clear();
            txtDocumento.Clear();
            txtObservacion.Clear();

            if (sesion != null)
                txtResponsable.Text = sesion.NombreCompleto;

            cmbTipoMovimiento.SelectedIndex = 0;
            cmbMotivo.SelectedIndex = 0;

            dtpFecha.Value = DateTime.Today;

            dgvMovimientos.ClearSelection();
        }

        // =====================================================
        // BUSCAR MEDICAMENTO POR ID
        // =====================================================

        private void BuscarMedicamento()
        {
            if (!int.TryParse(txtCodigoMedicamento.Text, out int id) || id <= 0)
                return;

            try
            {
                var medicamento = repositorio.ObtenerMedicamento(id);

                if (medicamento == null)
                {
                    txtMedicamento.Clear();

                    MessageBox.Show(
                        "No se encontró un medicamento activo con ese ID.",
                        "Consulta de medicamento",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    return;
                }

                txtMedicamento.Text = medicamento.Nombre;

                MessageBox.Show(
                    "Medicamento: " + medicamento.Nombre +
                    Environment.NewLine +
                    "Stock actual: " + medicamento.StockActual +
                    Environment.NewLine +
                    "Stock mínimo: " + medicamento.StockMinimo,
                    "Existencias",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al consultar el medicamento: " + ex.Message);
            }
        }

        // =====================================================
        // CARGAR HISTORIAL DESDE SQL SERVER
        // =====================================================

        private void CargarMovimientos()
        {
            try
            {
                movimientos.Clear();

                foreach (var registro in repositorio.ObtenerMovimientos())
                {
                    movimientos.Add(new MovimientoVista
                    {
                        IdMovimiento = registro.IdMovimiento,
                        IdInventario = registro.IdInventario,
                        CodigoMedicamento = registro.IdMedicamento.ToString(),
                        Medicamento = registro.Medicamento,
                        Tipo = registro.Tipo,
                        Cantidad = registro.Cantidad,
                        Fecha = registro.Fecha,
                        Motivo = registro.Motivo,
                        Responsable = registro.Responsable
                    });
                }

                dgvMovimientos.ClearSelection();
                indiceSeleccionado = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo cargar el historial: " + ex.Message,
                    "Inventario",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        // MODELO
        // =====================================================

        private class MovimientoVista
        {
            public int IdMovimiento { get; set; }
            public int IdInventario { get; set; }
            public string CodigoMedicamento { get; set; } = "";
            public string Medicamento { get; set; } = "";
            public string Tipo { get; set; } = "";
            public int Cantidad { get; set; }
            public DateTime Fecha { get; set; }
            public string Motivo { get; set; } = "";
            public string Responsable { get; set; } = "";
            public string Documento { get; set; } = "";
            public string Observacion { get; set; } = "";
        }
    }
}



