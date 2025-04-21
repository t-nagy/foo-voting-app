using CounterAPI.DataAccess;
using CounterAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Security;
using SharedLibrary;
using SharedLibrary.Models;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace CounterAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class VoteController : ControllerBase
    {
        private readonly IVoteData _voteData;
        private readonly IPollData _pollData;
        private readonly KeyService _keyService;
        private readonly ConfigHelper _config;

        public VoteController(IVoteData voteData, IPollData pollData, KeyService keyService, ConfigHelper config)
        {
            _voteData = voteData;
            _pollData = pollData;
            _keyService = keyService;
            _config = config;
        }

        [HttpGet(Name = "GetSubmittedVotes")]
        public async Task<IEnumerable<SubmittedVoteModel>> Get(int pollId)
        {
            return await _voteData.GetVotesByPoll(pollId);
        }

        [HttpPost(Name = "SubmitVotes")]
        public async Task<IActionResult> Post(string usageKey, IEnumerable<SignedBallotModel> votes)
        {
            if (usageKey != _config.GetAPIUsageKey())
            {
                return Forbid();
            }

            if (votes.Count() < 1)
            {
                return Ok();
            }
            int pollId = votes.First().PollId;

            var votingEndDate = await _pollData.GetVoteEndDate(pollId);
            if (votingEndDate == null || votingEndDate == default || votingEndDate > DateTime.UtcNow)
            {
                return BadRequest();
            }

            if (await _pollData.GetPollStatus(pollId) != PollStatus.Vote)
            {
                return Ok();
            }

            votes = RemoveTransportEncryption(votes);
            votes = await VerifyAdminSignature(votes);

            if (votes.Count() > 0)
            {
                await _voteData.SaveVotes(votes);
            }

            await _pollData.UpdatePollStatus(pollId, SharedLibrary.PollStatus.Validate);

            return Ok();
        }

        private IEnumerable<SignedBallotModel> RemoveTransportEncryption(IEnumerable<SignedBallotModel> votes)
        {
            List<SignedBallotModel> transEncRemovedVotes = new List<SignedBallotModel>();
            RSA transportRSA = _keyService.TransportEncryptionRSA;
            foreach (var vote in votes)
            {
                try
                {
                    SignedBallotModel encRemoved = new SignedBallotModel
                    {
                        PollId = vote.PollId,
                        Ballot = transportRSA.Decrypt(vote.Ballot, RSAEncryptionPadding.Pkcs1),
                        Signature = transportRSA.Decrypt(vote.Signature!, RSAEncryptionPadding.Pkcs1)
                    };

                    transEncRemovedVotes.Add(encRemoved);
                }
                catch (Exception) { }
            }

            return transEncRemovedVotes;
        }

        private async Task<IEnumerable<SignedBallotModel>> VerifyAdminSignature(IEnumerable<SignedBallotModel> votes)
        {
            List<SignedBallotModel> verifiedVotes = new List<SignedBallotModel>();
            var signer = new PssSigner(new RsaEngine(), new Sha256Digest(), 20);
            var adminRSA = await _keyService.GetVerificationRSA();
            if (adminRSA == null)
            {
                // Cannot obtain admin verification key, cannot verify votes!
                return new List<SignedBallotModel>();
            }
            signer.Init(false, DotNetUtilities.GetRsaPublicKey(adminRSA));
            foreach (var vote in votes)
            {
                signer.BlockUpdate(vote.Ballot, 0, vote.Ballot.Length);
                if (signer.VerifySignature(vote.Signature))
                {
                    verifiedVotes.Add(vote);
                }
            }

            return verifiedVotes;
        }
    }
}
