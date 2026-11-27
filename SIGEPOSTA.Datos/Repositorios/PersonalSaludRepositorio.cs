using Microsoft.Data.SqlClient;
using SIGEPOSTA.Datos.Infraestructura;
using SIGEPOSTA.Datos.Interfaces;
using SIGEPOSTA.Entidades.Personas;

namespace SIGEPOSTA.Datos.Repositorios;

public class PersonalSaludRepositorio : IPersonalSaludRepositorio
{
    public IReadOnlyList<PersonalSalud> ObtenerTodos()
    {
        var lista = new List<PersonalSalud>();

        using var conexion = ConexionBD.CrearConexion();
        conexion.Open();

        string sql = """
            SELECT IdPersonalSalud, DNI, Nombres, Apellidos, Sexo,
                   FechaNacimiento, Telefono, Correo, Direccion,
                   Especialidad, Cargo, Estado, FechaRegistro
            FROM PersonalSalud
            ORDER BY Nombres, Apellidos;
            """;

        using var comando = new SqlCommand(sql, conexion);
        using var lector = comando.ExecuteReader();

        while (lector.Read())
            lista.Add(MapearPersonal(lector));

        return lista;
    }

    public PersonalSalud? ObtenerPorId(int id)
    {
        using var conexion = ConexionBD.CrearConexion();
        conexion.Open();

        string sql = """
            SELECT IdPersonalSalud, DNI, Nombres, Apellidos, Sexo,
                   FechaNacimiento, Telefono, Correo, Direccion,
                   Especialidad, Cargo, Estado, FechaRegistro
            FROM PersonalSalud
            WHERE IdPersonalSalud = @Id;
            """;

        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@Id", id);

        using var lector = comando.ExecuteReader();

        return lector.Read() ? MapearPersonal(lector) : null;
    }

    public void Agregar(PersonalSalud entidad)
    {
        using var conexion = ConexionBD.CrearConexion();
        conexion.Open();

        string sql = """
            INSERT INTO PersonalSalud
            (DNI, Nombres, Apellidos, Sexo, FechaNacimiento,
             Telefono, Correo, Direccion, Especialidad, Cargo, Estado)
            VALUES
            (@DNI, @Nombres, @Apellidos, @Sexo, @FechaNacimiento,
             @Telefono, @Correo, @Direccion, @Especialidad, @Cargo, @Estado);
            """;

        using var comando = new SqlCommand(sql, conexion);
        AgregarParametros(comando, entidad);
        comando.ExecuteNonQuery();
    }

    public void Actualizar(PersonalSalud entidad)
    {
        using var conexion = ConexionBD.CrearConexion();
        conexion.Open();

        string sql = """
            UPDATE PersonalSalud SET
                DNI = @DNI,
                Nombres = @Nombres,
                Apellidos = @Apellidos,
                Sexo = @Sexo,
                FechaNacimiento = @FechaNacimiento,
                Telefono = @Telefono,
                Correo = @Correo,
                Direccion = @Direccion,
                Especialidad = @Especialidad,
                Cargo = @Cargo,
                Estado = @Estado
            WHERE IdPersonalSalud = @Id;
            """;

        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@Id", entidad.IdPersonalSalud);
        AgregarParametros(comando, entidad);
        comando.ExecuteNonQuery();
    }

    public void Eliminar(int id)
    {
        using var conexion = ConexionBD.CrearConexion();
        conexion.Open();

        string sql = """
            UPDATE PersonalSalud
            SET Estado = 0
            WHERE IdPersonalSalud = @Id;
            """;

        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@Id", id);
        comando.ExecuteNonQuery();
    }

    private static void AgregarParametros(
        SqlCommand comando, PersonalSalud entidad)
    {
        string apellidos =
            $"{entidad.ApellidoPaterno} {entidad.ApellidoMaterno}".Trim();

        comando.Parameters.AddWithValue("@DNI", entidad.NumeroDocumento);
        comando.Parameters.AddWithValue("@Nombres", entidad.Nombres);
        comando.Parameters.AddWithValue("@Apellidos", apellidos);
        comando.Parameters.AddWithValue("@Sexo", entidad.Sexo);
        comando.Parameters.AddWithValue("@FechaNacimiento",
            (object?)entidad.FechaNacimiento ?? DBNull.Value);
        comando.Parameters.AddWithValue("@Telefono", entidad.Telefono);
        comando.Parameters.AddWithValue("@Correo", entidad.CorreoElectronico);
        comando.Parameters.AddWithValue("@Direccion", entidad.Direccion);
        comando.Parameters.AddWithValue("@Especialidad", entidad.Especialidad);
        comando.Parameters.AddWithValue("@Cargo", entidad.Cargo);
        comando.Parameters.AddWithValue("@Estado", entidad.Estado);
    }

    private static PersonalSalud MapearPersonal(SqlDataReader lector)
    {
        string apellidos = lector.IsDBNull(3) ? "" : lector.GetString(3);

        return new PersonalSalud
        {
            IdPersonalSalud = lector.GetInt32(0),
            IdPersona = lector.GetInt32(0),
            TipoDocumento = "DNI",
            NumeroDocumento = lector.IsDBNull(1) ? "" : lector.GetString(1),
            Nombres = lector.IsDBNull(2) ? "" : lector.GetString(2),
            ApellidoPaterno = apellidos,
            ApellidoMaterno = "",
            Sexo = lector.IsDBNull(4) ? "" : lector.GetString(4),
            FechaNacimiento = lector.IsDBNull(5) ? null : lector.GetDateTime(5),
            Telefono = lector.IsDBNull(6) ? "" : lector.GetString(6),
            CorreoElectronico = lector.IsDBNull(7) ? "" : lector.GetString(7),
            Direccion = lector.IsDBNull(8) ? "" : lector.GetString(8),
            Especialidad = lector.IsDBNull(9) ? "" : lector.GetString(9),
            Cargo = lector.IsDBNull(10) ? "" : lector.GetString(10),
            Estado = lector.GetBoolean(11),
            EsActivo = lector.GetBoolean(11),
            FechaRegistro = lector.GetDateTime(12)
        };
    }
}
