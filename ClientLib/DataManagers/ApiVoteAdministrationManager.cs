using ClientLib.Authentication;
using SharedLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;

namespace ClientLib.DataManagers
{
    public class ApiVoteAdministrationManager : AdminApiAuthCaller, IVoteAdministrationManager
    {
        public ApiVoteAdministrationManager(ISessionManager sessionManager) : base(sessionManager)
        {
        }

        public async Task<bool> TryRegisterKey(string key)
        {
            string? token = await _sessionManager.GetAuthenticationToken();
            if (token == null) { return false; }
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response;
            try
            {
                var content = new HttpRequestMessage();
                response = await _client.PostAsJsonAsync("/verification", key);
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

        public async Task<string?> GetAdminVerificationKey()
        {
            HttpResponseMessage response;
            try
            {
                var content = new HttpRequestMessage();
                response = await _client.GetAsync("/verification");
            }
            catch (HttpRequestException ex)
            {
                throw new ServerUnreachableException(DefaultServerUnreachableExceptionMessage, ex);
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

                return wrapper.Key;
            }

            return null;
        }

        public async Task<SignedBallotModel?> GetAdminSignature(SignedBallotModel bcb)
        {
            string? token = await _sessionManager.GetAuthenticationToken();
            if (token == null) { return null; }
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response;
            try
            {
                var content = new HttpRequestMessage();
                response = await _client.PostAsJsonAsync("/vote", bcb);
            }
            catch (HttpRequestException ex)
            {
                throw new ServerUnreachableException(DefaultServerUnreachableExceptionMessage, ex);
            }

            if (response.IsSuccessStatusCode)
            {
                SignedBallotModel? scb;
                try
                {
                    scb = await response.Content.ReadFromJsonAsync<SignedBallotModel?>();
                }
                catch (Exception)
                {
                    return null;
                }

                if (scb != null)
                {
                    return scb;
                }
            }

            return null;
        }
    }
}
