using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SIGEPOSTA.Presentacion.Estilos;
using SIGEPOSTA.Entidades.Inventario;
using SIGEPOSTA.Datos.Repositorios;
using SIGEPOSTA.Negocio.Servicios;

namespace SIGEPOSTA.Presentacion.Formularios
{
    public class FormFarmacia : Form
    {
        private readonly BindingList<MedicamentoVista> medicamentos = new();

        private readonly MedicamentoServicio servicio = new(new MedicamentoRepositorio());

        private CampoInventario txtCodigo = new();
        private CampoInventario txtNombre = new();
        private CampoInventario txtPrincipioActivo = new();
        private CampoInventario txtConcentracion = new();
        private CampoInventario txtPresentacion = new();
        private CampoInventario txtLaboratorio = new();
        private CampoInventario txtStock = new();
        private CampoInventario txtStockMinimo = new();
        private CampoInventario txtPrecio = new();
        private CampoInventario txtObservacion = new();

        private ComboBox cmbCategoria = new();
        private ComboBox cmbEstado = new();
        private DateTimePicker dtpVencimiento = new();
        private DataGridView dgvMedicamentos = new();

        private int indiceSeleccionado = -1;

        private readonly Color colorVerde = Color.FromArgb(43, 125, 91);
        private readonly Color colorVerdeOscuro = Color.FromArgb(27, 65, 54);
        private readonly Color colorFondo = Color.FromArgb(240, 244, 242);
        private readonly Color colorTexto = Color.FromArgb(28, 40, 35);
        private readonly Color colorBorde = Color.FromArgb(215, 225, 220);

        public FormFarmacia()
        {
            ConstruirFormulario();
            ConfigurarTabla();
            AplicarEstilos();
        }

        // =========================================================
        // FORMULARIO PRINCIPAL
        // =========================================================

        private void ConstruirFormulario()
        {
            Text = "SIGEPOSTA - Farmacia";
            Size = new Size(1350, 900);
            MinimumSize = new Size(1150, 750);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = colorFondo;
            Font = new Font("Segoe UI", 9F);

            Panel encabezado = new Panel
            {
                Dock = DockStyle.Top,
                Height = 105,
                BackColor = colorVerdeOscuro
            };

            encabezado.Controls.Add(new Label
            {
                Text = "GESTIÓN DE FARMACIA",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 21F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(30, 17),
                BackColor = Color.Transparent
            });

            encabezado.Controls.Add(new Label
            {
                Text = "Administración y control del catálogo de medicamentos",
                ForeColor = Color.FromArgb(205, 229, 215),
                Font = new Font("Segoe UI", 10F),
                AutoSize = true,
                Location = new Point(34, 62),
                BackColor = Color.Transparent
            });

            Panel cuerpo = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(22, 18, 22, 20),
                BackColor = colorFondo
            };

            TableLayoutPanel contenido = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 1,
                RowCount = 3,
                BackColor = colorFondo,
                Margin = new Padding(0)
            };

            contenido.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100));

            contenido.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 390));

            contenido.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 72));

            contenido.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 390));

            contenido.Controls.Add(CrearGrupoMedicamento(), 0, 0);
            contenido.Controls.Add(CrearBotonera(), 0, 1);
            contenido.Controls.Add(CrearGrupoTabla(), 0, 2);

            cuerpo.Controls.Add(contenido);

            Controls.Add(cuerpo);
            Controls.Add(encabezado);
        }

        // =========================================================
        // GRUPO DE DATOS DEL MEDICAMENTO
        // =========================================================

        private GroupBox CrearGrupoMedicamento()
        {
            GroupBox grupo = CrearGrupo(
                "INFORMACIÓN DEL MEDICAMENTO");

            TableLayoutPanel rejilla = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 6,
                RowCount = 5,
                Padding = new Padding(12, 12, 12, 8),
                BackColor = Color.White
            };

            rejilla.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 125));
            rejilla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            rejilla.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 125));
            rejilla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            rejilla.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 125));
            rejilla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));

            for (int i = 0; i < 4; i++)
                rejilla.RowStyles.Add(
                    new RowStyle(SizeType.Absolute, 57));

            rejilla.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 82));

            txtCodigo = NuevoCampo();
            txtNombre = NuevoCampo();
            txtPrincipioActivo = NuevoCampo();
            txtConcentracion = NuevoCampo();
            txtPresentacion = NuevoCampo();
            txtLaboratorio = NuevoCampo();
            txtStock = NuevoCampo();
            txtStockMinimo = NuevoCampo();
            txtPrecio = NuevoCampo();
            txtObservacion = NuevaArea();

            cmbCategoria = NuevoCombo(
                "Analgésico",
                "Antibiótico",
                "Antiinflamatorio",
                "Antipirético",
                "Antihistamínico",
                "Antiséptico",
                "Otro");

            cmbEstado = NuevoCombo(
                "Disponible",
                "Suspendido",
                "Retirado");

            dtpVencimiento = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9.5F),
                Margin = new Padding(3, 8, 12, 8)
            };

            AgregarPar(rejilla, "Código:", txtCodigo, 0, 0);
            AgregarPar(rejilla, "Nombre:", txtNombre, 2, 0);
            AgregarPar(rejilla, "Principio activo:", txtPrincipioActivo, 4, 0);

            AgregarPar(rejilla, "Concentración:", txtConcentracion, 0, 1);
            AgregarPar(rejilla, "Presentación:", txtPresentacion, 2, 1);
            AgregarPar(rejilla, "Laboratorio:", txtLaboratorio, 4, 1);

            AgregarPar(rejilla, "Categoría:", cmbCategoria, 0, 2);
            AgregarPar(rejilla, "Stock actual:", txtStock, 2, 2);
            AgregarPar(rejilla, "Stock mínimo:", txtStockMinimo, 4, 2);

            AgregarPar(rejilla, "Precio unitario:", txtPrecio, 0, 3);
            AgregarPar(rejilla, "Vencimiento:", dtpVencimiento, 2, 3);
            AgregarPar(rejilla, "Estado:", cmbEstado, 4, 3);

            Label etiquetaObservacion = NuevaEtiqueta("Observación:");
            etiquetaObservacion.Anchor = AnchorStyles.Left | AnchorStyles.Top;
            etiquetaObservacion.Margin = new Padding(3, 12, 5, 0);

            rejilla.Controls.Add(etiquetaObservacion, 0, 4);
            rejilla.Controls.Add(txtObservacion, 1, 4);
            rejilla.SetColumnSpan(txtObservacion, 5);

            grupo.Controls.Add(rejilla);

            return grupo;
        }

        // =========================================================
        // BOTONES
        // =========================================================

        private Control CrearBotonera()
        {
            TableLayoutPanel contenedor = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 1,
                BackColor = colorFondo,
                Padding = new Padding(180, 10, 180, 5)
            };

            for (int i = 0; i < 4; i++)
                contenedor.ColumnStyles.Add(
                    new ColumnStyle(SizeType.Percent, 25));

            Button btnRegistrar = NuevoBoton("REGISTRAR", colorVerde);
            Button btnActualizar = NuevoBoton("ACTUALIZAR", colorVerde);
            Button btnEliminar = NuevoBoton("ELIMINAR", Color.FromArgb(190, 55, 55));
            Button btnLimpiar = NuevoBoton("LIMPIAR", Color.FromArgb(95, 110, 103));

            btnRegistrar.Click += (s, e) => Registrar();
            btnActualizar.Click += (s, e) => Actualizar();
            btnEliminar.Click += (s, e) => Eliminar();
            btnLimpiar.Click += (s, e) => Limpiar();

            contenedor.Controls.Add(btnRegistrar, 0, 0);
            contenedor.Controls.Add(btnActualizar, 1, 0);
            contenedor.Controls.Add(btnEliminar, 2, 0);
            contenedor.Controls.Add(btnLimpiar, 3, 0);

            return contenedor;
        }

        private Button NuevoBoton(string texto, Color color)
        {
            Button boton = new Button
            {
                Text = texto,
                Dock = DockStyle.Fill,
                Height = 44,
                Margin = new Padding(7, 2, 7, 2),
                BackColor = color,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                UseVisualStyleBackColor = false
            };

            boton.FlatAppearance.BorderSize = 0;
            boton.FlatAppearance.MouseOverBackColor = ControlPaint.Light(color, 0.12F);
            boton.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(color, 0.10F);

            return boton;
        }

        // =========================================================
        // TABLA DE MEDICAMENTOS
        // =========================================================

        private GroupBox CrearGrupoTabla()
        {
            GroupBox grupo = CrearGrupo(
                "CATÁLOGO DE MEDICAMENTOS");

            dgvMedicamentos = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                AutoGenerateColumns = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                RowHeadersVisible = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                GridColor = colorBorde,
                Font = new Font("Segoe UI", 9F),
                ColumnHeadersHeight = 42,
                RowTemplate = { Height = 35 },
                Margin = new Padding(8)
            };

            dgvMedicamentos.SelectionChanged +=
                (s, e) => Seleccionar();

            grupo.Controls.Add(dgvMedicamentos);

            return grupo;
        }

        private void ConfigurarTabla()
        {
            dgvMedicamentos.Columns.Clear();

            Columna("Codigo", "Código", 12);
            Columna("Nombre", "Medicamento", 19);
            Columna("Concentracion", "Concentración", 13);
            Columna("Presentacion", "Presentación", 13);
            Columna("Stock", "Stock", 8);
            Columna("Vencimiento", "Vencimiento", 13);
            Columna("Estado", "Estado", 12);

            dgvMedicamentos.Columns["Vencimiento"]!
                .DefaultCellStyle.Format = "dd/MM/yyyy";

            dgvMedicamentos.DataSource = medicamentos;
        }

        private void Columna(string propiedad, string titulo, float peso)
        {
            dgvMedicamentos.Columns.Add(
                new DataGridViewTextBoxColumn
                {
                    Name = propiedad,
                    DataPropertyName = propiedad,
                    HeaderText = titulo,
                    FillWeight = peso,
                    SortMode = DataGridViewColumnSortMode.NotSortable
                });
        }

        // =========================================================
        // CONTROLES Y ESTILOS
        // =========================================================

        private GroupBox CrearGrupo(string titulo)
        {
            return new GroupBox
            {
                Text = titulo,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = colorVerdeOscuro,
                BackColor = Color.White,
                Padding = new Padding(12, 15, 12, 10),
                Margin = new Padding(0, 0, 0, 10)
            };
        }

        private void AgregarPar(
            TableLayoutPanel rejilla,
            string texto,
            Control campo,
            int columna,
            int fila)
        {
            rejilla.Controls.Add(NuevaEtiqueta(texto), columna, fila);

            campo.Dock = DockStyle.Fill;
            campo.Margin = new Padding(3, 7, 12, 7);

            rejilla.Controls.Add(campo, columna + 1, fila);
        }

        private Label NuevaEtiqueta(string texto)
        {
            return new Label
            {
                Text = texto,
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                ForeColor = colorTexto,
                Font = new Font("Segoe UI", 9F),
                Margin = new Padding(3, 0, 5, 0)
            };
        }

        private CampoInventario NuevoCampo()
        {
            return new CampoInventario
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(3, 6, 12, 6),
                Font = new Font("Segoe UI", 9.5F),
                BackColor = Color.FromArgb(245, 248, 246),
                ForeColor = colorTexto
            };
        }

        private CampoInventario NuevaArea()
        {
            CampoInventario campo = NuevoCampo();

            campo.ConfigurarMultilinea(true, ScrollBars.Vertical);

            return campo;
        }

        private ComboBox NuevoCombo(params string[] opciones)
        {
            ComboBox combo = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9.5F),
                BackColor = Color.White,
                ForeColor = colorTexto,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(3, 8, 12, 8)
            };

            combo.Items.AddRange(opciones);
            combo.SelectedIndex = 0;

            return combo;
        }

        private void AplicarEstilos()
        {
            BackColor = colorFondo;

            dgvMedicamentos.EnableHeadersVisualStyles = false;

            dgvMedicamentos.ColumnHeadersDefaultCellStyle.BackColor =
                colorVerdeOscuro;

            dgvMedicamentos.ColumnHeadersDefaultCellStyle.ForeColor =
                Color.White;

            dgvMedicamentos.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 9F, FontStyle.Bold);

            dgvMedicamentos.ColumnHeadersDefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleCenter;

            dgvMedicamentos.DefaultCellStyle.BackColor = Color.White;
            dgvMedicamentos.DefaultCellStyle.ForeColor = colorTexto;

            dgvMedicamentos.DefaultCellStyle.SelectionBackColor =
                Color.FromArgb(215, 237, 225);

            dgvMedicamentos.DefaultCellStyle.SelectionForeColor =
                colorTexto;

            dgvMedicamentos.AlternatingRowsDefaultCellStyle.BackColor =
                Color.FromArgb(246, 249, 247);

            dgvMedicamentos.DefaultCellStyle.Padding =
                new Padding(5, 2, 5, 2);

            dgvMedicamentos.RowHeadersDefaultCellStyle.BackColor =
                colorVerdeOscuro;
        }

        // =========================================================
        // CARGAR MEDICAMENTOS DESDE SQL SERVER
        // =========================================================

        private void CargarMedicamentos()
        {
            try
            {
                var registros = servicio.ObtenerTodos();

                medicamentos.RaiseListChangedEvents = false;
                medicamentos.Clear();

                foreach (var m in registros)
                {
                    medicamentos.Add(new MedicamentoVista
                    {
                        IdMedicamento = m.IdMedicamento,
                        Codigo = m.Codigo,
                        Nombre = m.Nombre,
                        PrincipioActivo = m.PrincipioActivo,
                        Concentracion = m.Concentracion,
                        Presentacion = m.Presentacion,
                        Laboratorio = m.Laboratorio,
                        Categoria = m.Categoria,
                        Stock = m.Stock,
                        StockMinimo = m.StockMinimo,
                        Precio = m.Precio,
                        Vencimiento = m.FechaVencimiento ?? DateTime.Today,
                        Estado = m.EstadoDescripcion,
                        Observacion = m.Observacion
                    });
                }

                medicamentos.RaiseListChangedEvents = true;
                medicamentos.ResetBindings();

                dgvMedicamentos.ClearSelection();
            }
            catch (Exception ex)
            {
                medicamentos.RaiseListChangedEvents = true;

                MessageBox.Show(
                    "No se pudieron cargar los medicamentos desde la base de datos.\n\n" + ex.Message,
                    "SIGEPOSTA - Farmacia",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =========================================================
        // CONVERTIR FORMULARIO A ENTIDAD
        // =========================================================

        private Medicamento ConvertirEntidad(MedicamentoVista vista)
        {
            return new Medicamento
            {
                IdMedicamento = vista.IdMedicamento,
                Codigo = vista.Codigo,
                Nombre = vista.Nombre,
                PrincipioActivo = vista.PrincipioActivo,
                Concentracion = vista.Concentracion,
                FormaFarmaceutica = vista.Categoria,
                Presentacion = vista.Presentacion,
                UnidadMedida = "Unidad",
                Laboratorio = vista.Laboratorio,
                Categoria = vista.Categoria,
                Stock = vista.Stock,
                StockMinimo = vista.StockMinimo,
                Precio = vista.Precio,
                FechaVencimiento = vista.Vencimiento,
                Descripcion = vista.Observacion,
                Observacion = vista.Observacion,
                EstadoDescripcion = vista.Estado,
                Estado = vista.Estado.Equals(
                    "Disponible",
                    StringComparison.OrdinalIgnoreCase)
            };
        }

        // =========================================================
        // REGISTRAR
        // =========================================================

        private void Registrar()
        {
            if (!Validar())
                return;

            string codigo = txtCodigo.Text.Trim();

            if (medicamentos.Any(m =>
                m.Codigo.Equals(
                    codigo,
                    StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show(
                    "Ya existe un medicamento con ese código.",
                    "SIGEPOSTA",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtCodigo.Focus();
                return;
            }

            try
            {
                Medicamento entidad = ConvertirEntidad(ObtenerDatos());

                servicio.Registrar(entidad);

                CargarMedicamentos();
                Limpiar();

                MessageBox.Show(
                    "Medicamento registrado correctamente en la base de datos.",
                    "SIGEPOSTA",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo registrar el medicamento.\n\n" + ex.Message,
                    "SIGEPOSTA",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =========================================================
        // ACTUALIZAR
        // =========================================================

        private void Actualizar()
        {
            if (indiceSeleccionado < 0 ||
                indiceSeleccionado >= medicamentos.Count)
            {
                MessageBox.Show("Seleccione un medicamento de la tabla.");
                return;
            }

            if (!Validar())
                return;

            int id = medicamentos[indiceSeleccionado].IdMedicamento;
            string codigo = txtCodigo.Text.Trim();

            if (medicamentos.Where((m, i) => i != indiceSeleccionado)
                .Any(m => m.Codigo.Equals(
                    codigo,
                    StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("El código ya pertenece a otro medicamento.");
                return;
            }

            try
            {
                MedicamentoVista nuevo = ObtenerDatos();
                nuevo.IdMedicamento = id;

                servicio.Actualizar(ConvertirEntidad(nuevo));

                CargarMedicamentos();
                Limpiar();

                MessageBox.Show(
                    "Medicamento actualizado correctamente.",
                    "SIGEPOSTA",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo actualizar el medicamento.\n\n" + ex.Message,
                    "SIGEPOSTA",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =========================================================
        // ELIMINAR
        // =========================================================

        private void Eliminar()
        {
            if (indiceSeleccionado < 0 ||
                indiceSeleccionado >= medicamentos.Count)
            {
                MessageBox.Show("Seleccione un medicamento de la tabla.");
                return;
            }

            DialogResult respuesta = MessageBox.Show(
                "¿Desea retirar este medicamento del catálogo?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (respuesta != DialogResult.Yes)
                return;

            try
            {
                int id = medicamentos[indiceSeleccionado].IdMedicamento;

                servicio.Eliminar(id);

                CargarMedicamentos();
                Limpiar();

                MessageBox.Show(
                    "Medicamento retirado correctamente.",
                    "SIGEPOSTA",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo retirar el medicamento.\n\n" + ex.Message,
                    "SIGEPOSTA",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        // =========================================================
        // VALIDACIONES
        // =========================================================

        private bool Validar()
        {
            if (string.IsNullOrWhiteSpace(txtCodigo.Text) ||
                string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtPrincipioActivo.Text) ||
                string.IsNullOrWhiteSpace(txtConcentracion.Text) ||
                string.IsNullOrWhiteSpace(txtPresentacion.Text))
            {
                MessageBox.Show(
                    "Complete los campos obligatorios.",
                    "Validación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return false;
            }

            if (!int.TryParse(txtStock.Text, out int stock) || stock < 0)
            {
                MessageBox.Show("Ingrese un stock válido.");
                txtStock.Focus();
                return false;
            }

            if (!int.TryParse(txtStockMinimo.Text, out int minimo) ||
                minimo < 0)
            {
                MessageBox.Show("Ingrese un stock mínimo válido.");
                txtStockMinimo.Focus();
                return false;
            }

            if (!decimal.TryParse(txtPrecio.Text, out decimal precio) ||
                precio < 0)
            {
                MessageBox.Show("Ingrese un precio válido.");
                txtPrecio.Focus();
                return false;
            }

            if (cmbCategoria.SelectedItem == null ||
                cmbEstado.SelectedItem == null)
            {
                MessageBox.Show("Seleccione una categoría y un estado.");
                return false;
            }

            return true;
        }

        // =========================================================
        // OBTENER DATOS
        // =========================================================

        private MedicamentoVista ObtenerDatos()
        {
            return new MedicamentoVista
            {
                Codigo = txtCodigo.Text.Trim(),
                Nombre = txtNombre.Text.Trim(),
                PrincipioActivo = txtPrincipioActivo.Text.Trim(),
                Concentracion = txtConcentracion.Text.Trim(),
                Presentacion = txtPresentacion.Text.Trim(),
                Laboratorio = txtLaboratorio.Text.Trim(),
                Categoria = cmbCategoria.Text,
                Stock = int.Parse(txtStock.Text),
                StockMinimo = int.Parse(txtStockMinimo.Text),
                Precio = decimal.Parse(txtPrecio.Text),
                Vencimiento = dtpVencimiento.Value.Date,
                Estado = cmbEstado.Text,
                Observacion = txtObservacion.Text.Trim()
            };
        }

        // =========================================================
        // LIMPIAR
        // =========================================================

        private void Limpiar()
        {
            indiceSeleccionado = -1;

            txtCodigo.Clear();
            txtNombre.Clear();
            txtPrincipioActivo.Clear();
            txtConcentracion.Clear();
            txtPresentacion.Clear();
            txtLaboratorio.Clear();
            txtStock.Clear();
            txtStockMinimo.Clear();
            txtPrecio.Clear();
            txtObservacion.Clear();

            cmbCategoria.SelectedIndex = 0;
            cmbEstado.SelectedIndex = 0;
            dtpVencimiento.Value = DateTime.Today;

            dgvMedicamentos.ClearSelection();
        }

        // =========================================================
        // MODELO
        // =========================================================

        private class MedicamentoVista
        {
            public int IdMedicamento { get; set; }
            public string Codigo { get; set; } = "";
            public string Nombre { get; set; } = "";
            public string PrincipioActivo { get; set; } = "";
            public string Concentracion { get; set; } = "";
            public string Presentacion { get; set; } = "";
            public string Laboratorio { get; set; } = "";
            public string Categoria { get; set; } = "";
            public int Stock { get; set; }
            public int StockMinimo { get; set; }
            public decimal Precio { get; set; }
            public DateTime Vencimiento { get; set; }
            public string Estado { get; set; } = "";
            public string Observacion { get; set; } = "";
        }
        // =========================================================
        // SELECCIONAR MEDICAMENTO
        // =========================================================

        private void Seleccionar()
        {
            if (dgvMedicamentos.CurrentRow?.DataBoundItem
                is not MedicamentoVista m)
                return;

            indiceSeleccionado = medicamentos.IndexOf(m);

            txtCodigo.Text = m.Codigo;
            txtNombre.Text = m.Nombre;
            txtPrincipioActivo.Text = m.PrincipioActivo;
            txtConcentracion.Text = m.Concentracion;
            txtPresentacion.Text = m.Presentacion;
            txtLaboratorio.Text = m.Laboratorio;

            cmbCategoria.SelectedItem = m.Categoria;

            txtStock.Text = m.Stock.ToString();
            txtStockMinimo.Text = m.StockMinimo.ToString();
            txtPrecio.Text = m.Precio.ToString("0.00");

            dtpVencimiento.Value =
                m.Vencimiento < dtpVencimiento.MinDate ||
                m.Vencimiento > dtpVencimiento.MaxDate
                    ? DateTime.Today
                    : m.Vencimiento;

            cmbEstado.SelectedItem = m.Estado;
            txtObservacion.Text = m.Observacion ?? string.Empty;
        }
    }
}


