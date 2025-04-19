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
        private readonly IPollData _pollData;
        private readonly IParticipantData _participantData;

        public JoinController(UserManager<IdentityUser> userManager, IPollData pollData, IParticipantData participantData)
        {
            _userManager = userManager;
            _pollData = pollData;
            _participantData = participantData;
        }

        [HttpPost(Name = "JoinPoll"), Authorize]
        public async Task<PollModel?> Post(string inviteCode)
        {
            if (!ValidateInviteCode(inviteCode))
            {
                return null;
            }

            var currUser = await _userManager.GetUserAsync(HttpContext.User);

            PollModel? pollToJoin = await _pollData.GetPollByJoinCode(inviteCode);

            if (pollToJoin == null)
            {
                return null;
            }

            if ((await _participantData.GetParticipantByIdAndPoll(currUser!.Id, pollToJoin.Id)) != null)
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

            await _participantData.SaveParticipant(participant);

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
