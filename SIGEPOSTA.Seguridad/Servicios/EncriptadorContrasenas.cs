using System;
using System.Security.Cryptography;
using SIGEPOSTA.Seguridad.Interfaces;

namespace SIGEPOSTA.Seguridad.Servicios;

public class EncriptadorContrasenas : IEncriptadorContrasenas
{
    private const int Iteraciones = 210000;
    private const int LongitudSalt = 16;
    private const int LongitudHash = 32;

    public string Hash(string contrasena)
    {
        if (string.IsNullOrWhiteSpace(contrasena))
            throw new ArgumentException("La contraseña no puede estar vacía.");

        byte[] salt = RandomNumberGenerator.GetBytes(LongitudSalt);

        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            contrasena,
            salt,
            Iteraciones,
            HashAlgorithmName.SHA256,
            LongitudHash
        );

        return $"PBKDF2${Iteraciones}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
    }

    public bool Verificar(string contrasena, string hashAlmacenado)
    {
        if (string.IsNullOrWhiteSpace(contrasena) ||
            string.IsNullOrWhiteSpace(hashAlmacenado))
            return false;

        try
        {
            string[] partes = hashAlmacenado.Split('$');

            if (partes.Length != 4 || partes[0] != "PBKDF2")
                return false;

            if (!int.TryParse(partes[1], out int iteraciones) ||
                iteraciones < 100000 || iteraciones > 1000000)
                return false;

            byte[] salt = Convert.FromBase64String(partes[2]);
            byte[] hashOriginal = Convert.FromBase64String(partes[3]);

            byte[] hashCalculado = Rfc2898DeriveBytes.Pbkdf2(
                contrasena,
                salt,
                iteraciones,
                HashAlgorithmName.SHA256,
                hashOriginal.Length
            );

            return CryptographicOperations.FixedTimeEquals(
                hashOriginal,
                hashCalculado
            );
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
