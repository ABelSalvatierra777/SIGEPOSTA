using System;
using System.Linq;
using SIGEPOSTA.Datos.Interfaces;
using SIGEPOSTA.Entidades.Seguridad;
using SIGEPOSTA.Negocio.Interfaces;
using SIGEPOSTA.Seguridad.Interfaces;
using SIGEPOSTA.Seguridad.Modelos;

namespace SIGEPOSTA.Negocio.Servicios;

public class AutenticacionServicio : IAutenticacionServicio
{
    private readonly IUsuarioRepositorio _usuarioRepositorio;
    private readonly IEncriptadorContrasenas _encriptador;

    public AutenticacionServicio(
        IUsuarioRepositorio usuarioRepositorio,
        IEncriptadorContrasenas encriptador)
    {
        _usuarioRepositorio = usuarioRepositorio
            ?? throw new ArgumentNullException(nameof(usuarioRepositorio));

        _encriptador = encriptador
            ?? throw new ArgumentNullException(nameof(encriptador));
    }

    public ResultadoAutenticacion Autenticar(
        string nombreUsuario,
        string contrasena)
    {
        if (string.IsNullOrWhiteSpace(nombreUsuario) ||
            string.IsNullOrWhiteSpace(contrasena))
        {
            return ResultadoAutenticacion.Rechazado(
                "Ingrese su usuario y contraseña.");
        }

        Usuario? usuario = _usuarioRepositorio.ObtenerTodos()
            .FirstOrDefault(u =>
                string.Equals(
                    u.NombreUsuario,
                    nombreUsuario.Trim(),
                    StringComparison.OrdinalIgnoreCase));

        if (usuario == null)
        {
            return ResultadoAutenticacion.Rechazado(
                "Usuario o contraseña incorrectos.");
        }

        if (!usuario.Estado)
        {
            return ResultadoAutenticacion.Rechazado(
                "La cuenta se encuentra desactivada.");
        }

        if (!_encriptador.Verificar(
            contrasena,
            usuario.HashContrasena))
        {
            return ResultadoAutenticacion.Rechazado(
                "Usuario o contraseña incorrectos.");
        }

        var rol = usuario.Roles.FirstOrDefault(r => r.Estado);

        if (rol == null)
        {
            return ResultadoAutenticacion.Rechazado(
                "El usuario no tiene un rol activo asignado.");
        }

string nombreCompleto =
    usuario.PersonalSalud?.NombreCompleto ?? usuario.NombreUsuario;

        if (string.IsNullOrWhiteSpace(nombreCompleto))
        {
            nombreCompleto = usuario.NombreUsuario;
        }

        var sesion = new SesionUsuario
        {
            UsuarioId = usuario.IdUsuario,
            NombreUsuario = usuario.NombreUsuario,
            NombreCompleto = nombreCompleto,
            Rol = rol.Nombre,
            EstaAutenticado = true
        };

        return ResultadoAutenticacion.Correcto(sesion);
    }
}
