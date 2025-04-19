using ClientLib.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ClientLib.Persistance
{
    public class KeyManager : IKeyManager
    {
        private readonly ISessionManager _sessionManager;

        public KeyManager(ISessionManager sessionManager)
        {
            _sessionManager = sessionManager;
        }

        public string AdminSignatureVerifiactionKey()
        {

            CspParameters csp = new CspParameters
            {
                KeyContainerName = $"FOOVotingApp_SignForAdmin_{_sessionManager.LoggedInEmail!.ToLower()}",
                Flags = CspProviderFlags.UseMachineKeyStore

            };
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(csp)
            {
                PersistKeyInCsp = true
            };

            return rsa.ExportRSAPublicKeyPem();
        }

        public RSACryptoServiceProvider GetUserSigningKey()
        {
            CspParameters csp = new CspParameters
            {
                KeyContainerName = $"FOOVotingApp_UserSigningKey_{_sessionManager.LoggedInEmail!.ToLower()}",
                Flags = CspProviderFlags.UseMachineKeyStore

            };
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(csp)
            {
                PersistKeyInCsp = true
            };

            return rsa;
        }

        public bool TryGetPollKeys(int pollId, out RSACryptoServiceProvider? pollRSA)
        {
            pollRSA = null;
            return true;
        }
    }
}
