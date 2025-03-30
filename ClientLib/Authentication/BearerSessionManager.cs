﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.InteropServices.Marshalling;
using System.Runtime.Serialization;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace ClientLib.Authentication
{
    public class BearerSessionManager : ISessionManager
    {
        private const string server_addr = "https://localhost:7119";
        private HttpClient client = new HttpClient();

        public string? LoggedInEmail { get; private set; }
        private string? AuthenticationToken { get; set; }
        private string? RefreshToken { get; set; }
        private DateTime? _expiration;

        public bool IsLoggedIn
        {
            get
            {
                if (string.IsNullOrEmpty(AuthenticationToken) || string.IsNullOrEmpty(RefreshToken))
                {
                    return false;
                }
                return true;
            }
        }

        public event EventHandler<EventArgs>? LoginRequiredEvent;

        public BearerSessionManager()
        {
            client.BaseAddress = new Uri(server_addr);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<string?> GetAuthenticationToken()
        {
            if (DateTime.Now >= _expiration)
            {
                await RefreshAuthenticationToken();
            }
            if (!string.IsNullOrEmpty(AuthenticationToken))
            {
                return AuthenticationToken;
            }
            LoginRequiredEvent?.Invoke(this, EventArgs.Empty);
            return null;
        }

        public async Task<LoginResponse> StartNewSession(string email, string password)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync("/login", new { email, password });
            if (!response.IsSuccessStatusCode)
            {
                // TODO - Replace with error handling
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    string responseJsonString = await response.Content.ReadAsStringAsync();
                    JObject responseJson = JObject.Parse(responseJsonString);
                    string? detail = responseJson["detail"]!.ToString();
                    switch (detail.ToLower())
                    {
                        case "notallowed":
                            return LoginResponse.EmailNotConfirmed;
                        case "failed":
                            return LoginResponse.InvalidCredentials;
                        default:
                            return LoginResponse.UnknownFailure;
                    }
                }
                throw new HttpRequestException($"{response.StatusCode}: {response.Content}");
            }
            AuthenticationResponse? res = await response.Content.ReadFromJsonAsync<AuthenticationResponse>();
            if (res != null && res.TokenType == "Bearer")
            {
                AuthenticationToken = res.AccessToken;
                RefreshToken = res.RefreshToken;
                _expiration = DateTime.Now.AddSeconds(res.ExpiresIn);
                LoggedInEmail = email;
                return LoginResponse.Success;
            }

            return LoginResponse.UnknownFailure;
        }

        private async Task<bool> RefreshAuthenticationToken()
        {
            HttpResponseMessage response = await client.PostAsJsonAsync("/login", RefreshToken);
            if (!response.IsSuccessStatusCode)
            {
                // TODO - Replace with error handling
                throw new HttpRequestException($"{response.StatusCode}: {response.Content}");
            }
            AuthenticationResponse? res = await response.Content.ReadFromJsonAsync<AuthenticationResponse>();
            if (res != null && res.TokenType == "Bearer")
            {
                AuthenticationToken = res.AccessToken;
                RefreshToken = res.RefreshToken;
                _expiration = DateTime.Now.AddSeconds(res.ExpiresIn);
                return true;
            }
            return false;
        }

        public void Logout()
        {
            AuthenticationToken = null;
            RefreshToken = null;
            LoggedInEmail = null;
            _expiration = null;
            LoginRequiredEvent?.Invoke(this, EventArgs.Empty);
        }
    }
}
