using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SIGEPOSTA.Presentacion.Formularios
{
    public class FormDiagnosticos : Form
    {
        private readonly BindingList<DiagnosticoVista> diagnosticos = new();

        private TextBox txtDniPaciente = new();
        private TextBox txtPaciente = new();
        private TextBox txtDniProfesional = new();
        private TextBox txtProfesional = new();
        private TextBox txtCodigo = new();
        private TextBox txtDescripcion = new();
        private TextBox txtObservaciones = new();

        private DateTimePicker dtpFecha = new();
        private ComboBox cmbTipo = new();
        private ComboBox cmbEstado = new();

        private DataGridView dgvDiagnosticos = new();

        private int indiceSeleccionado = -1;

        public FormDiagnosticos()
        {
            ConstruirFormulario();
            ConfigurarTabla();
        }

        private void ConstruirFormulario()
        {
            Text = "SIGEPOSTA - Diagnósticos";
            Size = new Size(1200, 760);
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
                Text = "GESTIÓN DE DIAGNÓSTICOS",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 19, FontStyle.Bold),
                Location = new Point(25, 18),
                AutoSize = true
            });

            Controls.Add(encabezado);

            Controls.Add(new Label
            {
                Text = "Registro y seguimiento de diagnósticos clínicos",
                Location = new Point(25, 95),
                AutoSize = true,
                ForeColor = Color.DimGray
            });

            GroupBox grupo = new GroupBox
            {
                Text = "Información del diagnóstico",
                Location = new Point(25, 125),
                Size = new Size(1135, 340)
            };

            Controls.Add(grupo);

            Etiqueta(grupo, "DNI paciente:", 20, 35);
            txtDniPaciente = Campo(grupo, 140, 32, 200);

            Etiqueta(grupo, "Paciente:", 370, 35);
            txtPaciente = Campo(grupo, 460, 32, 250);

            Etiqueta(grupo, "DNI profesional:", 740, 35);
            txtDniProfesional = Campo(grupo, 875, 32, 220);

            Etiqueta(grupo, "Profesional:", 20, 90);
            txtProfesional = Campo(grupo, 140, 87, 200);

            Etiqueta(grupo, "Código CIE-10:", 370, 90);
            txtCodigo = Campo(grupo, 460, 87, 250);

            Etiqueta(grupo, "Fecha:", 740, 90);

            dtpFecha = new DateTimePicker
            {
                Location = new Point(875, 87),
                Width = 220,
                Format = DateTimePickerFormat.Short,
                MaxDate = DateTime.Today
            };

            grupo.Controls.Add(dtpFecha);

            Etiqueta(grupo, "Tipo:", 20, 145);

            cmbTipo = new ComboBox
            {
                Location = new Point(140, 142),
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            cmbTipo.Items.AddRange(new object[]
            {
                "Presuntivo",
                "Definitivo",
                "Control"
            });

            cmbTipo.SelectedIndex = 0;
            grupo.Controls.Add(cmbTipo);

            Etiqueta(grupo, "Estado:", 370, 145);

            cmbEstado = new ComboBox
            {
                Location = new Point(460, 142),
                Width = 250,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            cmbEstado.Items.AddRange(new object[]
            {
                "Activo",
                "Resuelto",
                "En seguimiento"
            });

            cmbEstado.SelectedIndex = 0;
            grupo.Controls.Add(cmbEstado);

            Etiqueta(grupo, "Descripción:", 20, 200);
            txtDescripcion = AreaTexto(grupo, 140, 197, 955, 55);

            Etiqueta(grupo, "Observaciones:", 20, 275);
            txtObservaciones = AreaTexto(grupo, 140, 272, 955, 50);

            Button btnRegistrar = Boton("REGISTRAR", 25, 485, Color.FromArgb(46, 125, 50));
            btnRegistrar.Click += (s, e) => Registrar();
            Controls.Add(btnRegistrar);

            Button btnActualizar = Boton("ACTUALIZAR", 190, 485, Color.FromArgb(25, 118, 210));
            btnActualizar.Click += (s, e) => Actualizar();
            Controls.Add(btnActualizar);

            Button btnEliminar = Boton("ELIMINAR", 355, 485, Color.FromArgb(198, 40, 40));
            btnEliminar.Click += (s, e) => Eliminar();
            Controls.Add(btnEliminar);

            Button btnLimpiar = Boton("LIMPIAR", 520, 485, Color.FromArgb(97, 97, 97));
            btnLimpiar.Click += (s, e) => Limpiar();
            Controls.Add(btnLimpiar);

            GroupBox grupoTabla = new GroupBox
            {
                Text = "Diagnósticos registrados",
                Location = new Point(25, 545),
                Size = new Size(1135, 155)
            };

            dgvDiagnosticos = new DataGridView
            {
                Location = new Point(12, 25),
                Size = new Size(1110, 115),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White,
                RowHeadersVisible = false
            };

            dgvDiagnosticos.SelectionChanged += (s, e) => Seleccionar();

            grupoTabla.Controls.Add(dgvDiagnosticos);
            Controls.Add(grupoTabla);
        }

        private void ConfigurarTabla()
        {
            dgvDiagnosticos.AutoGenerateColumns = false;

            Columna("DniPaciente", "DNI paciente");
            Columna("Paciente", "Paciente");
            Columna("Codigo", "Código CIE-10");
            Columna("Fecha", "Fecha");
            Columna("Tipo", "Tipo");
            Columna("Estado", "Estado");

            dgvDiagnosticos.DataSource = diagnosticos;
        }

        private void Columna(string propiedad, string titulo)
        {
            dgvDiagnosticos.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = propiedad,
                HeaderText = titulo
            });
        }

        private void Registrar()
        {
            if (!Validar())
                return;

            diagnosticos.Add(ObtenerDatos());

            MessageBox.Show("Diagnóstico registrado correctamente.");
            Limpiar();
        }

        private void Actualizar()
        {
            if (indiceSeleccionado < 0 || indiceSeleccionado >= diagnosticos.Count)
            {
                MessageBox.Show("Seleccione un diagnóstico.");
                return;
            }

            if (!Validar())
                return;

            DiagnosticoVista nuevo = ObtenerDatos();
            DiagnosticoVista anterior = diagnosticos[indiceSeleccionado];

            anterior.DniPaciente = nuevo.DniPaciente;
            anterior.Paciente = nuevo.Paciente;
            anterior.DniProfesional = nuevo.DniProfesional;
            anterior.Profesional = nuevo.Profesional;
            anterior.Codigo = nuevo.Codigo;
            anterior.Fecha = nuevo.Fecha;
            anterior.Tipo = nuevo.Tipo;
            anterior.Estado = nuevo.Estado;
            anterior.Descripcion = nuevo.Descripcion;
            anterior.Observaciones = nuevo.Observaciones;

            dgvDiagnosticos.Refresh();

            MessageBox.Show("Diagnóstico actualizado correctamente.");
            Limpiar();
        }

        private void Eliminar()
        {
            if (indiceSeleccionado < 0 || indiceSeleccionado >= diagnosticos.Count)
            {
                MessageBox.Show("Seleccione un diagnóstico.");
                return;
            }

            DialogResult respuesta = MessageBox.Show(
                "¿Desea eliminar este diagnóstico?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (respuesta == DialogResult.Yes)
            {
                diagnosticos.RemoveAt(indiceSeleccionado);

                MessageBox.Show("Diagnóstico eliminado.");
                Limpiar();
            }
        }

        private void Seleccionar()
        {
            if (dgvDiagnosticos.CurrentRow?.DataBoundItem is not DiagnosticoVista d)
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
                MessageBox.Show("Los DNI deben contener exactamente 8 números.");
                return false;
            }

            if (cmbTipo.SelectedItem == null || cmbEstado.SelectedItem == null)
            {
                MessageBox.Show("Seleccione el tipo y estado del diagnóstico.");
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

        private class DiagnosticoVista
        {
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
