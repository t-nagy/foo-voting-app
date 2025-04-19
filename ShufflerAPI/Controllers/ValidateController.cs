using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Models;
using ShufflerAPI.DataAccess;
using ShufflerAPI.Services;

namespace ShufflerAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ValidateController : ControllerBase
    {
        private readonly IVoteData _voteData;
        private readonly EndDateService _endService;

        public ValidateController(IVoteData voteData, EndDateService endService)
        {
            _voteData = voteData;
            _endService = endService;
        }

        [HttpPost(Name = "ValidateVote")]
        public async Task<IActionResult> Post(ValidationModel validation)
        {
            var votes = await _voteData.GetValidationsByPoll(validation.PollId);
            if (votes == null || !votes.Any(x => x.EncryptedBallot == validation.EncryptedBallot && x.EncryptionKey == null))
            {
                return BadRequest();
            }

            await _voteData.UpdateValidation(validation);

            return Ok();
        }
    }
}
