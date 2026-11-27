using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SIGEPOSTA.Presentacion.Formularios
{
    public class FormAtencion : Form
    {
        private readonly BindingList<AtencionVista> atenciones = new();

        private TextBox txtDniPaciente = new();
        private TextBox txtPaciente = new();
        private TextBox txtDniProfesional = new();
        private TextBox txtProfesional = new();
        private TextBox txtTemperatura = new();
        private TextBox txtPresion = new();
        private TextBox txtPeso = new();
        private TextBox txtTalla = new();
        private TextBox txtSintomas = new();
        private TextBox txtEvaluacion = new();
        private TextBox txtTratamiento = new();
        private TextBox txtObservaciones = new();

        private DateTimePicker dtpFecha = new();
        private ComboBox cmbEstado = new();
        private DataGridView dgvAtenciones = new();

        private int indiceSeleccionado = -1;

        public FormAtencion()
        {
            ConstruirFormulario();
            ConfigurarTabla();
        }

        private void ConstruirFormulario()
        {
            Text = "SIGEPOSTA - Atención médica";
            Size = new Size(1250, 850);
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
                Text = "REGISTRO DE ATENCIÓN MÉDICA",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 19, FontStyle.Bold),
                Location = new Point(25, 18),
                AutoSize = true
            });

            Controls.Add(encabezado);

            Controls.Add(new Label
            {
                Text = "Registro de consultas, evaluación clínica y tratamiento del paciente",
                Location = new Point(25, 95),
                AutoSize = true,
                ForeColor = Color.DimGray
            });

            GroupBox grupoPaciente = new GroupBox
            {
                Text = "Información del paciente y profesional",
                Location = new Point(25, 125),
                Size = new Size(1180, 125)
            };

            Controls.Add(grupoPaciente);

            Etiqueta(grupoPaciente, "DNI paciente:", 20, 30);
            txtDniPaciente = Campo(grupoPaciente, 125, 27, 160);

            Etiqueta(grupoPaciente, "Paciente:", 310, 30);
            txtPaciente = Campo(grupoPaciente, 390, 27, 240);

            Etiqueta(grupoPaciente, "DNI profesional:", 650, 30);
            txtDniProfesional = Campo(grupoPaciente, 785, 27, 160);

            Etiqueta(grupoPaciente, "Profesional:", 20, 80);
            txtProfesional = Campo(grupoPaciente, 125, 77, 240);

            Etiqueta(grupoPaciente, "Fecha:", 390, 80);

            dtpFecha = new DateTimePicker
            {
                Location = new Point(445, 77),
                Width = 185,
                Format = DateTimePickerFormat.Short,
                MaxDate = DateTime.Today
            };

            grupoPaciente.Controls.Add(dtpFecha);

            GroupBox grupoSignos = new GroupBox
            {
                Text = "Signos vitales y medidas",
                Location = new Point(25, 265),
                Size = new Size(1180, 100)
            };

            Controls.Add(grupoSignos);

            Etiqueta(grupoSignos, "Temperatura (°C):", 20, 35);
            txtTemperatura = Campo(grupoSignos, 155, 32, 120);

            Etiqueta(grupoSignos, "Presión arterial:", 300, 35);
            txtPresion = Campo(grupoSignos, 430, 32, 140);

            Etiqueta(grupoSignos, "Peso (kg):", 595, 35);
            txtPeso = Campo(grupoSignos, 680, 32, 120);

            Etiqueta(grupoSignos, "Talla (m):", 825, 35);
            txtTalla = Campo(grupoSignos, 905, 32, 120);

            GroupBox grupoClinico = new GroupBox
            {
                Text = "Evaluación clínica",
                Location = new Point(25, 380),
                Size = new Size(1180, 210)
            };

            Controls.Add(grupoClinico);

            Etiqueta(grupoClinico, "Síntomas:", 20, 30);
            txtSintomas = AreaTexto(grupoClinico, 125, 27, 480, 65);

            Etiqueta(grupoClinico, "Evaluación:", 625, 30);
            txtEvaluacion = AreaTexto(grupoClinico, 730, 27, 425, 65);

            Etiqueta(grupoClinico, "Tratamiento:", 20, 115);
            txtTratamiento = AreaTexto(grupoClinico, 125, 112, 480, 65);

            Etiqueta(grupoClinico, "Observaciones:", 625, 115);
            txtObservaciones = AreaTexto(grupoClinico, 730, 112, 425, 65);

            GroupBox grupoEstado = new GroupBox
            {
                Text = "Estado de la atención",
                Location = new Point(25, 605),
                Size = new Size(1180, 70)
            };

            Controls.Add(grupoEstado);

            cmbEstado = new ComboBox
            {
                Location = new Point(125, 27),
                Width = 220,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            cmbEstado.Items.AddRange(new object[]
            {
                "En proceso",
                "Finalizada",
                "Derivada"
            });

            cmbEstado.SelectedIndex = 0;

            grupoEstado.Controls.Add(cmbEstado);

            Button btnRegistrar = Boton("REGISTRAR", 25, 695, Color.FromArgb(46, 125, 50));
            btnRegistrar.Click += (s, e) => Registrar();
            Controls.Add(btnRegistrar);

            Button btnActualizar = Boton("ACTUALIZAR", 190, 695, Color.FromArgb(25, 118, 210));
            btnActualizar.Click += (s, e) => Actualizar();
            Controls.Add(btnActualizar);

            Button btnEliminar = Boton("ELIMINAR", 355, 695, Color.FromArgb(198, 40, 40));
            btnEliminar.Click += (s, e) => Eliminar();
            Controls.Add(btnEliminar);

            Button btnLimpiar = Boton("LIMPIAR", 520, 695, Color.FromArgb(97, 97, 97));
            btnLimpiar.Click += (s, e) => Limpiar();
            Controls.Add(btnLimpiar);

            GroupBox grupoTabla = new GroupBox
            {
                Text = "Atenciones registradas",
                Location = new Point(25, 745),
                Size = new Size(1180, 1)
            };

            // La tabla se mostrará en una ventana independiente
            // mediante el botón de consulta.
        }

        private void ConfigurarTabla()
        {
            dgvAtenciones = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White,
                RowHeadersVisible = false
            };

            dgvAtenciones.AutoGenerateColumns = false;

            Columna("DniPaciente", "DNI paciente");
            Columna("Paciente", "Paciente");
            Columna("Profesional", "Profesional");
            Columna("Fecha", "Fecha");
            Columna("Temperatura", "Temperatura");
            Columna("Presion", "Presión");
            Columna("Estado", "Estado");

            dgvAtenciones.DataSource = atenciones;
            dgvAtenciones.SelectionChanged += (s, e) => Seleccionar();

            Button btnConsultar = Boton(
                "CONSULTAR ATENCIONES", 685, 695,
                Color.FromArgb(0, 121, 107));

            btnConsultar.Click += (s, e) => MostrarListado();
            Controls.Add(btnConsultar);
        }

        private void Columna(string propiedad, string titulo)
        {
            dgvAtenciones.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = propiedad,
                HeaderText = titulo
            });
        }

        private void MostrarListado()
        {
            Form ventana = new Form
            {
                Text = "Listado de atenciones médicas",
                Size = new Size(1050, 450),
                StartPosition = FormStartPosition.CenterParent,
                BackColor = Color.White
            };

            ventana.Controls.Add(dgvAtenciones);
            ventana.ShowDialog(this);

            Controls.Add(dgvAtenciones);
            dgvAtenciones.Dock = DockStyle.None;
            dgvAtenciones.Size = Size.Empty;
        }

        private void Registrar()
        {
            if (!Validar())
                return;

            atenciones.Add(ObtenerDatos());

            MessageBox.Show("Atención médica registrada correctamente.");
            Limpiar();
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

            AtencionVista nuevo = ObtenerDatos();
            AtencionVista anterior = atenciones[indiceSeleccionado];

            anterior.DniPaciente = nuevo.DniPaciente;
            anterior.Paciente = nuevo.Paciente;
            anterior.DniProfesional = nuevo.DniProfesional;
            anterior.Profesional = nuevo.Profesional;
            anterior.Fecha = nuevo.Fecha;
            anterior.Temperatura = nuevo.Temperatura;
            anterior.Presion = nuevo.Presion;
            anterior.Peso = nuevo.Peso;
            anterior.Talla = nuevo.Talla;
            anterior.Sintomas = nuevo.Sintomas;
            anterior.Evaluacion = nuevo.Evaluacion;
            anterior.Tratamiento = nuevo.Tratamiento;
            anterior.Observaciones = nuevo.Observaciones;
            anterior.Estado = nuevo.Estado;

            dgvAtenciones.Refresh();

            MessageBox.Show("Atención actualizada correctamente.");
            Limpiar();
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
                MessageBoxIcon.Question
            );

            if (respuesta == DialogResult.Yes)
            {
                atenciones.RemoveAt(indiceSeleccionado);
                MessageBox.Show("Registro eliminado.");
                Limpiar();
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

        private bool NumeroValido(string valor, double minimo, double maximo)
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
                Size = new Size(155, 35),
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
        }

        private class AtencionVista
        {
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
