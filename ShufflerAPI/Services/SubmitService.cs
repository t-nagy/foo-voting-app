using SharedLibrary;
using ShufflerAPI.DataAccess;
using ShufflerAPI.Models;
using System.Net.Http.Headers;

namespace ShufflerAPI.Services
{
    public class SubmitService
    {
        private readonly ConfigHelper _config;
        private readonly IVoteData _voteData;
        private readonly EndDateService _endService;
        private readonly HttpClient _counterClient;
        private readonly TimeSpan _period = TimeSpan.FromHours(1);

        public DateTime LastSubmit { get; private set; }
        public TimeSpan SubmitPeriod
        {
            get
            {
                return _period;
            }
        }

        public SubmitService(ConfigHelper config, IVoteData voteData, EndDateService endService)
        {
            _config = config;
            _voteData = voteData;
            _endService = endService;
            _counterClient = new HttpClient();
            _counterClient.BaseAddress = new Uri(AddressService.CounterAddress);
            _counterClient.DefaultRequestHeaders.Accept.Clear();
            _counterClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task SubmitDataToCounter()
        {
            if (LastSubmit.Add(SubmitPeriod) > DateTime.UtcNow)
            {
                return;
            }

            IEnumerable<WaitingPollModel> waitingPolls = await _voteData.GetPollIdsAndSubmittedState();

            List<int> pollsToSubmit = new List<int>();
            List<int> pollsToValidate = new List<int>();

            foreach (var poll in waitingPolls)
            {
                if (poll.IsSubmitted)
                {
                    var validationEndDate = await _endService.GetValidationEndDate(poll.PollId);
                    if (validationEndDate != null && validationEndDate != default && validationEndDate < DateTime.UtcNow)
                    {
                        pollsToValidate.Add(poll.PollId);
                    }
                }
                else
                {
                    var votingEndDate = await _endService.GetVotingEndDate(poll.PollId);
                    if (votingEndDate != null && votingEndDate != default && votingEndDate < DateTime.UtcNow)
                    {
                        pollsToSubmit.Add(poll.PollId);
                    }
                }
            }

            bool submitSuccess = false;
            foreach (var poll in pollsToSubmit)
            {
                submitSuccess = await SubmitPoll(poll);
            }

            bool validateSuccess = false;
            foreach (var poll in pollsToValidate)
            {
                validateSuccess = await ValidatePoll(poll);
            }

            if ((submitSuccess && validateSuccess) || (pollsToSubmit.Count == 0 && pollsToValidate.Count == 0))
            {
                LastSubmit = DateTime.UtcNow;
            }
        }

        private async Task<bool> SubmitPoll(int pollId)
        {
            var votes = await _voteData.GetVotesByPoll(pollId);

            HttpResponseMessage response;
            try
            {
                response = await _counterClient.PostAsJsonAsync($"/vote/?usageKey={_config.GetAPIUsageKey()}", votes);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occoured while trying to submit votes: {ex.Message}");
                return false;
            }

            if (response.IsSuccessStatusCode)
            {
                await _voteData.UpdateIsSubmitted(pollId);
                return true;
            }
            return false;
        }

        private async Task<bool> ValidatePoll(int pollId)
        {
            var validations = await _voteData.GetValidationsByPoll(pollId);

            HttpResponseMessage response;
            try
            {
                response = await _counterClient.PostAsJsonAsync($"/validate/?usageKey={_config.GetAPIUsageKey()}", validations);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occoured while trying to validate votes: {ex.Message}");
                return false;
            }

            if (response.IsSuccessStatusCode)
            {
                await _voteData.DeleteValidatedVotes(pollId);
                return true;
            }
            return false;
        }
    }

    
}
