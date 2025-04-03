﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ClientLib
{
    public abstract class AdminApiCaller
    {
        protected readonly HttpClient _client;
        protected readonly string _baseAdress = "https://localhost:7119";

        private const string _defaultServerUnreachableExceptionMessage = "Communication with the server failed. Please check your internet connection!";

        protected string DefaultServerUnreachableExceptionMessage
        {
            get { return _defaultServerUnreachableExceptionMessage; }
        }

        public AdminApiCaller()
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri(_baseAdress);
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

        }
    }
}
