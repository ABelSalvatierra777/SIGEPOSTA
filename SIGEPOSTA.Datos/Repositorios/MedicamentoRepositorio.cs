using Microsoft.Data.SqlClient;
using System.Data;
using SIGEPOSTA.Datos.Infraestructura;
using SIGEPOSTA.Datos.Interfaces;
using SIGEPOSTA.Entidades.Inventario;

namespace SIGEPOSTA.Datos.Repositorios;

public class MedicamentoRepositorio : IMedicamentoRepositorio
{
    private const string ConsultaBase = """
        SELECT
            m.IdMedicamento,
            m.Codigo,
            m.NombreMedicamento,
            m.PrincipioActivo,
            m.Descripcion,
            m.Concentracion,
            m.FormaFarmaceutica,
            m.Presentacion,
            m.UnidadMedida,
            m.Laboratorio,
            m.Categoria,
            m.Precio,
            m.Observacion,
            m.Estado,
            m.EstadoDescripcion,
            ISNULL(i.StockActual, 0) AS StockActual,
            ISNULL(i.StockMinimo, 0) AS StockMinimo,
            i.FechaVencimiento,
            i.Lote
        FROM Medicamentos m
        OUTER APPLY
        (
            SELECT TOP 1
                StockActual,
                StockMinimo,
                FechaVencimiento,
                Lote
            FROM Inventario
            WHERE IdMedicamento = m.IdMedicamento
            ORDER BY IdInventario DESC
        ) i
        """;

    public IReadOnlyList<Medicamento> ObtenerTodos()
    {
        var lista = new List<Medicamento>();

        using var conexion = ConexionBD.CrearConexion();
        conexion.Open();

        using var comando = new SqlCommand(
            ConsultaBase + " ORDER BY m.NombreMedicamento;",
            conexion);

        using var lector = comando.ExecuteReader();

        while (lector.Read())
            lista.Add(Mapear(lector));

        return lista;
    }

    public Medicamento? ObtenerPorId(int id)
    {
        using var conexion = ConexionBD.CrearConexion();
        conexion.Open();

        using var comando = new SqlCommand(
            ConsultaBase + " WHERE m.IdMedicamento = @Id;",
            conexion);

        comando.Parameters.Add("@Id", SqlDbType.Int).Value = id;

        using var lector = comando.ExecuteReader();

        return lector.Read() ? Mapear(lector) : null;
    }

    public void Agregar(Medicamento entidad)
    {
        using var conexion = ConexionBD.CrearConexion();
        conexion.Open();

        using var transaccion = conexion.BeginTransaction();

        try
        {
            const string sql = """
                INSERT INTO Medicamentos
                (
                    Codigo,
                    NombreMedicamento,
                    PrincipioActivo,
                    Descripcion,
                    Concentracion,
                    FormaFarmaceutica,
                    Presentacion,
                    UnidadMedida,
                    Laboratorio,
                    Categoria,
                    Precio,
                    Observacion,
                    Estado,
                    EstadoDescripcion,
                    FechaRegistro
                )
                OUTPUT INSERTED.IdMedicamento
                VALUES
                (
                    @Codigo,
                    @Nombre,
                    @PrincipioActivo,
                    @Descripcion,
                    @Concentracion,
                    @FormaFarmaceutica,
                    @Presentacion,
                    @UnidadMedida,
                    @Laboratorio,
                    @Categoria,
                    @Precio,
                    @Observacion,
                    @Estado,
                    @EstadoDescripcion,
                    SYSDATETIME()
                );
                """;

            using var comando = new SqlCommand(sql, conexion, transaccion);
            AgregarParametros(comando, entidad);

            entidad.IdMedicamento =
                Convert.ToInt32(comando.ExecuteScalar());

            GuardarInventario(conexion, transaccion, entidad);

            transaccion.Commit();
        }
        catch
        {
            transaccion.Rollback();
            throw;
        }
    }

    public void Actualizar(Medicamento entidad)
    {
        using var conexion = ConexionBD.CrearConexion();
        conexion.Open();

        using var transaccion = conexion.BeginTransaction();

        try
        {
            const string sql = """
                UPDATE Medicamentos SET
                    Codigo = @Codigo,
                    NombreMedicamento = @Nombre,
                    PrincipioActivo = @PrincipioActivo,
                    Descripcion = @Descripcion,
                    Concentracion = @Concentracion,
                    FormaFarmaceutica = @FormaFarmaceutica,
                    Presentacion = @Presentacion,
                    UnidadMedida = @UnidadMedida,
                    Laboratorio = @Laboratorio,
                    Categoria = @Categoria,
                    Precio = @Precio,
                    Observacion = @Observacion,
                    Estado = @Estado,
                    EstadoDescripcion = @EstadoDescripcion
                WHERE IdMedicamento = @IdMedicamento;
                """;

            using var comando = new SqlCommand(sql, conexion, transaccion);
            AgregarParametros(comando, entidad);

            comando.Parameters.Add("@IdMedicamento", SqlDbType.Int)
                .Value = entidad.IdMedicamento;

            if (comando.ExecuteNonQuery() == 0)
                throw new InvalidOperationException(
                    "No se encontró el medicamento seleccionado.");

            GuardarInventario(conexion, transaccion, entidad);

            transaccion.Commit();
        }
        catch
        {
            transaccion.Rollback();
            throw;
        }
    }

    public void Eliminar(int id)
    {
        using var conexion = ConexionBD.CrearConexion();
        conexion.Open();

        // Baja lógica: conserva los movimientos y el historial.
        using var comando = new SqlCommand(
            """
            UPDATE Medicamentos
            SET Estado = 0,
                EstadoDescripcion = 'Retirado'
            WHERE IdMedicamento = @Id;
            """,
            conexion);

        comando.Parameters.Add("@Id", SqlDbType.Int).Value = id;

        if (comando.ExecuteNonQuery() == 0)
            throw new InvalidOperationException(
                "No se encontró el medicamento que desea retirar.");
    }

    private static void AgregarParametros(
        SqlCommand comando,
        Medicamento m)
    {
        comando.Parameters.Add("@Codigo", SqlDbType.NVarChar, 50)
            .Value = Valor(m.Codigo);

        comando.Parameters.Add("@Nombre", SqlDbType.NVarChar, 300)
            .Value = m.Nombre;

        comando.Parameters.Add("@PrincipioActivo", SqlDbType.NVarChar, 300)
            .Value = Valor(m.PrincipioActivo);

        comando.Parameters.Add("@Descripcion", SqlDbType.NVarChar, 600)
            .Value = Valor(m.Descripcion);

        comando.Parameters.Add("@Concentracion", SqlDbType.NVarChar, 200)
            .Value = Valor(m.Concentracion);

        comando.Parameters.Add("@FormaFarmaceutica", SqlDbType.NVarChar, 160)
            .Value = Valor(m.FormaFarmaceutica);

        comando.Parameters.Add("@Presentacion", SqlDbType.NVarChar, 200)
            .Value = Valor(m.Presentacion);

        comando.Parameters.Add("@UnidadMedida", SqlDbType.NVarChar, 100)
            .Value = Valor(m.UnidadMedida);

        comando.Parameters.Add("@Laboratorio", SqlDbType.NVarChar, 200)
            .Value = Valor(m.Laboratorio);

        comando.Parameters.Add("@Categoria", SqlDbType.NVarChar, 100)
            .Value = Valor(m.Categoria);

        comando.Parameters.Add("@Precio", SqlDbType.Decimal)
            .Value = m.Precio;

        comando.Parameters.Add("@Observacion", SqlDbType.NVarChar, 1000)
            .Value = Valor(m.Observacion);

        comando.Parameters.Add("@Estado", SqlDbType.Bit)
            .Value = m.Estado;

        comando.Parameters.Add("@EstadoDescripcion", SqlDbType.NVarChar, 30)
            .Value = m.EstadoDescripcion;
    }

    private static void GuardarInventario(
        SqlConnection conexion,
        SqlTransaction transaccion,
        Medicamento m)
    {
        const string sql = """
            DECLARE @IdInventario INT;

            SELECT TOP 1 @IdInventario = IdInventario
            FROM Inventario
            WHERE IdMedicamento = @IdMedicamento
            ORDER BY IdInventario DESC;

            IF @IdInventario IS NULL
            BEGIN
                INSERT INTO Inventario
                (
                    IdMedicamento,
                    StockActual,
                    StockMinimo,
                    FechaVencimiento,
                    Lote,
                    FechaActualizacion
                )
                VALUES
                (
                    @IdMedicamento,
                    @Stock,
                    @StockMinimo,
                    @Vencimiento,
                    @Lote,
                    SYSDATETIME()
                );
            END
            ELSE
            BEGIN
                UPDATE Inventario SET
                    StockActual = @Stock,
                    StockMinimo = @StockMinimo,
                    FechaVencimiento = @Vencimiento,
                    Lote = @Lote,
                    FechaActualizacion = SYSDATETIME()
                WHERE IdInventario = @IdInventario;
            END
            """;

        using var comando = new SqlCommand(sql, conexion, transaccion);

        comando.Parameters.Add("@IdMedicamento", SqlDbType.Int)
            .Value = m.IdMedicamento;

        comando.Parameters.Add("@Stock", SqlDbType.Int)
            .Value = m.Stock;

        comando.Parameters.Add("@StockMinimo", SqlDbType.Int)
            .Value = m.StockMinimo;

        comando.Parameters.Add("@Vencimiento", SqlDbType.Date)
            .Value = (object?)m.FechaVencimiento ?? DBNull.Value;

        comando.Parameters.Add("@Lote", SqlDbType.NVarChar, 100)
            .Value = Valor(m.Lote);

        comando.ExecuteNonQuery();
    }

    private static Medicamento Mapear(SqlDataReader lector)
    {
        return new Medicamento
        {
            IdMedicamento = lector.GetInt32(0),
            Codigo = Texto(lector, 1),
            Nombre = Texto(lector, 2),
            PrincipioActivo = Texto(lector, 3),
            Descripcion = Texto(lector, 4),
            Concentracion = Texto(lector, 5),
            FormaFarmaceutica = Texto(lector, 6),
            Presentacion = Texto(lector, 7),
            UnidadMedida = Texto(lector, 8),
            Laboratorio = Texto(lector, 9),
            Categoria = Texto(lector, 10),
            Precio = lector.GetDecimal(11),
            Observacion = Texto(lector, 12),
            Estado = lector.GetBoolean(13),
            EstadoDescripcion = Texto(lector, 14),
            Stock = lector.GetInt32(15),
            StockMinimo = lector.GetInt32(16),
            FechaVencimiento = lector.IsDBNull(17)
                ? null : lector.GetDateTime(17),
            Lote = Texto(lector, 18)
        };
    }

    private static string Texto(SqlDataReader lector, int indice)
    {
        return lector.IsDBNull(indice)
            ? ""
            : lector.GetString(indice);
    }

    private static object Valor(string? texto)
    {
        return string.IsNullOrWhiteSpace(texto)
            ? DBNull.Value
            : texto.Trim();
    }
}
