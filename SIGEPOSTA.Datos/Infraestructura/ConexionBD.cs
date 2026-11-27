using Microsoft.Data.SqlClient;

namespace SIGEPOSTA.Datos.Infraestructura;

public static class ConexionBD
{
    private const string VariableEntorno = "SIGEPOSTA_SQLSERVER";

    public static SqlConnection CrearConexion()
    {
        string? cadenaConexion =
            Environment.GetEnvironmentVariable(VariableEntorno);

        if (string.IsNullOrWhiteSpace(cadenaConexion))
        {
            throw new InvalidOperationException(
                "No se ha configurado la conexión a SQL Server. " +
                "Configure la variable de entorno SIGEPOSTA_SQLSERVER."
            );
        }

        return new SqlConnection(cadenaConexion);
    }

    public static async Task<SqlConnection> AbrirConexionAsync()
    {
        SqlConnection conexion = CrearConexion();

        try
        {
            await conexion.OpenAsync();
            return conexion;
        }
        catch
        {
            await conexion.DisposeAsync();
            throw;
        }
    }
}
