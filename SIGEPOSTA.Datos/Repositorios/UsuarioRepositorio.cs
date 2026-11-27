
using Microsoft.Data.SqlClient;
using SIGEPOSTA.Datos.Infraestructura;
using SIGEPOSTA.Datos.Interfaces;
using SIGEPOSTA.Entidades.Personas;
using SIGEPOSTA.Entidades.Seguridad;

namespace SIGEPOSTA.Datos.Repositorios;

public class UsuarioRepositorio : IUsuarioRepositorio
{
    public IReadOnlyList<Usuario> ObtenerTodos()
    {
        var usuarios = new List<Usuario>();

        using var conexion = ConexionBD.CrearConexion();
        conexion.Open();

        string sql = """
            SELECT
                u.IdUsuario,
                u.IdPersonalSalud,
                u.NombreUsuario,
                u.HashContrasena,
                u.Estado,
                u.FechaCreacion,
                u.UltimoAcceso,
                p.DNI,
                p.Nombres,
                p.Apellidos,
                p.Sexo,
                p.FechaNacimiento,
                p.Telefono,
                p.Correo,
                p.Direccion,
                p.Especialidad,
                p.Cargo,
                p.Estado AS EstadoPersonal,
                p.FechaRegistro
            FROM Usuarios u
            LEFT JOIN PersonalSalud p
                ON u.IdPersonalSalud = p.IdPersonalSalud
            ORDER BY u.IdUsuario;
            """;

        using (var comando = new SqlCommand(sql, conexion))
        using (var lector = comando.ExecuteReader())
        {
            while (lector.Read())
                usuarios.Add(MapearUsuario(lector));
        }

        foreach (var usuario in usuarios)
            usuario.Roles = ObtenerRoles(conexion, usuario.IdUsuario);

        return usuarios;
    }

    public Usuario? ObtenerPorId(int id)
    {
        using var conexion = ConexionBD.CrearConexion();
        conexion.Open();

        string sql = """
            SELECT
                u.IdUsuario,
                u.IdPersonalSalud,
                u.NombreUsuario,
                u.HashContrasena,
                u.Estado,
                u.FechaCreacion,
                u.UltimoAcceso,
                p.DNI,
                p.Nombres,
                p.Apellidos,
                p.Sexo,
                p.FechaNacimiento,
                p.Telefono,
                p.Correo,
                p.Direccion,
                p.Especialidad,
                p.Cargo,
                p.Estado AS EstadoPersonal,
                p.FechaRegistro
            FROM Usuarios u
            LEFT JOIN PersonalSalud p
                ON u.IdPersonalSalud = p.IdPersonalSalud
            WHERE u.IdUsuario = @IdUsuario;
            """;

        Usuario? usuario;

        using (var comando = new SqlCommand(sql, conexion))
        {
            comando.Parameters.AddWithValue("@IdUsuario", id);

            using var lector = comando.ExecuteReader();

            usuario = lector.Read() ? MapearUsuario(lector) : null;
        }

        if (usuario != null)
            usuario.Roles = ObtenerRoles(conexion, usuario.IdUsuario);

        return usuario;
    }

    public void Agregar(Usuario entidad)
    {
        using var conexion = ConexionBD.CrearConexion();
        conexion.Open();

        string sql = """
            INSERT INTO Usuarios
                (IdPersonalSalud, NombreUsuario, HashContrasena, Estado)
            VALUES
                (@IdPersonalSalud, @NombreUsuario, @HashContrasena, @Estado);
            """;

        using var comando = new SqlCommand(sql, conexion);

        comando.Parameters.AddWithValue(
            "@IdPersonalSalud",
            (object?)entidad.IdPersonalSalud ?? DBNull.Value);

        comando.Parameters.AddWithValue(
            "@NombreUsuario", entidad.NombreUsuario);

        comando.Parameters.AddWithValue(
            "@HashContrasena", entidad.HashContrasena);

        comando.Parameters.AddWithValue(
            "@Estado", entidad.Estado);

        comando.ExecuteNonQuery();
    }

    public void Actualizar(Usuario entidad)
    {
        using var conexion = ConexionBD.CrearConexion();
        conexion.Open();

        string sql = """
            UPDATE Usuarios
            SET
                IdPersonalSalud = @IdPersonalSalud,
                NombreUsuario = @NombreUsuario,
                HashContrasena = @HashContrasena,
                Estado = @Estado
            WHERE IdUsuario = @IdUsuario;
            """;

        using var comando = new SqlCommand(sql, conexion);

        comando.Parameters.AddWithValue("@IdUsuario", entidad.IdUsuario);

        comando.Parameters.AddWithValue(
            "@IdPersonalSalud",
            (object?)entidad.IdPersonalSalud ?? DBNull.Value);

        comando.Parameters.AddWithValue(
            "@NombreUsuario", entidad.NombreUsuario);

        comando.Parameters.AddWithValue(
            "@HashContrasena", entidad.HashContrasena);

        comando.Parameters.AddWithValue(
            "@Estado", entidad.Estado);

        comando.ExecuteNonQuery();
    }

    public void Eliminar(int id)
    {
        using var conexion = ConexionBD.CrearConexion();
        conexion.Open();

        using var transaccion = conexion.BeginTransaction();

        try
        {
            using (var comandoRoles = new SqlCommand(
                "DELETE FROM UsuarioRoles WHERE IdUsuario = @IdUsuario;",
                conexion, transaccion))
            {
                comandoRoles.Parameters.AddWithValue("@IdUsuario", id);
                comandoRoles.ExecuteNonQuery();
            }

            using (var comandoUsuario = new SqlCommand(
                "DELETE FROM Usuarios WHERE IdUsuario = @IdUsuario;",
                conexion, transaccion))
            {
                comandoUsuario.Parameters.AddWithValue("@IdUsuario", id);
                comandoUsuario.ExecuteNonQuery();
            }

            transaccion.Commit();
        }
        catch
        {
            transaccion.Rollback();
            throw;
        }
    }

    private static List<Rol> ObtenerRoles(
        SqlConnection conexion, int idUsuario)
    {
        var roles = new List<Rol>();

        string sql = """
            SELECT r.IdRol, r.NombreRol, r.Descripcion, r.Estado
            FROM UsuarioRoles ur
            INNER JOIN Roles r ON ur.IdRol = r.IdRol
            WHERE ur.IdUsuario = @IdUsuario;
            """;

        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@IdUsuario", idUsuario);

        using var lector = comando.ExecuteReader();

        while (lector.Read())
        {
            roles.Add(new Rol
            {
                IdRol = lector.GetInt32(0),
                Nombre = lector.GetString(1),
                Descripcion = lector.IsDBNull(2) ? "" : lector.GetString(2),
                Estado = lector.GetBoolean(3)
            });
        }

        return roles;
    }

    private static Usuario MapearUsuario(SqlDataReader lector)
    {
        var usuario = new Usuario
        {
            IdUsuario = lector.GetInt32(0),
            IdPersonalSalud = lector.IsDBNull(1)
                ? null
                : lector.GetInt32(1),
            NombreUsuario = lector.GetString(2),
            HashContrasena = lector.GetString(3),
            Estado = lector.GetBoolean(4),
            FechaCreacion = lector.GetDateTime(5),
            UltimoAcceso = lector.IsDBNull(6)
                ? null
                : lector.GetDateTime(6)
        };

        if (!lector.IsDBNull(7))
        {
            usuario.PersonalSalud = new PersonalSalud
            {
                IdPersonalSalud = lector.GetInt32(1),
                IdPersona = lector.GetInt32(1),
                TipoDocumento = "DNI",
                NumeroDocumento = lector.GetString(7),
                Nombres = lector.GetString(8),
                ApellidoPaterno = lector.IsDBNull(9)
                    ? ""
                    : lector.GetString(9),
                Sexo = lector.IsDBNull(10) ? "" : lector.GetString(10),
                FechaNacimiento = lector.IsDBNull(11)
                    ? null
                    : lector.GetDateTime(11),
                Telefono = lector.IsDBNull(12) ? "" : lector.GetString(12),
                CorreoElectronico = lector.IsDBNull(13)
                    ? ""
                    : lector.GetString(13),
                Direccion = lector.IsDBNull(14) ? "" : lector.GetString(14),
                Especialidad = lector.IsDBNull(15)
                    ? ""
                    : lector.GetString(15),
                Cargo = lector.IsDBNull(16) ? "" : lector.GetString(16),
                Estado = lector.GetBoolean(17),
                EsActivo = lector.GetBoolean(17),
                FechaRegistro = lector.GetDateTime(18)
            };
        }

        return usuario;
    }
}
