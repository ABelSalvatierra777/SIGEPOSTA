using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SIGEPOSTA.Presentacion.Formularios
{
    public class FormPacientes : Form
    {
        private readonly BindingList<PacienteVista> pacientes = new();

        private TextBox txtDni = new();
        private TextBox txtNombres = new();
        private TextBox txtApellidos = new();
        private TextBox txtTelefono = new();
        private TextBox txtDireccion = new();
        private DateTimePicker dtpNacimiento = new();
        private DataGridView dgvPacientes = new();

        private int indiceSeleccionado = -1;

        public FormPacientes()
        {
            ConstruirFormulario();
            ConfigurarTabla();
        }

        private void ConstruirFormulario()
        {
            Text = "SIGEPOSTA - Gestión de pacientes";
            Size = new Size(1150, 720);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(245, 247, 249);
            Font = new Font("Segoe UI", 10);

            Panel encabezado = new Panel
            {
                Dock = DockStyle.Top,
                Height = 85,
                BackColor = Color.FromArgb(31, 77, 54)
            };

            Label titulo = new Label
            {
                Text = "GESTIÓN DE PACIENTES",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 19, FontStyle.Bold),
                Location = new Point(25, 20),
                AutoSize = true
            };

            encabezado.Controls.Add(titulo);
            Controls.Add(encabezado);

            Label subtitulo = new Label
            {
                Text = "Registro, consulta y administración de pacientes",
                Location = new Point(25, 105),
                AutoSize = true,
                ForeColor = Color.DimGray
            };
            Controls.Add(subtitulo);

            GroupBox grupo = new GroupBox
            {
                Text = "Datos del paciente",
                Location = new Point(25, 140),
                Size = new Size(1080, 210),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            Controls.Add(grupo);

            AgregarEtiqueta(grupo, "DNI:", 25, 35);
            txtDni = AgregarTexto(grupo, 145, 32, 210);

            AgregarEtiqueta(grupo, "Nombres:", 390, 35);
            txtNombres = AgregarTexto(grupo, 510, 32, 220);

            AgregarEtiqueta(grupo, "Apellidos:", 755, 35);
            txtApellidos = AgregarTexto(grupo, 865, 32, 190);

            AgregarEtiqueta(grupo, "Fecha de nacimiento:", 25, 90);
            dtpNacimiento = new DateTimePicker
            {
                Location = new Point(145, 87),
                Width = 210,
                Format = DateTimePickerFormat.Short,
                MaxDate = DateTime.Today
            };
            grupo.Controls.Add(dtpNacimiento);

            AgregarEtiqueta(grupo, "Teléfono:", 390, 90);
            txtTelefono = AgregarTexto(grupo, 510, 87, 220);

            AgregarEtiqueta(grupo, "Dirección:", 755, 90);
            txtDireccion = AgregarTexto(grupo, 865, 87, 190);

            Button btnRegistrar = CrearBoton("REGISTRAR", 25, 145, Color.FromArgb(46, 125, 50));
            btnRegistrar.Click += (s, e) => RegistrarPaciente();
            grupo.Controls.Add(btnRegistrar);

            Button btnActualizar = CrearBoton("ACTUALIZAR", 190, 145, Color.FromArgb(25, 118, 210));
            btnActualizar.Click += (s, e) => ActualizarPaciente();
            grupo.Controls.Add(btnActualizar);

            Button btnEliminar = CrearBoton("ELIMINAR", 355, 145, Color.FromArgb(198, 40, 40));
            btnEliminar.Click += (s, e) => EliminarPaciente();
            grupo.Controls.Add(btnEliminar);

            Button btnLimpiar = CrearBoton("LIMPIAR", 520, 145, Color.FromArgb(97, 97, 97));
            btnLimpiar.Click += (s, e) => LimpiarFormulario();
            grupo.Controls.Add(btnLimpiar);

            GroupBox grupoTabla = new GroupBox
            {
                Text = "Listado de pacientes registrados",
                Location = new Point(25, 370),
                Size = new Size(1080, 285),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            dgvPacientes = new DataGridView
            {
                Location = new Point(15, 30),
                Size = new Size(1045, 235),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White,
                RowHeadersVisible = false
            };

            dgvPacientes.SelectionChanged += (s, e) => SeleccionarPaciente();

            grupoTabla.Controls.Add(dgvPacientes);
            Controls.Add(grupoTabla);
        }

        private void ConfigurarTabla()
        {
            dgvPacientes.AutoGenerateColumns = false;

            dgvPacientes.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Dni",
                HeaderText = "DNI"
            });

            dgvPacientes.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Nombres",
                HeaderText = "Nombres"
            });

            dgvPacientes.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Apellidos",
                HeaderText = "Apellidos"
            });

            dgvPacientes.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "FechaNacimiento",
                HeaderText = "Nacimiento"
            });

            dgvPacientes.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Telefono",
                HeaderText = "Teléfono"
            });

            dgvPacientes.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Direccion",
                HeaderText = "Dirección"
            });

            dgvPacientes.DataSource = pacientes;
        }

        private void RegistrarPaciente()
        {
            if (!ValidarDatos())
                return;

            if (pacientes.Any(p => p.Dni == txtDni.Text.Trim()))
            {
                MessageBox.Show("Ya existe un paciente con ese DNI.");
                return;
            }

            pacientes.Add(ObtenerDatosFormulario());

            MessageBox.Show("Paciente registrado correctamente.");
            LimpiarFormulario();
        }

        private void ActualizarPaciente()
        {
            if (indiceSeleccionado < 0 || indiceSeleccionado >= pacientes.Count)
            {
                MessageBox.Show("Seleccione un paciente de la tabla.");
                return;
            }

            if (!ValidarDatos())
                return;

            string dni = txtDni.Text.Trim();

            if (pacientes.Any(p => p.Dni == dni && p != pacientes[indiceSeleccionado]))
            {
                MessageBox.Show("El DNI ya pertenece a otro paciente.");
                return;
            }

            PacienteVista actualizado = ObtenerDatosFormulario();
            PacienteVista anterior = pacientes[indiceSeleccionado];

            anterior.Dni = actualizado.Dni;
            anterior.Nombres = actualizado.Nombres;
            anterior.Apellidos = actualizado.Apellidos;
            anterior.FechaNacimiento = actualizado.FechaNacimiento;
            anterior.Telefono = actualizado.Telefono;
            anterior.Direccion = actualizado.Direccion;

            dgvPacientes.Refresh();

            MessageBox.Show("Paciente actualizado correctamente.");
            LimpiarFormulario();
        }

        private void EliminarPaciente()
        {
            if (indiceSeleccionado < 0 || indiceSeleccionado >= pacientes.Count)
            {
                MessageBox.Show("Seleccione un paciente de la tabla.");
                return;
            }

            DialogResult respuesta = MessageBox.Show(
                "¿Está seguro de eliminar este paciente?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (respuesta == DialogResult.Yes)
            {
                pacientes.RemoveAt(indiceSeleccionado);
                MessageBox.Show("Paciente eliminado.");
                LimpiarFormulario();
            }
        }

        private void SeleccionarPaciente()
        {
            if (dgvPacientes.CurrentRow == null)
                return;

            if (dgvPacientes.CurrentRow.DataBoundItem is not PacienteVista paciente)
                return;

            indiceSeleccionado = pacientes.IndexOf(paciente);

            txtDni.Text = paciente.Dni;
            txtNombres.Text = paciente.Nombres;
            txtApellidos.Text = paciente.Apellidos;
            dtpNacimiento.Value = paciente.FechaNacimiento;
            txtTelefono.Text = paciente.Telefono;
            txtDireccion.Text = paciente.Direccion;
        }

        private bool ValidarDatos()
        {
            if (string.IsNullOrWhiteSpace(txtDni.Text) ||
                string.IsNullOrWhiteSpace(txtNombres.Text) ||
                string.IsNullOrWhiteSpace(txtApellidos.Text))
            {
                MessageBox.Show("Complete el DNI, los nombres y los apellidos.");
                return false;
            }

            if (txtDni.Text.Trim().Length != 8 ||
                !txtDni.Text.Trim().All(char.IsDigit))
            {
                MessageBox.Show("El DNI debe contener exactamente 8 números.");
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

        private void AgregarEtiqueta(Control padre, string texto, int x, int y)
        {
            padre.Controls.Add(new Label
            {
                Text = texto,
                Location = new Point(x, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Regular)
            });
        }

        private TextBox AgregarTexto(Control padre, int x, int y, int ancho)
        {
            TextBox campo = new TextBox
            {
                Location = new Point(x, y),
                Width = ancho,
                Font = new Font("Segoe UI", 10)
            };

            padre.Controls.Add(campo);
            return campo;
        }

        private Button CrearBoton(string texto, int x, int y, Color color)
        {
            return new Button
            {
                Text = texto,
                Location = new Point(x, y),
                Size = new Size(145, 35),
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
        }

        private class PacienteVista
        {
            public string Dni { get; set; } = "";
            public string Nombres { get; set; } = "";
            public string Apellidos { get; set; } = "";
            public DateTime FechaNacimiento { get; set; }
            public string Telefono { get; set; } = "";
            public string Direccion { get; set; } = "";
        }
    }
}
