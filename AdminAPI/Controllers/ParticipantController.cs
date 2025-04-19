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

        public ParticipantController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet(Name = "GetParticipants"), Authorize]
        public async Task<List<ParticipantModel>> Get(int pollId)
        {
            var currUser = await _userManager.GetUserAsync(HttpContext.User);
            ParticipantData data = new ParticipantData();
            var userQuery = await data.GetParticipantByIdAndPoll(currUser!.Id, pollId);
            if (userQuery == null || userQuery?.Role != SharedLibrary.PollRole.Owner)
            {
                Forbid();
            }

            var participants = await data.GetParticipantsByPoll(pollId);
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
            ParticipantData data = new ParticipantData();
            var userQuery = await data.GetParticipantByIdAndPoll(currUser!.Id, participant.PollId);
            if (userQuery == null || userQuery?.Role != SharedLibrary.PollRole.Owner)
            {
                return Forbid();
            }

            var userToAdd = await _userManager.FindByEmailAsync(participant.Username);
            if (userToAdd == null || await data.GetParticipantByIdAndPoll(userToAdd.Id, participant.PollId) != null)
            {
                return BadRequest();
            }
            participant.Username = userToAdd!.Id;
            participant.HasVoted = false;
            participant.Role = SharedLibrary.PollRole.Voter;
            await data.SaveParticipant(participant);

            return Ok();
        }
    }
}
