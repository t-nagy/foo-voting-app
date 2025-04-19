using AdminAPI.DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Models;
using System.IO.Compression;
using System.Runtime.InteropServices;

namespace AdminAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ParticipantController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IParticipantData _participantData;

        public ParticipantController(UserManager<IdentityUser> userManager, IParticipantData participantData)
        {
            _userManager = userManager;
            _participantData = participantData;
        }

        [HttpGet(Name = "GetParticipants"), Authorize]
        public async Task<List<ParticipantModel>> Get(int pollId)
        {
            var currUser = await _userManager.GetUserAsync(HttpContext.User);
            var userQuery = await _participantData.GetParticipantByIdAndPoll(currUser!.Id, pollId);
            if (userQuery == null || userQuery?.Role != SharedLibrary.PollRole.Owner)
            {
                Forbid();
            }

            var participants = await _participantData.GetParticipantsByPoll(pollId);
            foreach (var p in participants)
            {
                p.Username = (await _userManager.FindByIdAsync(p.Username))!.UserName!;
            }
            return participants;
        }


        [HttpPost(Name = "AddParticipant"), Authorize]
        public async Task<IActionResult> Post(ParticipantModel participant)
        {
            var currUser = await _userManager.GetUserAsync(HttpContext.User);
            var userQuery = await _participantData.GetParticipantByIdAndPoll(currUser!.Id, participant.PollId);
            if (userQuery == null || userQuery?.Role != SharedLibrary.PollRole.Owner)
            {
                return Forbid();
            }

            var userToAdd = await _userManager.FindByEmailAsync(participant.Username);
            if (userToAdd == null || await _participantData.GetParticipantByIdAndPoll(userToAdd.Id, participant.PollId) != null)
            {
                return BadRequest();
            }
            participant.Username = userToAdd!.Id;
            participant.HasVoted = false;
            participant.Role = SharedLibrary.PollRole.Voter;
            await _participantData.SaveParticipant(participant);

            return Ok();
        }
    }
}
