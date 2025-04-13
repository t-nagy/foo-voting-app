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
    public class ApiParticipantManager : AdminApiAuthCaller, IParticipantManager
    {
        public ApiParticipantManager(ISessionManager sessionManager) : base(sessionManager)
        {
        }

        public async Task<bool> AddParticipant(ParticipantModel participant)
        {
            string? token = await _sessionManager.GetAuthenticationToken();
            if (token == null) { return false; }
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response;
            try
            {
                response = await _client.PostAsJsonAsync("/participant", participant);
            }
            catch (HttpRequestException ex)
            {
                throw new ServerUnreachableException(DefaultServerUnreachableExceptionMessage, ex);
            }

            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            return false;
        }

        public async Task<PollModel?> JoinPollWithCode(string code)
        {
            string? token = await _sessionManager.GetAuthenticationToken();
            if (token == null) { return null; }
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response;
            try
            {
                response = await _client.PostAsync($"/join?inviteCode={code}", null);
            }
            catch (HttpRequestException ex)
            {
                throw new ServerUnreachableException(DefaultServerUnreachableExceptionMessage, ex);
            }

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    PollModel? poll = await response.Content.ReadFromJsonAsync<PollModel?>();
                    return poll;
                }
                catch (Exception)
                {
                    return null;
                }
            }

            return null;
        }
    }
}
