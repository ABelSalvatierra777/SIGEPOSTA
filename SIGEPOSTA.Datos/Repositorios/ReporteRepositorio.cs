using System.Data;
using Microsoft.Data.SqlClient;
using SIGEPOSTA.Datos.Infraestructura;

namespace SIGEPOSTA.Datos.Repositorios;

public class ReporteRepositorio
{
    public DataTable ObtenerReporte(
        string tipo,
        DateTime desde,
        DateTime hasta)
    {
        string sql = tipo switch
        {
            "Reporte de pacientes" => """
                SELECT IdPaciente AS Codigo,
                       DNI,
                       Nombres,
                       Apellidos,
                       Sexo,
                       FechaNacimiento,
                       Telefono,
                       Direccion,
                       Correo,
                       FechaRegistro,
                       CASE WHEN Estado = 1 THEN 'Activo' ELSE 'Inactivo' END AS Estado
                FROM Pacientes
                WHERE FechaRegistro >= @Desde AND FechaRegistro < @Hasta
                ORDER BY FechaRegistro DESC;
                """,

            "Reporte de personal de salud" => """
                SELECT IdPersonalSalud AS Codigo,
                       DNI,
                       Nombres,
                       Apellidos,
                       Sexo,
                       Telefono,
                       Correo,
                       Especialidad,
                       Cargo,
                       CASE WHEN Estado = 1 THEN 'Activo' ELSE 'Inactivo' END AS Estado,
                       FechaRegistro
                FROM PersonalSalud
                WHERE FechaRegistro >= @Desde AND FechaRegistro < @Hasta
                ORDER BY Apellidos, Nombres;
                """,

            "Reporte de citas médicas" => """
                SELECT c.IdCita AS Codigo,
                       p.DNI,
                       CONCAT(p.Nombres, ' ', p.Apellidos) AS Paciente,
                       CONCAT(ps.Nombres, ' ', ps.Apellidos) AS PersonalSalud,
                       c.FechaCita,
                       c.HoraCita,
                       c.MotivoConsulta,
                       c.Estado
                FROM Citas c
                INNER JOIN Pacientes p ON p.IdPaciente = c.IdPaciente
                INNER JOIN PersonalSalud ps ON ps.IdPersonalSalud = c.IdPersonalSalud
                WHERE c.FechaCita >= @Desde AND c.FechaCita < @Hasta
                ORDER BY c.FechaCita DESC, c.HoraCita;
                """,

            "Reporte de atenciones médicas" => """
                SELECT a.IdAtencion AS Codigo,
                       CONCAT(p.Nombres, ' ', p.Apellidos) AS Paciente,
                       CONCAT(ps.Nombres, ' ', ps.Apellidos) AS PersonalSalud,
                       a.FechaAtencion,
                       a.MotivoConsulta,
                       a.Diagnostico,
                       a.Tratamiento,
                       a.Observaciones
                FROM Atenciones a
                INNER JOIN Pacientes p ON p.IdPaciente = a.IdPaciente
                INNER JOIN PersonalSalud ps ON ps.IdPersonalSalud = a.IdPersonalSalud
                WHERE a.FechaAtencion >= @Desde AND a.FechaAtencion < @Hasta
                ORDER BY a.FechaAtencion DESC;
                """,

            "Reporte de diagnósticos" => """
                SELECT d.IdDiagnostico AS Codigo,
                       d.CodigoCIE10,
                       d.Descripcion,
                       d.TipoDiagnostico,
                       CONCAT(p.Nombres, ' ', p.Apellidos) AS Paciente,
                       d.FechaRegistro,
                       d.Estado,
                       d.Observaciones
                FROM Diagnosticos d
                INNER JOIN Atenciones a ON a.IdAtencion = d.IdAtencion
                INNER JOIN Pacientes p ON p.IdPaciente = a.IdPaciente
                WHERE d.FechaRegistro >= @Desde AND d.FechaRegistro < @Hasta
                ORDER BY d.FechaRegistro DESC;
                """,

            "Reporte de medicamentos" => """
                SELECT m.IdMedicamento AS Codigo,
                       m.Codigo AS CodigoMedicamento,
                       m.NombreMedicamento,
                       m.PrincipioActivo,
                       m.Concentracion,
                       m.FormaFarmaceutica,
                       m.Presentacion,
                       m.Laboratorio,
                       m.Categoria,
                       m.Precio,
                       m.EstadoDescripcion AS Estado,
                       m.FechaRegistro
                FROM Medicamentos m
                WHERE m.FechaRegistro >= @Desde AND m.FechaRegistro < @Hasta
                ORDER BY m.NombreMedicamento;
                """,

            "Reporte de movimientos de inventario" => """
                SELECT mi.IdMovimiento AS Codigo,
                       m.NombreMedicamento,
                       i.Lote,
                       mi.TipoMovimiento,
                       mi.Cantidad,
                       mi.Motivo,
                       mi.FechaMovimiento,
                       mi.IdUsuario
                FROM MovimientosInventario mi
                INNER JOIN Inventario i ON i.IdInventario = mi.IdInventario
                INNER JOIN Medicamentos m ON m.IdMedicamento = i.IdMedicamento
                WHERE mi.FechaMovimiento >= @Desde AND mi.FechaMovimiento < @Hasta
                ORDER BY mi.FechaMovimiento DESC;
                """,

            _ => throw new ArgumentException("Tipo de reporte no válido.")
        };

        using var conexion = ConexionBD.CrearConexion();
        using var comando = new SqlCommand(sql, conexion);

        comando.Parameters.Add("@Desde", SqlDbType.DateTime2).Value = desde.Date;
        comando.Parameters.Add("@Hasta", SqlDbType.DateTime2).Value = hasta.Date.AddDays(1);

        using var adaptador = new SqlDataAdapter(comando);

        DataTable resultado = new();
        adaptador.Fill(resultado);

        return resultado;
    }
}
