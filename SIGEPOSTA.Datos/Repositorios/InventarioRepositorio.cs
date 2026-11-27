using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace SIGEPOSTA.Datos.Repositorios;

public class InventarioRepositorio
{
    private readonly string _cadenaConexion;

    public InventarioRepositorio()
    {
        _cadenaConexion =
            Environment.GetEnvironmentVariable("SIGEPOSTA_SQLSERVER")
            ?? throw new InvalidOperationException(
                "No se encontró SIGEPOSTA_SQLSERVER.");
    }

    public class MedicamentoInventario
    {
        public int IdMedicamento { get; set; }
        public string Nombre { get; set; } = "";
        public int StockActual { get; set; }
        public int StockMinimo { get; set; }
    }

    public class MovimientoRegistro
    {
        public int IdMovimiento { get; set; }
        public int IdInventario { get; set; }
        public int IdMedicamento { get; set; }
        public string Medicamento { get; set; } = "";
        public string Tipo { get; set; } = "";
        public int Cantidad { get; set; }
        public DateTime Fecha { get; set; }
        public string Motivo { get; set; } = "";
        public string Responsable { get; set; } = "";
    }

    public MedicamentoInventario? ObtenerMedicamento(int idMedicamento)
    {
        using var conexion = new SqlConnection(_cadenaConexion);
        conexion.Open();

        const string sql = @"
            SELECT
                M.IdMedicamento,
                M.NombreMedicamento,
                ISNULL(SUM(I.StockActual), 0) AS StockActual,
                ISNULL(MAX(I.StockMinimo), 0) AS StockMinimo
            FROM Medicamentos M
            LEFT JOIN Inventario I
                ON I.IdMedicamento = M.IdMedicamento
            WHERE M.IdMedicamento = @IdMedicamento
              AND M.Estado = 1
            GROUP BY M.IdMedicamento, M.NombreMedicamento;";

        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@IdMedicamento", idMedicamento);

        using var lector = comando.ExecuteReader();

        if (!lector.Read())
            return null;

        return new MedicamentoInventario
        {
            IdMedicamento = lector.GetInt32(0),
            Nombre = lector.GetString(1),
            StockActual = lector.GetInt32(2),
            StockMinimo = lector.GetInt32(3)
        };
    }

    public List<MovimientoRegistro> ObtenerMovimientos()
    {
        var lista = new List<MovimientoRegistro>();

        using var conexion = new SqlConnection(_cadenaConexion);
        conexion.Open();

        const string sql = @"
            SELECT
                MI.IdMovimiento,
                MI.IdInventario,
                I.IdMedicamento,
                M.NombreMedicamento,
                MI.TipoMovimiento,
                MI.Cantidad,
                MI.FechaMovimiento,
                MI.Motivo,
                U.NombreUsuario
            FROM MovimientosInventario MI
            INNER JOIN Inventario I
                ON I.IdInventario = MI.IdInventario
            INNER JOIN Medicamentos M
                ON M.IdMedicamento = I.IdMedicamento
            INNER JOIN Usuarios U
                ON U.IdUsuario = MI.IdUsuario
            ORDER BY MI.FechaMovimiento DESC, MI.IdMovimiento DESC;";

        using var comando = new SqlCommand(sql, conexion);
        using var lector = comando.ExecuteReader();

        while (lector.Read())
        {
            lista.Add(new MovimientoRegistro
            {
                IdMovimiento = lector.GetInt32(0),
                IdInventario = lector.GetInt32(1),
                IdMedicamento = lector.GetInt32(2),
                Medicamento = lector.GetString(3),
                Tipo = lector.GetString(4),
                Cantidad = lector.GetInt32(5),
                Fecha = lector.GetDateTime(6),
                Motivo = lector.IsDBNull(7) ? "" : lector.GetString(7),
                Responsable = lector.IsDBNull(8) ? "" : lector.GetString(8)
            });
        }

        return lista;
    }

    public void RegistrarMovimiento(
        int idMedicamento,
        int idUsuario,
        string tipo,
        int cantidad,
        string motivo,
        DateTime fecha)
    {
        if (idMedicamento <= 0 || idUsuario <= 0)
            throw new InvalidOperationException("Medicamento o usuario inválido.");

        if (cantidad <= 0)
            throw new InvalidOperationException("La cantidad debe ser mayor que cero.");

        if (tipo is not ("Entrada" or "Salida" or "Ajuste positivo" or "Ajuste negativo"))
            throw new InvalidOperationException("Tipo de movimiento no válido.");

        using var conexion = new SqlConnection(_cadenaConexion);
        conexion.Open();

        using var transaccion = conexion.BeginTransaction();

        try
        {
            const string sqlInventario = @"
                SELECT IdInventario, StockActual
                FROM Inventario WITH (UPDLOCK, HOLDLOCK)
                WHERE IdMedicamento = @IdMedicamento
                ORDER BY
                    CASE WHEN FechaVencimiento IS NULL THEN 1 ELSE 0 END,
                    FechaVencimiento,
                    IdInventario;";

            int idInventario = 0;
            int stockActual = 0;
            int cantidadRegistros = 0;

            using (var comando = new SqlCommand(
                sqlInventario, conexion, transaccion))
            {
                comando.Parameters.AddWithValue(
                    "@IdMedicamento", idMedicamento);

                using var lector = comando.ExecuteReader();

                while (lector.Read())
                {
                    cantidadRegistros++;
                    idInventario = lector.GetInt32(0);
                    stockActual = lector.GetInt32(1);
                }
            }

            if (cantidadRegistros == 0)
                throw new InvalidOperationException(
                    "Este medicamento todavía no tiene un registro en Inventario.");

            if (cantidadRegistros > 1)
                throw new InvalidOperationException(
                    "Este medicamento tiene varios lotes. Se necesita seleccionar el lote antes de registrar el movimiento.");

            int nuevoStock = tipo switch
            {
                "Entrada" or "Ajuste positivo" => checked(stockActual + cantidad),
                "Salida" or "Ajuste negativo" => stockActual - cantidad,
                _ => stockActual
            };

            if (nuevoStock < 0)
                throw new InvalidOperationException(
                    "Stock insuficiente. Existencia actual: " + stockActual);

            const string sqlActualizar = @"
                UPDATE Inventario
                SET StockActual = @Stock,
                    FechaActualizacion = SYSDATETIME()
                WHERE IdInventario = @IdInventario;";

            using (var comando = new SqlCommand(
                sqlActualizar, conexion, transaccion))
            {
                comando.Parameters.AddWithValue("@Stock", nuevoStock);
                comando.Parameters.AddWithValue("@IdInventario", idInventario);
                comando.ExecuteNonQuery();
            }

            const string sqlMovimiento = @"
                INSERT INTO MovimientosInventario
                    (IdInventario, IdUsuario, TipoMovimiento, Cantidad, Motivo, FechaMovimiento)
                VALUES
                    (@IdInventario, @IdUsuario, @Tipo, @Cantidad, @Motivo, @Fecha);";

            using (var comando = new SqlCommand(
                sqlMovimiento, conexion, transaccion))
            {
                comando.Parameters.AddWithValue("@IdInventario", idInventario);
                comando.Parameters.AddWithValue("@IdUsuario", idUsuario);
                comando.Parameters.AddWithValue("@Tipo", tipo);
                comando.Parameters.AddWithValue("@Cantidad", cantidad);
                comando.Parameters.AddWithValue("@Motivo",
                    string.IsNullOrWhiteSpace(motivo) ? DBNull.Value : motivo);
                comando.Parameters.AddWithValue("@Fecha", fecha);

                comando.ExecuteNonQuery();
            }

            transaccion.Commit();
        }
        catch
        {
            transaccion.Rollback();
            throw;
        }
    }

    public void ActualizarMovimiento(
        int idMovimiento,
        int idMedicamento,
        int idUsuario,
        string tipo,
        int cantidad,
        string motivo,
        DateTime fecha)
    {
        if (cantidad <= 0)
            throw new InvalidOperationException("La cantidad debe ser mayor que cero.");

        if (idUsuario <= 0 || idMovimiento <= 0)
            throw new InvalidOperationException("Identificador inválido.");

        int nuevaVariacion = ObtenerVariacion(tipo, cantidad);

        using var conexion = new SqlConnection(_cadenaConexion);
        conexion.Open();

        using var transaccion = conexion.BeginTransaction();

        try
        {
            int idInventario;
            int stockActual;
            int variacionAnterior;
            int idMedicamentoAnterior;

            const string sqlAnterior = @"
                SELECT MI.IdInventario, MI.TipoMovimiento,
                       MI.Cantidad, I.StockActual, I.IdMedicamento
                FROM MovimientosInventario MI
                INNER JOIN Inventario I ON I.IdInventario = MI.IdInventario
                WHERE MI.IdMovimiento = @IdMovimiento;";

            using (var comando = new SqlCommand(
                sqlAnterior, conexion, transaccion))
            {
                comando.Parameters.AddWithValue("@IdMovimiento", idMovimiento);

                using var lector = comando.ExecuteReader();

                if (!lector.Read())
                    throw new InvalidOperationException("No existe el movimiento seleccionado.");

                idInventario = lector.GetInt32(0);
                variacionAnterior = ObtenerVariacion(
                    lector.GetString(1), lector.GetInt32(2));
                stockActual = lector.GetInt32(3);
                idMedicamentoAnterior = lector.GetInt32(4);
            }

            if (idMedicamentoAnterior != idMedicamento)
                throw new InvalidOperationException(
                    "No se puede cambiar el medicamento de un movimiento existente.");

            int stockRestaurado = checked(stockActual - variacionAnterior);
            int nuevoStock = checked(stockRestaurado + nuevaVariacion);

            if (nuevoStock < 0)
                throw new InvalidOperationException("La operación dejaría el stock negativo.");

            const string sqlActualizarStock = @"
                UPDATE Inventario
                SET StockActual = @Stock,
                    FechaActualizacion = SYSDATETIME()
                WHERE IdInventario = @IdInventario;";

            using (var comando = new SqlCommand(
                sqlActualizarStock, conexion, transaccion))
            {
                comando.Parameters.AddWithValue("@Stock", nuevoStock);
                comando.Parameters.AddWithValue("@IdInventario", idInventario);
                comando.ExecuteNonQuery();
            }

            const string sqlActualizarMovimiento = @"
                UPDATE MovimientosInventario
                SET TipoMovimiento = @Tipo,
                    Cantidad = @Cantidad,
                    Motivo = @Motivo,
                    FechaMovimiento = @Fecha,
                    IdUsuario = @IdUsuario
                WHERE IdMovimiento = @IdMovimiento;";

            using (var comando = new SqlCommand(
                sqlActualizarMovimiento, conexion, transaccion))
            {
                comando.Parameters.AddWithValue("@Tipo", tipo);
                comando.Parameters.AddWithValue("@Cantidad", cantidad);
                comando.Parameters.AddWithValue("@Motivo",
                    string.IsNullOrWhiteSpace(motivo) ? DBNull.Value : motivo);
                comando.Parameters.AddWithValue("@Fecha", fecha);
                comando.Parameters.AddWithValue("@IdUsuario", idUsuario);
                comando.Parameters.AddWithValue("@IdMovimiento", idMovimiento);
                comando.ExecuteNonQuery();
            }

            transaccion.Commit();
        }
        catch
        {
            transaccion.Rollback();
            throw;
        }
    }

    public void EliminarMovimiento(int idMovimiento)
    {
        using var conexion = new SqlConnection(_cadenaConexion);
        conexion.Open();

        using var transaccion = conexion.BeginTransaction();

        try
        {
            int idInventario;
            int cantidad;
            int stockActual;
            string tipo;

            const string sqlConsulta = @"
                SELECT MI.IdInventario, MI.TipoMovimiento,
                       MI.Cantidad, I.StockActual
                FROM MovimientosInventario MI
                INNER JOIN Inventario I ON I.IdInventario = MI.IdInventario
                WHERE MI.IdMovimiento = @IdMovimiento;";

            using (var comando = new SqlCommand(
                sqlConsulta, conexion, transaccion))
            {
                comando.Parameters.AddWithValue("@IdMovimiento", idMovimiento);

                using var lector = comando.ExecuteReader();

                if (!lector.Read())
                    throw new InvalidOperationException("No existe el movimiento.");

                idInventario = lector.GetInt32(0);
                tipo = lector.GetString(1);
                cantidad = lector.GetInt32(2);
                stockActual = lector.GetInt32(3);
            }

            int variacion = ObtenerVariacion(tipo, cantidad);
            int stockRestaurado = checked(stockActual - variacion);

            if (stockRestaurado < 0)
                throw new InvalidOperationException(
                    "No se puede revertir el movimiento porque produciría stock negativo.");

            const string sqlStock = @"
                UPDATE Inventario
                SET StockActual = @Stock,
                    FechaActualizacion = SYSDATETIME()
                WHERE IdInventario = @IdInventario;";

            using (var comando = new SqlCommand(
                sqlStock, conexion, transaccion))
            {
                comando.Parameters.AddWithValue("@Stock", stockRestaurado);
                comando.Parameters.AddWithValue("@IdInventario", idInventario);
                comando.ExecuteNonQuery();
            }

            using (var comando = new SqlCommand(
                "DELETE FROM MovimientosInventario WHERE IdMovimiento = @Id;",
                conexion, transaccion))
            {
                comando.Parameters.AddWithValue("@Id", idMovimiento);
                comando.ExecuteNonQuery();
            }

            transaccion.Commit();
        }
        catch
        {
            transaccion.Rollback();
            throw;
        }
    }

    private static int ObtenerVariacion(string tipo, int cantidad)
    {
        return tipo switch
        {
            "Entrada" or "Ajuste positivo" => cantidad,
            "Salida" or "Ajuste negativo" => -cantidad,
            _ => throw new InvalidOperationException("Tipo de movimiento no válido.")
        };
    }}

