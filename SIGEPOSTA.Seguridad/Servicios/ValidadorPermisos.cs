using SIGEPOSTA.Seguridad.Interfaces;

namespace SIGEPOSTA.Seguridad.Servicios;

public class ValidadorPermisos : IValidadorPermisos
{
    public bool TieneRol(string rolUsuario, string rolRequerido)
    {
        if (string.IsNullOrWhiteSpace(rolUsuario) ||
            string.IsNullOrWhiteSpace(rolRequerido))
            return false;

        return string.Equals(
            rolUsuario.Trim(),
            rolRequerido.Trim(),
            StringComparison.OrdinalIgnoreCase
        );
    }

    public bool TienePermiso(
        IEnumerable<string> permisosUsuario,
        string permisoRequerido)
    {
        if (permisosUsuario == null ||
            string.IsNullOrWhiteSpace(permisoRequerido))
            return false;

        return permisosUsuario.Any(p =>
            string.Equals(
                p?.Trim(),
                permisoRequerido.Trim(),
                StringComparison.OrdinalIgnoreCase
            ));
    }
}
