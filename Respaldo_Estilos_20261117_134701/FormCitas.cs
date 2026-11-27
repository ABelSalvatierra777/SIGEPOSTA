using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SIGEPOSTA.Presentacion.Formularios
{
    public class FormCitas : Form
    {
        private readonly BindingList<CitaVista> citas = new();

        private TextBox txtDniPaciente = new();
        private TextBox txtPaciente = new();
        private TextBox txtDniProfesional = new();
        private TextBox txtProfesional = new();
        private TextBox txtMotivo = new();

        private DateTimePicker dtpFecha = new();
        private DateTimePicker dtpHora = new();

        private ComboBox cmbEstado = new();
        private DataGridView dgvCitas = new();

        private int indiceSeleccionado = -1;

        public FormCitas()
        {
            ConstruirFormulario();
            ConfigurarTabla();
        }

        private void ConstruirFormulario()
        {
            Text = "SIGEPOSTA - Gestión de citas médicas";
            Size = new Size(1200, 760);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(245, 247, 249);
            Font = new Font("Segoe UI", 10);

            Panel encabezado = new Panel
            {
                Dock = DockStyle.Top,
                Height = 85,
                BackColor = Color.FromArgb(31, 77, 54)
            };

            encabezado.Controls.Add(new Label
            {
                Text = "GESTIÓN DE CITAS MÉDICAS",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 19, FontStyle.Bold),
                Location = new Point(25, 20),
                AutoSize = true
            });

            Controls.Add(encabezado);

            Controls.Add(new Label
            {
                Text = "Programación y seguimiento de citas de los pacientes",
                Location = new Point(25, 105),
                AutoSize = true,
                ForeColor = Color.DimGray
            });

            GroupBox grupo = new GroupBox
            {
                Text = "Información de la cita",
                Location = new Point(25, 140),
                Size = new Size(1125, 290),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            Controls.Add(grupo);

            AgregarEtiqueta(grupo, "DNI del paciente:", 25, 35);
            txtDniPaciente = AgregarTexto(grupo, 165, 32, 210);

            AgregarEtiqueta(grupo, "Paciente:", 410, 35);
            txtPaciente = AgregarTexto(grupo, 510, 32, 250);

            AgregarEtiqueta(grupo, "DNI profesional:", 790, 35);
            txtDniProfesional = AgregarTexto(grupo, 930, 32, 165);

            AgregarEtiqueta(grupo, "Profesional:", 25, 90);
            txtProfesional = AgregarTexto(grupo, 165, 87, 210);

            AgregarEtiqueta(grupo, "Fecha:", 410, 90);

            dtpFecha = new DateTimePicker
            {
                Location = new Point(510, 87),
                Width = 250,
                Format = DateTimePickerFormat.Short,
                MinDate = DateTime.Today
            };

            grupo.Controls.Add(dtpFecha);

            AgregarEtiqueta(grupo, "Hora:", 790, 90);

            dtpHora = new DateTimePicker
            {
                Location = new Point(930, 87),
                Width = 165,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "HH:mm",
                ShowUpDown = true
            };

            grupo.Controls.Add(dtpHora);

            AgregarEtiqueta(grupo, "Motivo:", 25, 145);

            txtMotivo = new TextBox
            {
                Location = new Point(165, 142),
                Width = 595,
                Height = 35,
                Multiline = true,
                Font = new Font("Segoe UI", 10)
            };

            grupo.Controls.Add(txtMotivo);

            AgregarEtiqueta(grupo, "Estado:", 790, 145);

            cmbEstado = new ComboBox
            {
                Location = new Point(930, 142),
                Width = 165,
                DropDownStyle = ComboBoxStyle.DropDownList
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
            grupo.Controls.Add(cmbEstado);

            Button btnRegistrar = CrearBoton(
                "REGISTRAR", 25, 220, Color.FromArgb(46, 125, 50));

            btnRegistrar.Click += (s, e) => RegistrarCita();
            grupo.Controls.Add(btnRegistrar);

            Button btnActualizar = CrearBoton(
                "ACTUALIZAR", 190, 220, Color.FromArgb(25, 118, 210));

            btnActualizar.Click += (s, e) => ActualizarCita();
            grupo.Controls.Add(btnActualizar);

            Button btnEliminar = CrearBoton(
                "ELIMINAR", 355, 220, Color.FromArgb(198, 40, 40));

            btnEliminar.Click += (s, e) => EliminarCita();
            grupo.Controls.Add(btnEliminar);

            Button btnLimpiar = CrearBoton(
                "LIMPIAR", 520, 220, Color.FromArgb(97, 97, 97));

            btnLimpiar.Click += (s, e) => LimpiarFormulario();
            grupo.Controls.Add(btnLimpiar);

            GroupBox grupoTabla = new GroupBox
            {
                Text = "Listado de citas médicas",
                Location = new Point(25, 450),
                Size = new Size(1125, 255),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            dgvCitas = new DataGridView
            {
                Location = new Point(15, 30),
                Size = new Size(1095, 205),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White,
                RowHeadersVisible = false
            };

            dgvCitas.SelectionChanged += (s, e) => SeleccionarCita();

            grupoTabla.Controls.Add(dgvCitas);
            Controls.Add(grupoTabla);
        }

        private void ConfigurarTabla()
        {
            dgvCitas.AutoGenerateColumns = false;

            AgregarColumna("DniPaciente", "DNI paciente");
            AgregarColumna("Paciente", "Paciente");
            AgregarColumna("DniProfesional", "DNI profesional");
            AgregarColumna("Profesional", "Profesional");
            AgregarColumna("Fecha", "Fecha");
            AgregarColumna("Hora", "Hora");
            AgregarColumna("Motivo", "Motivo");
            AgregarColumna("Estado", "Estado");

            dgvCitas.DataSource = citas;
        }

        private void AgregarColumna(string propiedad, string titulo)
        {
            dgvCitas.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = propiedad,
                HeaderText = titulo
            });
        }

        private void RegistrarCita()
        {
            if (!ValidarDatos())
                return;

            bool horarioOcupado = citas.Any(c =>
                c.DniProfesional == txtDniProfesional.Text.Trim() &&
                c.Fecha == dtpFecha.Value.Date &&
                c.Hora == dtpHora.Value.ToString("HH:mm") &&
                c.Estado != "Cancelada");

            if (horarioOcupado)
            {
                MessageBox.Show(
                    "El profesional ya tiene una cita registrada en ese horario.");

                return;
            }

            citas.Add(ObtenerDatos());

            MessageBox.Show("Cita médica registrada correctamente.");
            LimpiarFormulario();
        }

        private void ActualizarCita()
        {
            if (indiceSeleccionado < 0 || indiceSeleccionado >= citas.Count)
            {
                MessageBox.Show("Seleccione una cita de la tabla.");
                return;
            }

            if (!ValidarDatos())
                return;

            string dniProfesional = txtDniProfesional.Text.Trim();
            DateTime fecha = dtpFecha.Value.Date;
            string hora = dtpHora.Value.ToString("HH:mm");

            bool horarioOcupado = citas
                .Where((c, i) => i != indiceSeleccionado)
                .Any(c =>
                    c.DniProfesional == dniProfesional &&
                    c.Fecha == fecha &&
                    c.Hora == hora &&
                    c.Estado != "Cancelada");

            if (horarioOcupado)
            {
                MessageBox.Show(
                    "El profesional ya tiene una cita en ese horario.");

                return;
            }

            CitaVista nuevo = ObtenerDatos();
            CitaVista anterior = citas[indiceSeleccionado];

            anterior.DniPaciente = nuevo.DniPaciente;
            anterior.Paciente = nuevo.Paciente;
            anterior.DniProfesional = nuevo.DniProfesional;
            anterior.Profesional = nuevo.Profesional;
            anterior.Fecha = nuevo.Fecha;
            anterior.Hora = nuevo.Hora;
            anterior.Motivo = nuevo.Motivo;
            anterior.Estado = nuevo.Estado;

            dgvCitas.Refresh();

            MessageBox.Show("Cita actualizada correctamente.");
            LimpiarFormulario();
        }

        private void EliminarCita()
        {
            if (indiceSeleccionado < 0 || indiceSeleccionado >= citas.Count)
            {
                MessageBox.Show("Seleccione una cita de la tabla.");
                return;
            }

            DialogResult respuesta = MessageBox.Show(
                "¿Está seguro de eliminar esta cita?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (respuesta == DialogResult.Yes)
            {
                citas.RemoveAt(indiceSeleccionado);

                MessageBox.Show("Cita eliminada correctamente.");
                LimpiarFormulario();
            }
        }

        private void SeleccionarCita()
        {
            if (dgvCitas.CurrentRow?.DataBoundItem is not CitaVista cita)
                return;

            indiceSeleccionado = citas.IndexOf(cita);

            txtDniPaciente.Text = cita.DniPaciente;
            txtPaciente.Text = cita.Paciente;
            txtDniProfesional.Text = cita.DniProfesional;
            txtProfesional.Text = cita.Profesional;
            dtpFecha.Value = cita.Fecha;

            if (TimeSpan.TryParse(cita.Hora, out TimeSpan hora))
                dtpHora.Value = DateTime.Today.Add(hora);

            txtMotivo.Text = cita.Motivo;
            cmbEstado.SelectedItem = cita.Estado;
        }

        private bool ValidarDatos()
        {
            if (string.IsNullOrWhiteSpace(txtDniPaciente.Text) ||
                string.IsNullOrWhiteSpace(txtPaciente.Text) ||
                string.IsNullOrWhiteSpace(txtDniProfesional.Text) ||
                string.IsNullOrWhiteSpace(txtProfesional.Text) ||
                string.IsNullOrWhiteSpace(txtMotivo.Text))
            {
                MessageBox.Show("Complete todos los campos de la cita.");
                return false;
            }

            if (!ValidarDni(txtDniPaciente.Text) ||
                !ValidarDni(txtDniProfesional.Text))
            {
                MessageBox.Show("Los DNI deben contener exactamente 8 números.");
                return false;
            }

            if (dtpFecha.Value.Date < DateTime.Today)
            {
                MessageBox.Show("La fecha de la cita no puede ser anterior a hoy.");
                return false;
            }

            if (cmbEstado.SelectedItem == null)
            {
                MessageBox.Show("Seleccione el estado de la cita.");
                return false;
            }

            return true;
        }

        private bool ValidarDni(string dni)
        {
            dni = dni.Trim();

            return dni.Length == 8 && dni.All(char.IsDigit);
        }

        private CitaVista ObtenerDatos()
        {
            return new CitaVista
            {
                DniPaciente = txtDniPaciente.Text.Trim(),
                Paciente = txtPaciente.Text.Trim(),
                DniProfesional = txtDniProfesional.Text.Trim(),
                Profesional = txtProfesional.Text.Trim(),
                Fecha = dtpFecha.Value.Date,
                Hora = dtpHora.Value.ToString("HH:mm"),
                Motivo = txtMotivo.Text.Trim(),
                Estado = cmbEstado.Text
            };
        }

        private void LimpiarFormulario()
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
        }

        private void AgregarEtiqueta(Control padre, string texto, int x, int y)
        {
            padre.Controls.Add(new Label
            {
                Text = texto,
                Location = new Point(x, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 9)
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

        private class CitaVista
        {
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
