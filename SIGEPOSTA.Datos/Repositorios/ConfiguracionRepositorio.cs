using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using SIGEPOSTA.Datos.Infraestructura;

namespace SIGEPOSTA.Datos.Repositorios
{
    public class ConfiguracionRepositorio
    {
        public Dictionary<string, string> ObtenerConfiguracion()
        {
            var datos = new Dictionary<string, string>(
                StringComparer.OrdinalIgnoreCase);

            using var conexion = ConexionBD.CrearConexion();
            conexion.Open();

            const string sql = @"
                SELECT Clave, Valor
                FROM ConfiguracionSistema;";

            using var comando = new SqlCommand(sql, conexion);
            using var lector = comando.ExecuteReader();

            while (lector.Read())
            {
                string clave = lector["Clave"].ToString() ?? "";
                string valor = lector["Valor"] == DBNull.Value
                    ? ""
                    : lector["Valor"].ToString() ?? "";

                datos[clave] = valor;
            }

            return datos;
        }

        public void GuardarConfiguracion(
            Dictionary<string, string> datos)
        {
            using var conexion = ConexionBD.CrearConexion();
            conexion.Open();

            using var transaccion = conexion.BeginTransaction();

            try
            {
                const string sql = @"
                    UPDATE ConfiguracionSistema
                    SET Valor = @Valor,
                        FechaActualizacion = SYSDATETIME()
                    WHERE Clave = @Clave;

                    IF @@ROWCOUNT = 0
                    BEGIN
                        INSERT INTO ConfiguracionSistema
                            (Clave, Valor, Descripcion, FechaActualizacion)
                        VALUES
                            (@Clave, @Valor, @Descripcion, SYSDATETIME());
                    END;";

                foreach (var dato in datos)
                {
                    using var comando = new SqlCommand(
                        sql, conexion, transaccion);

                    comando.Parameters.Add(
                        "@Clave", System.Data.SqlDbType.NVarChar, 100)
                        .Value = dato.Key;

                    comando.Parameters.Add(
                        "@Valor", System.Data.SqlDbType.NVarChar, 500)
                        .Value = string.IsNullOrWhiteSpace(dato.Value)
                            ? DBNull.Value
                            : dato.Value;

                    comando.Parameters.Add(
                        "@Descripcion", System.Data.SqlDbType.NVarChar, 300)
                        .Value = "Configuración institucional de SIGEPOSTA";

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
    }
}
