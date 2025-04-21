using SharedLibrary;
using SharedLibrary.Models;
using System.Net.Http.Headers;
using System.Security.Cryptography;

namespace CounterAPI.Services
{
    public class KeyService
    {
        private readonly ConfigHelper _config;

        public RSA TransportEncryptionRSA { get; }
        public bool HasVerificationKey { get; private set; }
        public RSA? VerificationRSA { get; private set; }


        public KeyService(ConfigHelper config)
        {
            _config = config;
            TransportEncryptionRSA = new RSACryptoServiceProvider(2048);
            TransportEncryptionRSA.ImportFromPem(_config.GetTransportEncryptionKeyPem());
        }

        public async Task<RSA?> GetVerificationRSA()
        {
            if (!HasVerificationKey)
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(AddressService.AdminAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response;
                try
                {
                    var content = new HttpRequestMessage();
                    response = await client.GetAsync("/verification");
                }
                catch (HttpRequestException)
                {
                    return null;
                }

                if (response.IsSuccessStatusCode)
                {
                    RSAKeyWrapper? wrapper = null;
                    try
                    {
                        wrapper = await response.Content.ReadFromJsonAsync<RSAKeyWrapper?>();
                    }
                    catch (Exception)
                    {
                    }
                    if (wrapper == null)
                    {
                        return null;
                    }

                    VerificationRSA = new RSACryptoServiceProvider();
                    VerificationRSA.ImportFromPem(wrapper.Key);
                    HasVerificationKey = true;
                }
            }

            return VerificationRSA;
        }
    }
}
