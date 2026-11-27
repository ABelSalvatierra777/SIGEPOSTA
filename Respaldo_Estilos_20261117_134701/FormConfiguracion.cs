using System;
using System.Drawing;
using System.Windows.Forms;

namespace SIGEPOSTA.Presentacion.Formularios
{
    public class FormConfiguracion : Form
    {
        private TextBox txtNombre;
        private TextBox txtCodigo;
        private TextBox txtDireccion;
        private TextBox txtTelefono;
        private TextBox txtCorreo;
        private TextBox txtResponsable;
        private ComboBox cmbCategoria;
        private Label lblEstado;

        public FormConfiguracion()
        {
            ConfigurarVentana();
            CrearControles();
        }

        private void ConfigurarVentana()
        {
            Text = "Configuración institucional - SIGEPOSTA";
            Size = new Size(850, 650);
            MinimumSize = new Size(750, 550);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(245, 247, 246);
        }

        private void CrearControles()
        {
            Label titulo = new Label
            {
                Text = "Configuración institucional",
                Location = new Point(30, 25),
                AutoSize = true,
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                ForeColor = Color.FromArgb(31, 77, 54)
            };

            Label descripcion = new Label
            {
                Text = "Información general del establecimiento de salud.",
                Location = new Point(33, 70),
                AutoSize = true,
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.DimGray
            };

            Controls.Add(titulo);
            Controls.Add(descripcion);

            GroupBox grupo = new GroupBox
            {
                Text = "Datos del establecimiento",
                Location = new Point(30, 115),
                Size = new Size(770, 410),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Anchor = AnchorStyles.Top | AnchorStyles.Left |
                         AnchorStyles.Right
            };

            AgregarEtiqueta(grupo, "Nombre:", 25, 40);
            txtNombre = AgregarTexto(grupo, 190, 37, 530);
            txtNombre.Text = "Posta de Salud de Mollepata";

            AgregarEtiqueta(grupo, "Código del establecimiento:", 25, 95);
            txtCodigo = AgregarTexto(grupo, 190, 92, 250);

            AgregarEtiqueta(grupo, "Dirección:", 25, 150);
            txtDireccion = AgregarTexto(grupo, 190, 147, 530);

            AgregarEtiqueta(grupo, "Teléfono:", 25, 205);
            txtTelefono = AgregarTexto(grupo, 190, 202, 250);

            AgregarEtiqueta(grupo, "Correo institucional:", 25, 260);
            txtCorreo = AgregarTexto(grupo, 190, 257, 350);

            AgregarEtiqueta(grupo, "Responsable:", 25, 315);
            txtResponsable = AgregarTexto(grupo, 190, 312, 350);

            AgregarEtiqueta(grupo, "Categoría:", 25, 365);

            cmbCategoria = new ComboBox
            {
                Location = new Point(190, 362),
                Size = new Size(250, 28),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10)
            };

            cmbCategoria.Items.AddRange(new object[]
            {
                "I-1",
                "I-2",
                "I-3",
                "I-4"
            });

            cmbCategoria.SelectedIndex = 0;
            grupo.Controls.Add(cmbCategoria);

            Controls.Add(grupo);

            Button btnGuardar = new Button
            {
                Text = "Guardar configuración",
                Location = new Point(30, 545),
                Size = new Size(190, 40),
                BackColor = Color.FromArgb(47, 125, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            btnGuardar.Click += (s, e) => GuardarConfiguracion();

            Button btnLimpiar = new Button
            {
                Text = "Limpiar",
                Location = new Point(235, 545),
                Size = new Size(120, 40),
                BackColor = Color.Gray,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10),
                Cursor = Cursors.Hand
            };

            btnLimpiar.Click += (s, e) => LimpiarCampos();

            lblEstado = new Label
            {
                Text = "Configuración pendiente de guardar.",
                Location = new Point(375, 555),
                AutoSize = true,
                ForeColor = Color.DimGray,
                Font = new Font("Segoe UI", 9)
            };

            Controls.Add(btnGuardar);
            Controls.Add(btnLimpiar);
            Controls.Add(lblEstado);
        }

        private void AgregarEtiqueta(Control contenedor, string texto, int x, int y)
        {
            contenedor.Controls.Add(new Label
            {
                Text = texto,
                Location = new Point(x, y),
                Size = new Size(160, 25),
                Font = new Font("Segoe UI", 9)
            });
        }

        private TextBox AgregarTexto(Control contenedor, int x, int y, int ancho)
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

        private void GuardarConfiguracion()
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtDireccion.Text))
            {
                MessageBox.Show(
                    "Ingrese como mínimo el nombre y la dirección.",
                    "Validación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            MessageBox.Show(
                "Los datos fueron validados. Su almacenamiento permanente se implementará al conectar SQL Server.",
                "SIGEPOSTA",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

            lblEstado.Text = "Datos validados; pendientes de conexión con SQL Server.";
        }

        private void LimpiarCampos()
        {
            txtCodigo.Clear();
            txtDireccion.Clear();
            txtTelefono.Clear();
            txtCorreo.Clear();
            txtResponsable.Clear();
            cmbCategoria.SelectedIndex = 0;

            lblEstado.Text = "Formulario limpiado.";
        }
    }
}
