﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace ClientLib.Authentication
{
    public class ApiAccountOperationManager : IAccountOperationManager
    {
        private const string server_addr = "https://localhost:7119";
        private HttpClient client = new HttpClient();

        public ApiAccountOperationManager()
        {
            client.BaseAddress = new Uri(server_addr);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<string?> Register(string email, string password)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync("/register", new { email, password });
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
            HttpResponseMessage response = await client.PostAsJsonAsync("/resendConfirmationEmail", new { email });
            return response.IsSuccessStatusCode;

        }

        public async Task<bool> ForgotPassword(string email)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync("/forgotPassword", new { email });
            return response.IsSuccessStatusCode;
        }

        public async Task<string?> ResetPassword(string email, string resetCode, string newPassword)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync("/resetPassword", new { email, resetCode, newPassword });
            if (response.IsSuccessStatusCode)
            {
                return null;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                return ProcessBadPasswordErrors(await response.Content.ReadAsStringAsync());
            }

            return "An unkown error has occured while attempting to change password.";
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
