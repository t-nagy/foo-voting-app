using System.Security.Cryptography;

namespace AdminAPI.Services
{
    public class KeyService
    {
        private readonly ConfigHelper _config;

        public KeyService(ConfigHelper config)
        {
            _config = config;
            RSA = new RSACryptoServiceProvider();
            RSA.ImportFromPem(_config.GetSigningKeyPem());
        }

        public RSACryptoServiceProvider RSA { get; }

    }
}
