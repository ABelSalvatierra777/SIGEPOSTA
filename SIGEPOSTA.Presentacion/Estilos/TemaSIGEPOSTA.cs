using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Runtime.CompilerServices;

namespace SIGEPOSTA.Presentacion.Estilos;

public enum TipoBoton
{
    Primario,
    Secundario,
    Peligro
}

public static class TemaSIGEPOSTA
{
    // =========================================================
    // COLORES INSTITUCIONALES
    // =========================================================

    public static readonly Color VerdeOscuro = Color.FromArgb(27, 65, 54);
    public static readonly Color Verde = Color.FromArgb(43, 125, 91);
    public static readonly Color VerdePrincipal = Verde;
    public static readonly Color VerdeClaro = Color.FromArgb(225, 240, 229);

    public static readonly Color Fondo = Color.FromArgb(240, 244, 242);
    public static readonly Color Blanco = Color.White;

    public static readonly Color Texto = Color.FromArgb(28, 40, 35);
    public static readonly Color TextoSecundario = Color.FromArgb(110, 125, 119);

    public static readonly Color Borde = Color.FromArgb(215, 225, 220);
    public static readonly Color BordeCampo = Color.FromArgb(200, 215, 207);
    public static readonly Color FondoCampo = Color.FromArgb(245, 248, 246);

    public static readonly Color Rojo = Color.FromArgb(200, 55, 55);
    public static readonly Color Azul = Color.FromArgb(49, 103, 160);
    public static readonly Color Naranja = Color.FromArgb(214, 140, 20);
    public static readonly Color Advertencia = Naranja;

    public static readonly Color VerdeHover = Color.FromArgb(34, 106, 77);
    public static readonly Color VerdeActivo = Color.FromArgb(27, 85, 62);

    public static readonly Color RojoHover = Color.FromArgb(165, 43, 43);
    public static readonly Color RojoActivo = Color.FromArgb(140, 35, 35);

    public static readonly Color FilaAlterna = Color.FromArgb(247, 250, 248);

    // =========================================================
    // TIPOGRAFÍA
    // =========================================================

    public static readonly Font Fuente = new("Segoe UI", 10F);
    public static readonly Font FuenteNegrita = new("Segoe UI", 10F, FontStyle.Bold);
    public static readonly Font Titulo = new("Segoe UI", 22F, FontStyle.Bold);
    public static readonly Font FuenteTitulo = Titulo;
    public static readonly Font Subtitulo = new("Segoe UI", 12F);
    public static readonly Font TituloModulo = new("Segoe UI", 18F, FontStyle.Bold);
    public static readonly Font FuenteCampo = new("Segoe UI", 10F);
    public static readonly Font FuenteBoton = new("Segoe UI Semibold", 9.5F);
    public static readonly Font FuenteEtiqueta = new("Segoe UI Semibold", 9.5F);
    public static readonly Font FuentePequena = new("Segoe UI", 9F);

    // =========================================================
    // MEDIDAS
    // =========================================================

    public const int AlturaCampo = 36;
    public const int AlturaBoton = 42;
    public const int AnchoBoton = 140;
    public const int Espacio = 16;
    public const int AlturaFila = 38;
    public const int AlturaCabeceraTabla = 44;

    private const int RadioBoton = 10;
    private const int RadioTarjeta = 14;

    private sealed class EstadoBoton
    {
        public Color Fondo;
        public Color Hover;
        public Color Activo;
        public Color Texto;
        public Color Borde;
        public bool EstaEncima;
        public bool EstaPresionado;
    }

    private static readonly ConditionalWeakTable<Button, EstadoBoton>
        EstadosBotones = new();

    // =========================================================
    // BOTONES CON DEGRADADO Y ESQUINAS REDONDEADAS
    // =========================================================

    public static void EstilizarBoton(
        Button boton,
        TipoBoton tipo = TipoBoton.Primario)
    {
        Color fondo;
        Color hover;
        Color activo;
        Color texto;
        Color borde;

        switch (tipo)
        {
            case TipoBoton.Peligro:
                fondo = Rojo;
                hover = RojoHover;
                activo = RojoActivo;
                texto = Color.White;
                borde = Rojo;
                break;

            case TipoBoton.Secundario:
                fondo = Color.White;
                hover = VerdeClaro;
                activo = Color.FromArgb(205, 228, 212);
                texto = VerdeOscuro;
                borde = BordeCampo;
                break;

            default:
                fondo = Verde;
                hover = VerdeHover;
                activo = VerdeActivo;
                texto = Color.White;
                borde = Verde;
                break;
        }

        EstadoBoton estado = EstadosBotones.GetValue(
            boton,
            _ =>
            {
                var nuevo = new EstadoBoton();

                boton.Paint += PintarBoton;

                boton.MouseEnter += (_, _) =>
                {
                    nuevo.EstaEncima = true;
                    boton.Invalidate();
                };

                boton.MouseLeave += (_, _) =>
                {
                    nuevo.EstaEncima = false;
                    nuevo.EstaPresionado = false;
                    boton.Invalidate();
                };

                boton.MouseDown += (_, e) =>
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        nuevo.EstaPresionado = true;
                        boton.Invalidate();
                    }
                };

                boton.MouseUp += (_, _) =>
                {
                    nuevo.EstaPresionado = false;
                    boton.Invalidate();
                };

                boton.Resize += (_, _) => AplicarRegionBoton(boton);

                return nuevo;
            });

        estado.Fondo = fondo;
        estado.Hover = hover;
        estado.Activo = activo;
        estado.Texto = texto;
        estado.Borde = borde;

        boton.FlatStyle = FlatStyle.Flat;
        boton.FlatAppearance.BorderSize = 0;
        boton.UseVisualStyleBackColor = false;

        boton.BackColor = fondo;
        boton.ForeColor = texto;
        boton.Font = FuenteBoton;
        boton.Cursor = Cursors.Hand;

        boton.Height = AlturaBoton;
        boton.Width = AnchoBoton;
        boton.TextAlign = ContentAlignment.MiddleCenter;

        boton.FlatAppearance.MouseOverBackColor = hover;
        boton.FlatAppearance.MouseDownBackColor = activo;

        AplicarRegionBoton(boton);
        boton.Invalidate();
    }

    private static void PintarBoton(object? sender, PaintEventArgs e)
    {
        if (sender is not Button boton ||
            !EstadosBotones.TryGetValue(boton, out EstadoBoton? estado))
            return;

        if (boton.Width <= 0 || boton.Height <= 0)
            return;

        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.Clear(boton.Parent?.BackColor ?? Fondo);

        Rectangle rect = new(
            1,
            1,
            boton.Width - 3,
            boton.Height - 3);

        Color colorSuperior = estado.Fondo;
        Color colorInferior = estado.Fondo;

        if (estado.EstaPresionado)
        {
            colorSuperior = estado.Activo;
            colorInferior = estado.Activo;
        }
        else if (estado.EstaEncima)
        {
            colorSuperior = estado.Hover;
            colorInferior = estado.Fondo;
        }

        using GraphicsPath ruta = CrearRutaRedondeada(rect, RadioBoton);

        using LinearGradientBrush degradado = new(
            rect,
            colorSuperior,
            colorInferior,
            LinearGradientMode.Vertical);

        g.FillPath(degradado, ruta);

        using Pen lapiz = new(estado.Borde, 1F);
        g.DrawPath(lapiz, ruta);

        TextRenderer.DrawText(
            g,
            boton.Text,
            boton.Font,
            rect,
            estado.Texto,
            TextFormatFlags.HorizontalCenter |
            TextFormatFlags.VerticalCenter |
            TextFormatFlags.EndEllipsis);
    }

    private static void AplicarRegionBoton(Button boton)
    {
        if (boton.Width <= 0 || boton.Height <= 0)
            return;

        using GraphicsPath ruta = CrearRutaRedondeada(
            new Rectangle(0, 0, boton.Width, boton.Height),
            RadioBoton);

        boton.Region?.Dispose();
        boton.Region = new Region(ruta);
    }

    // =========================================================
    // CAMPOS DE TEXTO
    // =========================================================

    public static void EstilizarTextBox(TextBox caja)
    {
        caja.BorderStyle = BorderStyle.FixedSingle;
        caja.BackColor = FondoCampo;
        caja.ForeColor = Texto;
        caja.Font = FuenteCampo;

        caja.Margin = new Padding(3, 5, 3, 5);

        if (!caja.Multiline)
            caja.Height = AlturaCampo;
    }

    public static void EstilizarComboBox(ComboBox combo)
    {
        combo.FlatStyle = FlatStyle.Flat;
        combo.BackColor = FondoCampo;
        combo.ForeColor = Texto;
        combo.Font = FuenteCampo;
        combo.Cursor = Cursors.Hand;
        combo.Height = AlturaCampo;
    }

    public static void EstilizarFecha(DateTimePicker fecha)
    {
        fecha.Font = FuenteCampo;
        fecha.Format = DateTimePickerFormat.Custom;
        fecha.CustomFormat = "dd/MM/yyyy";

        fecha.CalendarMonthBackground = Blanco;
        fecha.CalendarTitleBackColor = VerdeOscuro;
        fecha.CalendarTitleForeColor = Blanco;
        fecha.CalendarForeColor = Texto;
    }

    // =========================================================
    // ETIQUETAS Y GRUPOS
    // =========================================================

    public static void EstilizarEtiqueta(Label etiqueta)
    {
        etiqueta.Font = FuenteEtiqueta;
        etiqueta.ForeColor = Texto;
        etiqueta.BackColor = Color.Transparent;
    }

    public static void EstilizarGrupo(GroupBox grupo)
    {
        grupo.Font = FuenteNegrita;
        grupo.ForeColor = VerdeOscuro;
        grupo.BackColor = Blanco;
    }

    // =========================================================
    // TARJETAS BLANCAS CON ESQUINAS REDONDEADAS
    // =========================================================

    public static void EstilizarTarjeta(Panel panel)
    {
        panel.BackColor = Blanco;

        panel.Paint -= PintarTarjeta;
        panel.Paint += PintarTarjeta;

        panel.Resize -= ActualizarRegionTarjeta;
        panel.Resize += ActualizarRegionTarjeta;

        ActualizarRegionTarjeta(panel, EventArgs.Empty);
        panel.Invalidate();
    }

    private static void PintarTarjeta(object? sender, PaintEventArgs e)
    {
        if (sender is not Panel panel ||
            panel.Width <= 0 ||
            panel.Height <= 0)
            return;

        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        Rectangle rect = new(
            1,
            1,
            panel.Width - 3,
            panel.Height - 3);

        using GraphicsPath ruta = CrearRutaRedondeada(
            rect,
            RadioTarjeta);

        using SolidBrush fondo = new(Blanco);
        using Pen borde = new(Borde, 1F);

        g.FillPath(fondo, ruta);
        g.DrawPath(borde, ruta);
    }

    private static void ActualizarRegionTarjeta(
        object? sender,
        EventArgs e)
    {
        if (sender is not Panel panel ||
            panel.Width <= 0 ||
            panel.Height <= 0)
            return;

        using GraphicsPath ruta = CrearRutaRedondeada(
            new Rectangle(0, 0, panel.Width, panel.Height),
            RadioTarjeta);

        panel.Region?.Dispose();
        panel.Region = new Region(ruta);
    }

    // =========================================================
    // TABLAS
    // =========================================================

    public static void EstilizarTabla(DataGridView tabla)
    {
        tabla.EnableHeadersVisualStyles = false;
        tabla.BackgroundColor = Blanco;
        tabla.BorderStyle = BorderStyle.None;

        tabla.CellBorderStyle =
            DataGridViewCellBorderStyle.SingleHorizontal;

        tabla.GridColor = Borde;
        tabla.RowHeadersVisible = false;

        tabla.AllowUserToAddRows = false;
        tabla.AllowUserToDeleteRows = false;
        tabla.AllowUserToResizeRows = false;

        tabla.MultiSelect = false;
        tabla.ReadOnly = true;

        tabla.SelectionMode =
            DataGridViewSelectionMode.FullRowSelect;

        tabla.AutoSizeColumnsMode =
            DataGridViewAutoSizeColumnsMode.Fill;

        tabla.ColumnHeadersBorderStyle =
            DataGridViewHeaderBorderStyle.None;

        tabla.ColumnHeadersHeightSizeMode =
            DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

        tabla.ColumnHeadersHeight = AlturaCabeceraTabla;

        tabla.RowTemplate.Height = AlturaFila;

        tabla.DefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor = Blanco,
            ForeColor = Texto,
            Font = Fuente,
            SelectionBackColor = VerdeClaro,
            SelectionForeColor = Texto,
            Padding = new Padding(10, 0, 0, 0)
        };

        tabla.ColumnHeadersDefaultCellStyle =
            new DataGridViewCellStyle
            {
                BackColor = VerdeOscuro,
                ForeColor = Blanco,
                Font = FuenteNegrita,
                SelectionBackColor = VerdeOscuro,
                SelectionForeColor = Blanco,
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0)
            };

        tabla.AlternatingRowsDefaultCellStyle =
            new DataGridViewCellStyle
            {
                BackColor = FilaAlterna,
                ForeColor = Texto,
                SelectionBackColor = VerdeClaro,
                SelectionForeColor = Texto,
                Font = Fuente
            };
    }

    // =========================================================
    // APLICAR TEMA A TODO EL FORMULARIO
    // =========================================================

    public static void AplicarTema(Control raiz)
    {
        if (raiz is Form formulario)
        {
            formulario.BackColor = Fondo;
            formulario.Font = Fuente;
        }

        foreach (Control control in raiz.Controls)
        {
            if (control is Button boton)
            {
                EstilizarBoton(
                    boton,
                    ClasificarBoton(boton.Text));
            }
            else if (control is TextBox caja)
            {
                EstilizarTextBox(caja);
            }
            else if (control is ComboBox combo)
            {
                EstilizarComboBox(combo);
            }
            else if (control is DateTimePicker fecha)
            {
                EstilizarFecha(fecha);
            }
            else if (control is DataGridView tabla)
            {
                EstilizarTabla(tabla);
            }
            else if (control is GroupBox grupo)
            {
                EstilizarGrupo(grupo);
            }
            else if (control is Label etiqueta)
            {
                EstilizarEtiqueta(etiqueta);
            }

            if (control.HasChildren)
                AplicarTema(control);
        }
    }

    // =========================================================
    // CLASIFICACIÓN AUTOMÁTICA DE BOTONES
    // =========================================================

    private static TipoBoton ClasificarBoton(string texto)
    {
        string t = (texto ?? string.Empty)
            .Trim()
            .ToLowerInvariant();

        if (t.Contains("eliminar") ||
            t.Contains("borrar") ||
            t.Contains("quitar"))
        {
            return TipoBoton.Peligro;
        }

        if (t.Contains("limpiar") ||
            t.Contains("cancelar") ||
            t.Contains("cerrar") ||
            t.Contains("actualizar") ||
            t.Contains("volver"))
        {
            return TipoBoton.Secundario;
        }

        return TipoBoton.Primario;
    }

    // =========================================================
    // MÉTODO PARA DIBUJAR ESQUINAS REDONDEADAS
    // =========================================================

    private static GraphicsPath CrearRutaRedondeada(
        Rectangle rectangulo,
        int radio)
    {
        GraphicsPath ruta = new();

        int diametro = radio * 2;

        if (diametro > rectangulo.Width)
            diametro = rectangulo.Width;

        if (diametro > rectangulo.Height)
            diametro = rectangulo.Height;

        if (diametro <= 0)
        {
            ruta.AddRectangle(rectangulo);
            return ruta;
        }

        ruta.AddArc(
            rectangulo.X,
            rectangulo.Y,
            diametro,
            diametro,
            180,
            90);

        ruta.AddArc(
            rectangulo.Right - diametro,
            rectangulo.Y,
            diametro,
            diametro,
            270,
            90);

        ruta.AddArc(
            rectangulo.Right - diametro,
            rectangulo.Bottom - diametro,
            diametro,
            diametro,
            0,
            90);

        ruta.AddArc(
            rectangulo.X,
            rectangulo.Bottom - diametro,
            diametro,
            diametro,
            90,
            90);

        ruta.CloseFigure();

        return ruta;
    }
}