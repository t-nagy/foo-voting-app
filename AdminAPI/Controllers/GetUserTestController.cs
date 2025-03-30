using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AdminAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class GetUserTestController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;

        public GetUserTestController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet(Name = "FindByEmailTest")]
        public async Task<IdentityUser?> GetUser(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            Console.WriteLine(user?.Id);
            return user;
        }

    }
}
