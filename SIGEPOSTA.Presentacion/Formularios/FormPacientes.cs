using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SIGEPOSTA.Presentacion.Estilos;
using SIGEPOSTA.Entidades.Personas;
using SIGEPOSTA.Datos.Repositorios;
using SIGEPOSTA.Negocio.Servicios;

namespace SIGEPOSTA.Presentacion.Formularios
{
    public class FormPacientes : Form
    {
        private readonly PacienteServicio servicio =
            new PacienteServicio(new PacienteRepositorio());

        private readonly BindingList<PacienteVista> pacientes = new();

        private CampoInventario txtDni = new();
        private CampoInventario txtNombres = new();
        private CampoInventario txtApellidos = new();
        private CampoInventario txtTelefono = new();
        private CampoInventario txtDireccion = new();

        private DateTimePicker dtpNacimiento = new();
        private DataGridView dgvPacientes = new();

        private int indiceSeleccionado = -1;
        private bool cargando = false;

        public FormPacientes()
        {
            ConstruirFormulario();
            ConfigurarTabla();
            AplicarEstilos();
            CargarPacientes();
        }

        private void ConstruirFormulario()
        {
            Text = "SIGEPOSTA - Gestión de pacientes";
            Size = new Size(1200, 850);
            MinimumSize = new Size(1000, 700);
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

            contenidoEncabezado.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100));
            contenidoEncabezado.RowStyles.Add(
                new RowStyle(SizeType.Percent, 58));
            contenidoEncabezado.RowStyles.Add(
                new RowStyle(SizeType.Percent, 42));

            Label titulo = new()
            {
                Text = "GESTIÓN DE PACIENTES",
                Dock = DockStyle.Fill,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 21F, FontStyle.Bold),
                TextAlign = ContentAlignment.BottomLeft,
                BackColor = Color.Transparent,
                Margin = new Padding(0)
            };

            Label subtitulo = new()
            {
                Text = "Registro, consulta y administración de pacientes",
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

            Panel contenido = new()
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
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

            TarjetaInventario tarjetaFormulario = new()
            {
                Dock = DockStyle.Top,
                Height = 255,
                Margin = new Padding(0, 0, 0, 15)
            };

            columna.Controls.Add(tarjetaFormulario, 0, 0);

            TableLayoutPanel distribucion = new()
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(20, 12, 20, 15),
                BackColor = Color.White
            };

            distribucion.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100));
            distribucion.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 48));
            distribucion.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100));

            tarjetaFormulario.Controls.Add(distribucion);

            Label tituloFormulario = new()
            {
                Text = "Información del paciente",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = InventarioEstilo.VerdeOscuro,
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.White,
                Margin = new Padding(0)
            };

            distribucion.Controls.Add(tituloFormulario, 0, 0);

            TableLayoutPanel rejilla = CrearRejilla();
            distribucion.Controls.Add(rejilla, 0, 1);

            txtDni = NuevoCampo();
            txtNombres = NuevoCampo();
            txtApellidos = NuevoCampo();
            txtTelefono = NuevoCampo();
            txtDireccion = NuevoCampo();
            dtpNacimiento = NuevoFecha();

            AgregarPar(rejilla, 0, 0, "DNI:", txtDni);
            AgregarPar(rejilla, 0, 2, "Nombres:", txtNombres);
            AgregarPar(rejilla, 0, 4, "Apellidos:", txtApellidos);

            AgregarPar(rejilla, 1, 0, "Nacimiento:", dtpNacimiento);
            AgregarPar(rejilla, 1, 2, "Teléfono:", txtTelefono);
            AgregarPar(rejilla, 1, 4, "Dirección:", txtDireccion);

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
                Padding = new Padding(10),
                Margin = new Padding(0),
                BackColor = InventarioEstilo.Fondo
            };

            BotonInventario btnRegistrar =
                NuevoBoton("REGISTRAR", TipoBoton.Primario);
            btnRegistrar.Click += (s, e) => RegistrarPaciente();

            BotonInventario btnActualizar =
                NuevoBoton("ACTUALIZAR", TipoBoton.Secundario);
            btnActualizar.Click += (s, e) => ActualizarPaciente();

            BotonInventario btnEliminar =
                NuevoBoton("ELIMINAR", TipoBoton.Peligro);
            btnEliminar.Click += (s, e) => EliminarPaciente();

            BotonInventario btnLimpiar =
                NuevoBoton("LIMPIAR", TipoBoton.Secundario);
            btnLimpiar.Click += (s, e) => LimpiarFormulario();

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
                Text = "Listado de pacientes registrados",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = InventarioEstilo.VerdeOscuro,
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.White,
                Margin = new Padding(0)
            };

            distribucionTabla.Controls.Add(tituloTabla, 0, 0);

            dgvPacientes = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White,
                RowHeadersVisible = false,
                BorderStyle = BorderStyle.None,
                Margin = new Padding(0)
            };

            dgvPacientes.SelectionChanged +=
                (s, e) => SeleccionarPaciente();

            distribucionTabla.Controls.Add(dgvPacientes, 0, 1);
        }

        private TableLayoutPanel CrearRejilla()
        {
            TableLayoutPanel tabla = new()
            {
                Dock = DockStyle.Fill,
                ColumnCount = 6,
                RowCount = 2,
                BackColor = Color.White,
                Padding = new Padding(5, 5, 5, 0),
                GrowStyle = TableLayoutPanelGrowStyle.FixedSize
            };

            tabla.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 115));
            tabla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            tabla.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
            tabla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            tabla.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 115));
            tabla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));

            tabla.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));
            tabla.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));

            return tabla;
        }

        private void AgregarPar(
            TableLayoutPanel tabla,
            int fila,
            int columna,
            string texto,
            Control campo)
        {
            tabla.Controls.Add(NuevaEtiqueta(texto), columna, fila);
            campo.Dock = DockStyle.Fill;
            campo.Margin = new Padding(3, 5, 12, 5);
            tabla.Controls.Add(campo, columna + 1, fila);
        }

        private Label NuevaEtiqueta(string texto)
        {
            return new Label
            {
                Text = texto,
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                ForeColor = InventarioEstilo.Texto,
                Font = new Font("Segoe UI Semibold", 9.5F),
                BackColor = Color.White,
                Margin = new Padding(3, 10, 3, 3)
            };
        }

        private CampoInventario NuevoCampo()
        {
            return new CampoInventario();
        }

        private DateTimePicker NuevoFecha()
        {
            return new DateTimePicker
            {
                Dock = DockStyle.Fill,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy",
                MaxDate = DateTime.Today,
                Value = DateTime.Today,
                Font = new Font("Segoe UI", 10F),
                Margin = new Padding(3, 5, 12, 5),
                CalendarMonthBackground = Color.White,
                CalendarTitleBackColor = InventarioEstilo.VerdeOscuro,
                CalendarTitleForeColor = Color.White
            };
        }

        private BotonInventario NuevoBoton(string texto, TipoBoton tipo)
        {
            return new BotonInventario(texto, tipo)
            {
                Size = new Size(155, 46),
                Margin = new Padding(8, 0, 8, 5)
            };
        }

        private void AplicarEstilos()
        {
            dgvPacientes.EnableHeadersVisualStyles = false;
            dgvPacientes.BackgroundColor = Color.White;
            dgvPacientes.BorderStyle = BorderStyle.None;
            dgvPacientes.GridColor = InventarioEstilo.Borde;
            dgvPacientes.RowHeadersVisible = false;
            dgvPacientes.ColumnHeadersHeight = 44;
            dgvPacientes.ColumnHeadersHeightSizeMode =
                DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            dgvPacientes.ColumnHeadersDefaultCellStyle =
                new DataGridViewCellStyle
                {
                    BackColor = InventarioEstilo.VerdeOscuro,
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                    SelectionBackColor = InventarioEstilo.VerdeOscuro,
                    SelectionForeColor = Color.White,
                    Padding = new Padding(8, 0, 0, 0)
                };

            dgvPacientes.DefaultCellStyle =
                new DataGridViewCellStyle
                {
                    BackColor = Color.White,
                    ForeColor = InventarioEstilo.Texto,
                    Font = new Font("Segoe UI", 9.5F),
                    SelectionBackColor = Color.FromArgb(225, 240, 229),
                    SelectionForeColor = InventarioEstilo.Texto,
                    Padding = new Padding(8, 0, 0, 0)
                };

            dgvPacientes.AlternatingRowsDefaultCellStyle =
                new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(247, 250, 248),
                    ForeColor = InventarioEstilo.Texto,
                    SelectionBackColor = Color.FromArgb(225, 240, 229),
                    SelectionForeColor = InventarioEstilo.Texto,
                    Font = new Font("Segoe UI", 9.5F)
                };

            dgvPacientes.RowTemplate.Height = 38;
        }

        private void ConfigurarTabla()
        {
            dgvPacientes.AutoGenerateColumns = false;

            dgvPacientes.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(PacienteVista.Dni),
                HeaderText = "DNI"
            });

            dgvPacientes.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(PacienteVista.Nombres),
                HeaderText = "Nombres"
            });

            dgvPacientes.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(PacienteVista.Apellidos),
                HeaderText = "Apellidos"
            });

            dgvPacientes.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(PacienteVista.FechaNacimiento),
                HeaderText = "Nacimiento",
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "dd/MM/yyyy"
                }
            });

            dgvPacientes.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(PacienteVista.Telefono),
                HeaderText = "Teléfono"
            });

            dgvPacientes.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(PacienteVista.Direccion),
                HeaderText = "Dirección"
            });

            dgvPacientes.DataSource = pacientes;
        }

        private void CargarPacientes()
        {
            cargando = true;

            try
            {
                pacientes.Clear();

                foreach (Paciente paciente in servicio.ObtenerTodos()
                    .Where(p => p.Estado))
                {
                    pacientes.Add(ConvertirVista(paciente));
                }

                indiceSeleccionado = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudieron cargar los pacientes desde la base de datos.\n\n" +
                    ex.Message,
                    "Error de conexión",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                cargando = false;
                dgvPacientes.ClearSelection();
            }
        }

        private PacienteVista ConvertirVista(Paciente paciente)
        {
            return new PacienteVista
            {
                IdPaciente = paciente.IdPaciente,
                Dni = paciente.NumeroDocumento,
                Nombres = paciente.Nombres,
                Apellidos = $"{paciente.ApellidoPaterno} {paciente.ApellidoMaterno}".Trim(),
                FechaNacimiento = paciente.FechaNacimiento ?? DateTime.Today,
                Telefono = paciente.Telefono,
                Direccion = paciente.Direccion
            };
        }

        private Paciente ConvertirEntidad(PacienteVista vista, Paciente? anterior = null)
        {
            string[] apellidos = vista.Apellidos
                .Split(' ', StringSplitOptions.RemoveEmptyEntries);

            return new Paciente
            {
                IdPaciente = vista.IdPaciente,
                IdPersona = vista.IdPaciente,
                TipoDocumento = "DNI",
                NumeroDocumento = vista.Dni,
                Nombres = vista.Nombres,
                ApellidoPaterno = apellidos.FirstOrDefault() ?? "",
                ApellidoMaterno = string.Join(" ", apellidos.Skip(1)),
                FechaNacimiento = vista.FechaNacimiento,
                Sexo = anterior?.Sexo ?? "No especificado",
                Telefono = vista.Telefono,
                Direccion = vista.Direccion,
                CorreoElectronico = anterior?.CorreoElectronico ?? "",
                Estado = true,
                FechaRegistro = anterior?.FechaRegistro ?? DateTime.Now,
                FechaRegistroPaciente = anterior?.FechaRegistroPaciente ?? DateTime.Now,
                CodigoPaciente = anterior?.CodigoPaciente ?? "",
                GrupoSanguineo = anterior?.GrupoSanguineo ?? "",
                Alergias = anterior?.Alergias ?? "",
                Observaciones = anterior?.Observaciones ?? ""
            };
        }

        private void RegistrarPaciente()
        {
            if (!ValidarDatos())
                return;

            string dni = txtDni.Text.Trim();

            if (pacientes.Any(p =>
                string.Equals(p.Dni, dni, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("Ya existe un paciente con ese DNI.");
                return;
            }

            try
            {
                Paciente entidad = ConvertirEntidad(ObtenerDatosFormulario());
                servicio.Registrar(entidad);

                MessageBox.Show(
                    "Paciente registrado correctamente.",
                    "SIGEPOSTA",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                CargarPacientes();
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo registrar el paciente.\n\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void ActualizarPaciente()
        {
            if (indiceSeleccionado < 0 ||
                indiceSeleccionado >= pacientes.Count)
            {
                MessageBox.Show("Seleccione un paciente de la tabla.");
                return;
            }

            if (!ValidarDatos())
                return;

            PacienteVista anteriorVista = pacientes[indiceSeleccionado];
            string dni = txtDni.Text.Trim();

            if (pacientes.Any(p =>
                p.IdPaciente != anteriorVista.IdPaciente &&
                string.Equals(p.Dni, dni, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("El DNI ya pertenece a otro paciente.");
                return;
            }

            try
            {
                Paciente? anterior = servicio.ObtenerPorId(anteriorVista.IdPaciente);

                if (anterior == null)
                {
                    MessageBox.Show("El paciente ya no existe en la base de datos.");
                    CargarPacientes();
                    return;
                }

                PacienteVista actualizado = ObtenerDatosFormulario();
                actualizado.IdPaciente = anteriorVista.IdPaciente;

                servicio.Actualizar(ConvertirEntidad(actualizado, anterior));

                MessageBox.Show(
                    "Paciente actualizado correctamente.",
                    "SIGEPOSTA",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                CargarPacientes();
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo actualizar el paciente.\n\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void EliminarPaciente()
        {
            if (indiceSeleccionado < 0 ||
                indiceSeleccionado >= pacientes.Count)
            {
                MessageBox.Show("Seleccione un paciente de la tabla.");
                return;
            }

            PacienteVista seleccionado = pacientes[indiceSeleccionado];

            DialogResult respuesta = MessageBox.Show(
                "¿Está seguro de desactivar al paciente seleccionado?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (respuesta != DialogResult.Yes)
                return;

            try
            {
                servicio.Eliminar(seleccionado.IdPaciente);

                MessageBox.Show(
                    "Paciente desactivado correctamente.",
                    "SIGEPOSTA",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                CargarPacientes();
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo desactivar el paciente.\n\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void SeleccionarPaciente()
        {
            if (cargando || dgvPacientes.CurrentRow == null)
                return;

            if (dgvPacientes.CurrentRow.DataBoundItem
                is not PacienteVista paciente)
                return;

            indiceSeleccionado = pacientes.IndexOf(paciente);

            txtDni.Text = paciente.Dni;
            txtNombres.Text = paciente.Nombres;
            txtApellidos.Text = paciente.Apellidos;

            DateTime nacimiento = paciente.FechaNacimiento;
            if (nacimiento.Date > DateTime.Today)
                nacimiento = DateTime.Today;

            dtpNacimiento.Value = nacimiento;
            txtTelefono.Text = paciente.Telefono;
            txtDireccion.Text = paciente.Direccion;
        }

        private bool ValidarDatos()
        {
            if (string.IsNullOrWhiteSpace(txtDni.Text) ||
                string.IsNullOrWhiteSpace(txtNombres.Text) ||
                string.IsNullOrWhiteSpace(txtApellidos.Text))
            {
                MessageBox.Show(
                    "Complete el DNI, los nombres y los apellidos.");
                return false;
            }

            if (txtDni.Text.Trim().Length != 8 ||
                !txtDni.Text.Trim().All(char.IsDigit))
            {
                MessageBox.Show(
                    "El DNI debe contener exactamente 8 números.");
                return false;
            }

            if (dtpNacimiento.Value.Date > DateTime.Today)
            {
                MessageBox.Show(
                    "La fecha de nacimiento no puede ser futura.");
                return false;
            }

            return true;
        }

        private PacienteVista ObtenerDatosFormulario()
        {
            return new PacienteVista
            {
                Dni = txtDni.Text.Trim(),
                Nombres = txtNombres.Text.Trim(),
                Apellidos = txtApellidos.Text.Trim(),
                FechaNacimiento = dtpNacimiento.Value.Date,
                Telefono = txtTelefono.Text.Trim(),
                Direccion = txtDireccion.Text.Trim()
            };
        }

        private void LimpiarFormulario()
        {
            indiceSeleccionado = -1;

            txtDni.Clear();
            txtNombres.Clear();
            txtApellidos.Clear();
            txtTelefono.Clear();
            txtDireccion.Clear();

            dtpNacimiento.Value = DateTime.Today;
            dgvPacientes.ClearSelection();
        }

        private class PacienteVista
        {
            public int IdPaciente { get; set; }
            public string Dni { get; set; } = "";
            public string Nombres { get; set; } = "";
            public string Apellidos { get; set; } = "";
            public DateTime FechaNacimiento { get; set; }
            public string Telefono { get; set; } = "";
            public string Direccion { get; set; } = "";
        }
    }
}
