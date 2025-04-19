using System.Net.Http.Headers;

namespace ShufflerAPI.Services
{
    public class EndDateService
    {
        private readonly HttpClient _client;
        private readonly string _baseAdress = "https://localhost:7119";

        public EndDateService()
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri(_baseAdress);
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<DateTime?> GetVotingEndDate(int pollId)
        {
            HttpResponseMessage response;
            try
            {
                response = await _client.GetAsync($"/VotingEnd/?pollId={pollId}");
            }
            catch (HttpRequestException)
            {
                Console.WriteLine("ERROR: Cannot communicate with admin server!");
                return null;
            }

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var endDate = await response.Content.ReadFromJsonAsync<DateTime>();
                    return endDate;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            return null;
        }

        public async Task<DateTime?> GetValidationEndDate(int pollId)
        {
            HttpResponseMessage response;
            try
            {
                response = await _client.GetAsync($"/validationEnd/{pollId}");
            }
            catch (HttpRequestException)
            {
                Console.WriteLine("ERROR: Cannot communicate with admin server!");
                return null;
            }

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var endDate = await response.Content.ReadFromJsonAsync<DateTime>();
                    return endDate;
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
