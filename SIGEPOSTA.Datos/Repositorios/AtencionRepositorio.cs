using Microsoft.Data.SqlClient;
using SIGEPOSTA.Datos.Infraestructura;
using SIGEPOSTA.Datos.Interfaces;
using SIGEPOSTA.Entidades.Clinica;
using System.Data;

namespace SIGEPOSTA.Datos.Repositorios;

public class AtencionRepositorio : IAtencionRepositorio
{
    private const string ConsultaBase = """
        SELECT
            a.IdAtencion,
            a.IdCita,
            a.IdPaciente,
            a.IdPersonalSalud,
            a.FechaAtencion,
            a.MotivoConsulta,
            a.Diagnostico,
            a.Tratamiento,
            a.Observaciones,
            a.Estado,
            p.DNI,
            p.Nombres,
            p.Apellidos,
            ps.DNI,
            ps.Nombres,
            ps.Apellidos,
            sv.Temperatura,
            sv.PresionArterial,
            sv.Peso,
            sv.Talla
        FROM Atenciones a
        INNER JOIN Pacientes p
            ON p.IdPaciente = a.IdPaciente
        INNER JOIN PersonalSalud ps
            ON ps.IdPersonalSalud = a.IdPersonalSalud
        OUTER APPLY
        (
            SELECT TOP 1
                s.Temperatura,
                s.PresionArterial,
                s.Peso,
                s.Talla
            FROM SignosVitales s
            WHERE s.IdAtencion = a.IdAtencion
            ORDER BY s.FechaRegistro DESC, s.IdSignoVital DESC
        ) sv
        """;

    public IReadOnlyList<Atencion> ObtenerTodos()
    {
        var lista = new List<Atencion>();

        using var conexion = ConexionBD.CrearConexion();
        conexion.Open();

        using var comando = new SqlCommand(
            ConsultaBase + " ORDER BY a.FechaAtencion DESC, a.IdAtencion DESC;",
            conexion);

        using var lector = comando.ExecuteReader();

        while (lector.Read())
            lista.Add(Mapear(lector));

        return lista;
    }

    public Atencion? ObtenerPorId(int id)
    {
        using var conexion = ConexionBD.CrearConexion();
        conexion.Open();

        using var comando = new SqlCommand(
            ConsultaBase + " WHERE a.IdAtencion = @Id;",
            conexion);

        comando.Parameters.Add("@Id", SqlDbType.Int).Value = id;

        using var lector = comando.ExecuteReader();

        return lector.Read() ? Mapear(lector) : null;
    }

    public void Agregar(Atencion entidad)
    {
        using var conexion = ConexionBD.CrearConexion();
        conexion.Open();

        using var transaccion = conexion.BeginTransaction();

        try
        {
            const string sql = """
                INSERT INTO Atenciones
                (
                    IdCita,
                    IdPaciente,
                    IdPersonalSalud,
                    FechaAtencion,
                    MotivoConsulta,
                    Diagnostico,
                    Tratamiento,
                    Observaciones,
                    Estado
                )
                OUTPUT INSERTED.IdAtencion
                VALUES
                (
                    @IdCita,
                    @IdPaciente,
                    @IdPersonalSalud,
                    @FechaAtencion,
                    @MotivoConsulta,
                    @Diagnostico,
                    @Tratamiento,
                    @Observaciones,
                    @Estado
                );
                """;

            using var comando = new SqlCommand(sql, conexion, transaccion);

            AgregarParametrosAtencion(comando, entidad);

            int id = Convert.ToInt32(comando.ExecuteScalar());
            entidad.IdAtencion = id;

            GuardarSignosVitales(conexion, transaccion, entidad);

            transaccion.Commit();
        }
        catch
        {
            transaccion.Rollback();
            throw;
        }
    }

    public void Actualizar(Atencion entidad)
    {
        using var conexion = ConexionBD.CrearConexion();
        conexion.Open();

        using var transaccion = conexion.BeginTransaction();

        try
        {
            const string sql = """
                UPDATE Atenciones SET
                    IdPaciente = @IdPaciente,
                    IdPersonalSalud = @IdPersonalSalud,
                    FechaAtencion = @FechaAtencion,
                    MotivoConsulta = @MotivoConsulta,
                    Diagnostico = @Diagnostico,
                    Tratamiento = @Tratamiento,
                    Observaciones = @Observaciones,
                    Estado = @Estado
                WHERE IdAtencion = @IdAtencion;
                """;

            using var comando = new SqlCommand(sql, conexion, transaccion);

            AgregarParametrosAtencion(comando, entidad);
            comando.Parameters.Add("@IdAtencion", SqlDbType.Int)
                .Value = entidad.IdAtencion;

            int afectados = comando.ExecuteNonQuery();

            if (afectados == 0)
                throw new InvalidOperationException(
                    "No se encontró la atención que desea actualizar.");

            GuardarSignosVitales(conexion, transaccion, entidad);

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

        using var transaccion = conexion.BeginTransaction();

        try
        {
            // Primero se eliminan los signos vitales relacionados.
            using (var comandoSignos = new SqlCommand(
                "DELETE FROM SignosVitales WHERE IdAtencion = @Id;",
                conexion, transaccion))
            {
                comandoSignos.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                comandoSignos.ExecuteNonQuery();
            }

            using (var comando = new SqlCommand(
                "DELETE FROM Atenciones WHERE IdAtencion = @Id;",
                conexion, transaccion))
            {
                comando.Parameters.Add("@Id", SqlDbType.Int).Value = id;

                if (comando.ExecuteNonQuery() == 0)
                    throw new InvalidOperationException(
                        "No se encontró la atención que desea eliminar.");
            }

            transaccion.Commit();
        }
        catch
        {
            transaccion.Rollback();
            throw;
        }
    }

    private static void AgregarParametrosAtencion(
        SqlCommand comando, Atencion entidad)
    {
        comando.Parameters.Add("@IdCita", SqlDbType.Int).Value =
            (object?)entidad.IdCita ?? DBNull.Value;

        comando.Parameters.Add("@IdPaciente", SqlDbType.Int).Value =
            entidad.IdPaciente;

        comando.Parameters.Add("@IdPersonalSalud", SqlDbType.Int).Value =
            entidad.IdPersonalSalud;

        comando.Parameters.Add("@FechaAtencion", SqlDbType.DateTime2).Value =
            entidad.FechaAtencion;

        // La tabla actual no tiene una columna Sintomas independiente.
        // Se utiliza MotivoConsulta para guardar el texto de síntomas
        // cuando el formulario no proporciona un motivo separado.
        string motivo = string.IsNullOrWhiteSpace(entidad.MotivoConsulta)
            ? entidad.Sintomas
            : entidad.MotivoConsulta;

        comando.Parameters.Add("@MotivoConsulta", SqlDbType.NVarChar, 300)
            .Value = (object)motivo ?? DBNull.Value;

        comando.Parameters.Add("@Diagnostico", SqlDbType.NVarChar, 500)
            .Value = (object)entidad.ExamenFisico ?? DBNull.Value;

        comando.Parameters.Add("@Tratamiento", SqlDbType.NVarChar, 1000)
            .Value = (object)entidad.Tratamiento ?? DBNull.Value;

        comando.Parameters.Add("@Observaciones", SqlDbType.NVarChar, 1000)
            .Value = (object)entidad.Observaciones ?? DBNull.Value;

        comando.Parameters.Add("@Estado", SqlDbType.Bit).Value =
            entidad.Estado;
    }

    private static void GuardarSignosVitales(
        SqlConnection conexion,
        SqlTransaction transaccion,
        Atencion entidad)
    {
        const string sql = """
            IF EXISTS
            (
                SELECT 1
                FROM SignosVitales
                WHERE IdAtencion = @IdAtencion
            )
            BEGIN
                UPDATE SignosVitales SET
                    Temperatura = @Temperatura,
                    PresionArterial = @PresionArterial,
                    Peso = @Peso,
                    Talla = @Talla
                WHERE IdAtencion = @IdAtencion;
            END
            ELSE
            BEGIN
                INSERT INTO SignosVitales
                (
                    IdAtencion,
                    Temperatura,
                    PresionArterial,
                    Peso,
                    Talla,
                    FechaRegistro
                )
                VALUES
                (
                    @IdAtencion,
                    @Temperatura,
                    @PresionArterial,
                    @Peso,
                    @Talla,
                    SYSDATETIME()
                );
            END
            """;

        using var comando = new SqlCommand(sql, conexion, transaccion);

        comando.Parameters.Add("@IdAtencion", SqlDbType.Int)
            .Value = entidad.IdAtencion;

        comando.Parameters.Add("@Temperatura", SqlDbType.Decimal).Value =
            (object?)entidad.Temperatura ?? DBNull.Value;

        comando.Parameters.Add("@PresionArterial", SqlDbType.NVarChar, 40)
            .Value = string.IsNullOrWhiteSpace(entidad.PresionArterial)
                ? DBNull.Value
                : entidad.PresionArterial;

        comando.Parameters.Add("@Peso", SqlDbType.Decimal).Value =
            (object?)entidad.Peso ?? DBNull.Value;

        comando.Parameters.Add("@Talla", SqlDbType.Decimal).Value =
            (object?)entidad.Talla ?? DBNull.Value;

        comando.ExecuteNonQuery();
    }

    private static Atencion Mapear(SqlDataReader lector)
    {
        string apellidosPaciente = lector.IsDBNull(12)
            ? "" : lector.GetString(12);

        string apellidosPersonal = lector.IsDBNull(15)
            ? "" : lector.GetString(15);

        string motivo = lector.IsDBNull(5) ? "" : lector.GetString(5);

        return new Atencion
        {
            IdAtencion = lector.GetInt32(0),
            IdCita = lector.IsDBNull(1) ? null : lector.GetInt32(1),
            IdPaciente = lector.GetInt32(2),
            IdPersonalSalud = lector.GetInt32(3),
            FechaAtencion = lector.GetDateTime(4),

            MotivoConsulta = motivo,
            Sintomas = motivo,
            ExamenFisico = lector.IsDBNull(6) ? "" : lector.GetString(6),
            Tratamiento = lector.IsDBNull(7) ? "" : lector.GetString(7),
            Observaciones = lector.IsDBNull(8) ? "" : lector.GetString(8),
            Estado = lector.GetBoolean(9),

            DniPaciente = lector.IsDBNull(10) ? "" : lector.GetString(10).Trim(),
            NombrePaciente =
                $"{lector.GetString(11)} {apellidosPaciente}".Trim(),

            DniPersonalSalud =
                lector.IsDBNull(13) ? "" : lector.GetString(13).Trim(),

            NombrePersonalSalud =
                $"{lector.GetString(14)} {apellidosPersonal}".Trim(),

            Temperatura = lector.IsDBNull(16) ? null : lector.GetDecimal(16),
            PresionArterial = lector.IsDBNull(17) ? "" : lector.GetString(17),
            Peso = lector.IsDBNull(18) ? null : lector.GetDecimal(18),
            Talla = lector.IsDBNull(19) ? null : lector.GetDecimal(19)
        };
    }
}
