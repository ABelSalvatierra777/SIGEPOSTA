namespace SIGEPOSTA.Seguridad.Interfaces;

public interface IEncriptadorContrasenas
{
    string Hash(string contrasena);

    bool Verificar(string contrasena, string hashAlmacenado);
}
