using System;
using System.Drawing;
using System.Windows.Forms;

namespace SIGEPOSTA.Presentacion.Formularios
{
    internal static class EstiloModulos
    {
        private static readonly Color VerdeOscuro = Color.FromArgb(31, 77, 54);
        private static readonly Color Verde = Color.FromArgb(47, 125, 69);
        private static readonly Color Fondo = Color.FromArgb(245, 247, 246);
        private static readonly Color Texto = Color.FromArgb(55, 65, 60);
        private static readonly Color Borde = Color.FromArgb(220, 229, 223);

        public static void Aplicar(Form formulario)
        {
            formulario.BackColor = Fondo;
            formulario.Font = new Font("Segoe UI", 10F);

            AplicarControles(formulario);
        }

        private static void AplicarControles(Control padre)
        {
            foreach (Control control in padre.Controls)
            {
                if (control is DataGridView tabla)
                {
                    EstilizarTabla(tabla);
                }
                else if (control is CampoInventario campo)
                {
                    campo.Font = new Font("Segoe UI", 10F);
                    campo.BackColor = Color.White;
                    campo.ForeColor = Texto;
                }
                else if (control is TextBox caja)
                {
                    caja.Font = new Font("Segoe UI", 10F);
                    caja.BackColor = Color.White;
                    caja.ForeColor = Texto;
                    caja.BorderStyle = BorderStyle.FixedSingle;

                    if (!caja.Multiline)
                        caja.Height = 36;
                }
                else if (control is ComboBox combo)
                {
                    combo.Font = new Font("Segoe UI", 10F);
                    combo.BackColor = Color.White;
                    combo.ForeColor = Texto;
                    combo.FlatStyle = FlatStyle.Flat;
                }
                else if (control is DateTimePicker fecha)
                {
                    fecha.Font = new Font("Segoe UI", 10F);
                    fecha.CalendarTitleBackColor = VerdeOscuro;
                    fecha.CalendarTitleForeColor = Color.White;
                }
                else if (control is Button boton)
                {
                    EstilizarBoton(boton);
                }
                else if (control is GroupBox grupo)
                {
                    grupo.BackColor = Color.White;
                    grupo.ForeColor = VerdeOscuro;
                    grupo.Font = new Font("Segoe UI", 10F);
                }
                else if (control is Label etiqueta)
                {
                    if (!EsEncabezado(etiqueta) &&
                        etiqueta.Font.Size <= 12F)
                    {
                        etiqueta.Font = new Font(
                            "Segoe UI", 9.5F, FontStyle.Regular);
                        etiqueta.ForeColor = Texto;
                    }
                }

                AplicarControles(control);
            }
        }

        private static bool EsEncabezado(Control control)
        {
            Control actual = control.Parent;

            while (actual != null)
            {
                if (actual is Panel &&
                    actual.Dock == DockStyle.Top &&
                    actual.Height >= 80 &&
                    actual.BackColor == VerdeOscuro)
                    return true;

                actual = actual.Parent;
            }

            return false;
        }

        private static void EstilizarBoton(Button boton)
        {
            string texto = boton.Text.ToUpperInvariant();

            boton.FlatStyle = FlatStyle.Flat;
            boton.FlatAppearance.BorderSize = 0;
            boton.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            boton.Cursor = Cursors.Hand;
            boton.ForeColor = Color.White;
            boton.Height = Math.Max(boton.Height, 40);

            if (texto.Contains("ELIMINAR"))
            {
                boton.BackColor = Color.FromArgb(190, 55, 55);
            }
            else if (texto.Contains("LIMPIAR") ||
                     texto.Contains("CANCELAR"))
            {
                boton.BackColor = Color.FromArgb(105, 115, 108);
            }
            else
            {
                boton.BackColor = Verde;
            }

            boton.FlatAppearance.MouseOverBackColor =
                Color.FromArgb(38, 105, 59);
        }

        private static void EstilizarTabla(DataGridView tabla)
        {
            tabla.EnableHeadersVisualStyles = false;
            tabla.BackgroundColor = Color.White;
            tabla.BorderStyle = BorderStyle.None;
            tabla.GridColor = Borde;
            tabla.RowHeadersVisible = false;

            tabla.ColumnHeadersHeight = 44;
            tabla.ColumnHeadersHeightSizeMode =
                DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            tabla.ColumnHeadersDefaultCellStyle =
                new DataGridViewCellStyle
                {
                    BackColor = VerdeOscuro,
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                    SelectionBackColor = VerdeOscuro,
                    SelectionForeColor = Color.White,
                    Padding = new Padding(8, 0, 0, 0)
                };

            tabla.DefaultCellStyle =
                new DataGridViewCellStyle
                {
                    BackColor = Color.White,
                    ForeColor = Texto,
                    Font = new Font("Segoe UI", 9.5F),
                    SelectionBackColor = Color.FromArgb(225, 240, 229),
                    SelectionForeColor = Texto,
                    Padding = new Padding(8, 0, 0, 0)
                };

            tabla.AlternatingRowsDefaultCellStyle =
                new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(247, 250, 248),
                    ForeColor = Texto,
                    SelectionBackColor = Color.FromArgb(225, 240, 229),
                    SelectionForeColor = Texto,
                    Font = new Font("Segoe UI", 9.5F)
                };

            tabla.RowTemplate.Height = 38;
        }
    }
}

