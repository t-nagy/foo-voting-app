using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Models;

namespace CounterAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PublicKeyController : ControllerBase
    {
        private readonly KeyService _keyService;

        public PublicKeyController(KeyService keyService)
        {
            _keyService = keyService;
        }

        [HttpGet(Name = "GetTransportEncryptionKey")]
        public RSAKeyWrapper Get()
        {
            return new RSAKeyWrapper { Key = _keyService.TransportEncryptionRSA.ExportRSAPublicKeyPem() };
        }
    }
}
