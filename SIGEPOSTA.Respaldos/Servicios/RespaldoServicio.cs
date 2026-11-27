using Microsoft.Data.SqlClient;
using SIGEPOSTA.Datos.Infraestructura;
using SIGEPOSTA.Respaldos.Interfaces;
using SIGEPOSTA.Respaldos.Modelos;

namespace SIGEPOSTA.Respaldos.Servicios;

public class RespaldoServicio : IRespaldoServicio
{
    public ResultadoRespaldo CrearRespaldo(string carpetaDestino)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(carpetaDestino))
                return Error("Seleccione una carpeta de destino.");

            Directory.CreateDirectory(carpetaDestino);

            using var conexionAplicacion = ConexionBD.CrearConexion();

            string nombreBD = conexionAplicacion.Database;

            if (string.IsNullOrWhiteSpace(nombreBD))
                return Error("No se pudo identificar la base de datos.");

            string nombreArchivo =
                $"SIGEPOSTA_Respaldo_{DateTime.Now:yyyyMMdd_HHmmss}.bak";

            string rutaArchivo = Path.GetFullPath(
                Path.Combine(carpetaDestino, nombreArchivo));

            string rutaSQL = rutaArchivo.Replace("'", "''");

            conexionAplicacion.Open();

            string sql = $"""
                BACKUP DATABASE [{nombreBD.Replace("]", "]]")}]
                TO DISK = N'{rutaSQL}'
                WITH INIT, COMPRESSION, CHECKSUM, STATS = 10;
                """;

            using var comando = new SqlCommand(sql, conexionAplicacion)
            {
                CommandTimeout = 0
            };

            comando.ExecuteNonQuery();

            return new ResultadoRespaldo
            {
                Exitoso = true,
                Mensaje = "Respaldo de SQL Server creado correctamente.",
                RutaArchivo = rutaArchivo
            };
        }
        catch (Exception ex)
        {
            return Error(
                "No se pudo crear el respaldo SQL Server. " +
                "Verifique los permisos de escritura de la cuenta del servicio SQL Server sobre la carpeta elegida. " +
                ex.Message);
        }
    }

    public ResultadoRespaldo RestaurarRespaldo(string archivoRespaldo)
    {
        string nombreBD = string.Empty;
        bool modoUnicoActivado = false;

        try
        {
            if (string.IsNullOrWhiteSpace(archivoRespaldo) ||
                !File.Exists(archivoRespaldo))
                return Error("El archivo de respaldo no existe.");

            if (!string.Equals(
                Path.GetExtension(archivoRespaldo),
                ".bak",
                StringComparison.OrdinalIgnoreCase))
                return Error("Seleccione un respaldo SQL Server con extensión .bak.");

            using var conexionAplicacion = ConexionBD.CrearConexion();

            nombreBD = conexionAplicacion.Database;

            if (string.IsNullOrWhiteSpace(nombreBD))
                return Error("No se pudo identificar la base de datos.");

            var constructor =
                new SqlConnectionStringBuilder(conexionAplicacion.ConnectionString)
                {
                    InitialCatalog = "master",
                    ConnectTimeout = 30
                };

            string rutaSQL = Path.GetFullPath(archivoRespaldo)
                .Replace("'", "''");

            string identificadorBD = "[" + nombreBD.Replace("]", "]]") + "]";

            using var conexionMaster = new SqlConnection(constructor.ConnectionString);
            conexionMaster.Open();

            SqlConnection.ClearAllPools();

            using (var comandoUnico = new SqlCommand(
                $"ALTER DATABASE {identificadorBD} SET SINGLE_USER WITH ROLLBACK IMMEDIATE;",
                conexionMaster))
            {
                comandoUnico.CommandTimeout = 0;
                comandoUnico.ExecuteNonQuery();
                modoUnicoActivado = true;
            }

            try
            {
                string sqlRestaurar = $"""
                    RESTORE DATABASE {identificadorBD}
                    FROM DISK = N'{rutaSQL}'
                    WITH REPLACE, RECOVERY, CHECKSUM, STATS = 10;
                    """;

                using var comandoRestaurar =
                    new SqlCommand(sqlRestaurar, conexionMaster)
                    {
                        CommandTimeout = 0
                    };

                comandoRestaurar.ExecuteNonQuery();
            }
            finally
            {
                if (modoUnicoActivado)
                {
                    using var comandoMultiple = new SqlCommand(
                        $"ALTER DATABASE {identificadorBD} SET MULTI_USER;",
                        conexionMaster);

                    comandoMultiple.CommandTimeout = 0;
                    comandoMultiple.ExecuteNonQuery();
                    modoUnicoActivado = false;
                }
            }

            SqlConnection.ClearAllPools();

            return new ResultadoRespaldo
            {
                Exitoso = true,
                Mensaje = "La base de datos fue restaurada correctamente.",
                RutaArchivo = archivoRespaldo
            };
        }
        catch (Exception ex)
        {
            return Error(
                "No se pudo restaurar la base de datos. " +
                "Verifique que el archivo pertenezca a una copia compatible y que SQL Server tenga acceso al archivo. " +
                ex.Message);
        }
    }

    public List<RegistroRespaldo> ListarRespaldos(string carpetaDestino)
    {
        if (string.IsNullOrWhiteSpace(carpetaDestino) ||
            !Directory.Exists(carpetaDestino))
            return new List<RegistroRespaldo>();

        return Directory.GetFiles(carpetaDestino, "SIGEPOSTA_Respaldo_*.bak")
            .Select(ruta => new FileInfo(ruta))
            .OrderByDescending(archivo => archivo.CreationTime)
            .Select(archivo => new RegistroRespaldo
            {
                NombreArchivo = archivo.Name,
                RutaArchivo = archivo.FullName,
                FechaCreacion = archivo.CreationTime,
                TamanoBytes = archivo.Length
            })
            .ToList();
    }

    private static ResultadoRespaldo Error(string mensaje)
    {
        return new ResultadoRespaldo
        {
            Exitoso = false,
            Mensaje = mensaje
        };
    }
}
