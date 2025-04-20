using AdminAPI.DataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdminAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ValidationEndController : ControllerBase
    {
        private readonly IPollData _pollData;

        public ValidationEndController(IPollData pollData)
        {
            _pollData = pollData;
        }

        [HttpGet(Name = "GetValidationEndDate")]
        public async Task<DateTime?> Get(int pollId)
        {
            return await _pollData.GetValidationEndDate(pollId);
        }
    }
}
