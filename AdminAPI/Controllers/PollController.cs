using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Models;

namespace AdminAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PollController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;

        public PollController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet(Name = "GetPolls")]
        public IEnumerable<PollModel> Get(int id = -1, bool allDetails = true)
        {
            return new List<PollModel>();
        }

        [HttpPost(Name = "CreatePoll"), Authorize]
        public PollModel Post(PollModel poll)
        {
            Console.WriteLine($"A poll with the title: \"{poll.Title}\" has arrived to the server!");
            poll.Id = 42;
            return poll;
        }
    }
}
