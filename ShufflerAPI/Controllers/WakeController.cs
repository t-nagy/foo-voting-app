using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShufflerAPI.Services;

namespace ShufflerAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class WakeController : ControllerBase
    {
        private readonly SubmitService _submitService;

        public WakeController(SubmitService submitService)
        {
            _submitService = submitService;
        }

        [HttpGet(Name = "WakeAPI")]
        public async Task Get()
        {
            await _submitService.SubmitDataToCounter();
        }
    }
}
