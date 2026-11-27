using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SIGEPOSTA.Presentacion.Estilos;
using SIGEPOSTA.Entidades.Personas;
using SIGEPOSTA.Datos.Repositorios;
using SIGEPOSTA.Negocio.Servicios;

namespace SIGEPOSTA.Presentacion.Formularios
{
    public class FormPersonalSalud : Form
    {
        private readonly PersonalSaludServicio servicio =
            new PersonalSaludServicio(new PersonalSaludRepositorio());

        private readonly BindingList<PersonalVista> personal = new();

        private CampoInventario txtDni = new();
        private CampoInventario txtNombres = new();
        private CampoInventario txtApellidos = new();
        private CampoInventario txtTelefono = new();
        private CampoInventario txtCorreo = new();

        private ComboBox cmbCargo = new();
        private ComboBox cmbEspecialidad = new();
        private DataGridView dgvPersonal = new();

        private int indiceSeleccionado = -1;

        public FormPersonalSalud()
        {
            ConstruirFormulario();
            ConfigurarTabla();
            AplicarEstilos();
            CargarPersonal();
        }

        private void ConstruirFormulario()
        {
            Text = "SIGEPOSTA - Personal de salud";
            Size = new Size(1250, 850);
            MinimumSize = new Size(1050, 700);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = InventarioEstilo.Fondo;
            Font = new Font("Segoe UI", 10F);
            DoubleBuffered = true;

            Panel encabezado = new()
            {
                Dock = DockStyle.Top,
                Height = 110,
                BackColor = InventarioEstilo.VerdeOscuro,
                Padding = new Padding(30, 12, 30, 12)
            };

            TableLayoutPanel contenidoEncabezado = new()
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                BackColor = InventarioEstilo.VerdeOscuro,
                Margin = new Padding(0),
                Padding = new Padding(0)
            };

            contenidoEncabezado.RowStyles.Add(
                new RowStyle(SizeType.Percent, 58));
            contenidoEncabezado.RowStyles.Add(
                new RowStyle(SizeType.Percent, 42));

            contenidoEncabezado.Controls.Add(new Label
            {
                Text = "GESTIÓN DEL PERSONAL DE SALUD",
                Dock = DockStyle.Fill,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 21F, FontStyle.Bold),
                TextAlign = ContentAlignment.BottomLeft,
                BackColor = Color.Transparent,
                Margin = new Padding(0)
            }, 0, 0);

            contenidoEncabezado.Controls.Add(new Label
            {
                Text = "Administración de médicos, enfermeros y trabajadores del establecimiento",
                Dock = DockStyle.Fill,
                ForeColor = Color.FromArgb(220, 235, 225),
                Font = new Font("Segoe UI", 10F),
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent,
                Margin = new Padding(2, 0, 0, 0)
            }, 0, 1);

            encabezado.Controls.Add(contenidoEncabezado);

            Panel contenido = new()
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(28, 25, 28, 20),
                BackColor = InventarioEstilo.Fondo
            };

            TableLayoutPanel columna = new()
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 1,
                RowCount = 3,
                BackColor = InventarioEstilo.Fondo,
                Padding = new Padding(0)
            };

            columna.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100));

            for (int i = 0; i < 3; i++)
                columna.RowStyles.Add(
                    new RowStyle(SizeType.AutoSize));

            TarjetaInventario tarjetaFormulario = new()
            {
                Dock = DockStyle.Top,
                Height = 300,
                Margin = new Padding(0, 0, 0, 15)
            };

            columna.Controls.Add(tarjetaFormulario, 0, 0);

            TableLayoutPanel distribucion = new()
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(20, 12, 20, 15),
                BackColor = Color.White
            };

            distribucion.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 48));
            distribucion.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100));

            tarjetaFormulario.Controls.Add(distribucion);

            distribucion.Controls.Add(new Label
            {
                Text = "Información del personal",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = InventarioEstilo.VerdeOscuro,
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.White,
                Margin = new Padding(0)
            }, 0, 0);

            TableLayoutPanel rejilla = new()
            {
                Dock = DockStyle.Fill,
                ColumnCount = 6,
                RowCount = 3,
                BackColor = Color.White,
                Padding = new Padding(5, 5, 5, 0)
            };

            rejilla.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 115));
            rejilla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            rejilla.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 115));
            rejilla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            rejilla.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 115));
            rejilla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));

            for (int i = 0; i < 3; i++)
                rejilla.RowStyles.Add(
                    new RowStyle(SizeType.Absolute, 65));

            txtDni = new CampoInventario();
            txtNombres = new CampoInventario();
            txtApellidos = new CampoInventario();
            txtTelefono = new CampoInventario();
            txtCorreo = new CampoInventario();

            cmbCargo = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10F),
                BackColor = Color.FromArgb(245, 248, 246),
                ForeColor = InventarioEstilo.Texto,
                FlatStyle = FlatStyle.Flat
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

            cmbEspecialidad = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDown,
                Font = new Font("Segoe UI", 10F),
                BackColor = Color.FromArgb(245, 248, 246),
                ForeColor = InventarioEstilo.Texto,
                FlatStyle = FlatStyle.Flat
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

            void AgregarCampo(string etiqueta, Control campo, int fila, int col)
            {
                rejilla.Controls.Add(new Label
                {
                    Text = etiqueta,
                    AutoSize = true,
                    Anchor = AnchorStyles.Left,
                    ForeColor = InventarioEstilo.Texto,
                    Font = new Font("Segoe UI", 9.5F),
                    BackColor = Color.White,
                    Margin = new Padding(3, 10, 3, 3)
                }, col, fila);

                campo.Dock = DockStyle.Fill;
                campo.Margin = new Padding(3, 5, 12, 5);

                rejilla.Controls.Add(campo, col + 1, fila);
            }

            AgregarCampo("DNI:", txtDni, 0, 0);
            AgregarCampo("Nombres:", txtNombres, 0, 2);
            AgregarCampo("Apellidos:", txtApellidos, 0, 4);

            AgregarCampo("Cargo:", cmbCargo, 1, 0);
            AgregarCampo("Especialidad:", cmbEspecialidad, 1, 2);
            AgregarCampo("Teléfono:", txtTelefono, 1, 4);

            AgregarCampo("Correo:", txtCorreo, 2, 0);
            rejilla.SetColumnSpan(txtCorreo, 5);

            distribucion.Controls.Add(rejilla, 0, 1);

            Panel panelBotones = new()
            {
                Dock = DockStyle.Fill,
                Height = 78,
                BackColor = InventarioEstilo.Fondo,
                Padding = new Padding(0, 8, 0, 8),
                Margin = new Padding(0, 0, 0, 12)
            };

            FlowLayoutPanel botones = new()
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Anchor = AnchorStyles.None,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Padding = new Padding(10),
                Margin = new Padding(0),
                BackColor = InventarioEstilo.Fondo
            };

            BotonInventario btnRegistrar =
                new("REGISTRAR", TipoBoton.Primario);
            btnRegistrar.Click += (s, e) => RegistrarPersonal();

            BotonInventario btnActualizar =
                new("ACTUALIZAR", TipoBoton.Secundario);
            btnActualizar.Click += (s, e) => ActualizarPersonal();

            BotonInventario btnEliminar =
                new("ELIMINAR", TipoBoton.Peligro);
            btnEliminar.Click += (s, e) => EliminarPersonal();

            BotonInventario btnLimpiar =
                new("LIMPIAR", TipoBoton.Secundario);
            btnLimpiar.Click += (s, e) => LimpiarFormulario();

            foreach (BotonInventario boton in new[]
            {
                btnRegistrar, btnActualizar, btnEliminar, btnLimpiar
            })
            {
                boton.Size = new Size(155, 46);
                boton.Margin = new Padding(8, 0, 8, 5);
            }

            botones.Controls.AddRange(new Control[]
            {
                btnRegistrar, btnActualizar, btnEliminar, btnLimpiar
            });

            panelBotones.Controls.Add(botones);

            void CentrarBotones()
            {
                botones.Location = new Point(
                    Math.Max(0, (panelBotones.ClientSize.Width - botones.Width) / 2),
                    Math.Max(0, (panelBotones.ClientSize.Height - botones.Height) / 2));
            }

            panelBotones.Resize += (s, e) => CentrarBotones();
            botones.SizeChanged += (s, e) => CentrarBotones();

            columna.Controls.Add(panelBotones, 0, 1);

            TarjetaInventario tarjetaTabla = new()
            {
                Dock = DockStyle.Top,
                Height = 370,
                Margin = new Padding(0, 0, 0, 15)
            };

            columna.Controls.Add(tarjetaTabla, 0, 2);

            TableLayoutPanel distribucionTabla = new()
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(20, 12, 20, 15),
                BackColor = Color.White
            };

            distribucionTabla.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 48));
            distribucionTabla.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100));

            tarjetaTabla.Controls.Add(distribucionTabla);

            distribucionTabla.Controls.Add(new Label
            {
                Text = "Listado del personal registrado",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = InventarioEstilo.VerdeOscuro,
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.White,
                Margin = new Padding(0)
            }, 0, 0);

            dgvPersonal = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White,
                RowHeadersVisible = false,
                BorderStyle = BorderStyle.None,
                Margin = new Padding(0)
            };

            dgvPersonal.SelectionChanged +=
                (s, e) => SeleccionarPersonal();

            distribucionTabla.Controls.Add(dgvPersonal, 0, 1);

            contenido.Controls.Add(columna);

            Controls.Add(contenido);
            Controls.Add(encabezado);
        }

        private void AplicarEstilos()
        {
            dgvPersonal.EnableHeadersVisualStyles = false;
            dgvPersonal.BackgroundColor = Color.White;
            dgvPersonal.BorderStyle = BorderStyle.None;
            dgvPersonal.GridColor = InventarioEstilo.Borde;
            dgvPersonal.RowHeadersVisible = false;
            dgvPersonal.ColumnHeadersHeight = 44;
            dgvPersonal.ColumnHeadersHeightSizeMode =
                DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            dgvPersonal.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = InventarioEstilo.VerdeOscuro,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                SelectionBackColor = InventarioEstilo.VerdeOscuro,
                SelectionForeColor = Color.White,
                Padding = new Padding(8, 0, 0, 0)
            };

            dgvPersonal.DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.White,
                ForeColor = InventarioEstilo.Texto,
                Font = new Font("Segoe UI", 9.5F),
                SelectionBackColor = Color.FromArgb(225, 240, 229),
                SelectionForeColor = InventarioEstilo.Texto,
                Padding = new Padding(8, 0, 0, 0)
            };

            dgvPersonal.AlternatingRowsDefaultCellStyle =
                new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(247, 250, 248),
                    ForeColor = InventarioEstilo.Texto,
                    SelectionBackColor = Color.FromArgb(225, 240, 229),
                    SelectionForeColor = InventarioEstilo.Texto,
                    Font = new Font("Segoe UI", 9.5F)
                };

            dgvPersonal.RowTemplate.Height = 38;
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

        private void CargarPersonal()
        {
            try
            {
                personal.Clear();

                var registros = servicio.ObtenerTodos()
                    .Where(p => p.Estado)
                    .OrderBy(p => p.Nombres)
                    .ToList();

                foreach (PersonalSalud trabajador in registros)
                {
                    personal.Add(new PersonalVista
                    {
                        IdPersonalSalud = trabajador.IdPersonalSalud,
                        Dni = trabajador.NumeroDocumento,
                        Nombres = trabajador.Nombres,
                        Apellidos = $"{trabajador.ApellidoPaterno} {trabajador.ApellidoMaterno}".Trim(),
                        Cargo = trabajador.Cargo,
                        Especialidad = trabajador.Especialidad,
                        Telefono = trabajador.Telefono,
                        Correo = trabajador.CorreoElectronico
                    });
                }

                dgvPersonal.ClearSelection();
                indiceSeleccionado = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo cargar el personal de salud.\n\n" + ex.Message,
                    "Error de base de datos",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void RegistrarPersonal()
        {
            if (!ValidarDatos())
                return;

            try
            {
                string dni = txtDni.Text.Trim();

                bool existe = servicio.ObtenerTodos()
                    .Any(p => p.NumeroDocumento.Trim() == dni);

                if (existe)
                {
                    MessageBox.Show("Ya existe un trabajador registrado con ese DNI.");
                    return;
                }

                PersonalSalud trabajador = new()
                {
                    TipoDocumento = "DNI",
                    NumeroDocumento = dni,
                    Nombres = txtNombres.Text.Trim(),
                    ApellidoPaterno = txtApellidos.Text.Trim(),
                    ApellidoMaterno = "",
                    Sexo = "No especificado",
                    Telefono = txtTelefono.Text.Trim(),
                    CorreoElectronico = txtCorreo.Text.Trim(),
                    Direccion = "",
                    Especialidad = cmbEspecialidad.Text.Trim(),
                    Cargo = cmbCargo.Text.Trim(),
                    Estado = true,
                    EsActivo = true
                };

                servicio.Registrar(trabajador);

                MessageBox.Show(
                    "Personal registrado correctamente.",
                    "SIGEPOSTA",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                CargarPersonal();
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo registrar el trabajador.\n\n" + ex.Message,
                    "Error al registrar",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
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

            try
            {
                PersonalVista seleccionado = personal[indiceSeleccionado];
                string dni = txtDni.Text.Trim();

                bool dniDuplicado = servicio.ObtenerTodos().Any(p =>
                    p.NumeroDocumento.Trim() == dni &&
                    p.IdPersonalSalud != seleccionado.IdPersonalSalud);

                if (dniDuplicado)
                {
                    MessageBox.Show("El DNI pertenece a otro trabajador.");
                    return;
                }

                PersonalSalud? trabajador =
                    servicio.ObtenerPorId(seleccionado.IdPersonalSalud);

                if (trabajador == null)
                {
                    MessageBox.Show("No se encontró el trabajador seleccionado.");
                    CargarPersonal();
                    return;
                }

                trabajador.NumeroDocumento = dni;
                trabajador.Nombres = txtNombres.Text.Trim();
                trabajador.ApellidoPaterno = txtApellidos.Text.Trim();
                trabajador.ApellidoMaterno = "";
                trabajador.Cargo = cmbCargo.Text.Trim();
                trabajador.Especialidad = cmbEspecialidad.Text.Trim();
                trabajador.Telefono = txtTelefono.Text.Trim();
                trabajador.CorreoElectronico = txtCorreo.Text.Trim();

                servicio.Actualizar(trabajador);

                MessageBox.Show(
                    "Información actualizada correctamente.",
                    "SIGEPOSTA",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                CargarPersonal();
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo actualizar el trabajador.\n\n" + ex.Message,
                    "Error al actualizar",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void EliminarPersonal()
        {
            if (indiceSeleccionado < 0 || indiceSeleccionado >= personal.Count)
            {
                MessageBox.Show("Seleccione un trabajador de la tabla.");
                return;
            }

            DialogResult respuesta = MessageBox.Show(
                "¿Desea desactivar este registro de personal?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (respuesta != DialogResult.Yes)
                return;

            try
            {
                int id = personal[indiceSeleccionado].IdPersonalSalud;

                servicio.Eliminar(id);

                MessageBox.Show(
                    "Registro desactivado correctamente.",
                    "SIGEPOSTA",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                CargarPersonal();
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo eliminar el registro.\n\n" + ex.Message,
                    "Error al eliminar",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
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

            if (cmbCargo.SelectedIndex < 0)
                cmbCargo.SelectedIndex = 0;

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
                MessageBox.Show(
                    "Complete el DNI, los nombres y los apellidos.");
                return false;
            }

            string dni = txtDni.Text.Trim();

            if (dni.Length != 8 || !dni.All(char.IsDigit))
            {
                MessageBox.Show(
                    "El DNI debe contener exactamente 8 números.");
                return false;
            }

            if (cmbCargo.SelectedItem == null)
            {
                MessageBox.Show("Seleccione el cargo del trabajador.");
                return false;
            }

            return true;
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

        private class PersonalVista
        {
            public int IdPersonalSalud { get; set; }

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
