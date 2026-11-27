using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using SIGEPOSTA.Presentacion.Estilos;
using SIGEPOSTA.Entidades.Clinica;
using SIGEPOSTA.Entidades.Personas;
using SIGEPOSTA.Datos.Repositorios;
using SIGEPOSTA.Negocio.Servicios;

namespace SIGEPOSTA.Presentacion.Formularios
{
    public class FormCitas : Form
    {
        private readonly BindingList<CitaVista> citas = new();

        private readonly CitaServicio servicio =
            new CitaServicio(new CitaRepositorio());

        private readonly PacienteServicio pacienteServicio =
            new PacienteServicio(new PacienteRepositorio());

        private readonly PersonalSaludServicio personalServicio =
            new PersonalSaludServicio(new PersonalSaludRepositorio());

        private List<Paciente> pacientes = new();
        private List<PersonalSalud> profesionales = new();

        private CampoInventario txtDniPaciente = new();
        private CampoInventario txtPaciente = new();
        private CampoInventario txtDniProfesional = new();
        private CampoInventario txtProfesional = new();
        private CampoInventario txtMotivo = new();

        private DateTimePicker dtpFecha = new();
        private DateTimePicker dtpHora = new();

        private ComboBox cmbEstado = new();
        private DataGridView dgvCitas = new();

        private int indiceSeleccionado = -1;
        private bool cargando = false;

        public FormCitas()
        {
            ConstruirFormulario();
            ConfigurarTabla();
            AplicarEstilos();

            txtDniPaciente.TextChanged +=
                (s, e) => BuscarPacientePorDni();

            txtDniProfesional.TextChanged +=
                (s, e) => BuscarProfesionalPorDni();

            CargarDatosIniciales();
        }

        // =========================================================
        // INTERFAZ
        // =========================================================

        private void ConstruirFormulario()
        {
            Text = "SIGEPOSTA - Gestión de citas médicas";
            Size = new Size(1250, 850);
            MinimumSize = new Size(1100, 750);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = InventarioEstilo.Fondo;
            Font = new Font("Segoe UI", 10F);
            DoubleBuffered = true;

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

            contenidoEncabezado.RowStyles.Add(
                new RowStyle(SizeType.Percent, 58));

            contenidoEncabezado.RowStyles.Add(
                new RowStyle(SizeType.Percent, 42));

            contenidoEncabezado.Controls.Add(new Label
            {
                Text = "GESTIÓN DE CITAS MÉDICAS",
                Dock = DockStyle.Fill,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 21F, FontStyle.Bold),
                TextAlign = ContentAlignment.BottomLeft,
                BackColor = Color.Transparent,
                Margin = new Padding(0)
            }, 0, 0);

            contenidoEncabezado.Controls.Add(new Label
            {
                Text = "Programación, organización y seguimiento de citas de los pacientes",
                Dock = DockStyle.Fill,
                ForeColor = Color.FromArgb(220, 235, 225),
                Font = new Font("Segoe UI", 10F),
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent,
                Margin = new Padding(2, 0, 0, 0)
            }, 0, 1);

            encabezado.Controls.Add(contenidoEncabezado);

            Panel contenido = new()
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(30, 22, 30, 25),
                BackColor = InventarioEstilo.Fondo
            };

            TableLayoutPanel columna = new()
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 1,
                RowCount = 4,
                BackColor = InventarioEstilo.Fondo,
                Padding = new Padding(0)
            };

            columna.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100));

            for (int i = 0; i < 4; i++)
                columna.RowStyles.Add(
                    new RowStyle(SizeType.AutoSize));

            // TARJETA DE PACIENTE Y PROFESIONAL

            TarjetaInventario tarjetaDatos = new()
            {
                Dock = DockStyle.Top,
                Height = 245,
                Margin = new Padding(0, 0, 0, 15)
            };

            columna.Controls.Add(tarjetaDatos, 0, 0);

            TableLayoutPanel distribucionDatos = new()
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(22, 14, 22, 17),
                BackColor = Color.White
            };

            distribucionDatos.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 48));

            distribucionDatos.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100));

            tarjetaDatos.Controls.Add(distribucionDatos);

            distribucionDatos.Controls.Add(new Label
            {
                Text = "Información del paciente y profesional",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = InventarioEstilo.VerdeOscuro,
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.White,
                Margin = new Padding(0)
            }, 0, 0);

            TableLayoutPanel rejilla = new()
            {
                Dock = DockStyle.Fill,
                ColumnCount = 6,
                RowCount = 2,
                BackColor = Color.White,
                Padding = new Padding(5)
            };

            rejilla.ColumnStyles.Add(
                new ColumnStyle(SizeType.Absolute, 118));

            rejilla.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 33.33F));

            rejilla.ColumnStyles.Add(
                new ColumnStyle(SizeType.Absolute, 118));

            rejilla.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 33.33F));

            rejilla.ColumnStyles.Add(
                new ColumnStyle(SizeType.Absolute, 118));

            rejilla.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 33.33F));

            for (int i = 0; i < 2; i++)
                rejilla.RowStyles.Add(
                    new RowStyle(SizeType.Absolute, 65));

            txtDniPaciente = new CampoInventario();
            txtPaciente = new CampoInventario();
            txtDniProfesional = new CampoInventario();
            txtProfesional = new CampoInventario();


            dtpFecha = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10F),
                Margin = new Padding(3, 8, 12, 5)
            };

            dtpHora = new DateTimePicker
            {
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "HH:mm",
                ShowUpDown = true,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10F),
                Margin = new Padding(3, 8, 12, 5)
            };

            void AgregarCampo(
                string etiqueta,
                Control campo,
                int fila,
                int columnaCampo)
            {
                rejilla.Controls.Add(new Label
                {
                    Text = etiqueta,
                    AutoSize = true,
                    Anchor = AnchorStyles.Left,
                    ForeColor = InventarioEstilo.Texto,
                    Font = new Font("Segoe UI", 9.5F),
                    BackColor = Color.White,
                    Margin = new Padding(3, 10, 3, 3)
                }, columnaCampo, fila);

                campo.Dock = DockStyle.Fill;
                campo.Margin = new Padding(3, 5, 12, 5);

                rejilla.Controls.Add(
                    campo, columnaCampo + 1, fila);
            }

            AgregarCampo(
                "DNI paciente:", txtDniPaciente, 0, 0);

            AgregarCampo(
                "Paciente:", txtPaciente, 0, 2);

            AgregarCampo(
                "DNI profesional:", txtDniProfesional, 0, 4);

            AgregarCampo(
                "Profesional:", txtProfesional, 1, 0);

            AgregarCampo(
                "Fecha:", dtpFecha, 1, 2);

            AgregarCampo(
                "Hora:", dtpHora, 1, 4);

            distribucionDatos.Controls.Add(rejilla, 0, 1);

            // TARJETA DE DETALLES

            TarjetaInventario tarjetaDetalles = new()
            {
                Dock = DockStyle.Top,
                Height = 225,
                Margin = new Padding(0, 0, 0, 15)
            };

            columna.Controls.Add(tarjetaDetalles, 0, 1);

            TableLayoutPanel distribucionDetalles = new()
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(22, 14, 22, 17),
                BackColor = Color.White
            };

            distribucionDetalles.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 45));

            distribucionDetalles.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100));

            tarjetaDetalles.Controls.Add(distribucionDetalles);

            distribucionDetalles.Controls.Add(new Label
            {
                Text = "Detalles de la cita",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = InventarioEstilo.VerdeOscuro,
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.White,
                Margin = new Padding(0)
            }, 0, 0);

            TableLayoutPanel rejillaDetalles = new()
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2,
                BackColor = Color.White,
                Padding = new Padding(8, 5, 8, 5),
                Margin = new Padding(0)
            };

            rejillaDetalles.ColumnStyles.Add(
                new ColumnStyle(SizeType.Absolute, 125));

            rejillaDetalles.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100));

            rejillaDetalles.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 48));

            rejillaDetalles.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 115));

            cmbEstado = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10F),
                BackColor = InventarioEstilo.Campo,
                ForeColor = InventarioEstilo.Texto,
                FlatStyle = FlatStyle.Flat
            };

            cmbEstado.Items.AddRange(new object[]
            {
                "Programada",
                "Confirmada",
                "Atendida",
                "Cancelada",
                "No asistió"
            });

            cmbEstado.SelectedIndex = 0;

            txtMotivo = new CampoInventario();
            txtMotivo.ConfigurarMultilinea(
                true, ScrollBars.Vertical);

            rejillaDetalles.Controls.Add(new Label
            {
                Text = "Estado:",
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                ForeColor = InventarioEstilo.Texto,
                Font = new Font("Segoe UI", 9.5F),
                BackColor = Color.White,
                Margin = new Padding(3, 10, 3, 3)
            }, 0, 0);

            cmbEstado.Dock = DockStyle.Fill;
            cmbEstado.Margin = new Padding(5, 5, 12, 5);

            rejillaDetalles.Controls.Add(cmbEstado, 1, 0);

            rejillaDetalles.Controls.Add(new Label
            {
                Text = "Motivo:",
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                ForeColor = InventarioEstilo.Texto,
                Font = new Font("Segoe UI", 9.5F),
                BackColor = Color.White,
                Margin = new Padding(3, 10, 3, 3)
            }, 0, 1);

            txtMotivo.Dock = DockStyle.Fill;
            txtMotivo.Margin = new Padding(5, 8, 12, 8);
            txtMotivo.MinimumSize = new Size(0, 100);

            rejillaDetalles.Controls.Add(txtMotivo, 1, 1);

            distribucionDetalles.Controls.Add(
                rejillaDetalles, 0, 1);

            // BOTONERA CENTRADA

            Panel panelBotones = new()
            {
                Dock = DockStyle.Fill,
                Height = 78,
                BackColor = InventarioEstilo.Fondo,
                Padding = new Padding(0, 8, 0, 8),
                Margin = new Padding(0, 0, 0, 12)
            };

            FlowLayoutPanel botones = new()
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Anchor = AnchorStyles.None,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Padding = new Padding(10),
                Margin = new Padding(0),
                BackColor = InventarioEstilo.Fondo
            };

            BotonInventario btnRegistrar =
                new("REGISTRAR", TipoBoton.Primario);

            btnRegistrar.Click +=
                (s, e) => RegistrarCita();

            BotonInventario btnActualizar =
                new("ACTUALIZAR", TipoBoton.Secundario);

            btnActualizar.Click +=
                (s, e) => ActualizarCita();

            BotonInventario btnEliminar =
                new("ELIMINAR", TipoBoton.Peligro);

            btnEliminar.Click +=
                (s, e) => EliminarCita();

            BotonInventario btnLimpiar =
                new("LIMPIAR", TipoBoton.Secundario);

            btnLimpiar.Click +=
                (s, e) => LimpiarFormulario();

            foreach (BotonInventario boton in new[]
            {
                btnRegistrar,
                btnActualizar,
                btnEliminar,
                btnLimpiar
            })
            {
                boton.Size = new Size(155, 46);
                boton.Margin = new Padding(8, 0, 8, 5);
            }

            botones.Controls.AddRange(new Control[]
            {
                btnRegistrar,
                btnActualizar,
                btnEliminar,
                btnLimpiar
            });

            panelBotones.Controls.Add(botones);

            void CentrarBotones()
            {
                botones.Location = new Point(
                    Math.Max(
                        0,
                        (panelBotones.ClientSize.Width -
                         botones.Width) / 2),

                    Math.Max(
                        0,
                        (panelBotones.ClientSize.Height -
                         botones.Height) / 2));
            }

            panelBotones.Resize +=
                (s, e) => CentrarBotones();

            botones.SizeChanged +=
                (s, e) => CentrarBotones();

            columna.Controls.Add(panelBotones, 0, 2);

            // TARJETA DE LISTADO

            TarjetaInventario tarjetaTabla = new()
            {
                Dock = DockStyle.Top,
                Height = 390,
                Margin = new Padding(0, 0, 0, 15)
            };

            columna.Controls.Add(tarjetaTabla, 0, 3);

            TableLayoutPanel distribucionTabla = new()
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(22, 14, 22, 17),
                BackColor = Color.White
            };

            distribucionTabla.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 48));

            distribucionTabla.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100));

            tarjetaTabla.Controls.Add(distribucionTabla);

            distribucionTabla.Controls.Add(new Label
            {
                Text = "Listado de citas médicas",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = InventarioEstilo.VerdeOscuro,
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.White,
                Margin = new Padding(0)
            }, 0, 0);

            dgvCitas = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoGenerateColumns = false,
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

            dgvCitas.SelectionChanged +=
                (s, e) => SeleccionarCita();

            distribucionTabla.Controls.Add(dgvCitas, 0, 1);

            contenido.Controls.Add(columna);

            Controls.Add(contenido);
            Controls.Add(encabezado);
        }

        // =========================================================
        // TABLA
        // =========================================================

        private void ConfigurarTabla()
        {
            dgvCitas.Columns.Clear();

            AgregarColumna(
                "IdCita", "IdCita", "ID", false);

            AgregarColumna(
                "DniPaciente", "DniPaciente", "DNI paciente");

            AgregarColumna(
                "Paciente", "Paciente", "Paciente");

            AgregarColumna(
                "DniProfesional", "DniProfesional", "DNI profesional");

            AgregarColumna(
                "Profesional", "Profesional", "Profesional");

            AgregarColumna(
                "Fecha", "Fecha", "Fecha");

            AgregarColumna(
                "Hora", "Hora", "Hora");

            AgregarColumna(
                "Motivo", "Motivo", "Motivo");

            AgregarColumna(
                "Estado", "Estado", "Estado");

            dgvCitas.Columns["Fecha"]!.DefaultCellStyle.Format =
                "dd/MM/yyyy";

            dgvCitas.DataSource = citas;
        }

        private void AgregarColumna(
            string nombre,
            string propiedad,
            string titulo,
            bool visible = true)
        {
            dgvCitas.Columns.Add(
                new DataGridViewTextBoxColumn
                {
                    Name = nombre,
                    DataPropertyName = propiedad,
                    HeaderText = titulo,
                    Visible = visible
                });
        }

        // =========================================================
        // ESTILOS
        // =========================================================

        private void AplicarEstilos()
        {
            BackColor = InventarioEstilo.Fondo;
            Font = new Font("Segoe UI", 10F);

            dtpFecha.Font = new Font("Segoe UI", 10F);
            dtpHora.Font = new Font("Segoe UI", 10F);

            cmbEstado.Font = new Font("Segoe UI", 10F);
            cmbEstado.BackColor = InventarioEstilo.Campo;
            cmbEstado.ForeColor = InventarioEstilo.Texto;
            cmbEstado.FlatStyle = FlatStyle.Flat;

            dgvCitas.BackgroundColor = Color.White;
            dgvCitas.BorderStyle = BorderStyle.None;
            dgvCitas.GridColor = InventarioEstilo.Borde;
            dgvCitas.EnableHeadersVisualStyles = false;

            dgvCitas.ColumnHeadersDefaultCellStyle.BackColor =
                InventarioEstilo.VerdeOscuro;

            dgvCitas.ColumnHeadersDefaultCellStyle.ForeColor =
                Color.White;

            dgvCitas.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 9.5F, FontStyle.Bold);

            dgvCitas.DefaultCellStyle.Font =
                new Font("Segoe UI", 9.5F);

            dgvCitas.DefaultCellStyle.ForeColor =
                InventarioEstilo.Texto;

            dgvCitas.DefaultCellStyle.BackColor =
                Color.White;

            dgvCitas.DefaultCellStyle.SelectionBackColor =
                Color.FromArgb(220, 238, 228);

            dgvCitas.DefaultCellStyle.SelectionForeColor =
                InventarioEstilo.Texto;

            dgvCitas.AlternatingRowsDefaultCellStyle.BackColor =
                InventarioEstilo.Campo;

            dgvCitas.RowTemplate.Height = 35;
            dgvCitas.ColumnHeadersHeight = 42;
        }

        // =========================================================
        // CARGA DE DATOS
        // =========================================================

        private void CargarDatosIniciales()
        {
            try
            {
                pacientes = pacienteServicio.ObtenerTodos();
                profesionales = personalServicio.ObtenerTodos();

                CargarCitas();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudieron cargar los datos de las citas.\n\n" +
                    ex.Message,
                    "Error de conexión",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void CargarCitas()
        {
            cargando = true;

            try
            {
                citas.Clear();

                var registros = servicio.ObtenerTodos();

                foreach (Cita registro in registros)
                {
                    Paciente? paciente = pacientes.FirstOrDefault(
                        p => p.IdPaciente == registro.IdPaciente);

                    PersonalSalud? profesional =
                        registro.IdPersonalSalud.HasValue
                            ? profesionales.FirstOrDefault(
                                p => p.IdPersonalSalud ==
                                     registro.IdPersonalSalud.Value)
                            : null;

                    citas.Add(new CitaVista
                    {
                        IdCita = registro.IdCita,
                        IdPaciente = registro.IdPaciente,
                        IdPersonalSalud =
                            registro.IdPersonalSalud,

                        DniPaciente =
                            paciente?.NumeroDocumento ?? "",

                        Paciente =
                            NombreCompleto(paciente),

                        DniProfesional =
                            profesional?.NumeroDocumento ?? "",

                        Profesional =
                            NombreCompleto(profesional),

                        Fecha = registro.FechaHora.Date,

                        Hora = registro.FechaHora.ToString("HH:mm"),

                        Motivo = registro.Motivo,

                        Estado = registro.Estado
                    });
                }

                dgvCitas.ClearSelection();
            }
            finally
            {
                cargando = false;
            }
        }

        private string NombreCompleto(Persona? persona)
        {
            if (persona == null)
                return "";

            return string.Join(
                " ",
                new[]
                {
                    persona.Nombres,
                    persona.ApellidoPaterno,
                    persona.ApellidoMaterno
                }.Where(x => !string.IsNullOrWhiteSpace(x)));
        }

        // =========================================================
        // BÚSQUEDA POR DNI
        // =========================================================

        private void BuscarPacientePorDni()
        {
            string dni = txtDniPaciente.Text.Trim();

            Paciente? paciente = pacientes.FirstOrDefault(
                p => p.NumeroDocumento.Trim() == dni);

            txtPaciente.Text = paciente == null
                ? ""
                : NombreCompleto(paciente);
        }

        private void BuscarProfesionalPorDni()
        {
            string dni = txtDniProfesional.Text.Trim();

            PersonalSalud? profesional =
                profesionales.FirstOrDefault(
                    p => p.NumeroDocumento.Trim() == dni);

            txtProfesional.Text = profesional == null
                ? ""
                : NombreCompleto(profesional);
        }

        private Paciente? ObtenerPacienteSeleccionado()
        {
            string dni = txtDniPaciente.Text.Trim();

            return pacientes.FirstOrDefault(
                p => p.NumeroDocumento.Trim() == dni &&
                     p.Estado);
        }

        private PersonalSalud? ObtenerProfesionalSeleccionado()
        {
            string dni = txtDniProfesional.Text.Trim();

            return profesionales.FirstOrDefault(
                p => p.NumeroDocumento.Trim() == dni &&
                     p.Estado &&
                     p.EsActivo);
        }

        // =========================================================
        // VALIDACIONES
        // =========================================================

        private bool ValidarDni(string dni)
        {
            dni = dni.Trim();

            return dni.Length == 8 &&
                   dni.All(char.IsDigit);
        }

        private bool ValidarDatos()
        {
            if (string.IsNullOrWhiteSpace(txtDniPaciente.Text) ||
                string.IsNullOrWhiteSpace(txtDniProfesional.Text) ||
                string.IsNullOrWhiteSpace(txtMotivo.Text))
            {
                MessageBox.Show(
                    "Complete todos los campos de la cita.");

                return false;
            }

            if (!ValidarDni(txtDniPaciente.Text) ||
                !ValidarDni(txtDniProfesional.Text))
            {
                MessageBox.Show(
                    "Los DNI deben contener exactamente 8 números.");

                return false;
            }

            if (ObtenerPacienteSeleccionado() == null)
            {
                MessageBox.Show(
                    "No se encontró un paciente activo con ese DNI.");

                txtDniPaciente.Focus();
                return false;
            }

            if (ObtenerProfesionalSeleccionado() == null)
            {
                MessageBox.Show(
                    "No se encontró un profesional de salud activo con ese DNI.");

                txtDniProfesional.Focus();
                return false;
            }

            if (dtpFecha.Value.Date < DateTime.Today)
            {
                MessageBox.Show(
                    "La fecha de la cita no puede ser anterior a hoy.");

                return false;
            }

            if (cmbEstado.SelectedItem == null)
            {
                MessageBox.Show(
                    "Seleccione el estado de la cita.");

                return false;
            }

            return true;
        }

        // =========================================================
        // HORARIOS
        // =========================================================

        private bool HorarioOcupado(
            int idProfesional,
            DateTime fechaHora,
            int idCitaExcluir = 0)
        {
            return citas.Any(c =>
                c.IdPersonalSalud == idProfesional &&
                c.Fecha.Date == fechaHora.Date &&
                c.Hora == fechaHora.ToString("HH:mm") &&
                c.Estado != "Cancelada" &&
                c.IdCita != idCitaExcluir);
        }

        // =========================================================
        // REGISTRAR
        // =========================================================

        private void RegistrarCita()
        {
            if (!ValidarDatos())
                return;

            try
            {
                Paciente paciente =
                    ObtenerPacienteSeleccionado()!;

                PersonalSalud profesional =
                    ObtenerProfesionalSeleccionado()!;

                DateTime fechaHora = dtpFecha.Value.Date.Add(
                    dtpHora.Value.TimeOfDay);

                if (HorarioOcupado(
                    profesional.IdPersonalSalud,
                    fechaHora))
                {
                    MessageBox.Show(
                        "El profesional ya tiene una cita registrada en ese horario.");

                    return;
                }

                Cita nueva = new()
                {
                    IdPaciente = paciente.IdPaciente,
                    IdPersonalSalud =
                        profesional.IdPersonalSalud,

                    FechaHora = fechaHora,

                    Motivo = txtMotivo.Text.Trim(),

                    Estado = cmbEstado.Text
                };

                servicio.Registrar(nueva);

                CargarCitas();

                MessageBox.Show(
                    "Cita médica registrada correctamente.",
                    "SIGEPOSTA",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo registrar la cita.\n\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =========================================================
        // ACTUALIZAR
        // =========================================================

        private void ActualizarCita()
        {
            if (indiceSeleccionado < 0 ||
                indiceSeleccionado >= citas.Count)
            {
                MessageBox.Show(
                    "Seleccione una cita de la tabla.");

                return;
            }

            if (!ValidarDatos())
                return;

            try
            {
                CitaVista seleccionada =
                    citas[indiceSeleccionado];

                Paciente paciente =
                    ObtenerPacienteSeleccionado()!;

                PersonalSalud profesional =
                    ObtenerProfesionalSeleccionado()!;

                DateTime fechaHora = dtpFecha.Value.Date.Add(
                    dtpHora.Value.TimeOfDay);

                if (HorarioOcupado(
                    profesional.IdPersonalSalud,
                    fechaHora,
                    seleccionada.IdCita))
                {
                    MessageBox.Show(
                        "El profesional ya tiene una cita en ese horario.");

                    return;
                }

                Cita actualizada = new()
                {
                    IdCita = seleccionada.IdCita,

                    IdPaciente = paciente.IdPaciente,

                    IdPersonalSalud =
                        profesional.IdPersonalSalud,

                    FechaHora = fechaHora,

                    Motivo = txtMotivo.Text.Trim(),

                    Estado = cmbEstado.Text
                };

                servicio.Actualizar(actualizada);

                CargarCitas();

                MessageBox.Show(
                    "Cita actualizada correctamente.",
                    "SIGEPOSTA",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo actualizar la cita.\n\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =========================================================
        // CANCELAR
        // =========================================================

        private void EliminarCita()
        {
            if (indiceSeleccionado < 0 ||
                indiceSeleccionado >= citas.Count)
            {
                MessageBox.Show(
                    "Seleccione una cita de la tabla.");

                return;
            }

            CitaVista seleccionada =
                citas[indiceSeleccionado];

            if (seleccionada.Estado == "Cancelada")
            {
                MessageBox.Show(
                    "La cita seleccionada ya se encuentra cancelada.");

                return;
            }

            DialogResult respuesta = MessageBox.Show(
                "¿Está seguro de cancelar esta cita?",
                "Confirmar cancelación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (respuesta != DialogResult.Yes)
                return;

            try
            {
                servicio.Eliminar(seleccionada.IdCita);

                CargarCitas();

                MessageBox.Show(
                    "Cita cancelada correctamente.",
                    "SIGEPOSTA",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo cancelar la cita.\n\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =========================================================
        // SELECCIONAR CITA
        // =========================================================

        private void SeleccionarCita()
        {
            if (cargando)
                return;

            if (dgvCitas.CurrentRow?.DataBoundItem
                is not CitaVista cita)
                return;

            indiceSeleccionado = citas.IndexOf(cita);

            cargando = true;

            try
            {
                txtDniPaciente.Text = cita.DniPaciente;
                txtPaciente.Text = cita.Paciente;

                txtDniProfesional.Text =
                    cita.DniProfesional;

                txtProfesional.Text = cita.Profesional;

                DateTime fecha = cita.Fecha;

                if (fecha < dtpFecha.MinDate)
                    fecha = dtpFecha.MinDate;

                if (fecha > dtpFecha.MaxDate)
                    fecha = dtpFecha.MaxDate;

                dtpFecha.Value = fecha;

                if (TimeSpan.TryParse(
                    cita.Hora,
                    out TimeSpan hora))
                {
                    dtpHora.Value =
                        DateTime.Today.Add(hora);
                }

                txtMotivo.Text = cita.Motivo;

                cmbEstado.SelectedItem =
                    cmbEstado.Items.Contains(cita.Estado)
                        ? cita.Estado
                        : "Programada";
            }
            finally
            {
                cargando = false;
            }
        }

        // =========================================================
        // LIMPIAR
        // =========================================================

        private void LimpiarFormulario()
        {
            cargando = true;

            try
            {
                indiceSeleccionado = -1;

                txtDniPaciente.Clear();
                txtPaciente.Clear();

                txtDniProfesional.Clear();
                txtProfesional.Clear();

                txtMotivo.Clear();

                dtpFecha.Value = DateTime.Today;
                dtpHora.Value = DateTime.Today;

                cmbEstado.SelectedIndex = 0;

                dgvCitas.ClearSelection();
                dgvCitas.CurrentCell = null;
            }
            finally
            {
                cargando = false;
            }
        }

        // =========================================================
        // MODELO DE VISTA
        // =========================================================

        private class CitaVista
        {
            public int IdCita { get; set; }

            public int IdPaciente { get; set; }

            public int? IdPersonalSalud { get; set; }

            public string DniPaciente { get; set; } = "";

            public string Paciente { get; set; } = "";

            public string DniProfesional { get; set; } = "";

            public string Profesional { get; set; } = "";

            public DateTime Fecha { get; set; }

            public string Hora { get; set; } = "";

            public string Motivo { get; set; } = "";

            public string Estado { get; set; } = "";
        }
    }
}

