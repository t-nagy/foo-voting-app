using AdminAPI.DataAccess;
using AdminAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SharedLibrary.Models;
using System.Text.Json.Nodes;

namespace AdminAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class VerificationController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;

        public VerificationController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet(Name = "GetVerificationKey")]
        public VerificationKeyWrapper Get()
        {
            KeyService keyService = new KeyService();
            return new VerificationKeyWrapper { VerificationKey = keyService.RSA.ExportRSAPublicKeyPem() };
        }


        [HttpPost(Name = "TryRegisterNewUserKey"), Authorize]
        public async Task<bool> Post([FromBody] string key)
        {
            var currUser = await _userManager.GetUserAsync(HttpContext.User);

            KeyData keyData = new KeyData();
            var registeredKeys = await keyData.GetKeyByUser(currUser!.Id);
            if (registeredKeys != null)
            {
                if (registeredKeys.Any(x => x == key))
                {
                    return false;
                }
            }

            await keyData.SaveKey(currUser!.Id, key);
            return true;
        }
    }
}
