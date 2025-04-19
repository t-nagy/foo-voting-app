using AdminAPI.DataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdminAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class VotingEndController : ControllerBase
    {


        [HttpGet(Name = "GetVotingEndDate")]
        public async Task<DateTime?> Get(int pollId)
        {
            PollData pollData = new PollData();
            return await pollData.GetVoteEndDate(pollId);
        }
    }
}
