using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using SIGEPOSTA.Entidades.Clinica;

namespace SIGEPOSTA.Datos.Repositorios
{
    public class DiagnosticoRepositorio
    {
        private SqlConnection CrearConexion()
        {
            string cadena =
                Environment.GetEnvironmentVariable("SIGEPOSTA_SQLSERVER")
                ?? throw new InvalidOperationException(
                    "No se encontró la variable SIGEPOSTA_SQLSERVER.");

            return new SqlConnection(cadena);
        }

        public List<Diagnostico> ObtenerTodos()
        {
            var lista = new List<Diagnostico>();

            const string sql = @"
                SELECT IdDiagnostico, IdAtencion, CodigoCIE10,
                       Descripcion, TipoDiagnostico, FechaRegistro,
                       Observaciones, Estado
                FROM Diagnosticos
                ORDER BY FechaRegistro DESC;";

            using var conexion = CrearConexion();
            using var comando = new SqlCommand(sql, conexion);

            conexion.Open();

            using var lector = comando.ExecuteReader();

            while (lector.Read())
            {
                lista.Add(new Diagnostico
                {
                    IdDiagnostico = lector.GetInt32(0),
                    IdAtencion = lector.GetInt32(1),
                    CodigoCIE10 = lector.GetString(2),
                    Descripcion = lector.GetString(3),
                    TipoDiagnostico = lector.GetString(4),
                    FechaRegistro = lector.GetDateTime(5),
                    Observaciones = lector.IsDBNull(6) ? "" : lector.GetString(6),
                    Estado = lector.GetString(7)
                });
            }

            return lista;
        }

        public int ObtenerAtencionPorDni(string dniPaciente, string dniProfesional)
        {
            const string sql = @"
                SELECT TOP 1 a.IdAtencion
                FROM Atenciones a
                INNER JOIN Pacientes p
                    ON p.IdPaciente = a.IdPaciente
                INNER JOIN PersonalSalud ps
                    ON ps.IdPersonalSalud = a.IdPersonalSalud
                WHERE LTRIM(RTRIM(p.DNI)) = @DniPaciente
                  AND LTRIM(RTRIM(ps.DNI)) = @DniProfesional
                ORDER BY a.FechaAtencion DESC;";

            using var conexion = CrearConexion();
            using var comando = new SqlCommand(sql, conexion);

            comando.Parameters.AddWithValue("@DniPaciente", dniPaciente.Trim());
            comando.Parameters.AddWithValue("@DniProfesional", dniProfesional.Trim());

            conexion.Open();

            object? resultado = comando.ExecuteScalar();

            return resultado == null || resultado == DBNull.Value
                ? 0
                : Convert.ToInt32(resultado);
        }

        public void Agregar(Diagnostico diagnostico)
        {
            const string sql = @"
                INSERT INTO Diagnosticos
                (
                    IdAtencion, CodigoCIE10, Descripcion,
                    TipoDiagnostico, FechaRegistro,
                    Observaciones, Estado
                )
                VALUES
                (
                    @IdAtencion, @Codigo, @Descripcion,
                    @Tipo, @Fecha, @Observaciones, @Estado
                );";

            using var conexion = CrearConexion();
            using var comando = new SqlCommand(sql, conexion);

            AgregarParametros(comando, diagnostico);

            conexion.Open();
            comando.ExecuteNonQuery();
        }

        public void Actualizar(Diagnostico diagnostico)
        {
            const string sql = @"
                UPDATE Diagnosticos
                SET IdAtencion = @IdAtencion,
                    CodigoCIE10 = @Codigo,
                    Descripcion = @Descripcion,
                    TipoDiagnostico = @Tipo,
                    FechaRegistro = @Fecha,
                    Observaciones = @Observaciones,
                    Estado = @Estado
                WHERE IdDiagnostico = @IdDiagnostico;";

            using var conexion = CrearConexion();
            using var comando = new SqlCommand(sql, conexion);

            AgregarParametros(comando, diagnostico);

            comando.Parameters.AddWithValue(
                "@IdDiagnostico", diagnostico.IdDiagnostico);

            conexion.Open();

            if (comando.ExecuteNonQuery() == 0)
                throw new InvalidOperationException(
                    "No se encontró el diagnóstico seleccionado.");
        }

        public void Eliminar(int id)
        {
            const string sql = @"
                UPDATE Diagnosticos
                SET Estado = 'Eliminado'
                WHERE IdDiagnostico = @Id;";

            using var conexion = CrearConexion();
            using var comando = new SqlCommand(sql, conexion);

            comando.Parameters.AddWithValue("@Id", id);

            conexion.Open();
            comando.ExecuteNonQuery();
        }


        public List<DiagnosticoRegistro> ObtenerTodosConDatos()
        {
            var lista = new List<DiagnosticoRegistro>();

            const string sql = @"
                SELECT d.IdDiagnostico, d.IdAtencion,
                       d.CodigoCIE10, d.Descripcion,
                       d.TipoDiagnostico, d.FechaRegistro,
                       d.Observaciones, d.Estado,
                       p.DNI,
                       LTRIM(RTRIM(p.Nombres + ' ' + p.Apellidos)),
                       ps.DNI,
                       LTRIM(RTRIM(ps.Nombres + ' ' + ps.Apellidos))
                FROM Diagnosticos d
                INNER JOIN Atenciones a ON a.IdAtencion = d.IdAtencion
                INNER JOIN Pacientes p ON p.IdPaciente = a.IdPaciente
                INNER JOIN PersonalSalud ps
                    ON ps.IdPersonalSalud = a.IdPersonalSalud
                WHERE d.Estado <> 'Eliminado'
                ORDER BY d.FechaRegistro DESC;";

            using var conexion = CrearConexion();
            using var comando = new SqlCommand(sql, conexion);

            conexion.Open();

            using var lector = comando.ExecuteReader();

            while (lector.Read())
            {
                lista.Add(new DiagnosticoRegistro
                {
                    IdDiagnostico = lector.GetInt32(0),
                    IdAtencion = lector.GetInt32(1),
                    CodigoCIE10 = lector.GetString(2),
                    Descripcion = lector.GetString(3),
                    TipoDiagnostico = lector.GetString(4),
                    FechaRegistro = lector.GetDateTime(5),
                    Observaciones = lector.IsDBNull(6) ? "" : lector.GetString(6),
                    Estado = lector.GetString(7),
                    DniPaciente = lector.GetString(8),
                    Paciente = lector.GetString(9),
                    DniProfesional = lector.GetString(10),
                    Profesional = lector.GetString(11)
                });
            }

            return lista;
        }
        private static void AgregarParametros(
            SqlCommand comando,
            Diagnostico diagnostico)
        {
            comando.Parameters.AddWithValue(
                "@IdAtencion", diagnostico.IdAtencion);

            comando.Parameters.AddWithValue(
                "@Codigo", diagnostico.CodigoCIE10);

            comando.Parameters.AddWithValue(
                "@Descripcion", diagnostico.Descripcion);

            comando.Parameters.AddWithValue(
                "@Tipo", diagnostico.TipoDiagnostico);

            comando.Parameters.AddWithValue(
                "@Fecha", diagnostico.FechaRegistro);

            comando.Parameters.AddWithValue(
                "@Observaciones",
                string.IsNullOrWhiteSpace(diagnostico.Observaciones)
                    ? DBNull.Value
                    : diagnostico.Observaciones);

            comando.Parameters.AddWithValue(
                "@Estado", diagnostico.Estado);
        }
    }
}


public class DiagnosticoRegistro
{
    public int IdDiagnostico { get; set; }
    public int IdAtencion { get; set; }
    public string CodigoCIE10 { get; set; } = "";
    public string Descripcion { get; set; } = "";
    public string TipoDiagnostico { get; set; } = "";
    public DateTime FechaRegistro { get; set; }
    public string Observaciones { get; set; } = "";
    public string Estado { get; set; } = "";
    public string DniPaciente { get; set; } = "";
    public string Paciente { get; set; } = "";
    public string DniProfesional { get; set; } = "";
    public string Profesional { get; set; } = "";
}
