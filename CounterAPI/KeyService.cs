using SharedLibrary;
using SharedLibrary.Models;
using System.Net.Http.Headers;
using System.Security.Cryptography;

namespace CounterAPI
{
    public class KeyService
    {
        public RSA TransportEncryptionRSA { get; }
        public bool HasVerificationKey { get; private set; }
        public RSA? VerificationRSA { get; private set; }


        public KeyService()
        {
            TransportEncryptionRSA = new RSACryptoServiceProvider(2048);
            TransportEncryptionRSA.ImportFromPem("-----BEGIN RSA PRIVATE KEY-----\r\nMIIEogIBAAKCAQEAqw/Pt/4vKN2T6G0KqpdSzKsaBFIwSJebfWP+rPkKYxJXm15M\r\n5ua7bHupZBi2JKQzxi1W2xY4lIguaUmG/b4BDNMhyGdZwLQP7rZ1Rqj5WBmO4fOM\r\n7vZfnx8f50+sc3bqdUx8lD4hH2pBDbTAPdYKI+NlBjiLs0yjgL4KiUA+5Edop+D6\r\nhyuiKbcu2z85J37ppPGdpg5lTuJq59epW58saPM1ZPxTxiUUH7ECu7QOsUaB/o/5\r\nMtBNwNdPEVU9fGfZKXVlOxRi7sy/4WYyvr9V3DTOgY3g/g0T5Bvs+LTBXzGxLGes\r\nmkdQvvw9Bx1qc/z430lJmG9KRDDwMj+cBXyhmQIDAQABAoIBACaL+ZLMhHQA+kN1\r\nzuGCF/Zm2AYBP0Acab1nuwvfUqfgij1Ikqu1cqaPdyxHJUQ8fC48KUZVS4qs5Uh0\r\neSbUmgf5y7MNFvyFltGD7AlwKj/gAoKID0oJ2qJQsS++lG2wbkTgZ3JATdZ2axJb\r\nmLZWdRKBO1Kbi19r9awdJuKmx4VI88tkrUqaSFogTlnnTuPvOKsHE33qXMeX3lQ/\r\n/xrwQ4n0lkDtnlKkT4C0QyBzElXBfVXnbcT20y0Pk2RBl/8RI5U5V9dIPuh9Q+p/\r\n8qjRN7Xgmezo3cFCQNSdkzK6FOQpiB71GwSdsSAlUu7L23Xb1UwDTyS+GbdoWoS8\r\njHkk78ECgYEA0zgegT7NhKYfQZTr3kVN+6mhWehoNaRMlUR5TIFA5Gvkrb9IkaEh\r\nc+kJcLW8mCPv9Jt6FCHU2L5WM0hZw0ioydblAx/B6qf9IsWou9WABGQLU9kbfilN\r\nZuyQ0tDuTo9oI15oyrgydqOiC2hIbHZLDslqI96GgvEfd1KH3LffVPcCgYEAz1Qn\r\nju3RccEEOezrMaPRk/DHRrSImdwmyg51DDrmNlaAhQ4yzPT8YJyoDweF+d20hcUv\r\n5xIDFPoizKwNBAluBp055h4YHh9+IhrXTtHh1wg+SMboWuO50KB7KEuZ8HwNkhhg\r\nD8F60dLZuXQj3Gavx46mX43bfgHEVWw3CG01ae8CgYBC9/kcC3iIBU/FsFz0lU67\r\nazYgVoKr91LcxOActKKJiffwasSsf8Umhg2bMdQA/Ov4x072kR92NSsJiXgEZcY2\r\ncK8dxXTrXKXy3263V2MGgV3iEOUZpCjJaFomrV3RhMCE7ksVcz90xD79lOvZj8wO\r\nfSftBYHqhF7wqzcucRIbUwKBgEFaM4X4LNcAkoZ3ycNB8iQB966v5YcqQAfc+sQ6\r\nJKroJWbLjPPSHWOOveLO6kpSuj6YY3sg0FviZOnDKe8l0cq/8Ko6vjtwjd/OOiQc\r\nQGX5sThbio5rzfyXOXsAKawGTnjIui79O+u73Ol6VHt62tm6e3MPUiqr/+k1TTuY\r\nwQd9AoGAZFLQuivJ5fZYx+P5Sao2r1Ez47PYfurBI0/bojsCkPagkeLua0J/yBLf\r\nogR8W9h+zTtSGTxIq/ruPa4BjqvZX3+PCUh9aFLncdJzpAc1OFR/oOqeGiDbWrYS\r\nskc8RRn2u1v4l7u3+BhtTW0rg0MIXlVpuQfQf9uQ//rrlq5ktss=\r\n-----END RSA PRIVATE KEY-----");
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
