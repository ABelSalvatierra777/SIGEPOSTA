using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SIGEPOSTA.Presentacion.Formularios
{
    public class FormFarmacia : Form
    {
        private readonly BindingList<MedicamentoVista> medicamentos = new();

        private TextBox txtCodigo = new();
        private TextBox txtNombre = new();
        private TextBox txtPrincipioActivo = new();
        private TextBox txtConcentracion = new();
        private TextBox txtPresentacion = new();
        private TextBox txtLaboratorio = new();
        private TextBox txtStock = new();
        private TextBox txtStockMinimo = new();
        private TextBox txtPrecio = new();

        private ComboBox cmbCategoria = new();
        private ComboBox cmbEstado = new();

        private DateTimePicker dtpVencimiento = new();

        private DataGridView dgvMedicamentos = new();

        private int indiceSeleccionado = -1;

        public FormFarmacia()
        {
            ConstruirFormulario();
            ConfigurarTabla();
        }

        private void ConstruirFormulario()
        {
            Text = "SIGEPOSTA - Farmacia";
            Size = new Size(1200, 780);
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
                Text = "GESTIÓN DE FARMACIA",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 19, FontStyle.Bold),
                Location = new Point(25, 18),
                AutoSize = true
            });

            Controls.Add(encabezado);

            Controls.Add(new Label
            {
                Text = "Administración del catálogo de medicamentos",
                Location = new Point(25, 95),
                AutoSize = true,
                ForeColor = Color.DimGray
            });

            GroupBox grupo = new GroupBox
            {
                Text = "Información del medicamento",
                Location = new Point(25, 125),
                Size = new Size(1135, 390)
            };

            Controls.Add(grupo);

            Etiqueta(grupo, "Código:", 20, 35);
            txtCodigo = Campo(grupo, 140, 32, 200);

            Etiqueta(grupo, "Nombre:", 370, 35);
            txtNombre = Campo(grupo, 460, 32, 250);

            Etiqueta(grupo, "Principio activo:", 740, 35);
            txtPrincipioActivo = Campo(grupo, 875, 32, 220);

            Etiqueta(grupo, "Concentración:", 20, 90);
            txtConcentracion = Campo(grupo, 140, 87, 200);

            Etiqueta(grupo, "Presentación:", 370, 90);
            txtPresentacion = Campo(grupo, 460, 87, 250);

            Etiqueta(grupo, "Laboratorio:", 740, 90);
            txtLaboratorio = Campo(grupo, 875, 87, 220);

            Etiqueta(grupo, "Categoría:", 20, 145);

            cmbCategoria = Lista(grupo, 140, 142, 200,
                "Analgésico",
                "Antibiótico",
                "Antiinflamatorio",
                "Antipirético",
                "Antihistamínico",
                "Antiséptico",
                "Otro"
            );

            Etiqueta(grupo, "Stock actual:", 370, 145);
            txtStock = Campo(grupo, 460, 142, 250);

            Etiqueta(grupo, "Stock mínimo:", 740, 145);
            txtStockMinimo = Campo(grupo, 875, 142, 220);

            Etiqueta(grupo, "Precio unitario:", 20, 200);
            txtPrecio = Campo(grupo, 140, 197, 200);

            Etiqueta(grupo, "Vencimiento:", 370, 200);

            dtpVencimiento = new DateTimePicker
            {
                Location = new Point(460, 197),
                Width = 250,
                Format = DateTimePickerFormat.Short
            };

            grupo.Controls.Add(dtpVencimiento);

            Etiqueta(grupo, "Estado:", 740, 200);

            cmbEstado = Lista(grupo, 875, 197, 220,
                "Disponible",
                "Suspendido",
                "Retirado"
            );

            Etiqueta(grupo, "Observación:", 20, 255);

            TextBox txtObservacion = AreaTexto(grupo, 140, 252, 955, 55);
            txtObservacion.Name = "txtObservacion";

            Button btnRegistrar = Boton("REGISTRAR", 25, 535, Color.FromArgb(46, 125, 50));
            btnRegistrar.Click += (s, e) => Registrar();
            Controls.Add(btnRegistrar);

            Button btnActualizar = Boton("ACTUALIZAR", 190, 535, Color.FromArgb(25, 118, 210));
            btnActualizar.Click += (s, e) => Actualizar();
            Controls.Add(btnActualizar);

            Button btnEliminar = Boton("ELIMINAR", 355, 535, Color.FromArgb(198, 40, 40));
            btnEliminar.Click += (s, e) => Eliminar();
            Controls.Add(btnEliminar);

            Button btnLimpiar = Boton("LIMPIAR", 520, 535, Color.FromArgb(97, 97, 97));
            btnLimpiar.Click += (s, e) => Limpiar();
            Controls.Add(btnLimpiar);

            GroupBox grupoTabla = new GroupBox
            {
                Text = "Catálogo de medicamentos",
                Location = new Point(25, 590),
                Size = new Size(1135, 145)
            };

            dgvMedicamentos = new DataGridView
            {
                Location = new Point(12, 25),
                Size = new Size(1110, 105),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White,
                RowHeadersVisible = false
            };

            dgvMedicamentos.SelectionChanged += (s, e) => Seleccionar();

            grupoTabla.Controls.Add(dgvMedicamentos);
            Controls.Add(grupoTabla);
        }

        private void ConfigurarTabla()
        {
            dgvMedicamentos.AutoGenerateColumns = false;

            Columna("Codigo", "Código");
            Columna("Nombre", "Medicamento");
            Columna("Concentracion", "Concentración");
            Columna("Presentacion", "Presentación");
            Columna("Stock", "Stock");
            Columna("Vencimiento", "Vencimiento");
            Columna("Estado", "Estado");

            dgvMedicamentos.DataSource = medicamentos;
        }

        private void Columna(string propiedad, string titulo)
        {
            dgvMedicamentos.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = propiedad,
                HeaderText = titulo
            });
        }

        private void Registrar()
        {
            if (!Validar())
                return;

            if (medicamentos.Any(m =>
                m.Codigo.Equals(txtCodigo.Text.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("Ya existe un medicamento con ese código.");
                return;
            }

            medicamentos.Add(ObtenerDatos());

            MessageBox.Show("Medicamento registrado correctamente.");
            Limpiar();
        }

        private void Actualizar()
        {
            if (indiceSeleccionado < 0 || indiceSeleccionado >= medicamentos.Count)
            {
                MessageBox.Show("Seleccione un medicamento.");
                return;
            }

            if (!Validar())
                return;

            string codigo = txtCodigo.Text.Trim();

            if (medicamentos.Where((m, i) => i != indiceSeleccionado)
                .Any(m => m.Codigo.Equals(codigo, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("El código ya pertenece a otro medicamento.");
                return;
            }

            MedicamentoVista nuevo = ObtenerDatos();
            MedicamentoVista anterior = medicamentos[indiceSeleccionado];

            anterior.Codigo = nuevo.Codigo;
            anterior.Nombre = nuevo.Nombre;
            anterior.PrincipioActivo = nuevo.PrincipioActivo;
            anterior.Concentracion = nuevo.Concentracion;
            anterior.Presentacion = nuevo.Presentacion;
            anterior.Laboratorio = nuevo.Laboratorio;
            anterior.Categoria = nuevo.Categoria;
            anterior.Stock = nuevo.Stock;
            anterior.StockMinimo = nuevo.StockMinimo;
            anterior.Precio = nuevo.Precio;
            anterior.Vencimiento = nuevo.Vencimiento;
            anterior.Estado = nuevo.Estado;
            anterior.Observacion = nuevo.Observacion;

            dgvMedicamentos.Refresh();

            MessageBox.Show("Medicamento actualizado correctamente.");
            Limpiar();
        }

        private void Eliminar()
        {
            if (indiceSeleccionado < 0 || indiceSeleccionado >= medicamentos.Count)
            {
                MessageBox.Show("Seleccione un medicamento.");
                return;
            }

            DialogResult respuesta = MessageBox.Show(
                "¿Desea eliminar este medicamento del catálogo?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (respuesta == DialogResult.Yes)
            {
                medicamentos.RemoveAt(indiceSeleccionado);

                MessageBox.Show("Medicamento eliminado.");
                Limpiar();
            }
        }

        private void Seleccionar()
        {
            if (dgvMedicamentos.CurrentRow?.DataBoundItem is not MedicamentoVista m)
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
            dtpVencimiento.Value = m.Vencimiento;
            cmbEstado.SelectedItem = m.Estado;

            if (Controls.Find("txtObservacion", true)
    .FirstOrDefault() is TextBox txtObs)
{
    txtObs.Text = m.Observacion ?? string.Empty;
}
        }

        private bool Validar()
        {
            if (string.IsNullOrWhiteSpace(txtCodigo.Text) ||
                string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtPrincipioActivo.Text) ||
                string.IsNullOrWhiteSpace(txtConcentracion.Text) ||
                string.IsNullOrWhiteSpace(txtPresentacion.Text))
            {
                MessageBox.Show("Complete los campos obligatorios.");
                return false;
            }

            if (!int.TryParse(txtStock.Text, out int stock) || stock < 0)
            {
                MessageBox.Show("Ingrese un stock válido.");
                return false;
            }

            if (!int.TryParse(txtStockMinimo.Text, out int minimo) || minimo < 0)
            {
                MessageBox.Show("Ingrese un stock mínimo válido.");
                return false;
            }

            if (!decimal.TryParse(txtPrecio.Text, out decimal precio) || precio < 0)
            {
                MessageBox.Show("Ingrese un precio válido.");
                return false;
            }

            if (cmbCategoria.SelectedItem == null || cmbEstado.SelectedItem == null)
            {
                MessageBox.Show("Seleccione una categoría y un estado.");
                return false;
            }

            return true;
        }

        private MedicamentoVista ObtenerDatos()
        {
            TextBox txtObs = Controls.Find("txtObservacion", true)
                .FirstOrDefault() as TextBox ?? new TextBox();

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
                Observacion = txtObs.Text.Trim()
            };
        }

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

            if (Controls.Find("txtObservacion", true)
                .FirstOrDefault() is TextBox txtObs)
                txtObs.Clear();

            cmbCategoria.SelectedIndex = 0;
            cmbEstado.SelectedIndex = 0;
            dtpVencimiento.Value = DateTime.Today;

            dgvMedicamentos.ClearSelection();
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

        private ComboBox Lista(Control padre, int x, int y, int ancho, params string[] opciones)
        {
            ComboBox combo = new ComboBox
            {
                Location = new Point(x, y),
                Width = ancho,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            combo.Items.AddRange(opciones);
            combo.SelectedIndex = 0;
            padre.Controls.Add(combo);

            return combo;
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

        private class MedicamentoVista
        {
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
    }
}

