using System;
using System.Drawing;
using System.Windows.Forms;

namespace SIGEPOSTA.Presentacion.Formularios
{
    public class FormUsuarios : Form
    {
        private TextBox txtUsuario;
        private TextBox txtNombre;
        private TextBox txtContrasena;
        private ComboBox cmbRol;
        private CheckBox chkActivo;
        private DataGridView tabla;

        public FormUsuarios()
        {
            ConfigurarVentana();
            CrearFormulario();
            CrearTabla();
        }

        private void ConfigurarVentana()
        {
            Text = "Administración de usuarios - SIGEPOSTA";
            Size = new Size(1050, 680);
            MinimumSize = new Size(900, 600);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(245, 247, 246);
        }

        private void CrearFormulario()
        {
            Label titulo = new Label
            {
                Text = "Administración de usuarios",
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                ForeColor = Color.FromArgb(31, 77, 54),
                Location = new Point(25, 20),
                AutoSize = true
            };

            Label descripcion = new Label
            {
                Text = "Registre y administre las cuentas de acceso del personal.",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.DimGray,
                Location = new Point(28, 65),
                AutoSize = true
            };

            Controls.Add(titulo);
            Controls.Add(descripcion);

            GroupBox grupo = new GroupBox
            {
                Text = "Datos de la cuenta",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(25, 105),
                Size = new Size(970, 205),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            CrearEtiqueta(grupo, "Nombre completo:", 20, 35);
            txtNombre = CrearTexto(grupo, 160, 32, 300);

            CrearEtiqueta(grupo, "Nombre de usuario:", 500, 35);
            txtUsuario = CrearTexto(grupo, 650, 32, 280);

            CrearEtiqueta(grupo, "Contraseña:", 20, 85);
            txtContrasena = CrearTexto(grupo, 160, 82, 300);
            txtContrasena.UseSystemPasswordChar = true;

            CrearEtiqueta(grupo, "Rol:", 500, 85);
            cmbRol = new ComboBox
            {
                Location = new Point(650, 82),
                Size = new Size(280, 30),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10)
            };

            cmbRol.Items.AddRange(new object[]
            {
                "Administrador",
                "Personal de salud",
                "Vendedor",
                "Almacenero"
            });

            cmbRol.SelectedIndex = 1;
            grupo.Controls.Add(cmbRol);

            chkActivo = new CheckBox
            {
                Text = "Usuario activo",
                Location = new Point(160, 130),
                AutoSize = true,
                Checked = true,
                Font = new Font("Segoe UI", 10)
            };

            grupo.Controls.Add(chkActivo);

            Button btnRegistrar = CrearBoton("Registrar", 500, 125);
            btnRegistrar.Click += (s, e) => RegistrarUsuario();

            Button btnEditar = CrearBoton("Actualizar", 625, 125);
            btnEditar.Click += (s, e) => ActualizarUsuario();

            Button btnEliminar = CrearBoton("Eliminar", 750, 125);
            btnEliminar.Click += (s, e) => EliminarUsuario();

            Button btnLimpiar = CrearBoton("Limpiar", 855, 125);
            btnLimpiar.Click += (s, e) => LimpiarCampos();

            grupo.Controls.Add(btnRegistrar);
            grupo.Controls.Add(btnEditar);
            grupo.Controls.Add(btnEliminar);
            grupo.Controls.Add(btnLimpiar);

            Controls.Add(grupo);
        }

        private void CrearTabla()
        {
            tabla = new DataGridView
            {
                Location = new Point(25, 330),
                Size = new Size(970, 285),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom |
                         AnchorStyles.Left | AnchorStyles.Right,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                RowHeadersVisible = false,
                Font = new Font("Segoe UI", 9)
            };

            tabla.Columns.Add("Nombre", "Nombre completo");
            tabla.Columns.Add("Usuario", "Nombre de usuario");
            tabla.Columns.Add("Rol", "Rol");
            tabla.Columns.Add("Estado", "Estado");

            tabla.CellClick += (s, e) => SeleccionarUsuario();

            Controls.Add(tabla);
        }

        private void CrearEtiqueta(Control contenedor, string texto, int x, int y)
        {
            contenedor.Controls.Add(new Label
            {
                Text = texto,
                Location = new Point(x, y),
                Size = new Size(135, 25),
                Font = new Font("Segoe UI", 9)
            });
        }

        private TextBox CrearTexto(Control contenedor, int x, int y, int ancho)
        {
            TextBox campo = new TextBox
            {
                Location = new Point(x, y),
                Size = new Size(ancho, 28),
                Font = new Font("Segoe UI", 10)
            };

            contenedor.Controls.Add(campo);
            return campo;
        }

        private Button CrearBoton(string texto, int x, int y)
        {
            return new Button
            {
                Text = texto,
                Location = new Point(x, y),
                Size = new Size(95, 32),
                BackColor = Color.FromArgb(47, 125, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
        }

        private void RegistrarUsuario()
        {
            if (!ValidarCampos())
                return;

            foreach (DataGridViewRow fila in tabla.Rows)
            {
                if (Convert.ToString(fila.Cells["Usuario"].Value)
                    .Equals(txtUsuario.Text.Trim(),
                    StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("El nombre de usuario ya está registrado.");
                    return;
                }
            }

            tabla.Rows.Add(
                txtNombre.Text.Trim(),
                txtUsuario.Text.Trim(),
                cmbRol.Text,
                chkActivo.Checked ? "Activo" : "Inactivo"
            );

            MessageBox.Show(
                "Usuario agregado temporalmente.",
                "SIGEPOSTA",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

            LimpiarCampos();
        }

        private void ActualizarUsuario()
        {
            if (tabla.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione un usuario de la tabla.");
                return;
            }

            if (!ValidarCampos())
                return;

            DataGridViewRow fila = tabla.SelectedRows[0];
            string usuarioNuevo = txtUsuario.Text.Trim();

            foreach (DataGridViewRow otraFila in tabla.Rows)
            {
                if (otraFila == fila)
                    continue;

                if (Convert.ToString(otraFila.Cells["Usuario"].Value)
                    .Equals(usuarioNuevo, StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("Ese nombre de usuario ya pertenece a otra cuenta.");
                    return;
                }
            }

            fila.Cells["Nombre"].Value = txtNombre.Text.Trim();
            fila.Cells["Usuario"].Value = usuarioNuevo;
            fila.Cells["Rol"].Value = cmbRol.Text;
            fila.Cells["Estado"].Value = chkActivo.Checked ? "Activo" : "Inactivo";

            MessageBox.Show("Usuario actualizado temporalmente.");
            LimpiarCampos();
        }

        private void EliminarUsuario()
        {
            if (tabla.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione un usuario de la tabla.");
                return;
            }

            DialogResult respuesta = MessageBox.Show(
                "¿Está seguro de eliminar este usuario?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (respuesta == DialogResult.Yes)
            {
                tabla.Rows.RemoveAt(tabla.SelectedRows[0].Index);
                LimpiarCampos();
            }
        }

        private void SeleccionarUsuario()
        {
            if (tabla.SelectedRows.Count == 0)
                return;

            DataGridViewRow fila = tabla.SelectedRows[0];

            txtNombre.Text = Convert.ToString(fila.Cells["Nombre"].Value);
            txtUsuario.Text = Convert.ToString(fila.Cells["Usuario"].Value);
            cmbRol.Text = Convert.ToString(fila.Cells["Rol"].Value);
            chkActivo.Checked =
                Convert.ToString(fila.Cells["Estado"].Value) == "Activo";

            txtContrasena.Clear();
        }

        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtUsuario.Text) ||
                string.IsNullOrWhiteSpace(txtContrasena.Text))
            {
                MessageBox.Show("Complete el nombre, usuario y contraseña.");
                return false;
            }

            if (txtContrasena.Text.Trim().Length < 8)
            {
                MessageBox.Show("La contraseña debe tener al menos 8 caracteres.");
                return false;
            }

            if (cmbRol.SelectedIndex < 0)
            {
                MessageBox.Show("Seleccione un rol.");
                return false;
            }

            return true;
        }

        private void LimpiarCampos()
        {
            txtNombre.Clear();
            txtUsuario.Clear();
            txtContrasena.Clear();
            cmbRol.SelectedIndex = 1;
            chkActivo.Checked = true;
            tabla.ClearSelection();
            txtNombre.Focus();
        }
    }
}
