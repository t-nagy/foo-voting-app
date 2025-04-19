using AdminAPI.DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Models;

namespace AdminAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class JoinController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;

        public JoinController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost(Name = "JoinPoll"), Authorize]
        public async Task<PollModel?> Post(string inviteCode)
        {
            if (!ValidateInviteCode(inviteCode))
            {
                return null;
            }

            var currUser = await _userManager.GetUserAsync(HttpContext.User);

            PollData pollData = new PollData();
            PollModel? pollToJoin = await pollData.GetPollByJoinCode(inviteCode);

            if (pollToJoin == null)
            {
                return null;
            }

            ParticipantData participantData = new ParticipantData();
            if ((await participantData.GetParticipantByIdAndPoll(currUser!.Id, pollToJoin.Id)) != null)
            {
                return null;
            }

            ParticipantModel participant = new ParticipantModel
            {
                Username = currUser!.Id,
                HasVoted = false,
                Role = SharedLibrary.PollRole.Voter,
                PollId = pollToJoin.Id
            };

            await participantData.SaveParticipant(participant);

            participant.Username = currUser.UserName!;
            pollToJoin.Participants = [participant];
            return pollToJoin;
        } 

        private bool ValidateInviteCode(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return false;
            }
            if (code.Length > 128)
            {
                return false;
            }
            return true;
        }
    }
}
