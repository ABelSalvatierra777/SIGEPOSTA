using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SIGEPOSTA.Presentacion.Formularios
{
    public class FormInventario : Form
    {
        private readonly BindingList<MovimientoVista> movimientos = new();

        private TextBox txtCodigoMedicamento = new();
        private TextBox txtMedicamento = new();
        private TextBox txtCantidad = new();
        private TextBox txtResponsable = new();
        private TextBox txtDocumento = new();
        private TextBox txtObservacion = new();

        private ComboBox cmbTipoMovimiento = new();
        private ComboBox cmbMotivo = new();

        private DateTimePicker dtpFecha = new();

        private DataGridView dgvMovimientos = new();

        private int indiceSeleccionado = -1;

        public FormInventario()
        {
            ConstruirFormulario();
            ConfigurarTabla();
        }

        private void ConstruirFormulario()
        {
            Text = "SIGEPOSTA - Inventario";
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
                Text = "CONTROL DE INVENTARIO",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 19, FontStyle.Bold),
                Location = new Point(25, 18),
                AutoSize = true
            });

            Controls.Add(encabezado);

            Controls.Add(new Label
            {
                Text = "Registro y seguimiento de movimientos de existencias",
                Location = new Point(25, 95),
                AutoSize = true,
                ForeColor = Color.DimGray
            });

            GroupBox grupo = new GroupBox
            {
                Text = "Información del movimiento",
                Location = new Point(25, 125),
                Size = new Size(1135, 300)
            };

            Controls.Add(grupo);

            Etiqueta(grupo, "Código medicamento:", 20, 35);
            txtCodigoMedicamento = Campo(grupo, 165, 32, 175);

            Etiqueta(grupo, "Medicamento:", 370, 35);
            txtMedicamento = Campo(grupo, 475, 32, 250);

            Etiqueta(grupo, "Tipo movimiento:", 750, 35);

            cmbTipoMovimiento = Lista(grupo, 875, 32, 220,
                "Entrada",
                "Salida",
                "Ajuste positivo",
                "Ajuste negativo"
            );

            Etiqueta(grupo, "Cantidad:", 20, 90);
            txtCantidad = Campo(grupo, 165, 87, 175);

            Etiqueta(grupo, "Fecha:", 370, 90);

            dtpFecha = new DateTimePicker
            {
                Location = new Point(475, 87),
                Width = 250,
                Format = DateTimePickerFormat.Short,
                MaxDate = DateTime.Today
            };

            grupo.Controls.Add(dtpFecha);

            Etiqueta(grupo, "Motivo:", 750, 90);

            cmbMotivo = Lista(grupo, 875, 87, 220,
                "Compra",
                "Reposición",
                "Consumo médico",
                "Entrega a paciente",
                "Vencimiento",
                "Deterioro",
                "Corrección de inventario",
                "Otro"
            );

            Etiqueta(grupo, "Responsable:", 20, 145);
            txtResponsable = Campo(grupo, 165, 142, 175);

            Etiqueta(grupo, "Documento:", 370, 145);
            txtDocumento = Campo(grupo, 475, 142, 250);

            Etiqueta(grupo, "Observación:", 20, 200);
            txtObservacion = AreaTexto(grupo, 165, 197, 930, 55);

            Button btnRegistrar = Boton("REGISTRAR", 25, 445, Color.FromArgb(46, 125, 50));
            btnRegistrar.Click += (s, e) => Registrar();
            Controls.Add(btnRegistrar);

            Button btnActualizar = Boton("ACTUALIZAR", 190, 445, Color.FromArgb(25, 118, 210));
            btnActualizar.Click += (s, e) => Actualizar();
            Controls.Add(btnActualizar);

            Button btnEliminar = Boton("ELIMINAR", 355, 445, Color.FromArgb(198, 40, 40));
            btnEliminar.Click += (s, e) => Eliminar();
            Controls.Add(btnEliminar);

            Button btnLimpiar = Boton("LIMPIAR", 520, 445, Color.FromArgb(97, 97, 97));
            btnLimpiar.Click += (s, e) => Limpiar();
            Controls.Add(btnLimpiar);

            GroupBox grupoTabla = new GroupBox
            {
                Text = "Historial de movimientos",
                Location = new Point(25, 505),
                Size = new Size(1135, 205)
            };

            dgvMovimientos = new DataGridView
            {
                Location = new Point(12, 25),
                Size = new Size(1110, 165),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White,
                RowHeadersVisible = false
            };

            dgvMovimientos.SelectionChanged += (s, e) => Seleccionar();

            grupoTabla.Controls.Add(dgvMovimientos);
            Controls.Add(grupoTabla);
        }

        private void ConfigurarTabla()
        {
            dgvMovimientos.AutoGenerateColumns = false;

            Columna("CodigoMedicamento", "Código");
            Columna("Medicamento", "Medicamento");
            Columna("Tipo", "Movimiento");
            Columna("Cantidad", "Cantidad");
            Columna("Fecha", "Fecha");
            Columna("Motivo", "Motivo");
            Columna("Responsable", "Responsable");

            dgvMovimientos.DataSource = movimientos;
        }

        private void Columna(string propiedad, string titulo)
        {
            dgvMovimientos.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = propiedad,
                HeaderText = titulo
            });
        }

        private void Registrar()
        {
            if (!Validar())
                return;

            MovimientoVista movimiento = ObtenerDatos();

            if (movimiento.Tipo == "Salida" ||
                movimiento.Tipo == "Ajuste negativo")
            {
                MessageBox.Show(
                    "El registro definitivo de salidas y ajustes requiere verificar las existencias del medicamento. Esa validación se integrará con la base de datos."
                );
            }

            movimientos.Add(movimiento);

            MessageBox.Show("Movimiento registrado en el historial temporal.");
            Limpiar();
        }

        private void Actualizar()
        {
            if (indiceSeleccionado < 0 || indiceSeleccionado >= movimientos.Count)
            {
                MessageBox.Show("Seleccione un movimiento.");
                return;
            }

            if (!Validar())
                return;

            MovimientoVista nuevo = ObtenerDatos();
            MovimientoVista anterior = movimientos[indiceSeleccionado];

            anterior.CodigoMedicamento = nuevo.CodigoMedicamento;
            anterior.Medicamento = nuevo.Medicamento;
            anterior.Tipo = nuevo.Tipo;
            anterior.Cantidad = nuevo.Cantidad;
            anterior.Fecha = nuevo.Fecha;
            anterior.Motivo = nuevo.Motivo;
            anterior.Responsable = nuevo.Responsable;
            anterior.Documento = nuevo.Documento;
            anterior.Observacion = nuevo.Observacion;

            dgvMovimientos.Refresh();

            MessageBox.Show("Movimiento actualizado.");
            Limpiar();
        }

        private void Eliminar()
        {
            if (indiceSeleccionado < 0 || indiceSeleccionado >= movimientos.Count)
            {
                MessageBox.Show("Seleccione un movimiento.");
                return;
            }

            DialogResult respuesta = MessageBox.Show(
                "¿Desea eliminar este registro del historial temporal?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (respuesta == DialogResult.Yes)
            {
                movimientos.RemoveAt(indiceSeleccionado);

                MessageBox.Show("Registro eliminado.");
                Limpiar();
            }
        }

        private void Seleccionar()
        {
            if (dgvMovimientos.CurrentRow?.DataBoundItem is not MovimientoVista m)
                return;

            indiceSeleccionado = movimientos.IndexOf(m);

            txtCodigoMedicamento.Text = m.CodigoMedicamento;
            txtMedicamento.Text = m.Medicamento;
            cmbTipoMovimiento.SelectedItem = m.Tipo;
            txtCantidad.Text = m.Cantidad.ToString();
            dtpFecha.Value = m.Fecha;
            cmbMotivo.SelectedItem = m.Motivo;
            txtResponsable.Text = m.Responsable;
            txtDocumento.Text = m.Documento;
            txtObservacion.Text = m.Observacion;
        }

        private bool Validar()
        {
            if (string.IsNullOrWhiteSpace(txtCodigoMedicamento.Text) ||
                string.IsNullOrWhiteSpace(txtMedicamento.Text) ||
                string.IsNullOrWhiteSpace(txtResponsable.Text))
            {
                MessageBox.Show("Complete los campos obligatorios.");
                return false;
            }

            if (!int.TryParse(txtCantidad.Text, out int cantidad) || cantidad <= 0)
            {
                MessageBox.Show("La cantidad debe ser un número mayor que cero.");
                return false;
            }

            if (cmbTipoMovimiento.SelectedItem == null ||
                cmbMotivo.SelectedItem == null)
            {
                MessageBox.Show("Seleccione el tipo de movimiento y el motivo.");
                return false;
            }

            return true;
        }

        private MovimientoVista ObtenerDatos()
        {
            return new MovimientoVista
            {
                CodigoMedicamento = txtCodigoMedicamento.Text.Trim(),
                Medicamento = txtMedicamento.Text.Trim(),
                Tipo = cmbTipoMovimiento.Text,
                Cantidad = int.Parse(txtCantidad.Text),
                Fecha = dtpFecha.Value.Date,
                Motivo = cmbMotivo.Text,
                Responsable = txtResponsable.Text.Trim(),
                Documento = txtDocumento.Text.Trim(),
                Observacion = txtObservacion.Text.Trim()
            };
        }

        private void Limpiar()
        {
            indiceSeleccionado = -1;

            txtCodigoMedicamento.Clear();
            txtMedicamento.Clear();
            txtCantidad.Clear();
            txtResponsable.Clear();
            txtDocumento.Clear();
            txtObservacion.Clear();

            cmbTipoMovimiento.SelectedIndex = 0;
            cmbMotivo.SelectedIndex = 0;
            dtpFecha.Value = DateTime.Today;

            dgvMovimientos.ClearSelection();
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

        private class MovimientoVista
        {
            public string CodigoMedicamento { get; set; } = "";
            public string Medicamento { get; set; } = "";
            public string Tipo { get; set; } = "";
            public int Cantidad { get; set; }
            public DateTime Fecha { get; set; }
            public string Motivo { get; set; } = "";
            public string Responsable { get; set; } = "";
            public string Documento { get; set; } = "";
            public string Observacion { get; set; } = "";
        }
    }
}
