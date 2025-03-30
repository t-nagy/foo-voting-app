using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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

    }
}
