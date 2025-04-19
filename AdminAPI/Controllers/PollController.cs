using AdminAPI.DataAccess;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Models;

namespace AdminAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PollController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IPollData _pollData;
        private readonly IParticipantData _participantData;

        public PollController(UserManager<IdentityUser> userManager, IPollData pollData, IParticipantData participantData)
        {
            _userManager = userManager;
            _pollData = pollData;
            _participantData = participantData;
        }

        [HttpGet(Name = "GetPolls"), Authorize]
        public async Task<IEnumerable<PollModel>?> Get(int id = -1)
        {
            if (id != -1)
            {
                return await GetPollById(id);
            }

            var currUser = await _userManager.GetUserAsync(HttpContext.User);

            return await _pollData.LoadPollsAllMinimal(currUser!.Id);
        }

        [HttpPost(Name = "CreatePoll"), Authorize]
        public async Task<PollModel?> Post(PollModel poll)
        {
            if (!ValidatePoll(poll))
            {
                return null;
            }

            var currUser = await _userManager.GetUserAsync(HttpContext.User);

            poll.OwnerName = currUser!.UserName;
            poll.Status = SharedLibrary.PollStatus.Vote;
            if (!poll.IsPublic)
            {
                poll.JoinCode = Guid.NewGuid().ToString();
            }

            poll.Participants = new List<ParticipantModel>();
            poll.Participants.Add(new ParticipantModel
            {
                Username = currUser!.Id!,
                Role = SharedLibrary.PollRole.Owner,
                HasVoted = false
            });

            poll = await _pollData.SavePoll(poll);

            foreach (var participant in poll.Participants!)
            {
                participant.Username = (await _userManager.FindByIdAsync(participant.Username))!.UserName!;
            }

            return poll;
        }

        private async Task<IEnumerable<PollModel>?> GetPollById(int pollId)
        {
            PollModel? poll = await _pollData.LoadPoll(pollId);
            if (poll == null)
            {
                return null;
            }

            var currUser = await _userManager.GetUserAsync(HttpContext.User);
            var userQuery = await _participantData.GetParticipantByIdAndPoll(currUser!.Id, pollId);
            if (userQuery == null && !poll.IsPublic)
            {
                return null;
            }

            if (userQuery?.Role == SharedLibrary.PollRole.Owner)
            {
                poll.Participants = await _participantData.GetParticipantsByPoll(poll.Id);
                foreach (var p in poll.Participants)
                {
                    p.Username = (await _userManager.FindByIdAsync(p.Username))!.UserName!;
                }
            }
            else
            {
                ParticipantModel? selfAsParticipant = await _participantData.GetParticipantByIdAndPoll(currUser.Id, poll.Id);
                if (selfAsParticipant != null)
                {
                    selfAsParticipant.Username = currUser.UserName!;
                    poll.Participants = [selfAsParticipant];
                }
            }

            List<PollModel> result = new List<PollModel>();
            result.Add(poll);
            return result;
        }

        private bool ValidatePoll(PollModel poll)
        {
            if (poll.Title.Length > 200 || string.IsNullOrEmpty(poll.Title))
            {
                return false;
            }
            if (poll.Description?.Length > 500)
            {
                return false;
            }
            if (poll.VoteCollectionEndDate < DateTime.UtcNow.AddDays(1))
            {
                return false;
            }
            if (poll.VoteValidationEndDate < poll.VoteCollectionEndDate.AddDays(1))
            {
                return false;
            }
            if (!ValidatePollOptions(poll.PollOptions!))
            {
                return false;
            }

            return true;
        }

        private bool ValidatePollOptions(List<OptionModel> options)
        {
            return !options.Any(x => x.OptionText.Length > 200 || string.IsNullOrWhiteSpace(x.OptionText));
        }
    }
}
