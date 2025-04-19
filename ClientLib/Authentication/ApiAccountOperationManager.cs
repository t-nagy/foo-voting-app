using ClientLib.DataManagers;
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
    public class ApiAccountOperationManager : AdminApiAuthCaller, IAccountOperationManager
    {

        public ApiAccountOperationManager(ISessionManager sessionManager) : base(sessionManager)
        {
        }

        public async Task<LoginResponse> Login(string email, string password)
        {
            return await _sessionManager.StartNewSession(email, password);
        }

        public void Logout()
        {
            _sessionManager.Logout();
        }

        public async Task<string?> Register(string email, string password)
        {
            HttpResponseMessage response;
            try
            {
                response = await _client.PostAsJsonAsync("/register", new { email, password });
            }
            catch (HttpRequestException ex)
            {
                throw new ServerUnreachableException(DefaultServerUnreachableExceptionMessage, ex);
            }

            if (response.IsSuccessStatusCode)
            {
                return null;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                return ProcessBadPasswordErrors(await response.Content.ReadAsStringAsync());
            }

            return "An unkown error has occured while attempting to register.";
        }

        public async Task<bool> ResendEmailConfirmation(string email)
        {
            HttpResponseMessage response;
            try
            {
                response = await _client.PostAsJsonAsync("/resendConfirmationEmail", new { email });
            }
            catch (HttpRequestException ex)
            {
                throw new ServerUnreachableException(DefaultServerUnreachableExceptionMessage, ex);
            }

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ForgotPassword(string email)
        {
            HttpResponseMessage response;
            try
            {
                response = await _client.PostAsJsonAsync("/forgotPassword", new { email });
            }
            catch (HttpRequestException ex)
            {
                throw new ServerUnreachableException(DefaultServerUnreachableExceptionMessage, ex);
            }

            return response.IsSuccessStatusCode;
        }

        public async Task<string?> ResetPassword(string email, string resetCode, string newPassword)
        {
            HttpResponseMessage response;
            try
            {
                response = await _client.PostAsJsonAsync("/resetPassword", new { email, resetCode, newPassword });
            }
            catch (HttpRequestException ex)
            {
                throw new ServerUnreachableException(DefaultServerUnreachableExceptionMessage, ex);
            }

            if (response.IsSuccessStatusCode)
            {
                return null;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                return ProcessBadPasswordErrors(await response.Content.ReadAsStringAsync());
            }

            return "An unkown error has occured while attempting to change your password.";
        }

        public async Task<string?> ChangePassword(string oldPassword, string newPassword)
        {
            string? token = await _sessionManager.GetAuthenticationToken();
            if (token == null) { return "User login required!"; }
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response;
            try
            {
                response = await _client.PostAsJsonAsync("/manage/info", new { oldPassword, newPassword });
            }
            catch (HttpRequestException ex)
            {
                throw new ServerUnreachableException(DefaultServerUnreachableExceptionMessage, ex);
            }

            if (response.IsSuccessStatusCode)
            {
                return null;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                return ProcessBadPasswordErrors(await response.Content.ReadAsStringAsync());
            }

            return "An unkown error has occured while attemtping to change your password.";
        }

        private string ProcessBadPasswordErrors(string JsonString)
        {
            StringBuilder sb = new StringBuilder();
            JObject json = JObject.Parse(JsonString);
            var errors = json["errors"]?.Children();
            if (errors == null)
            {
                return "An uknown error has occured.";
            }
            foreach (var error in errors)
            {
                var str = error.Children().First().Children().First().ToString();
                sb.AppendLine(str);
            }

            return sb.ToString();
        }
    }
}
