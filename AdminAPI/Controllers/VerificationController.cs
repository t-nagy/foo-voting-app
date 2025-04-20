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
        private readonly IKeyData _keyData;
        private readonly KeyService _keyService;

        public VerificationController(UserManager<IdentityUser> userManager, IKeyData keyData, KeyService keyService)
        {
            _userManager = userManager;
            _keyData = keyData;
            _keyService = keyService;
        }

        [HttpGet(Name = "GetVerificationKey")]
        public RSAKeyWrapper Get()
        {
            return new RSAKeyWrapper { Key = _keyService.RSA.ExportRSAPublicKeyPem() };
        }


        [HttpPost(Name = "TryRegisterNewUserKey"), Authorize]
        public async Task<bool> Post([FromBody] string key)
        {
            var currUser = await _userManager.GetUserAsync(HttpContext.User);

            var registeredKeys = await _keyData.GetKeyByUser(currUser!.Id);
            if (registeredKeys != null)
            {
                if (registeredKeys.Any(x => x == key))
                {
                    return false;
                }
            }

            await _keyData.SaveKey(currUser!.Id, key);
            return true;
        }
    }
}
