using ClientLib.Authentication;
using SharedLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace ClientLib
{
    public class ApiPollManager : AdminApiAuthCaller, IPollManager
    {
        public ApiPollManager(ISessionManager sessionManager) : base(sessionManager)
        {
        }

        public async Task<PollModel?> CreatePoll(PollModel poll)
        {
            string? token = await _sessionManager.GetAuthenticationToken();
            if (token == null) { return null; }
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response;
            try
            {
                response = await _client.PostAsJsonAsync<PollModel>("/poll", poll);
            }
            catch (HttpRequestException ex)
            {
                throw new ServerUnreachableException(DefaultServerUnreachableExceptionMessage, ex);
            }

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<PollModel?>();
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }
    }
}
