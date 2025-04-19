using AdminAPI.DataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdminAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class VotingEndController : ControllerBase
    {
        private readonly IPollData _pollData;

        public VotingEndController(IPollData pollData)
        {
            _pollData = pollData;
        }


        [HttpGet(Name = "GetVotingEndDate")]
        public async Task<DateTime?> Get(int pollId)
        {
            return await _pollData.GetVoteEndDate(pollId);
        }
    }
}
