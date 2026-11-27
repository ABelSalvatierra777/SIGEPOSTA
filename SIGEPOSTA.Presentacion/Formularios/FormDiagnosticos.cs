using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SIGEPOSTA.Presentacion.Estilos;
using SIGEPOSTA.Datos.Repositorios;
using SIGEPOSTA.Entidades.Clinica;

namespace SIGEPOSTA.Presentacion.Formularios
{
    public class FormDiagnosticos : Form
    {
        private readonly DiagnosticoRepositorio repositorio = new();
        private readonly BindingList<DiagnosticoVista> diagnosticos = new();

        private CampoInventario txtDniPaciente = new();
        private CampoInventario txtPaciente = new();
        private CampoInventario txtDniProfesional = new();
        private CampoInventario txtProfesional = new();
        private CampoInventario txtCodigo = new();
        private CampoInventario txtDescripcion = new();
        private CampoInventario txtObservaciones = new();

        private DateTimePicker dtpFecha = new();
        private ComboBox cmbTipo = new();
        private ComboBox cmbEstado = new();

        private DataGridView dgvDiagnosticos = new();
        private int indiceSeleccionado = -1;

        public FormDiagnosticos()
        {
            ConstruirFormulario();
            ConfigurarTabla();
            AplicarEstilos();
            CargarDiagnosticos();
        }

        private void ConstruirFormulario()
        {
            Text = "SIGEPOSTA - Gestión de diagnósticos";
            Size = new Size(1300, 900);
            MinimumSize = new Size(1100, 760);
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
                Margin = new Padding(0)
            };

            contenidoEncabezado.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100));
            contenidoEncabezado.RowStyles.Add(
                new RowStyle(SizeType.Percent, 58));
            contenidoEncabezado.RowStyles.Add(
                new RowStyle(SizeType.Percent, 42));

            contenidoEncabezado.Controls.Add(new Label
            {
                Text = "GESTIÓN DE DIAGNÓSTICOS",
                Dock = DockStyle.Fill,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 21F, FontStyle.Bold),
                TextAlign = ContentAlignment.BottomLeft,
                BackColor = Color.Transparent,
                Margin = new Padding(0)
            }, 0, 0);

            contenidoEncabezado.Controls.Add(new Label
            {
                Text = "Registro, evaluación y seguimiento de diagnósticos clínicos",
                Dock = DockStyle.Fill,
                ForeColor = Color.FromArgb(220, 235, 225),
                Font = new Font("Segoe UI", 10F),
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent,
                Margin = new Padding(2, 0, 0, 0)
            }, 0, 1);

            encabezado.Controls.Add(contenidoEncabezado);

            Panel cuerpo = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(28, 25, 28, 20),
                BackColor = InventarioEstilo.Fondo
            };

            TableLayoutPanel columna = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 1,
                RowCount = 4,
                BackColor = InventarioEstilo.Fondo,
                Margin = new Padding(0)
            };

            columna.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100));

            columna.RowStyles.Add(new RowStyle(SizeType.Absolute, 165));
            columna.RowStyles.Add(new RowStyle(SizeType.Absolute, 320));
            columna.RowStyles.Add(new RowStyle(SizeType.Absolute, 65));
            columna.RowStyles.Add(new RowStyle(SizeType.Absolute, 320));

            columna.Controls.Add(CrearTarjetaPaciente(), 0, 0);
            columna.Controls.Add(CrearTarjetaDiagnostico(), 0, 1);
            columna.Controls.Add(CrearBotonera(), 0, 2);
            columna.Controls.Add(CrearTarjetaTabla(), 0, 3);

            cuerpo.Controls.Add(columna);

            Controls.Add(cuerpo);
            Controls.Add(encabezado);
        }

        private Control CrearTarjetaPaciente()
        {
            TarjetaInventario tarjeta = new TarjetaInventario
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20, 12, 20, 12),
                BackColor = Color.White,
                Margin = new Padding(0, 0, 0, 12)
            };

            TableLayoutPanel contenido = CrearContenedorTarjeta(
                "INFORMACIÓN DEL PACIENTE Y PROFESIONAL", 2, 42);

            TableLayoutPanel rejilla = CrearRejilla(4, 2);

            txtDniPaciente = NuevoCampo();
            txtPaciente = NuevoCampo();
            txtDniProfesional = NuevoCampo();
            txtProfesional = NuevoCampo();

            AgregarCampo(rejilla, "DNI paciente:", txtDniPaciente, 0, 0);
            AgregarCampo(rejilla, "Paciente:", txtPaciente, 2, 0);
            AgregarCampo(rejilla, "DNI profesional:", txtDniProfesional, 0, 1);
            AgregarCampo(rejilla, "Profesional:", txtProfesional, 2, 1);

            contenido.Controls.Add(rejilla, 0, 1);
            tarjeta.Controls.Add(contenido);

            return tarjeta;
        }

        private Control CrearTarjetaDiagnostico()
        {
            TarjetaInventario tarjeta = new TarjetaInventario
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20, 12, 20, 12),
                BackColor = Color.White,
                Margin = new Padding(0, 0, 0, 12)
            };

            TableLayoutPanel contenido = CrearContenedorTarjeta(
                "INFORMACIÓN DEL DIAGNÓSTICO CLÍNICO", 2, 42);

            TableLayoutPanel rejilla = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 4,
                BackColor = Color.White,
                Margin = new Padding(0),
                Padding = new Padding(0, 5, 0, 0)
            };

            rejilla.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
            rejilla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            rejilla.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130));
            rejilla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

            rejilla.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
            rejilla.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
            rejilla.RowStyles.Add(new RowStyle(SizeType.Absolute, 65));
            rejilla.RowStyles.Add(new RowStyle(SizeType.Absolute, 65));

            txtCodigo = NuevoCampo();

            dtpFecha = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                MaxDate = DateTime.Today,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10F),
                Margin = new Padding(3, 7, 12, 7)
            };

            cmbTipo = NuevoCombo(
                "Presuntivo",
                "Definitivo",
                "Control");

            cmbEstado = NuevoCombo(
                "Activo",
                "Resuelto",
                "En seguimiento");

            txtDescripcion = NuevaArea();
            txtObservaciones = NuevaArea();

            AgregarCampo(rejilla, "Código CIE-10:", txtCodigo, 0, 0);
            AgregarCampo(rejilla, "Fecha:", dtpFecha, 2, 0);

            AgregarCampo(rejilla, "Tipo:", cmbTipo, 0, 1);
            AgregarCampo(rejilla, "Estado:", cmbEstado, 2, 1);

            AgregarArea(rejilla, "Descripción:", txtDescripcion, 2);
            AgregarArea(rejilla, "Observaciones:", txtObservaciones, 3);

            contenido.Controls.Add(rejilla, 0, 1);
            tarjeta.Controls.Add(contenido);

            return tarjeta;
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
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                BackColor = InventarioEstilo.Fondo,
                Margin = new Padding(0)
            };

            BotonInventario btnRegistrar = CrearBoton(
                "REGISTRAR", TipoBoton.Primario);
            btnRegistrar.Click += (s, e) => Registrar();

            BotonInventario btnActualizar = CrearBoton(
                "ACTUALIZAR", TipoBoton.Secundario);
            btnActualizar.Click += (s, e) => Actualizar();

            BotonInventario btnEliminar = CrearBoton(
                "ELIMINAR", TipoBoton.Peligro);
            btnEliminar.Click += (s, e) => Eliminar();

            BotonInventario btnLimpiar = CrearBoton(
                "LIMPIAR", TipoBoton.Secundario);
            btnLimpiar.Click += (s, e) => Limpiar();

            botones.Controls.Add(btnRegistrar);
            botones.Controls.Add(btnActualizar);
            botones.Controls.Add(btnEliminar);
            botones.Controls.Add(btnLimpiar);

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

            return panel;
        }

        private Control CrearTarjetaTabla()
        {
            TarjetaInventario tarjeta = new TarjetaInventario
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20, 12, 20, 12),
                BackColor = Color.White,
                Margin = new Padding(0, 0, 0, 12)
            };

            TableLayoutPanel contenido = CrearContenedorTarjeta(
                "DIAGNÓSTICOS REGISTRADOS", 2, 42);

            dgvDiagnosticos = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                AutoGenerateColumns = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                RowHeadersVisible = false,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                GridColor = InventarioEstilo.Borde,
                BackgroundColor = Color.White,
                EnableHeadersVisualStyles = false,
                ColumnHeadersHeight = 42,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Regular),
                Margin = new Padding(0, 5, 0, 0)
            };

            dgvDiagnosticos.SelectionChanged +=
                (s, e) => Seleccionar();

            contenido.Controls.Add(dgvDiagnosticos, 0, 1);
            tarjeta.Controls.Add(contenido);

            return tarjeta;
        }

        private TableLayoutPanel CrearContenedorTarjeta(
            string titulo,
            int filas,
            int altoTitulo)
        {
            TableLayoutPanel contenido = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = filas,
                BackColor = Color.White,
                Margin = new Padding(0)
            };

            contenido.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100));
            contenido.RowStyles.Add(
                new RowStyle(SizeType.Absolute, altoTitulo));
            contenido.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100));

            contenido.Controls.Add(new Label
            {
                Text = titulo,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = InventarioEstilo.VerdeOscuro,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(0)
            }, 0, 0);

            return contenido;
        }

        private TableLayoutPanel CrearRejilla(int columnas, int filas)
        {
            TableLayoutPanel rejilla = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = columnas,
                RowCount = filas,
                BackColor = Color.White,
                Margin = new Padding(0)
            };

            rejilla.ColumnStyles.Add(
                new ColumnStyle(SizeType.Absolute, 150));
            rejilla.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 50));
            rejilla.ColumnStyles.Add(
                new ColumnStyle(SizeType.Absolute, 150));
            rejilla.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 50));

            for (int i = 0; i < filas; i++)
                rejilla.RowStyles.Add(
                    new RowStyle(SizeType.Absolute, 42));

            return rejilla;
        }

        private void AgregarCampo(
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
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = InventarioEstilo.Texto,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Regular),
                Margin = new Padding(3, 0, 5, 0)
            };

            campo.Dock = DockStyle.Fill;
            campo.Margin = new Padding(3, 7, 12, 7);

            rejilla.Controls.Add(etiqueta, columna, fila);
            rejilla.Controls.Add(campo, columna + 1, fila);
        }

        private void AgregarArea(
            TableLayoutPanel rejilla,
            string texto,
            CampoInventario campo,
            int fila)
        {
            Label etiqueta = new Label
            {
                Text = texto,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = InventarioEstilo.Texto,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Regular),
                Margin = new Padding(3, 0, 5, 0)
            };

            campo.Dock = DockStyle.Fill;
            campo.Margin = new Padding(3, 5, 12, 7);

            rejilla.Controls.Add(etiqueta, 0, fila);
            rejilla.Controls.Add(campo, 1, fila);
            rejilla.SetColumnSpan(campo, 3);
        }

        private CampoInventario NuevoCampo()
{
    return new CampoInventario
    {
        Dock = DockStyle.Fill,
        Margin = new Padding(3, 5, 12, 5)
    };
}

        private CampoInventario NuevaArea()
{
    CampoInventario campo = new CampoInventario
    {
        Dock = DockStyle.Fill,
        Margin = new Padding(3, 5, 12, 5)
    };

    campo.ConfigurarMultilinea(true, ScrollBars.Vertical);

    return campo;
}

        private ComboBox NuevoCombo(params string[] opciones)
        {
            ComboBox combo = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10F),
                BackColor = InventarioEstilo.Campo,
                ForeColor = InventarioEstilo.Texto,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(3, 7, 12, 7)
            };

            combo.Items.AddRange(opciones);
            combo.SelectedIndex = 0;

            return combo;
        }

        private BotonInventario CrearBoton(
            string texto,
            TipoBoton tipo)
        {
            return new BotonInventario(texto, tipo)
            {
                Size = new Size(155, 46),
                Margin = new Padding(7, 5, 7, 5)
            };
        }

        private void ConfigurarTabla()
        {
            dgvDiagnosticos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DniPaciente",
                DataPropertyName = "DniPaciente",
                HeaderText = "DNI paciente"
            });

            dgvDiagnosticos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Paciente",
                DataPropertyName = "Paciente",
                HeaderText = "Paciente"
            });

            dgvDiagnosticos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Codigo",
                DataPropertyName = "Codigo",
                HeaderText = "Código CIE-10"
            });

            dgvDiagnosticos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Fecha",
                DataPropertyName = "Fecha",
                HeaderText = "Fecha",
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "dd/MM/yyyy"
                }
            });

            dgvDiagnosticos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Tipo",
                DataPropertyName = "Tipo",
                HeaderText = "Tipo"
            });

            dgvDiagnosticos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Estado",
                DataPropertyName = "Estado",
                HeaderText = "Estado"
            });

            dgvDiagnosticos.DataSource = diagnosticos;
        }

        private void AplicarEstilos()
        {
            dgvDiagnosticos.EnableHeadersVisualStyles = false;

            dgvDiagnosticos.ColumnHeadersDefaultCellStyle =
                new DataGridViewCellStyle
                {
                    BackColor = InventarioEstilo.VerdeOscuro,
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                    SelectionBackColor = InventarioEstilo.VerdeOscuro,
                    SelectionForeColor = Color.White,
                    Padding = new Padding(8, 0, 0, 0)
                };

            dgvDiagnosticos.DefaultCellStyle =
                new DataGridViewCellStyle
                {
                    BackColor = Color.White,
                    ForeColor = InventarioEstilo.Texto,
                    Font = new Font("Segoe UI", 9.5F, FontStyle.Regular),
                    SelectionBackColor = Color.FromArgb(225, 240, 229),
                    SelectionForeColor = InventarioEstilo.Texto,
                    Padding = new Padding(8, 0, 0, 0)
                };

            dgvDiagnosticos.AlternatingRowsDefaultCellStyle =
                new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(247, 250, 248),
                    ForeColor = InventarioEstilo.Texto,
                    SelectionBackColor = Color.FromArgb(225, 240, 229),
                    SelectionForeColor = InventarioEstilo.Texto
                };

            dgvDiagnosticos.RowTemplate.Height = 38;
        }

        private void CargarDiagnosticos()
        {
            try
            {
                diagnosticos.Clear();

                foreach (DiagnosticoRegistro registro in repositorio.ObtenerTodosConDatos())
                {
                    diagnosticos.Add(new DiagnosticoVista
                    {
                        IdDiagnostico = registro.IdDiagnostico,
                        IdAtencion = registro.IdAtencion,
                        DniPaciente = registro.DniPaciente,
                        Paciente = registro.Paciente,
                        DniProfesional = registro.DniProfesional,
                        Profesional = registro.Profesional,
                        Codigo = registro.CodigoCIE10,
                        Fecha = registro.FechaRegistro,
                        Tipo = registro.TipoDiagnostico,
                        Estado = registro.Estado,
                        Descripcion = registro.Descripcion,
                        Observaciones = registro.Observaciones
                    });
                }

                dgvDiagnosticos.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudieron cargar los diagnósticos:\n" + ex.Message,
                    "Error de conexión",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        private void Registrar()
        {
            if (!Validar())
                return;

            try
            {
                int idAtencion = repositorio.ObtenerAtencionPorDni(
                    txtDniPaciente.Text.Trim(),
                    txtDniProfesional.Text.Trim());

                if (idAtencion == 0)
                {
                    MessageBox.Show(
                        "No existe una atención médica registrada para este paciente y profesional.",
                        "Atención no encontrada",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                Diagnostico diagnostico = new()
                {
                    IdAtencion = idAtencion,
                    CodigoCIE10 = txtCodigo.Text.Trim(),
                    Descripcion = txtDescripcion.Text.Trim(),
                    TipoDiagnostico = cmbTipo.Text,
                    FechaRegistro = dtpFecha.Value.Date,
                    Observaciones = txtObservaciones.Text.Trim(),
                    Estado = cmbEstado.Text
                };

                repositorio.Agregar(diagnostico);
                CargarDiagnosticos();

                MessageBox.Show("Diagnóstico registrado correctamente.");
                Limpiar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo registrar el diagnóstico:\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void Actualizar()
        {
            if (indiceSeleccionado < 0 ||
                indiceSeleccionado >= diagnosticos.Count)
            {
                MessageBox.Show("Seleccione un diagnóstico.");
                return;
            }

            if (!Validar())
                return;

            try
            {
                DiagnosticoVista anterior = diagnosticos[indiceSeleccionado];

                int idAtencion = repositorio.ObtenerAtencionPorDni(
                    txtDniPaciente.Text.Trim(),
                    txtDniProfesional.Text.Trim());

                if (idAtencion == 0)
                {
                    MessageBox.Show("No se encontró una atención médica para esos datos.");
                    return;
                }

                Diagnostico diagnostico = new()
                {
                    IdDiagnostico = anterior.IdDiagnostico,
                    IdAtencion = idAtencion,
                    CodigoCIE10 = txtCodigo.Text.Trim(),
                    Descripcion = txtDescripcion.Text.Trim(),
                    TipoDiagnostico = cmbTipo.Text,
                    FechaRegistro = dtpFecha.Value.Date,
                    Observaciones = txtObservaciones.Text.Trim(),
                    Estado = cmbEstado.Text
                };

                repositorio.Actualizar(diagnostico);
                CargarDiagnosticos();

                MessageBox.Show("Diagnóstico actualizado correctamente.");
                Limpiar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo actualizar el diagnóstico:\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void Eliminar()
        {
            if (indiceSeleccionado < 0 ||
                indiceSeleccionado >= diagnosticos.Count)
            {
                MessageBox.Show("Seleccione un diagnóstico.");
                return;
            }

            DialogResult respuesta = MessageBox.Show(
                "¿Desea eliminar este diagnóstico?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (respuesta != DialogResult.Yes)
                return;

            try
            {
                int id = diagnosticos[indiceSeleccionado].IdDiagnostico;

                repositorio.Eliminar(id);
                CargarDiagnosticos();

                MessageBox.Show("Diagnóstico eliminado correctamente.");
                Limpiar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo eliminar el diagnóstico:\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void Seleccionar()
        {
            if (dgvDiagnosticos.CurrentRow?.DataBoundItem
                is not DiagnosticoVista d)
                return;

            indiceSeleccionado = diagnosticos.IndexOf(d);

            txtDniPaciente.Text = d.DniPaciente;
            txtPaciente.Text = d.Paciente;
            txtDniProfesional.Text = d.DniProfesional;
            txtProfesional.Text = d.Profesional;
            txtCodigo.Text = d.Codigo;
            dtpFecha.Value = d.Fecha;
            cmbTipo.SelectedItem = d.Tipo;
            cmbEstado.SelectedItem = d.Estado;
            txtDescripcion.Text = d.Descripcion;
            txtObservaciones.Text = d.Observaciones;
        }

        private bool Validar()
        {
            if (string.IsNullOrWhiteSpace(txtDniPaciente.Text) ||
                string.IsNullOrWhiteSpace(txtPaciente.Text) ||
                string.IsNullOrWhiteSpace(txtDniProfesional.Text) ||
                string.IsNullOrWhiteSpace(txtProfesional.Text) ||
                string.IsNullOrWhiteSpace(txtCodigo.Text) ||
                string.IsNullOrWhiteSpace(txtDescripcion.Text))
            {
                MessageBox.Show("Complete los campos obligatorios.");
                return false;
            }

            if (!DniValido(txtDniPaciente.Text) ||
                !DniValido(txtDniProfesional.Text))
            {
                MessageBox.Show(
                    "Los DNI deben contener exactamente 8 números.");
                return false;
            }

            if (cmbTipo.SelectedItem == null ||
                cmbEstado.SelectedItem == null)
            {
                MessageBox.Show(
                    "Seleccione el tipo y estado del diagnóstico.");
                return false;
            }

            return true;
        }

        private bool DniValido(string dni)
        {
            return dni.Trim().Length == 8 &&
                   dni.Trim().All(char.IsDigit);
        }

        private DiagnosticoVista ObtenerDatos()
        {
            return new DiagnosticoVista
            {
                DniPaciente = txtDniPaciente.Text.Trim(),
                Paciente = txtPaciente.Text.Trim(),
                DniProfesional = txtDniProfesional.Text.Trim(),
                Profesional = txtProfesional.Text.Trim(),
                Codigo = txtCodigo.Text.Trim(),
                Fecha = dtpFecha.Value.Date,
                Tipo = cmbTipo.Text,
                Estado = cmbEstado.Text,
                Descripcion = txtDescripcion.Text.Trim(),
                Observaciones = txtObservaciones.Text.Trim()
            };
        }

        private void Limpiar()
        {
            indiceSeleccionado = -1;

            txtDniPaciente.Clear();
            txtPaciente.Clear();
            txtDniProfesional.Clear();
            txtProfesional.Clear();
            txtCodigo.Clear();
            txtDescripcion.Clear();
            txtObservaciones.Clear();

            dtpFecha.Value = DateTime.Today;
            cmbTipo.SelectedIndex = 0;
            cmbEstado.SelectedIndex = 0;

            dgvDiagnosticos.ClearSelection();
        }

        private class DiagnosticoVista
        {
            public int IdDiagnostico { get; set; }
            public int IdAtencion { get; set; }
            public string DniPaciente { get; set; } = "";
            public string Paciente { get; set; } = "";
            public string DniProfesional { get; set; } = "";
            public string Profesional { get; set; } = "";
            public string Codigo { get; set; } = "";
            public DateTime Fecha { get; set; }
            public string Tipo { get; set; } = "";
            public string Estado { get; set; } = "";
            public string Descripcion { get; set; } = "";
            public string Observaciones { get; set; } = "";
        }
    }
}





