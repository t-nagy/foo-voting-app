using System.Security.Cryptography;

namespace ClientLib.Persistance
{
    public interface IKeyManager
    {
        string AdminSignatureVerifiactionKey();
        RSACryptoServiceProvider GetUserSigningKey();
    }
}