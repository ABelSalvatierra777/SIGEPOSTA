using System;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Windows.Forms;
using SIGEPOSTA.Presentacion.Estilos;
using SIGEPOSTA.Respaldos.Servicios;

namespace SIGEPOSTA.Presentacion.Formularios
{
    public class FormRespaldos : Form
    {
        private TextBox txtCarpetaOrigen = new();
        private TextBox txtCarpetaRespaldos = new();
        private DataGridView dgvRespaldos = new();
        private Label lblEstado = new();
        private readonly RespaldoServicio servicio = new();

        public FormRespaldos()
        {
            ConstruirFormulario();
            AplicarEstilos();
        }

        private void ConstruirFormulario()
        {
            Text = "SIGEPOSTA - Respaldos";
            Size = new Size(1200, 850);
            MinimumSize = new Size(1000, 700);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = InventarioEstilo.Fondo;
            Font = new Font("Segoe UI", 9.5F);
            DoubleBuffered = true;

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

            Label titulo = new Label
            {
                Text = "COPIAS DE SEGURIDAD Y RESPALDOS",
                Dock = DockStyle.Fill,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 21F, FontStyle.Bold),
                TextAlign = ContentAlignment.BottomLeft,
                BackColor = Color.Transparent,
                Margin = new Padding(0)
            };

            Label subtitulo = new Label
            {
                Text = "Administración y restauración de copias de seguridad de SIGEPOSTA",
                Dock = DockStyle.Fill,
                ForeColor = Color.FromArgb(220, 235, 225),
                Font = new Font("Segoe UI", 10F),
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent,
                Margin = new Padding(2, 0, 0, 0)
            };

            contenidoEncabezado.Controls.Add(titulo, 0, 0);
            contenidoEncabezado.Controls.Add(subtitulo, 0, 1);
            encabezado.Controls.Add(contenidoEncabezado);

            Panel cuerpo = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(28, 40, 28, 20),
                BackColor = InventarioEstilo.Fondo
            };

            TableLayoutPanel contenido = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 1,
                RowCount = 4,
                BackColor = InventarioEstilo.Fondo,
                Margin = new Padding(0)
            };

            contenido.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100));
            contenido.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 220));
            contenido.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 75));
            contenido.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 370));
            contenido.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 55));

            contenido.Controls.Add(CrearTarjetaCarpetas(), 0, 0);
            contenido.Controls.Add(CrearBotonera(), 0, 1);
            contenido.Controls.Add(CrearTarjetaTabla(), 0, 2);
            contenido.Controls.Add(CrearEstado(), 0, 3);

            cuerpo.Controls.Add(contenido);

            Controls.Add(cuerpo);
            Controls.Add(encabezado);
        }

        private Control CrearTarjetaCarpetas()
        {
            TarjetaInventario tarjeta = new TarjetaInventario
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20, 14, 20, 14),
                Margin = new Padding(0, 0, 0, 12),
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
                new RowStyle(SizeType.Absolute, 38));
            distribucion.RowStyles.Add(
                new RowStyle(SizeType.Percent, 50));
            distribucion.RowStyles.Add(
                new RowStyle(SizeType.Percent, 50));

            Label titulo = new Label
            {
                Text = "CONFIGURACIÓN DE CARPETAS",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = InventarioEstilo.VerdeOscuro,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(0)
            };

            txtCarpetaOrigen = CrearCampoRuta();
            txtCarpetaRespaldos = CrearCampoRuta();

            Panel filaOrigen = CrearFilaCarpeta(
                "Base de datos:",
                txtCarpetaOrigen,
                "SELECCIONAR",
                (s, e) => SeleccionarOrigen());

            Panel filaDestino = CrearFilaCarpeta(
                "Carpeta de respaldos:",
                txtCarpetaRespaldos,
                "SELECCIONAR",
                (s, e) => SeleccionarDestino());

            distribucion.Controls.Add(titulo, 0, 0);
            distribucion.Controls.Add(filaOrigen, 0, 1);
            distribucion.Controls.Add(filaDestino, 0, 2);

            tarjeta.Controls.Add(distribucion);
            return tarjeta;
        }

        private TextBox CrearCampoRuta()
        {
            return new TextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = InventarioEstilo.Campo,
                ForeColor = InventarioEstilo.Texto,
                Font = new Font("Segoe UI", 9.5F),
                Margin = new Padding(3, 7, 12, 7)
            };
        }

        private Panel CrearFilaCarpeta(
            string texto,
            TextBox campo,
            string textoBoton,
            EventHandler accion)
        {
            Panel fila = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Margin = new Padding(0)
            };

            TableLayoutPanel rejilla = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1,
                BackColor = Color.White,
                Margin = new Padding(0)
            };

            rejilla.ColumnStyles.Add(
                new ColumnStyle(SizeType.Absolute, 155));
            rejilla.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100));
            rejilla.ColumnStyles.Add(
                new ColumnStyle(SizeType.Absolute, 165));

            Label etiqueta = new Label
            {
                Text = texto,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = InventarioEstilo.Texto,
                Font = new Font("Segoe UI", 9.5F),
                Margin = new Padding(3, 0, 5, 0)
            };

            BotonInventario boton = new BotonInventario(
                textoBoton, TipoBoton.Secundario)
            {
                Dock = DockStyle.Fill,
                Size = new Size(150, 40),
                Margin = new Padding(5, 4, 5, 4)
            };

            boton.Click += accion;

            rejilla.Controls.Add(etiqueta, 0, 0);
            rejilla.Controls.Add(campo, 1, 0);
            rejilla.Controls.Add(boton, 2, 0);

            fila.Controls.Add(rejilla);
            return fila;
        }

        private Control CrearBotonera()
        {
            Panel panel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = InventarioEstilo.Fondo,
                Margin = new Padding(0)
            };

            FlowLayoutPanel botones = new FlowLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                BackColor = InventarioEstilo.Fondo,
                Margin = new Padding(0),
                Padding = new Padding(5)
            };

            BotonInventario btnCrear = new BotonInventario(
                "CREAR RESPALDO", TipoBoton.Primario)
            {
                Size = new Size(175, 46),
                Margin = new Padding(8, 5, 8, 5)
            };
            btnCrear.Click += (s, e) => CrearRespaldo();

            BotonInventario btnListar = new BotonInventario(
                "ACTUALIZAR LISTA", TipoBoton.Secundario)
            {
                Size = new Size(175, 46),
                Margin = new Padding(8, 5, 8, 5)
            };
            btnListar.Click += (s, e) => ListarRespaldos();

            BotonInventario btnRestaurar = new BotonInventario(
                "RESTAURAR", TipoBoton.Peligro)
            {
                Size = new Size(155, 46),
                Margin = new Padding(8, 5, 8, 5)
            };
            btnRestaurar.Click += (s, e) => RestaurarRespaldo();

            botones.Controls.Add(btnCrear);
            botones.Controls.Add(btnListar);
            botones.Controls.Add(btnRestaurar);

            panel.Controls.Add(botones);

            void Centrar()
            {
                botones.Left = Math.Max(
                    0, (panel.ClientSize.Width - botones.Width) / 2);
                botones.Top = Math.Max(
                    0, (panel.ClientSize.Height - botones.Height) / 2);
            }

            panel.Resize += (s, e) => Centrar();
            botones.SizeChanged += (s, e) => Centrar();
            Centrar();

            return panel;
        }

        private Control CrearTarjetaTabla()
        {
            TarjetaInventario tarjeta = new TarjetaInventario
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20, 14, 20, 16),
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
                new RowStyle(SizeType.Absolute, 42));
            distribucion.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100));

            Label titulo = new Label
            {
                Text = "RESPALDOS DISPONIBLES",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = InventarioEstilo.VerdeOscuro,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(0)
            };

            dgvRespaldos = new DataGridView
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
                ColumnHeadersHeight = 44,
                Font = new Font("Segoe UI", 9.5F),
                Margin = new Padding(0, 5, 0, 0)
            };

            distribucion.Controls.Add(titulo, 0, 0);
            distribucion.Controls.Add(dgvRespaldos, 0, 1);

            tarjeta.Controls.Add(distribucion);
            return tarjeta;
        }

        private Control CrearEstado()
        {
            Panel panel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = InventarioEstilo.Fondo,
                Padding = new Padding(5),
                Margin = new Padding(0)
            };

            lblEstado = new Label
            {
                Text = "Estado: esperando operación.",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = InventarioEstilo.VerdeOscuro,
                Font = new Font("Segoe UI", 9.5F),
                Margin = new Padding(0)
            };

            panel.Controls.Add(lblEstado);
            return panel;
        }

        private void AplicarEstilos()
        {
            dgvRespaldos.EnableHeadersVisualStyles = false;
            dgvRespaldos.BackgroundColor = Color.White;
            dgvRespaldos.BorderStyle = BorderStyle.None;
            dgvRespaldos.GridColor = InventarioEstilo.Borde;
            dgvRespaldos.RowHeadersVisible = false;

            dgvRespaldos.ColumnHeadersHeight = 44;
            dgvRespaldos.ColumnHeadersHeightSizeMode =
                DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            dgvRespaldos.ColumnHeadersDefaultCellStyle =
                new DataGridViewCellStyle
                {
                    BackColor = InventarioEstilo.VerdeOscuro,
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                    SelectionBackColor = InventarioEstilo.VerdeOscuro,
                    SelectionForeColor = Color.White,
                    Padding = new Padding(8, 0, 0, 0)
                };

            dgvRespaldos.DefaultCellStyle =
                new DataGridViewCellStyle
                {
                    BackColor = Color.White,
                    ForeColor = InventarioEstilo.Texto,
                    Font = new Font("Segoe UI", 9.5F),
                    SelectionBackColor = Color.FromArgb(225, 240, 229),
                    SelectionForeColor = InventarioEstilo.Texto,
                    Padding = new Padding(8, 0, 0, 0)
                };

            dgvRespaldos.AlternatingRowsDefaultCellStyle =
                new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(247, 250, 248),
                    ForeColor = InventarioEstilo.Texto,
                    SelectionBackColor = Color.FromArgb(225, 240, 229),
                    SelectionForeColor = InventarioEstilo.Texto,
                    Font = new Font("Segoe UI", 9.5F)
                };

            dgvRespaldos.RowTemplate.Height = 38;
        }

        private void SeleccionarOrigen()
        {
            txtCarpetaOrigen.Text = "SIGEPOSTA_DB";
            lblEstado.Text = "Estado: base de datos SQL Server SIGEPOSTA_DB.";
        }
        private void SeleccionarDestino()
        {
            using FolderBrowserDialog dialogo = new FolderBrowserDialog
            {
                Description = "Seleccione la carpeta donde se guardarán los respaldos"
            };

            if (dialogo.ShowDialog() == DialogResult.OK)
            {
                txtCarpetaRespaldos.Text = dialogo.SelectedPath;
                ListarRespaldos();
            }
        }

        private void CrearRespaldo()
        {
            string destino = txtCarpetaRespaldos.Text;

            if (string.IsNullOrWhiteSpace(destino))
            {
                MessageBox.Show(
                    "Seleccione la carpeta donde se guardará el respaldo.",
                    "Validación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Cursor = Cursors.WaitCursor;
                lblEstado.Text = "Estado: creando respaldo de SIGEPOSTA_DB...";

                var resultado = servicio.CrearRespaldo(destino);

                lblEstado.Text = "Estado: " + resultado.Mensaje;

                if (resultado.Exitoso)
                {
                    ListarRespaldos();

                    MessageBox.Show(
                        resultado.Mensaje + "\n\n" + resultado.RutaArchivo,
                        "Respaldo completado",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(
                        resultado.Mensaje,
                        "Error al crear respaldo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }
        private void ListarRespaldos()
        {
            string carpeta = txtCarpetaRespaldos.Text;

            if (!Directory.Exists(carpeta))
            {
                dgvRespaldos.DataSource = null;
                lblEstado.Text = "Estado: seleccione una carpeta de respaldos.";
                return;
            }

            try
            {
                var archivos = servicio.ListarRespaldos(carpeta)
                    .Select(a => new
                    {
                        Nombre = a.NombreArchivo,
                        Ubicacion = a.RutaArchivo,
                        Fecha = a.FechaCreacion,
                        Tamano = a.TamanoMB
                    })
                    .ToList();

                dgvRespaldos.DataSource = archivos;

                if (dgvRespaldos.Columns["Ubicacion"] != null)
                    dgvRespaldos.Columns["Ubicacion"].Visible = false;

                lblEstado.Text = "Estado: " + archivos.Count +
                                 " respaldo(s) SQL Server encontrado(s).";
            }
            catch (Exception ex)
            {
                lblEstado.Text = "Estado: error al consultar respaldos.";

                MessageBox.Show(
                    ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        private void RestaurarRespaldo()
        {
            if (dgvRespaldos.CurrentRow == null)
            {
                MessageBox.Show(
                    "Seleccione un respaldo de la lista.",
                    "Restaurar respaldo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            string archivo = dgvRespaldos.CurrentRow
                .Cells["Ubicacion"].Value?.ToString() ?? "";

            if (!File.Exists(archivo))
            {
                MessageBox.Show(
                    "El archivo seleccionado no existe.",
                    "Restaurar respaldo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            DialogResult respuesta = MessageBox.Show(
                "ATENCIÓN: se reemplazará el contenido actual de SIGEPOSTA_DB " +
                "por el contenido de la copia seleccionada.\n\n" +
                "Esta operación puede provocar pérdida de datos recientes.\n\n" +
                "¿Desea continuar?",
                "Confirmar restauración de base de datos",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (respuesta != DialogResult.Yes)
                return;

            try
            {
                Cursor = Cursors.WaitCursor;
                lblEstado.Text = "Estado: restaurando SIGEPOSTA_DB...";

                var resultado = servicio.RestaurarRespaldo(archivo);

                lblEstado.Text = "Estado: " + resultado.Mensaje;

                MessageBox.Show(
                    resultado.Mensaje,
                    resultado.Exitoso ? "Restauración completada" : "Error de restauración",
                    MessageBoxButtons.OK,
                    resultado.Exitoso ? MessageBoxIcon.Information : MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }
    }
}


