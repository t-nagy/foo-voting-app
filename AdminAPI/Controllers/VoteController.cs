using AdminAPI.DataAccess;
using AdminAPI.Services;
using Azure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.JSInterop.Infrastructure;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Security;
using SharedLibrary.Models;
using System.Security.Cryptography;

namespace AdminAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class VoteController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IPollData _pollData;
        private readonly IParticipantData _participantData;
        private readonly IKeyData _keyData;
        private readonly KeyService _keyService;

        public VoteController(UserManager<IdentityUser> userManager, IPollData pollData, IParticipantData participantData, IKeyData keyData, KeyService keyService)
        {
            _userManager = userManager;
            _pollData = pollData;
            _participantData = participantData;
            _keyData = keyData;
            _keyService = keyService;
        }

        [HttpPost(Name = "ValidateBallot"), Authorize]
        public async Task<SignedBallotModel?> Post(SignedBallotModel blindedBallot)
        {
            PollModel? poll = await _pollData.LoadPoll(blindedBallot.PollId);
            if (poll == null)
            {
                return null;
            }

            var currUser = await _userManager.GetUserAsync(HttpContext.User);
            var participantQuery = await _participantData.GetParticipantByIdAndPoll(currUser!.Id, blindedBallot.PollId);
            if ((participantQuery == null && !poll.IsPublic) || participantQuery != null && participantQuery.HasVoted)
            {
                return null;
            }

            IEnumerable<string>? userKeys = await _keyData.GetKeyByUser(currUser.Id);
            if (userKeys == null)
            {
                // User did not register the key used for verification
                return null;
            }

            // Signature cannot be verified
            RSACryptoServiceProvider verifyRSA = new RSACryptoServiceProvider();
            bool isVerified = userKeys.Any((x) =>
            {
                verifyRSA.ImportFromPem(x);
                return verifyRSA.VerifyData(blindedBallot.Ballot, blindedBallot.Signature!, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            });

            if (!isVerified)
            {
                // Couldn't find key to verify signature or signature is not valid
                return null;
            }

            if (poll.IsPublic && participantQuery == null)
            {
                await _participantData.SaveParticipant(new ParticipantModel { Username = currUser!.Id, HasVoted = true, PollId = poll.Id, Role = SharedLibrary.PollRole.Voter }); 
            }

            await _participantData.ParticipantSetVoted(currUser!.Id, blindedBallot.PollId);

            AsymmetricCipherKeyPair pair = DotNetUtilities.GetRsaKeyPair(_keyService.RSA);
            RsaEngine engine = new RsaEngine();
            engine.Init(true, pair.Private);
            var sa = engine.ProcessBlock(blindedBallot.Ballot, 0, blindedBallot.Ballot.Length);

            return new SignedBallotModel { PollId = poll.Id, Ballot = blindedBallot.Ballot, Signature = sa };
        }
    }
}
