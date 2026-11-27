using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SIGEPOSTA.Presentacion.Formularios
{
    public class FormPersonalSalud : Form
    {
        private readonly BindingList<PersonalVista> personal = new();

        private TextBox txtDni = new();
        private TextBox txtNombres = new();
        private TextBox txtApellidos = new();
        private TextBox txtTelefono = new();
        private TextBox txtCorreo = new();
        private ComboBox cmbCargo = new();
        private ComboBox cmbEspecialidad = new();
        private DataGridView dgvPersonal = new();

        private int indiceSeleccionado = -1;

        public FormPersonalSalud()
        {
            ConstruirFormulario();
            ConfigurarTabla();
        }

        private void ConstruirFormulario()
        {
            Text = "SIGEPOSTA - Personal de salud";
            Size = new Size(1150, 730);
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
                Text = "GESTIÓN DEL PERSONAL DE SALUD",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 19, FontStyle.Bold),
                Location = new Point(25, 20),
                AutoSize = true
            });

            Controls.Add(encabezado);

            Controls.Add(new Label
            {
                Text = "Administración de médicos, enfermeros y trabajadores del establecimiento",
                Location = new Point(25, 105),
                AutoSize = true,
                ForeColor = Color.DimGray
            });

            GroupBox grupo = new GroupBox
            {
                Text = "Información del personal",
                Location = new Point(25, 140),
                Size = new Size(1080, 265),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            Controls.Add(grupo);

            AgregarEtiqueta(grupo, "DNI:", 25, 35);
            txtDni = AgregarTexto(grupo, 145, 32, 210);

            AgregarEtiqueta(grupo, "Nombres:", 390, 35);
            txtNombres = AgregarTexto(grupo, 510, 32, 220);

            AgregarEtiqueta(grupo, "Apellidos:", 755, 35);
            txtApellidos = AgregarTexto(grupo, 865, 32, 190);

            AgregarEtiqueta(grupo, "Cargo:", 25, 90);
            cmbCargo = new ComboBox
            {
                Location = new Point(145, 87),
                Width = 210,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbCargo.Items.AddRange(new object[]
            {
                "Médico",
                "Enfermero(a)",
                "Obstetra",
                "Técnico en enfermería",
                "Odontólogo(a)",
                "Personal administrativo",
                "Otro"
            });
            cmbCargo.SelectedIndex = 0;
            grupo.Controls.Add(cmbCargo);

            AgregarEtiqueta(grupo, "Especialidad:", 390, 90);
            cmbEspecialidad = new ComboBox
            {
                Location = new Point(510, 87),
                Width = 220,
                DropDownStyle = ComboBoxStyle.DropDown
            };
            cmbEspecialidad.Items.AddRange(new object[]
            {
                "Medicina general",
                "Enfermería",
                "Obstetricia",
                "Odontología",
                "Pediatría",
                "Administración",
                "No aplica"
            });
            cmbEspecialidad.SelectedIndex = 0;
            grupo.Controls.Add(cmbEspecialidad);

            AgregarEtiqueta(grupo, "Teléfono:", 755, 90);
            txtTelefono = AgregarTexto(grupo, 865, 87, 190);

            AgregarEtiqueta(grupo, "Correo:", 25, 145);
            txtCorreo = AgregarTexto(grupo, 145, 142, 400);

            Button btnRegistrar = CrearBoton("REGISTRAR", 25, 205, Color.FromArgb(46, 125, 50));
            btnRegistrar.Click += (s, e) => RegistrarPersonal();
            grupo.Controls.Add(btnRegistrar);

            Button btnActualizar = CrearBoton("ACTUALIZAR", 190, 205, Color.FromArgb(25, 118, 210));
            btnActualizar.Click += (s, e) => ActualizarPersonal();
            grupo.Controls.Add(btnActualizar);

            Button btnEliminar = CrearBoton("ELIMINAR", 355, 205, Color.FromArgb(198, 40, 40));
            btnEliminar.Click += (s, e) => EliminarPersonal();
            grupo.Controls.Add(btnEliminar);

            Button btnLimpiar = CrearBoton("LIMPIAR", 520, 205, Color.FromArgb(97, 97, 97));
            btnLimpiar.Click += (s, e) => LimpiarFormulario();
            grupo.Controls.Add(btnLimpiar);

            GroupBox grupoTabla = new GroupBox
            {
                Text = "Listado del personal registrado",
                Location = new Point(25, 425),
                Size = new Size(1080, 245),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            dgvPersonal = new DataGridView
            {
                Location = new Point(15, 30),
                Size = new Size(1045, 195),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White,
                RowHeadersVisible = false
            };

            dgvPersonal.SelectionChanged += (s, e) => SeleccionarPersonal();

            grupoTabla.Controls.Add(dgvPersonal);
            Controls.Add(grupoTabla);
        }

        private void ConfigurarTabla()
        {
            dgvPersonal.AutoGenerateColumns = false;

            AgregarColumna("Dni", "DNI");
            AgregarColumna("Nombres", "Nombres");
            AgregarColumna("Apellidos", "Apellidos");
            AgregarColumna("Cargo", "Cargo");
            AgregarColumna("Especialidad", "Especialidad");
            AgregarColumna("Telefono", "Teléfono");
            AgregarColumna("Correo", "Correo");

            dgvPersonal.DataSource = personal;
        }

        private void AgregarColumna(string propiedad, string titulo)
        {
            dgvPersonal.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = propiedad,
                HeaderText = titulo
            });
        }

        private void RegistrarPersonal()
        {
            if (!ValidarDatos())
                return;

            if (personal.Any(p => p.Dni == txtDni.Text.Trim()))
            {
                MessageBox.Show("Ya existe personal registrado con ese DNI.");
                return;
            }

            personal.Add(ObtenerDatos());

            MessageBox.Show("Personal registrado correctamente.");
            LimpiarFormulario();
        }

        private void ActualizarPersonal()
        {
            if (indiceSeleccionado < 0 || indiceSeleccionado >= personal.Count)
            {
                MessageBox.Show("Seleccione un trabajador de la tabla.");
                return;
            }

            if (!ValidarDatos())
                return;

            string dni = txtDni.Text.Trim();

            if (personal.Any(p => p.Dni == dni && p != personal[indiceSeleccionado]))
            {
                MessageBox.Show("El DNI pertenece a otro trabajador.");
                return;
            }

            PersonalVista nuevo = ObtenerDatos();
            PersonalVista anterior = personal[indiceSeleccionado];

            anterior.Dni = nuevo.Dni;
            anterior.Nombres = nuevo.Nombres;
            anterior.Apellidos = nuevo.Apellidos;
            anterior.Cargo = nuevo.Cargo;
            anterior.Especialidad = nuevo.Especialidad;
            anterior.Telefono = nuevo.Telefono;
            anterior.Correo = nuevo.Correo;

            dgvPersonal.Refresh();

            MessageBox.Show("Información actualizada correctamente.");
            LimpiarFormulario();
        }

        private void EliminarPersonal()
        {
            if (indiceSeleccionado < 0 || indiceSeleccionado >= personal.Count)
            {
                MessageBox.Show("Seleccione un trabajador de la tabla.");
                return;
            }

            DialogResult respuesta = MessageBox.Show(
                "¿Desea eliminar este registro?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (respuesta == DialogResult.Yes)
            {
                personal.RemoveAt(indiceSeleccionado);
                MessageBox.Show("Registro eliminado correctamente.");
                LimpiarFormulario();
            }
        }

        private void SeleccionarPersonal()
        {
            if (dgvPersonal.CurrentRow?.DataBoundItem is not PersonalVista trabajador)
                return;

            indiceSeleccionado = personal.IndexOf(trabajador);

            txtDni.Text = trabajador.Dni;
            txtNombres.Text = trabajador.Nombres;
            txtApellidos.Text = trabajador.Apellidos;
            cmbCargo.SelectedItem = trabajador.Cargo;
            cmbEspecialidad.Text = trabajador.Especialidad;
            txtTelefono.Text = trabajador.Telefono;
            txtCorreo.Text = trabajador.Correo;
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

            if (cmbCargo.SelectedItem == null)
            {
                MessageBox.Show("Seleccione el cargo del trabajador.");
                return false;
            }

            return true;
        }

        private PersonalVista ObtenerDatos()
        {
            return new PersonalVista
            {
                Dni = txtDni.Text.Trim(),
                Nombres = txtNombres.Text.Trim(),
                Apellidos = txtApellidos.Text.Trim(),
                Cargo = cmbCargo.Text,
                Especialidad = cmbEspecialidad.Text.Trim(),
                Telefono = txtTelefono.Text.Trim(),
                Correo = txtCorreo.Text.Trim()
            };
        }

        private void LimpiarFormulario()
        {
            indiceSeleccionado = -1;

            txtDni.Clear();
            txtNombres.Clear();
            txtApellidos.Clear();
            txtTelefono.Clear();
            txtCorreo.Clear();

            cmbCargo.SelectedIndex = 0;
            cmbEspecialidad.SelectedIndex = 0;

            dgvPersonal.ClearSelection();
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

        private class PersonalVista
        {
            public string Dni { get; set; } = "";
            public string Nombres { get; set; } = "";
            public string Apellidos { get; set; } = "";
            public string Cargo { get; set; } = "";
            public string Especialidad { get; set; } = "";
            public string Telefono { get; set; } = "";
            public string Correo { get; set; } = "";
        }
    }
}
