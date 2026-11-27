using Microsoft.Data.SqlClient;
using SIGEPOSTA.Datos.Infraestructura;
using SIGEPOSTA.Datos.Interfaces;
using SIGEPOSTA.Entidades.Clinica;

namespace SIGEPOSTA.Datos.Repositorios;

public class HistoriaClinicaRepositorio : IHistoriaClinicaRepositorio
{
    private const string Consulta = """
        SELECT
            h.IdHistoriaClinica,
            h.IdPaciente,
            h.GrupoSanguineo,
            h.Alergias,
            h.AntecedentesPersonales,
            h.AntecedentesFamiliares,
            h.Observaciones,
            h.FechaCreacion,
            p.DNI,
            p.Nombres,
            p.Apellidos
        FROM HistoriasClinicas h
        INNER JOIN Pacientes p ON p.IdPaciente = h.IdPaciente
        """;

    public IReadOnlyList<HistoriaClinica> ObtenerTodos()
    {
        var lista = new List<HistoriaClinica>();

        using var conexion = ConexionBD.CrearConexion();
        conexion.Open();

        using var comando = new SqlCommand(
            Consulta + " ORDER BY h.FechaCreacion DESC;",
            conexion);

        using var lector = comando.ExecuteReader();

        while (lector.Read())
            lista.Add(Mapear(lector));

        return lista;
    }

    public HistoriaClinica? ObtenerPorId(int id)
    {
        using var conexion = ConexionBD.CrearConexion();
        conexion.Open();

        using var comando = new SqlCommand(
            Consulta + " WHERE h.IdHistoriaClinica = @Id;",
            conexion);

        comando.Parameters.AddWithValue("@Id", id);

        using var lector = comando.ExecuteReader();

        return lector.Read() ? Mapear(lector) : null;
    }

    public void Agregar(HistoriaClinica entidad)
    {
        using var conexion = ConexionBD.CrearConexion();
        conexion.Open();

        const string sql = """
            INSERT INTO HistoriasClinicas
            (
                IdPaciente,
                GrupoSanguineo,
                Alergias,
                AntecedentesPersonales,
                AntecedentesFamiliares,
                Observaciones,
                FechaCreacion
            )
            OUTPUT INSERTED.IdHistoriaClinica
            VALUES
            (
                @IdPaciente,
                @GrupoSanguineo,
                @Alergias,
                @AntecedentesPersonales,
                @AntecedentesFamiliares,
                @Observaciones,
                @FechaCreacion
            );
            """;

        using var comando = new SqlCommand(sql, conexion);
        AgregarParametros(comando, entidad);

        entidad.IdHistoriaClinica =
            Convert.ToInt32(comando.ExecuteScalar());
    }

    public void Actualizar(HistoriaClinica entidad)
    {
        using var conexion = ConexionBD.CrearConexion();
        conexion.Open();

        const string sql = """
            UPDATE HistoriasClinicas SET
                IdPaciente = @IdPaciente,
                GrupoSanguineo = @GrupoSanguineo,
                Alergias = @Alergias,
                AntecedentesPersonales = @AntecedentesPersonales,
                AntecedentesFamiliares = @AntecedentesFamiliares,
                Observaciones = @Observaciones,
                FechaActualizacion = SYSDATETIME()
            WHERE IdHistoriaClinica = @Id;
            """;

        using var comando = new SqlCommand(sql, conexion);

        comando.Parameters.AddWithValue("@Id", entidad.IdHistoriaClinica);
        AgregarParametros(comando, entidad);

        if (comando.ExecuteNonQuery() == 0)
            throw new InvalidOperationException(
                "No se encontró la historia clínica.");
    }

    public void Eliminar(int id)
    {
        using var conexion = ConexionBD.CrearConexion();
        conexion.Open();

        const string sql = """
            DELETE FROM HistoriasClinicas
            WHERE IdHistoriaClinica = @Id;
            """;

        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@Id", id);

        if (comando.ExecuteNonQuery() == 0)
            throw new InvalidOperationException(
                "No se encontró la historia clínica.");
    }

    private static void AgregarParametros(
        SqlCommand comando, HistoriaClinica entidad)
    {
        comando.Parameters.AddWithValue(
            "@IdPaciente", entidad.IdPaciente);

        comando.Parameters.AddWithValue(
            "@GrupoSanguineo",
            (object?)entidad.GrupoSanguineo ?? DBNull.Value);

        comando.Parameters.AddWithValue(
            "@Alergias",
            (object?)entidad.Alergias ?? DBNull.Value);

        comando.Parameters.AddWithValue(
            "@AntecedentesPersonales",
            (object?)entidad.AntecedentesPersonales ?? DBNull.Value);

        comando.Parameters.AddWithValue(
            "@AntecedentesFamiliares",
            (object?)entidad.AntecedentesFamiliares ?? DBNull.Value);

        comando.Parameters.AddWithValue(
            "@Observaciones",
            (object?)entidad.Observaciones ?? DBNull.Value);

        comando.Parameters.AddWithValue(
            "@FechaCreacion", entidad.FechaApertura);
    }

    private static HistoriaClinica Mapear(SqlDataReader lector)
    {
        string dni = lector.IsDBNull(8) ? "" : lector.GetString(8).Trim();
        string nombres = lector.IsDBNull(9) ? "" : lector.GetString(9);
        string apellidos = lector.IsDBNull(10) ? "" : lector.GetString(10);

        return new HistoriaClinica
        {
            IdHistoriaClinica = lector.GetInt32(0),
            IdPaciente = lector.GetInt32(1),
            NumeroHistoria = "HC-" + lector.GetInt32(0),
            GrupoSanguineo = lector.IsDBNull(2) ? "" : lector.GetString(2),
            Alergias = lector.IsDBNull(3) ? "" : lector.GetString(3),
            AntecedentesPersonales =
                lector.IsDBNull(4) ? "" : lector.GetString(4),
            AntecedentesFamiliares =
                lector.IsDBNull(5) ? "" : lector.GetString(5),
            Observaciones = lector.IsDBNull(6) ? "" : lector.GetString(6),
            FechaApertura = lector.GetDateTime(7),
            Estado = true
        };
    }
}
