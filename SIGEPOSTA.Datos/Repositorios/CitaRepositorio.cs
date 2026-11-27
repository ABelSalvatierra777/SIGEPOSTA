using Microsoft.Data.SqlClient;
using System.Data;
using SIGEPOSTA.Datos.Infraestructura;
using SIGEPOSTA.Datos.Interfaces;
using SIGEPOSTA.Entidades.Clinica;

namespace SIGEPOSTA.Datos.Repositorios;

public class CitaRepositorio : ICitaRepositorio
{
    public IReadOnlyList<Cita> ObtenerTodos()
    {
        var lista = new List<Cita>();

        using var conexion = ConexionBD.CrearConexion();
        conexion.Open();

        string sql = """
            SELECT IdCita, IdPaciente, IdPersonalSalud,
                   FechaCita, HoraCita, MotivoConsulta,
                   Estado, FechaRegistro
            FROM Citas
            ORDER BY FechaCita DESC, HoraCita DESC;
            """;

        using var comando = new SqlCommand(sql, conexion);
        using var lector = comando.ExecuteReader();

        while (lector.Read())
            lista.Add(MapearCita(lector));

        return lista;
    }

    public Cita? ObtenerPorId(int id)
    {
        using var conexion = ConexionBD.CrearConexion();
        conexion.Open();

        string sql = """
            SELECT IdCita, IdPaciente, IdPersonalSalud,
                   FechaCita, HoraCita, MotivoConsulta,
                   Estado, FechaRegistro
            FROM Citas
            WHERE IdCita = @Id;
            """;

        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@Id", id);

        using var lector = comando.ExecuteReader();

        return lector.Read() ? MapearCita(lector) : null;
    }

    public void Agregar(Cita entidad)
    {
        using var conexion = ConexionBD.CrearConexion();
        conexion.Open();

        string sql = """
            INSERT INTO Citas
            (IdPaciente, IdPersonalSalud, FechaCita,
             HoraCita, MotivoConsulta, Estado)
            VALUES
            (@IdPaciente, @IdPersonalSalud, @FechaCita,
             @HoraCita, @MotivoConsulta, @Estado);
            """;

        using var comando = new SqlCommand(sql, conexion);

        AgregarParametros(comando, entidad);
        comando.ExecuteNonQuery();
    }

    public void Actualizar(Cita entidad)
    {
        using var conexion = ConexionBD.CrearConexion();
        conexion.Open();

        string sql = """
            UPDATE Citas SET
                IdPaciente = @IdPaciente,
                IdPersonalSalud = @IdPersonalSalud,
                FechaCita = @FechaCita,
                HoraCita = @HoraCita,
                MotivoConsulta = @MotivoConsulta,
                Estado = @Estado
            WHERE IdCita = @Id;
            """;

        using var comando = new SqlCommand(sql, conexion);

        comando.Parameters.AddWithValue("@Id", entidad.IdCita);
        AgregarParametros(comando, entidad);
        comando.ExecuteNonQuery();
    }

    public void Eliminar(int id)
    {
        using var conexion = ConexionBD.CrearConexion();
        conexion.Open();

        string sql = """
            UPDATE Citas
            SET Estado = N'Cancelada'
            WHERE IdCita = @Id;
            """;

        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@Id", id);
        comando.ExecuteNonQuery();
    }

    private static void AgregarParametros(
        SqlCommand comando, Cita entidad)
    {
        comando.Parameters.AddWithValue(
            "@IdPaciente", entidad.IdPaciente);

        comando.Parameters.Add("@IdPersonalSalud", SqlDbType.Int).Value =
            (object?)entidad.IdPersonalSalud ?? DBNull.Value;

        comando.Parameters.Add("@FechaCita", SqlDbType.Date).Value =
            entidad.FechaHora.Date;

        comando.Parameters.Add("@HoraCita", SqlDbType.Time).Value =
            entidad.FechaHora.TimeOfDay;

        comando.Parameters.AddWithValue(
            "@MotivoConsulta", entidad.Motivo);

        comando.Parameters.AddWithValue(
            "@Estado", entidad.Estado);
    }

    private static Cita MapearCita(SqlDataReader lector)
    {
        DateTime fecha = lector.GetDateTime(3);
        TimeSpan hora = lector.GetTimeSpan(4);

        return new Cita
        {
            IdCita = lector.GetInt32(0),
            IdPaciente = lector.GetInt32(1),
            IdPersonalSalud = lector.IsDBNull(2)
                ? null
                : lector.GetInt32(2),
            FechaHora = fecha.Date.Add(hora),
            Motivo = lector.IsDBNull(5) ? "" : lector.GetString(5),
            Estado = lector.IsDBNull(6) ? "" : lector.GetString(6),
            FechaRegistro = lector.GetDateTime(7)
        };
    }
}
