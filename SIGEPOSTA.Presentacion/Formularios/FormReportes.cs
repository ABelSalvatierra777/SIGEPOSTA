using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Text;
using SIGEPOSTA.Presentacion.Estilos;
using SIGEPOSTA.Datos.Repositorios;
using SIGEPOSTA.Negocio.Servicios;

namespace SIGEPOSTA.Presentacion.Formularios
{
    public class FormReportes : Form
    {
        private ComboBox cmbReporte = new();
        private DateTimePicker dtpDesde = new();
        private DateTimePicker dtpHasta = new();
        private DataGridView dgvReporte = new();
        private Label lblResumen = new();
        private readonly ReporteServicio servicio = new(new ReporteRepositorio());

        public FormReportes()
        {
            ConstruirFormulario();
            AplicarEstilos();
        }

        private void ConstruirFormulario()
        {
            Text = "SIGEPOSTA - Reportes";
            Size = new Size(1250, 820);
            MinimumSize = new Size(1050, 700);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = InventarioEstilo.Fondo;
            Font = new Font("Segoe UI", 9.5F);

            Panel encabezado = new Panel
            {
                Dock = DockStyle.Top,
                Height = 110,
                BackColor = InventarioEstilo.VerdeOscuro,
                Padding = new Padding(30, 12, 30, 12)
            };

            TableLayoutPanel contenidoEncabezado = new TableLayoutPanel
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

            Label lblTitulo = new Label
            {
                Text = "REPORTES DEL ESTABLECIMIENTO",
                Dock = DockStyle.Fill,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 21F, FontStyle.Bold),
                TextAlign = ContentAlignment.BottomLeft,
                BackColor = Color.Transparent,
                Margin = new Padding(0)
            };

            Label lblSubtitulo = new Label
            {
                Text = "Consulta y generación de reportes administrativos y clínicos",
                Dock = DockStyle.Fill,
                ForeColor = Color.FromArgb(220, 235, 225),
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent,
                Margin = new Padding(2, 0, 0, 0)
            };

            contenidoEncabezado.Controls.Add(lblTitulo, 0, 0);
            contenidoEncabezado.Controls.Add(lblSubtitulo, 0, 1);
            encabezado.Controls.Add(contenidoEncabezado);

            Panel cuerpo = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(28, 28, 28, 20),
                BackColor = InventarioEstilo.Fondo
            };

            TableLayoutPanel contenido = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 1,
                RowCount = 3,
                BackColor = InventarioEstilo.Fondo,
                Margin = new Padding(0)
            };

            contenido.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100));
            contenido.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 205));
            contenido.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 390));
            contenido.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 75));

            contenido.Controls.Add(CrearTarjetaFiltros(), 0, 0);
            contenido.Controls.Add(CrearTarjetaResultados(), 0, 1);
            contenido.Controls.Add(CrearPie(), 0, 2);

            cuerpo.Controls.Add(contenido);

            Controls.Add(cuerpo);
            Controls.Add(encabezado);
        }

        private Control CrearTarjetaFiltros()
        {
            TarjetaInventario tarjeta = new TarjetaInventario
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20, 14, 20, 12),
                Margin = new Padding(0, 0, 0, 14),
                BackColor = Color.White
            };

            TableLayoutPanel distribucion = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                BackColor = Color.White,
                Margin = new Padding(0)
            };

            distribucion.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100));
            distribucion.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 30));
            distribucion.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100));
            distribucion.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 48));

            Label titulo = new Label
            {
                Text = "FILTROS DEL REPORTE",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = InventarioEstilo.VerdeOscuro,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(0)
            };

            TableLayoutPanel rejilla = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 6,
                RowCount = 2,
                BackColor = Color.White,
                Padding = new Padding(0, 5, 0, 2),
                Margin = new Padding(0)
            };

            rejilla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 13));
            rejilla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
            rejilla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10));
            rejilla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
            rejilla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10));
            rejilla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 27));

            rejilla.RowStyles.Add(new RowStyle(SizeType.Absolute, 25));
            rejilla.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            cmbReporte = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9.5F),
                BackColor = InventarioEstilo.Campo,
                ForeColor = InventarioEstilo.Texto,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(3, 2, 12, 5)
            };

            cmbReporte.Items.AddRange(new object[]
            {
                "Reporte de pacientes",
                "Reporte de personal de salud",
                "Reporte de citas médicas",
                "Reporte de atenciones médicas",
                "Reporte de diagnósticos",
                "Reporte de medicamentos",
                "Reporte de movimientos de inventario"
            });

            cmbReporte.SelectedIndex = 0;

            dtpDesde = CrearSelectorFecha();
            dtpHasta = CrearSelectorFecha();

            AgregarCampoFiltro(rejilla, "Tipo de reporte:", cmbReporte, 0);
            AgregarCampoFiltro(rejilla, "Desde:", dtpDesde, 2);
            AgregarCampoFiltro(rejilla, "Hasta:", dtpHasta, 4);

            FlowLayoutPanel botones = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoSize = false,
                BackColor = Color.White,
                Anchor = AnchorStyles.None,
                Width = 280,
                Height = 43,
                Margin = new Padding(0)
            };

            BotonInventario btnConsultar =
                new BotonInventario("CONSULTAR", TipoBoton.Primario)
                {
                    Size = new Size(135, 38),
                    Margin = new Padding(3, 2, 8, 2)
                };
            btnConsultar.Click += (s, e) => Consultar();

            BotonInventario btnLimpiar =
                new BotonInventario("LIMPIAR", TipoBoton.Secundario)
                {
                    Size = new Size(125, 38),
                    Margin = new Padding(3, 2, 3, 2)
                };
            btnLimpiar.Click += (s, e) => Limpiar();

            botones.Controls.Add(btnConsultar);
            botones.Controls.Add(btnLimpiar);

            Panel panelBotones = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Margin = new Padding(0)
            };

            panelBotones.Controls.Add(botones);

            void CentrarBotones()
            {
                botones.Left = Math.Max(
                    0, (panelBotones.ClientSize.Width - botones.Width) / 2);
                botones.Top = Math.Max(
                    0, (panelBotones.ClientSize.Height - botones.Height) / 2);
            }

            panelBotones.Resize += (s, e) => CentrarBotones();
            CentrarBotones();

            distribucion.Controls.Add(titulo, 0, 0);
            distribucion.Controls.Add(rejilla, 0, 1);
            distribucion.Controls.Add(panelBotones, 0, 2);

            tarjeta.Controls.Add(distribucion);
            return tarjeta;
        }

        private DateTimePicker CrearSelectorFecha()
        {
            return new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9.5F),
                CalendarForeColor = InventarioEstilo.Texto,
                CalendarMonthBackground = Color.White,
                Margin = new Padding(3, 2, 12, 5)
            };
        }

        private void AgregarCampoFiltro(
            TableLayoutPanel rejilla,
            string texto,
            Control campo,
            int columna)
        {
            Label etiqueta = new Label
            {
                Text = texto,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.BottomLeft,
                ForeColor = InventarioEstilo.Texto,
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                Margin = new Padding(3, 0, 3, 3)
            };

            rejilla.Controls.Add(etiqueta, columna, 0);
            rejilla.Controls.Add(campo, columna, 1);
            rejilla.SetColumnSpan(campo, 2);
        }

        private Control CrearTarjetaResultados()
        {
            TarjetaInventario tarjeta = new TarjetaInventario
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(18, 14, 18, 16),
                Margin = new Padding(0, 0, 0, 12),
                BackColor = Color.White
            };

            TableLayoutPanel distribucion = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                BackColor = Color.White,
                Margin = new Padding(0)
            };

            distribucion.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100));
            distribucion.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 35));
            distribucion.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100));

            Label titulo = new Label
            {
                Text = "RESULTADOS DEL REPORTE",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = InventarioEstilo.VerdeOscuro,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(0)
            };

            dgvReporte = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White,
                RowHeadersVisible = false,
                AutoGenerateColumns = true,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                GridColor = InventarioEstilo.Borde,
                EnableHeadersVisualStyles = false,
                ColumnHeadersHeight = 42,
                RowTemplate = { Height = 38 },
                Font = new Font("Segoe UI", 9F),
                Margin = new Padding(0, 5, 0, 0)
            };

            dgvReporte.ColumnHeadersDefaultCellStyle.BackColor =
                InventarioEstilo.VerdeOscuro;
            dgvReporte.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvReporte.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvReporte.ColumnHeadersDefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleLeft;
            dgvReporte.ColumnHeadersDefaultCellStyle.Padding =
                new Padding(10, 0, 10, 0);

            dgvReporte.DefaultCellStyle.BackColor = Color.White;
            dgvReporte.DefaultCellStyle.ForeColor = InventarioEstilo.Texto;
            dgvReporte.DefaultCellStyle.SelectionBackColor =
                Color.FromArgb(220, 238, 228);
            dgvReporte.DefaultCellStyle.SelectionForeColor =
                InventarioEstilo.VerdeOscuro;
            dgvReporte.DefaultCellStyle.Padding =
                new Padding(10, 0, 6, 0);

            dgvReporte.AlternatingRowsDefaultCellStyle.BackColor =
                Color.FromArgb(244, 248, 245);

            dgvReporte.RowHeadersDefaultCellStyle.BackColor =
                InventarioEstilo.VerdeOscuro;

            distribucion.Controls.Add(titulo, 0, 0);
            distribucion.Controls.Add(dgvReporte, 0, 1);

            tarjeta.Controls.Add(distribucion);
            return tarjeta;
        }

        private Control CrearPie()
        {
            Panel pie = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = InventarioEstilo.Fondo,
                Margin = new Padding(0)
            };

            lblResumen = new Label
            {
                Text = "Sin información para mostrar.",
                AutoSize = false,
                Dock = DockStyle.Left,
                Width = 750,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = InventarioEstilo.VerdeOscuro,
                Font = new Font("Segoe UI", 9.5F),
                Padding = new Padding(5, 0, 5, 0)
            };

            BotonInventario btnExportar =
                new BotonInventario("EXPORTAR CSV", TipoBoton.Primario)
                {
                    Size = new Size(165, 42),
                    Anchor = AnchorStyles.Right
                };

            btnExportar.Click += (s, e) => ExportarCSV();

            pie.Controls.Add(lblResumen);
            pie.Controls.Add(btnExportar);

            void PosicionarBoton()
            {
                btnExportar.Left = pie.ClientSize.Width - btnExportar.Width;
                btnExportar.Top = Math.Max(
                    0, (pie.ClientSize.Height - btnExportar.Height) / 2);
            }

            pie.Resize += (s, e) => PosicionarBoton();
            PosicionarBoton();

            return pie;
        }

        private void AplicarEstilos()
        {
            BackColor = InventarioEstilo.Fondo;
            Font = new Font("Segoe UI", 9.5F);
        }

        private void Consultar()
        {
            if (dtpDesde.Value.Date > dtpHasta.Value.Date)
            {
                MessageBox.Show(
                    "La fecha inicial no puede ser posterior a la fecha final.",
                    "Validación de fechas",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Cursor = Cursors.WaitCursor;

                var resultados = servicio.ObtenerReporte(
                    cmbReporte.Text,
                    dtpDesde.Value.Date,
                    dtpHasta.Value.Date);

                dgvReporte.DataSource = resultados;

                lblResumen.Text =
                    "Reporte: " + cmbReporte.Text +
                    " | Registros encontrados: " + resultados.Rows.Count +
                    " | Periodo: " +
                    dtpDesde.Value.ToShortDateString() + " - " +
                    dtpHasta.Value.ToShortDateString();
            }
            catch (Exception ex)
            {
                dgvReporte.DataSource = null;

                MessageBox.Show(
                    "No se pudo generar el reporte.\n\n" + ex.Message,
                    "Error al consultar",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }
        private void ExportarCSV()
        {
            if (dgvReporte.Columns.Count == 0 ||
                dgvReporte.Rows.Count == 0)
            {
                MessageBox.Show(
                    "No existen resultados para exportar.",
                    "Exportar reporte",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            using SaveFileDialog guardar = new SaveFileDialog
            {
                Filter = "Archivo CSV (*.csv)|*.csv",
                FileName = "Reporte_SIGEPOSTA.csv",
                Title = "Guardar reporte"
            };

            if (guardar.ShowDialog() != DialogResult.OK)
                return;

            try
            {
                using StreamWriter escritor = new StreamWriter(
                    guardar.FileName,
                    false,
                    Encoding.UTF8);

                string encabezados = string.Join(";",
                    dgvReporte.Columns.Cast<DataGridViewColumn>()
                    .Select(c => EscaparCSV(c.HeaderText)));

                escritor.WriteLine(encabezados);

                foreach (DataGridViewRow fila in dgvReporte.Rows)
                {
                    if (fila.IsNewRow)
                        continue;

                    string valores = string.Join(";",
                        fila.Cells.Cast<DataGridViewCell>()
                        .Select(c => EscaparCSV(c.Value?.ToString() ?? "")));

                    escritor.WriteLine(valores);
                }

                MessageBox.Show(
                    "Reporte exportado correctamente.",
                    "Exportación completada",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al exportar: " + ex.Message,
                    "Error de exportación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private string EscaparCSV(string valor)
        {
            return "\"" + valor.Replace("\"", "\"\"") + "\"";
        }

        private void Limpiar()
        {
            cmbReporte.SelectedIndex = 0;
            dtpDesde.Value = DateTime.Today;
            dtpHasta.Value = DateTime.Today;
            dgvReporte.DataSource = null;
            lblResumen.Text = "Sin información para mostrar.";
        }
    }
}

