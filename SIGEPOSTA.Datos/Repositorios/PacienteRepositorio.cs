using Microsoft.Data.SqlClient;
using SIGEPOSTA.Datos.Infraestructura;
using SIGEPOSTA.Datos.Interfaces;
using SIGEPOSTA.Entidades.Personas;

namespace SIGEPOSTA.Datos.Repositorios;

public class PacienteRepositorio : IPacienteRepositorio
{
    public IReadOnlyList<Paciente> ObtenerTodos()
    {
        var lista = new List<Paciente>();

        using var conexion = ConexionBD.CrearConexion();
        conexion.Open();

        string sql = """
            SELECT IdPaciente, DNI, Nombres, Apellidos, Sexo,
                   FechaNacimiento, Telefono, Direccion, Correo,
                   FechaRegistro, Estado
            FROM Pacientes
            ORDER BY Nombres, Apellidos;
            """;

        using var comando = new SqlCommand(sql, conexion);
        using var lector = comando.ExecuteReader();

        while (lector.Read())
            lista.Add(MapearPaciente(lector));

        return lista;
    }

    public Paciente? ObtenerPorId(int id)
    {
        using var conexion = ConexionBD.CrearConexion();
        conexion.Open();

        string sql = """
            SELECT IdPaciente, DNI, Nombres, Apellidos, Sexo,
                   FechaNacimiento, Telefono, Direccion, Correo,
                   FechaRegistro, Estado
            FROM Pacientes
            WHERE IdPaciente = @Id;
            """;

        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@Id", id);

        using var lector = comando.ExecuteReader();

        return lector.Read() ? MapearPaciente(lector) : null;
    }

    public void Agregar(Paciente entidad)
    {
        using var conexion = ConexionBD.CrearConexion();
        conexion.Open();

        string sql = """
            INSERT INTO Pacientes
            (DNI, Nombres, Apellidos, Sexo, FechaNacimiento,
             Telefono, Direccion, Correo, FechaRegistro, Estado)
            VALUES
            (@DNI, @Nombres, @Apellidos, @Sexo, @FechaNacimiento,
             @Telefono, @Direccion, @Correo, @FechaRegistro, @Estado);
            """;

        using var comando = new SqlCommand(sql, conexion);
        AgregarParametros(comando, entidad);
        comando.ExecuteNonQuery();
    }

    public void Actualizar(Paciente entidad)
    {
        using var conexion = ConexionBD.CrearConexion();
        conexion.Open();

        string sql = """
            UPDATE Pacientes SET
                DNI = @DNI,
                Nombres = @Nombres,
                Apellidos = @Apellidos,
                Sexo = @Sexo,
                FechaNacimiento = @FechaNacimiento,
                Telefono = @Telefono,
                Direccion = @Direccion,
                Correo = @Correo,
                Estado = @Estado
            WHERE IdPaciente = @Id;
            """;

        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@Id", entidad.IdPaciente);
        AgregarParametros(comando, entidad);
        comando.ExecuteNonQuery();
    }

    public void Eliminar(int id)
    {
        using var conexion = ConexionBD.CrearConexion();
        conexion.Open();

        string sql = """
            UPDATE Pacientes
            SET Estado = 0
            WHERE IdPaciente = @Id;
            """;

        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@Id", id);
        comando.ExecuteNonQuery();
    }

    private static void AgregarParametros(
        SqlCommand comando, Paciente entidad)
    {
        string apellidos =
            $"{entidad.ApellidoPaterno} {entidad.ApellidoMaterno}".Trim();

        comando.Parameters.AddWithValue(
            "@DNI", entidad.NumeroDocumento.Trim());

        comando.Parameters.AddWithValue("@Nombres", entidad.Nombres);
        comando.Parameters.AddWithValue("@Apellidos", apellidos);
        comando.Parameters.AddWithValue("@Sexo", entidad.Sexo);

        comando.Parameters.AddWithValue(
            "@FechaNacimiento",
            (object?)entidad.FechaNacimiento ?? DBNull.Value);

        comando.Parameters.AddWithValue("@Telefono", entidad.Telefono);
        comando.Parameters.AddWithValue("@Direccion", entidad.Direccion);
        comando.Parameters.AddWithValue(
            "@Correo", entidad.CorreoElectronico);

        comando.Parameters.AddWithValue(
            "@FechaRegistro", entidad.FechaRegistroPaciente);

        comando.Parameters.AddWithValue("@Estado", entidad.Estado);
    }

    private static Paciente MapearPaciente(SqlDataReader lector)
    {
        string apellidos = lector.IsDBNull(3)
            ? ""
            : lector.GetString(3);

        return new Paciente
        {
            IdPaciente = lector.GetInt32(0),
            IdPersona = lector.GetInt32(0),
            TipoDocumento = "DNI",
            NumeroDocumento = lector.IsDBNull(1)
                ? ""
                : lector.GetString(1).Trim(),
            Nombres = lector.IsDBNull(2) ? "" : lector.GetString(2),
            ApellidoPaterno = apellidos,
            ApellidoMaterno = "",
            Sexo = lector.IsDBNull(4) ? "" : lector.GetString(4),
            FechaNacimiento = lector.IsDBNull(5)
                ? null
                : lector.GetDateTime(5),
            Telefono = lector.IsDBNull(6) ? "" : lector.GetString(6),
            Direccion = lector.IsDBNull(7) ? "" : lector.GetString(7),
            CorreoElectronico = lector.IsDBNull(8)
                ? ""
                : lector.GetString(8),
            FechaRegistro = lector.GetDateTime(9),
            FechaRegistroPaciente = lector.GetDateTime(9),
            Estado = lector.GetBoolean(10)
        };
    }
}
