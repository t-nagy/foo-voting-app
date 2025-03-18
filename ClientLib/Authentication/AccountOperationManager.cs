using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace ClientLib.Authentication
{
    public class AccountOperationManager
    {
        private const string server_addr = "https://localhost:7119";
        private HttpClient client = new HttpClient();
        private ISessionManager _sessionManager;

        public AccountOperationManager(ISessionManager sessionManager)
        {
            client.BaseAddress = new Uri(server_addr);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            _sessionManager = sessionManager;
        }

        public async Task<bool> Register(string email, string password)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync("/register", new { email, password });
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    await _sessionManager.StartNewSession(email, password);
                }
                catch (Exception)
                {

                }

                return true;

            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                return false;
            }
            // TODO - Replace with error handling
            throw new HttpRequestException($"{response.StatusCode}: {response.Content}");
        }
    }
}
