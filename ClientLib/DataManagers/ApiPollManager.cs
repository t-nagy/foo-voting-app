using ClientLib.Authentication;
using SharedLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace ClientLib.DataManagers
{
    public class ApiPollManager : AdminApiAuthCaller, IPollManager
    {
        public ApiPollManager(ISessionManager sessionManager) : base(sessionManager)
        {
        }

        public async Task<IEnumerable<PollModel>?> GetAllPollsMinimal()
        {
            string? token = await _sessionManager.GetAuthenticationToken();
            if (token == null) { return null; }
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response;
            try
            {
                response = await _client.GetAsync("/poll");
            }
            catch (HttpRequestException ex)
            {
                throw new ServerUnreachableException(DefaultServerUnreachableExceptionMessage, ex);
            }

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<IEnumerable<PollModel>?>();
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        public async Task<PollModel?> GetPollById(int pollId)
        {
            string? token = await _sessionManager.GetAuthenticationToken();
            if (token == null) { return null; }
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response;
            try
            {
                response = await _client.GetAsync($"/poll?id={pollId}");
            }
            catch (HttpRequestException ex)
            {
                throw new ServerUnreachableException(DefaultServerUnreachableExceptionMessage, ex);
            }

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<IEnumerable<PollModel>?>();
                if (result != null)
                {
                    return result.FirstOrDefault();
                }
            }

            return null;
        }

        public async Task<PollModel?> CreatePoll(PollModel poll)
        {
            string? token = await _sessionManager.GetAuthenticationToken();
            if (token == null) { return null; }
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response;
            try
            {
                response = await _client.PostAsJsonAsync("/poll", poll);
            }
            catch (HttpRequestException ex)
            {
                throw new ServerUnreachableException(DefaultServerUnreachableExceptionMessage, ex);
            }

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var result = await response.Content.ReadFromJsonAsync<PollModel?>();
                    return result;
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
