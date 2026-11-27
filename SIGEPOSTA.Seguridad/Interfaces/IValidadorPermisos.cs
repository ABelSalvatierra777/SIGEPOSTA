namespace SIGEPOSTA.Seguridad.Interfaces;

public interface IValidadorPermisos
{
    bool TieneRol(string rolUsuario, string rolRequerido);

    bool TienePermiso(IEnumerable<string> permisosUsuario, string permisoRequerido);
}
