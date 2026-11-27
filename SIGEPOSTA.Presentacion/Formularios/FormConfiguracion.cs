using System;
using System.Drawing;
using System.Windows.Forms;
using SIGEPOSTA.Presentacion.Estilos;
using SIGEPOSTA.Negocio.Servicios;

namespace SIGEPOSTA.Presentacion.Formularios
{
    public class FormConfiguracion : Form
    {
        private TextBox txtNombre = new();
        private TextBox txtCodigo = new();
        private TextBox txtDireccion = new();
        private TextBox txtTelefono = new();
        private TextBox txtCorreo = new();
        private TextBox txtResponsable = new();

        private ComboBox cmbCategoria = new();
        private Label lblEstado = new();
        private readonly ConfiguracionServicio servicio = new();

        public FormConfiguracion()
        {
            ConstruirFormulario();
            AplicarEstilos();
            CargarConfiguracion();
        }

        private void ConstruirFormulario()
        {
            Text = "SIGEPOSTA - Configuración institucional";
            Size = new Size(1250, 820);
            MinimumSize = new Size(1050, 700);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = InventarioEstilo.Fondo;
            Font = new Font("Segoe UI", 9.5F);
            DoubleBuffered = true;

            Panel encabezado = new Panel
            {
                Dock = DockStyle.Top,
                Height = 110,
                BackColor = InventarioEstilo.VerdeOscuro,
                Padding = new Padding(30, 12, 30, 12)
            };

            TableLayoutPanel contenidoEncabezado = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                BackColor = InventarioEstilo.VerdeOscuro,
                Margin = new Padding(0)
            };

            contenidoEncabezado.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100));
            contenidoEncabezado.RowStyles.Add(
                new RowStyle(SizeType.Percent, 58));
            contenidoEncabezado.RowStyles.Add(
                new RowStyle(SizeType.Percent, 42));

            Label titulo = new Label
            {
                Text = "CONFIGURACIÓN INSTITUCIONAL",
                Dock = DockStyle.Fill,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 21F, FontStyle.Bold),
                TextAlign = ContentAlignment.BottomLeft,
                BackColor = Color.Transparent,
                Margin = new Padding(0)
            };

            Label subtitulo = new Label
            {
                Text = "Administración de la información general del establecimiento de salud",
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

            Panel cuerpo = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(28, 35, 28, 25),
                BackColor = InventarioEstilo.Fondo
            };

            TableLayoutPanel columna = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 1,
                RowCount = 2,
                BackColor = InventarioEstilo.Fondo,
                Margin = new Padding(0)
            };

            columna.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100));
            columna.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 390));
            columna.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 100));

            columna.Controls.Add(CrearTarjetaDatos(), 0, 0);
            columna.Controls.Add(CrearBotonera(), 0, 1);

            cuerpo.Controls.Add(columna);

            Controls.Add(cuerpo);
            Controls.Add(encabezado);
        }

        private Control CrearTarjetaDatos()
        {
            TarjetaInventario tarjeta = new TarjetaInventario
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(24, 18, 24, 20),
                BackColor = Color.White,
                Margin = new Padding(0, 0, 0, 15)
            };

            TableLayoutPanel contenido = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                BackColor = Color.White,
                Margin = new Padding(0)
            };

            contenido.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100));
            contenido.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 45));
            contenido.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100));

            Label titulo = new Label
            {
                Text = "DATOS DEL ESTABLECIMIENTO DE SALUD",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = InventarioEstilo.VerdeOscuro,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(0)
            };

            TableLayoutPanel rejilla = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 4,
                BackColor = Color.White,
                Margin = new Padding(0),
                Padding = new Padding(0, 8, 0, 0)
            };

            rejilla.ColumnStyles.Add(
                new ColumnStyle(SizeType.Absolute, 155));
            rejilla.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 50));
            rejilla.ColumnStyles.Add(
                new ColumnStyle(SizeType.Absolute, 155));
            rejilla.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 50));

            for (int i = 0; i < 4; i++)
                rejilla.RowStyles.Add(
                    new RowStyle(SizeType.Absolute, 65));

            txtNombre = NuevoCampo();
            txtNombre.Text = "Posta de Salud de Mollepata";

            txtCodigo = NuevoCampo();
            txtDireccion = NuevoCampo();
            txtTelefono = NuevoCampo();
            txtCorreo = NuevoCampo();
            txtResponsable = NuevoCampo();

            cmbCategoria = new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10F),
                BackColor = InventarioEstilo.Campo,
                ForeColor = InventarioEstilo.Texto,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(3, 7, 15, 7)
            };

            cmbCategoria.Items.AddRange(new object[]
            {
                "I-1",
                "I-2",
                "I-3",
                "I-4"
            });

            cmbCategoria.SelectedIndex = 0;

            AgregarCampo(rejilla, "Nombre:", txtNombre, 0, 0);
            AgregarCampo(rejilla, "Código:", txtCodigo, 2, 0);

            AgregarCampo(rejilla, "Dirección:", txtDireccion, 0, 1);
            AgregarCampo(rejilla, "Teléfono:", txtTelefono, 2, 1);

            AgregarCampo(rejilla, "Correo institucional:", txtCorreo, 0, 2);
            AgregarCampo(rejilla, "Responsable:", txtResponsable, 2, 2);

            AgregarCampo(rejilla, "Categoría:", cmbCategoria, 0, 3);

            contenido.Controls.Add(titulo, 0, 0);
            contenido.Controls.Add(rejilla, 0, 1);

            tarjeta.Controls.Add(contenido);
            return tarjeta;
        }

        private Control CrearBotonera()
        {
            Panel panel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = InventarioEstilo.Fondo,
                Margin = new Padding(0)
            };

            FlowLayoutPanel botones = new FlowLayoutPanel
            {
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                BackColor = InventarioEstilo.Fondo,
                Margin = new Padding(0)
            };

            BotonInventario btnGuardar = new BotonInventario(
                "GUARDAR CONFIGURACIÓN", TipoBoton.Primario)
            {
                Size = new Size(220, 46),
                Margin = new Padding(7, 5, 7, 5)
            };

            btnGuardar.Click += (s, e) => GuardarConfiguracion();

            BotonInventario btnLimpiar = new BotonInventario(
                "LIMPIAR", TipoBoton.Secundario)
            {
                Size = new Size(145, 46),
                Margin = new Padding(7, 5, 7, 5)
            };

            btnLimpiar.Click += (s, e) => LimpiarCampos();

            botones.Controls.Add(btnGuardar);
            botones.Controls.Add(btnLimpiar);

            lblEstado = new Label
            {
                Text = "Configuración pendiente de guardar.",
                AutoSize = true,
                ForeColor = InventarioEstilo.Texto,
                Font = new Font("Segoe UI", 9F),
                BackColor = InventarioEstilo.Fondo,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(15, 18, 0, 0)
            };

            botones.Controls.Add(lblEstado);

            panel.Controls.Add(botones);

            void Centrar()
            {
                botones.Left = Math.Max(
                    0, (panel.ClientSize.Width - botones.Width) / 2);
                botones.Top = Math.Max(
                    0, (panel.ClientSize.Height - botones.Height) / 2);
            }

            panel.Resize += (s, e) => Centrar();
            botones.SizeChanged += (s, e) => Centrar();

            return panel;
        }

        private void AgregarCampo(
            TableLayoutPanel rejilla,
            string texto,
            Control campo,
            int columna,
            int fila)
        {
            Label etiqueta = new Label
            {
                Text = texto,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = InventarioEstilo.Texto,
                Font = new Font("Segoe UI", 9.5F),
                Margin = new Padding(3, 0, 5, 0)
            };

            rejilla.Controls.Add(etiqueta, columna, fila);
            rejilla.Controls.Add(campo, columna + 1, fila);
        }

        private TextBox NuevoCampo()
        {
            return new TextBox
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = InventarioEstilo.Campo,
                ForeColor = InventarioEstilo.Texto,
                Font = new Font("Segoe UI", 10F),
                Margin = new Padding(3, 7, 15, 7)
            };
        }

        private void AplicarEstilos()
        {
            foreach (TextBox caja in TodosLosControles<TextBox>(this))
            {
                caja.BackColor = InventarioEstilo.Campo;
                caja.ForeColor = InventarioEstilo.Texto;
                caja.Font = new Font("Segoe UI", 10F);
                caja.BorderStyle = BorderStyle.FixedSingle;
            }
        }

        private static System.Collections.Generic.IEnumerable<T>
            TodosLosControles<T>(Control raiz) where T : Control
        {
            foreach (Control control in raiz.Controls)
            {
                if (control is T tipado)
                    yield return tipado;

                foreach (T hijo in TodosLosControles<T>(control))
                    yield return hijo;
            }
        }

        private void CargarConfiguracion()
        {
            try
            {
                var datos = servicio.ObtenerConfiguracion();

                txtNombre.Text = ObtenerValor(datos, "NombreEstablecimiento");
                txtCodigo.Text = ObtenerValor(datos, "CodigoEstablecimiento");
                txtDireccion.Text = ObtenerValor(datos, "Direccion");
                txtTelefono.Text = ObtenerValor(datos, "Telefono");
                txtCorreo.Text = ObtenerValor(datos, "Correo");
                txtResponsable.Text = ObtenerValor(datos, "Responsable");

                string categoria = ObtenerValor(datos, "Categoria");

                if (!string.IsNullOrWhiteSpace(categoria))
                {
                    int indice = cmbCategoria.Items.IndexOf(categoria);

                    if (indice >= 0)
                        cmbCategoria.SelectedIndex = indice;
                }

                lblEstado.Text = datos.Count > 0
                    ? "Configuración institucional cargada."
                    : "No existen datos guardados. Complete el formulario.";
            }
            catch (Exception ex)
            {
                lblEstado.Text = "No se pudo cargar la configuración.";

                MessageBox.Show(
                    "No fue posible cargar la configuración desde SQL Server.\n\n" + ex.Message,
                    "Error de configuración",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private static string ObtenerValor(
            System.Collections.Generic.Dictionary<string, string> datos,
            string clave)
        {
            return datos.TryGetValue(clave, out string? valor)
                ? valor
                : "";
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
                    MessageBoxIcon.Warning);

                return;
            }

            var datos = new System.Collections.Generic.Dictionary<string, string>
            {
                ["NombreEstablecimiento"] = txtNombre.Text.Trim(),
                ["CodigoEstablecimiento"] = txtCodigo.Text.Trim(),
                ["Direccion"] = txtDireccion.Text.Trim(),
                ["Telefono"] = txtTelefono.Text.Trim(),
                ["Correo"] = txtCorreo.Text.Trim(),
                ["Responsable"] = txtResponsable.Text.Trim(),
                ["Categoria"] = cmbCategoria.SelectedItem?.ToString() ?? "I-1"
            };

            try
            {
                servicio.GuardarConfiguracion(datos);

                lblEstado.Text = "Configuración guardada correctamente.";

                MessageBox.Show(
                    "Los datos institucionales se guardaron correctamente en SQL Server.",
                    "SIGEPOSTA",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                lblEstado.Text = "Error al guardar la configuración.";

                MessageBox.Show(
                    "No fue posible guardar los datos.\n\n" + ex.Message,
                    "Error de almacenamiento",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
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

