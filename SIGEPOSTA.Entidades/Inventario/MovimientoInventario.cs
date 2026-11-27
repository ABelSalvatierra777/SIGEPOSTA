namespace SIGEPOSTA.Entidades.Inventario;

public class MovimientoInventario
{
    public int IdMovimiento { get; set; }
    public int IdInventario { get; set; }
    public int IdUsuario { get; set; }
    public string TipoMovimiento { get; set; } = "";
    public int Cantidad { get; set; }
    public string Motivo { get; set; } = "";
    public DateTime FechaMovimiento { get; set; } = DateTime.Now;
}
