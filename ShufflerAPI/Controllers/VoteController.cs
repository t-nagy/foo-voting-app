using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Models;
using ShufflerAPI.DataAccess;
using ShufflerAPI.Services;
using System.Security.Cryptography;

namespace ShufflerAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class VoteController : ControllerBase
    {
        private readonly IVoteData _voteData;
        private readonly EndDateService _endService;

        public VoteController(IVoteData voteData, EndDateService endService)
        {
            _voteData = voteData;
            _endService = endService;
        }

        [HttpPost(Name = "SubmitVote")]
        public async Task<IActionResult> Post(SignedBallotModel vote)
        {
            var votes = await _voteData.GetVotesByPoll(vote.PollId);
            if (votes != null && votes.Any(x => x.Ballot == vote.Ballot))
            {
                return BadRequest();
            }

            var votingEndDate = await _endService.GetVotingEndDate(vote.PollId);
            if (votingEndDate == null || votingEndDate == default || votingEndDate < DateTime.UtcNow)
            {
                return BadRequest();
            }

            await _voteData.SaveVote(vote);


            return Ok();
        }
    }
}
