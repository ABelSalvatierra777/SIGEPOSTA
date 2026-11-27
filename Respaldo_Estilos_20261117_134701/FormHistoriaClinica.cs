using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SIGEPOSTA.Presentacion.Formularios
{
    public class FormHistoriaClinica : Form
    {
        private readonly BindingList<HistoriaVista> historias = new();

        private TextBox txtDni = new();
        private TextBox txtPaciente = new();
        private TextBox txtGrupoSanguineo = new();
        private TextBox txtAlergias = new();
        private TextBox txtAntecedentesPersonales = new();
        private TextBox txtAntecedentesFamiliares = new();
        private TextBox txtObservaciones = new();

        private DateTimePicker dtpFechaRegistro = new();
        private DataGridView dgvHistorias = new();

        private int indiceSeleccionado = -1;

        public FormHistoriaClinica()
        {
            ConstruirFormulario();
            ConfigurarTabla();
        }

        private void ConstruirFormulario()
        {
            Text = "SIGEPOSTA - Historias clínicas";
            Size = new Size(1200, 800);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(245, 247, 249);
            Font = new Font("Segoe UI", 10);

            Panel encabezado = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.FromArgb(31, 77, 54)
            };

            encabezado.Controls.Add(new Label
            {
                Text = "GESTIÓN DE HISTORIAS CLÍNICAS",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 19, FontStyle.Bold),
                Location = new Point(25, 18),
                AutoSize = true
            });

            Controls.Add(encabezado);

            Controls.Add(new Label
            {
                Text = "Registro y consulta de información clínica de los pacientes",
                Location = new Point(25, 95),
                AutoSize = true,
                ForeColor = Color.DimGray
            });

            GroupBox grupoPaciente = new GroupBox
            {
                Text = "Identificación del paciente",
                Location = new Point(25, 125),
                Size = new Size(1135, 115)
            };

            Controls.Add(grupoPaciente);

            Etiqueta(grupoPaciente, "DNI:", 20, 35);
            txtDni = Campo(grupoPaciente, 100, 32, 200);

            Etiqueta(grupoPaciente, "Paciente:", 330, 35);
            txtPaciente = Campo(grupoPaciente, 420, 32, 300);

            Etiqueta(grupoPaciente, "Grupo sanguíneo:", 750, 35);

            txtGrupoSanguineo = Campo(grupoPaciente, 885, 32, 210);

            GroupBox grupoAntecedentes = new GroupBox
            {
                Text = "Información clínica",
                Location = new Point(25, 255),
                Size = new Size(1135, 300)
            };

            Controls.Add(grupoAntecedentes);

            Etiqueta(grupoAntecedentes, "Alergias:", 20, 30);
            txtAlergias = AreaTexto(grupoAntecedentes, 180, 27, 390, 65);

            Etiqueta(grupoAntecedentes, "Antecedentes personales:", 590, 30);
            txtAntecedentesPersonales = AreaTexto(
                grupoAntecedentes, 790, 27, 320, 65);

            Etiqueta(grupoAntecedentes, "Antecedentes familiares:", 20, 125);
            txtAntecedentesFamiliares = AreaTexto(
                grupoAntecedentes, 220, 122, 350, 65);

            Etiqueta(grupoAntecedentes, "Observaciones:", 590, 125);
            txtObservaciones = AreaTexto(
                grupoAntecedentes, 790, 122, 320, 65);

            Etiqueta(grupoAntecedentes, "Fecha de registro:", 20, 225);

            dtpFechaRegistro = new DateTimePicker
            {
                Location = new Point(180, 222),
                Width = 200,
                Format = DateTimePickerFormat.Short,
                MaxDate = DateTime.Today
            };

            grupoAntecedentes.Controls.Add(dtpFechaRegistro);

            Button btnRegistrar = Boton(
                "REGISTRAR", 25, 575, Color.FromArgb(46, 125, 50));

            btnRegistrar.Click += (s, e) => Registrar();
            Controls.Add(btnRegistrar);

            Button btnActualizar = Boton(
                "ACTUALIZAR", 190, 575, Color.FromArgb(25, 118, 210));

            btnActualizar.Click += (s, e) => Actualizar();
            Controls.Add(btnActualizar);

            Button btnEliminar = Boton(
                "ELIMINAR", 355, 575, Color.FromArgb(198, 40, 40));

            btnEliminar.Click += (s, e) => Eliminar();
            Controls.Add(btnEliminar);

            Button btnLimpiar = Boton(
                "LIMPIAR", 520, 575, Color.FromArgb(97, 97, 97));

            btnLimpiar.Click += (s, e) => Limpiar();
            Controls.Add(btnLimpiar);

            GroupBox grupoTabla = new GroupBox
            {
                Text = "Historias clínicas registradas",
                Location = new Point(25, 630),
                Size = new Size(1135, 125)
            };

            dgvHistorias = new DataGridView
            {
                Location = new Point(12, 25),
                Size = new Size(1105, 90),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White,
                RowHeadersVisible = false
            };

            dgvHistorias.SelectionChanged += (s, e) => Seleccionar();

            grupoTabla.Controls.Add(dgvHistorias);
            Controls.Add(grupoTabla);
        }

        private void ConfigurarTabla()
        {
            dgvHistorias.AutoGenerateColumns = false;

            Columna("Dni", "DNI");
            Columna("Paciente", "Paciente");
            Columna("GrupoSanguineo", "Grupo sanguíneo");
            Columna("FechaRegistro", "Fecha de registro");

            dgvHistorias.DataSource = historias;
        }

        private void Columna(string propiedad, string titulo)
        {
            dgvHistorias.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = propiedad,
                HeaderText = titulo
            });
        }

        private void Registrar()
        {
            if (!Validar())
                return;

            if (historias.Any(h => h.Dni == txtDni.Text.Trim()))
            {
                MessageBox.Show(
                    "Este paciente ya tiene una historia clínica registrada.");

                return;
            }

            historias.Add(ObtenerDatos());

            MessageBox.Show("Historia clínica registrada correctamente.");
            Limpiar();
        }

        private void Actualizar()
        {
            if (indiceSeleccionado < 0 || indiceSeleccionado >= historias.Count)
            {
                MessageBox.Show("Seleccione una historia clínica.");
                return;
            }

            if (!Validar())
                return;

            string dni = txtDni.Text.Trim();

            if (historias.Any(h =>
                h.Dni == dni && h != historias[indiceSeleccionado]))
            {
                MessageBox.Show("El DNI pertenece a otra historia clínica.");
                return;
            }

            HistoriaVista nuevo = ObtenerDatos();
            HistoriaVista anterior = historias[indiceSeleccionado];

            anterior.Dni = nuevo.Dni;
            anterior.Paciente = nuevo.Paciente;
            anterior.GrupoSanguineo = nuevo.GrupoSanguineo;
            anterior.Alergias = nuevo.Alergias;
            anterior.AntecedentesPersonales = nuevo.AntecedentesPersonales;
            anterior.AntecedentesFamiliares = nuevo.AntecedentesFamiliares;
            anterior.Observaciones = nuevo.Observaciones;
            anterior.FechaRegistro = nuevo.FechaRegistro;

            dgvHistorias.Refresh();

            MessageBox.Show("Historia clínica actualizada correctamente.");
            Limpiar();
        }

        private void Eliminar()
        {
            if (indiceSeleccionado < 0 || indiceSeleccionado >= historias.Count)
            {
                MessageBox.Show("Seleccione una historia clínica.");
                return;
            }

            DialogResult respuesta = MessageBox.Show(
                "¿Está seguro de eliminar esta historia clínica?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (respuesta == DialogResult.Yes)
            {
                historias.RemoveAt(indiceSeleccionado);

                MessageBox.Show("Registro eliminado.");
                Limpiar();
            }
        }

        private void Seleccionar()
        {
            if (dgvHistorias.CurrentRow?.DataBoundItem is not HistoriaVista h)
                return;

            indiceSeleccionado = historias.IndexOf(h);

            txtDni.Text = h.Dni;
            txtPaciente.Text = h.Paciente;
            txtGrupoSanguineo.Text = h.GrupoSanguineo;
            txtAlergias.Text = h.Alergias;
            txtAntecedentesPersonales.Text = h.AntecedentesPersonales;
            txtAntecedentesFamiliares.Text = h.AntecedentesFamiliares;
            txtObservaciones.Text = h.Observaciones;
            dtpFechaRegistro.Value = h.FechaRegistro;
        }

        private bool Validar()
        {
            if (string.IsNullOrWhiteSpace(txtDni.Text) ||
                string.IsNullOrWhiteSpace(txtPaciente.Text))
            {
                MessageBox.Show("Ingrese el DNI y el nombre del paciente.");
                return false;
            }

            string dni = txtDni.Text.Trim();

            if (dni.Length != 8 || !dni.All(char.IsDigit))
            {
                MessageBox.Show("El DNI debe contener exactamente 8 números.");
                return false;
            }

            return true;
        }

        private HistoriaVista ObtenerDatos()
        {
            return new HistoriaVista
            {
                Dni = txtDni.Text.Trim(),
                Paciente = txtPaciente.Text.Trim(),
                GrupoSanguineo = txtGrupoSanguineo.Text.Trim(),
                Alergias = txtAlergias.Text.Trim(),
                AntecedentesPersonales = txtAntecedentesPersonales.Text.Trim(),
                AntecedentesFamiliares = txtAntecedentesFamiliares.Text.Trim(),
                Observaciones = txtObservaciones.Text.Trim(),
                FechaRegistro = dtpFechaRegistro.Value.Date
            };
        }

        private void Limpiar()
        {
            indiceSeleccionado = -1;

            txtDni.Clear();
            txtPaciente.Clear();
            txtGrupoSanguineo.Clear();
            txtAlergias.Clear();
            txtAntecedentesPersonales.Clear();
            txtAntecedentesFamiliares.Clear();
            txtObservaciones.Clear();

            dtpFechaRegistro.Value = DateTime.Today;
            dgvHistorias.ClearSelection();
        }

        private void Etiqueta(Control padre, string texto, int x, int y)
        {
            padre.Controls.Add(new Label
            {
                Text = texto,
                Location = new Point(x, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 9)
            });
        }

        private TextBox Campo(Control padre, int x, int y, int ancho)
        {
            TextBox campo = new TextBox
            {
                Location = new Point(x, y),
                Width = ancho
            };

            padre.Controls.Add(campo);
            return campo;
        }

        private TextBox AreaTexto(Control padre, int x, int y, int ancho, int alto)
        {
            TextBox campo = new TextBox
            {
                Location = new Point(x, y),
                Width = ancho,
                Height = alto,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };

            padre.Controls.Add(campo);
            return campo;
        }

        private Button Boton(string texto, int x, int y, Color color)
        {
            return new Button
            {
                Text = texto,
                Location = new Point(x, y),
                Size = new Size(150, 35),
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
        }

        private class HistoriaVista
        {
            public string Dni { get; set; } = "";
            public string Paciente { get; set; } = "";
            public string GrupoSanguineo { get; set; } = "";
            public string Alergias { get; set; } = "";
            public string AntecedentesPersonales { get; set; } = "";
            public string AntecedentesFamiliares { get; set; } = "";
            public string Observaciones { get; set; } = "";
            public DateTime FechaRegistro { get; set; }
        }
    }
}
