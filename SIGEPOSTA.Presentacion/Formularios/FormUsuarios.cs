using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SIGEPOSTA.Datos.Repositorios;
using SIGEPOSTA.Entidades.Seguridad;

namespace SIGEPOSTA.Presentacion.Formularios
{
    public class FormUsuarios : Form
    {
        private readonly UsuarioRepositorio repositorio = new();
        private readonly DataGridView tabla = new();
        private readonly TextBox txtBuscar = new();
        private readonly Label lblTotal = new();
        private System.Collections.Generic.List<Usuario> usuarios = new();

        private readonly Color verde = Color.FromArgb(43, 125, 91);
        private readonly Color oscuro = Color.FromArgb(27, 65, 54);
        private readonly Color fondo = Color.FromArgb(240, 244, 242);

        public FormUsuarios()
        {
            ConstruirFormulario();
            CargarUsuarios();
        }

        private void ConstruirFormulario()
        {
            Text = "SIGEPOSTA - Usuarios";
            BackColor = fondo;
            Dock = DockStyle.Fill;
            MinimumSize = new Size(900, 600);

            var principal = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(22),
                BackColor = fondo
            };

            principal.RowStyles.Add(new RowStyle(SizeType.Absolute, 90));
            principal.RowStyles.Add(new RowStyle(SizeType.Absolute, 95));
            principal.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            var encabezado = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = oscuro,
                Padding = new Padding(22)
            };

            encabezado.Controls.Add(new Label
            {
                Text = "GESTIÓN DE USUARIOS",
                Dock = DockStyle.Top,
                Height = 35,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 18, FontStyle.Bold)
            });

            encabezado.Controls.Add(new Label
            {
                Text = "Administración de cuentas y accesos al sistema",
                Dock = DockStyle.Top,
                Height = 25,
                ForeColor = Color.WhiteSmoke,
                Font = new Font("Segoe UI", 9.5F)
            });

            var herramientas = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(12),
                WrapContents = false
            };

            herramientas.Controls.Add(new Label
            {
                Text = "Buscar usuario:",
                AutoSize = true,
                Margin = new Padding(5, 12, 5, 5),
                Font = new Font("Segoe UI", 10)
            });

            txtBuscar.Width = 260;
            txtBuscar.Font = new Font("Segoe UI", 10);
            txtBuscar.Margin = new Padding(5, 5, 15, 5);
            txtBuscar.TextChanged += (s, e) => MostrarUsuarios();

            herramientas.Controls.Add(txtBuscar);

            var btnActualizar = CrearBoton("ACTUALIZAR", verde);
            btnActualizar.Click += (s, e) => CargarUsuarios();
            herramientas.Controls.Add(btnActualizar);

            var btnEstado = CrearBoton("CAMBIAR ESTADO", oscuro);
            btnEstado.Click += (s, e) => CambiarEstado();
            herramientas.Controls.Add(btnEstado);

            var contenedor = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(12)
            };

            tabla.Dock = DockStyle.Fill;
            tabla.BackgroundColor = Color.White;
            tabla.BorderStyle = BorderStyle.None;
            tabla.AllowUserToAddRows = false;
            tabla.AllowUserToDeleteRows = false;
            tabla.ReadOnly = true;
            tabla.MultiSelect = false;
            tabla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            tabla.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            tabla.RowHeadersVisible = false;
            tabla.Font = new Font("Segoe UI", 9.5F);
            tabla.RowTemplate.Height = 36;
            tabla.EnableHeadersVisualStyles = false;
            tabla.ColumnHeadersDefaultCellStyle.BackColor = oscuro;
            tabla.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            tabla.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 9.5F, FontStyle.Bold);
            tabla.ColumnHeadersHeight = 42;
            tabla.AlternatingRowsDefaultCellStyle.BackColor =
                Color.FromArgb(245, 248, 246);

            contenedor.Controls.Add(tabla);

            lblTotal.Text = "Usuarios registrados";
            lblTotal.Dock = DockStyle.Bottom;
            lblTotal.Height = 28;
            lblTotal.Font = new Font("Segoe UI", 9.5F);
            lblTotal.ForeColor = oscuro;

            var zonaTabla = new Panel { Dock = DockStyle.Fill };
            zonaTabla.Controls.Add(contenedor);
            zonaTabla.Controls.Add(lblTotal);

            principal.Controls.Add(encabezado, 0, 0);
            principal.Controls.Add(herramientas, 0, 1);
            principal.Controls.Add(zonaTabla, 0, 2);

            Controls.Add(principal);
        }

        private Button CrearBoton(string texto, Color color)
        {
            return new Button
            {
                Text = texto,
                Width = 145,
                Height = 38,
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Margin = new Padding(5)
            };
        }

        private void CargarUsuarios()
        {
            try
            {
                usuarios = repositorio.ObtenerTodos().ToList();
                MostrarUsuarios();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudieron cargar los usuarios: " + ex.Message,
                    "Usuarios",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        private void MostrarUsuarios()
        {
            string buscar = txtBuscar.Text.Trim();

            var filtrados = usuarios.Where(u =>
                (u.NombreUsuario ?? "").Contains(
                    buscar, StringComparison.OrdinalIgnoreCase)
                ||
                ((u.PersonalSalud?.Nombres ?? "") + " " +
                 (u.PersonalSalud?.ApellidoPaterno ?? "")).Contains(
                    buscar, StringComparison.OrdinalIgnoreCase)
            ).ToList();

            var datos = new DataTable();
            datos.Columns.Add("IdUsuario", typeof(int));
            datos.Columns.Add("NombreUsuario", typeof(string));
            datos.Columns.Add("PersonalSalud", typeof(string));
            datos.Columns.Add("Roles", typeof(string));
            datos.Columns.Add("Estado", typeof(string));
            datos.Columns.Add("FechaCreacion", typeof(DateTime));

            foreach (var usuario in filtrados)
            {
                string personal = usuario.PersonalSalud == null
                    ? "Sin personal asignado"
                    : $"{usuario.PersonalSalud.Nombres} {usuario.PersonalSalud.ApellidoPaterno}";

                string roles = string.Join(
                    ", ",
                    usuario.Roles.Select(r => r.Nombre));

                datos.Rows.Add(
                    usuario.IdUsuario,
                    usuario.NombreUsuario,
                    personal,
                    string.IsNullOrWhiteSpace(roles) ? "Sin rol" : roles,
                    usuario.Estado ? "Activo" : "Inactivo",
                    usuario.FechaCreacion);
            }

            tabla.DataSource = datos;

            tabla.Columns["IdUsuario"].HeaderText = "ID";
            tabla.Columns["NombreUsuario"].HeaderText = "USUARIO";
            tabla.Columns["PersonalSalud"].HeaderText = "PERSONAL DE SALUD";
            tabla.Columns["Roles"].HeaderText = "ROLES";
            tabla.Columns["Estado"].HeaderText = "ESTADO";
            tabla.Columns["FechaCreacion"].HeaderText = "FECHA DE REGISTRO";
            tabla.Columns["FechaCreacion"].DefaultCellStyle.Format = "dd/MM/yyyy";

            lblTotal.Text = $"Total de usuarios: {filtrados.Count}";
        }

        private void CambiarEstado()
        {
            if (tabla.CurrentRow == null ||
                tabla.CurrentRow.Cells["IdUsuario"].Value == null)
            {
                MessageBox.Show("Seleccione un usuario.");
                return;
            }

            int id = Convert.ToInt32(
                tabla.CurrentRow.Cells["IdUsuario"].Value);

            Usuario? usuario = usuarios.FirstOrDefault(u => u.IdUsuario == id);

            if (usuario == null)
            {
                MessageBox.Show("No se encontró el usuario seleccionado.");
                return;
            }

            if (FormInicioSesion.UsuarioAutenticado?.UsuarioId == id)
            {
                MessageBox.Show(
                    "No puede desactivar la cuenta con la que inició sesión.",
                    "Usuarios",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            string accion = usuario.Estado ? "desactivar" : "activar";

            if (MessageBox.Show(
                $"¿Desea {accion} la cuenta de {usuario.NombreUsuario}?",
                "Confirmar cambio",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                usuario.Estado = !usuario.Estado;

                // Reutiliza el repositorio existente.
                // Conserva el hash y los datos actuales del usuario.
                repositorio.Actualizar(usuario);

                CargarUsuarios();

                MessageBox.Show(
                    "Estado del usuario actualizado correctamente.",
                    "Usuarios",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                usuario.Estado = !usuario.Estado;

                MessageBox.Show(
                    "No se pudo cambiar el estado: " + ex.Message,
                    "Usuarios",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }
    }
}
