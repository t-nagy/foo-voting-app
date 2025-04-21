using CounterAPI.DataAccess;
using CounterAPI.Models;
using CounterAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Utilities;
using SharedLibrary;
using SharedLibrary.Models;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace CounterAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ValidateController : ControllerBase
    {
        private readonly ConfigHelper _config;
        private readonly IPollData _pollData;
        private readonly IVoteData _voteData;
        private readonly KeyService _keyService;

        public ValidateController(ConfigHelper config, IPollData pollData, IVoteData voteData, KeyService keyService)
        {
            _config = config;
            _pollData = pollData;
            _voteData = voteData;
            _keyService = keyService;
        }

        [HttpGet(Name = "GetValidatedVotes")]
        public async Task<IEnumerable<SubmittedVoteModel>> Get(int pollId)
        {
            return await _voteData.GetValidatedVotesByPoll(pollId);
        }

        [HttpPost(Name = "ValidateVotes")]
        public async Task<IActionResult> Post(string usageKey, IEnumerable<ValidationModel> validations)
        {
            if (usageKey != _config.GetAPIUsageKey())
            {
                return Forbid();
            }

            if (validations.Count() < 1)
            {
                return Ok();
            }
            int pollId = validations.First().PollId;

            var validationEndDate = await _pollData.GetValidationEndDate(pollId);
            if (validationEndDate == null || validationEndDate == default || validationEndDate > DateTime.UtcNow)
            {
                return BadRequest();
            }

            if (await _pollData.GetPollStatus(pollId) != PollStatus.Validate)
            {
                return Ok();
            }

            validations = RemoveTransportEncryption(validations);
            var decryptedValidations = await DecryptValidations(pollId, validations);

            if (decryptedValidations.Count() > 0)
            {
                await _voteData.UpdateVotesWithValidation(decryptedValidations);
            }

            await _pollData.UpdatePollStatus(pollId, SharedLibrary.PollStatus.Closed);

            return Ok();
        }


        private IEnumerable<ValidationModel> RemoveTransportEncryption(IEnumerable<ValidationModel> validations)
        {
            List<ValidationModel> transEncRemovedValidations = new List<ValidationModel>();
            RSA transportRSA = _keyService.TransportEncryptionRSA;
            foreach (var validation in validations)
            {
                try
                {
                    ValidationModel encRemoved = new ValidationModel
                    {
                        PollId = validation.PollId,
                        EncryptedBallot = transportRSA.Decrypt(validation.EncryptedBallot, RSAEncryptionPadding.Pkcs1),
                        EncryptionKey = validation.EncryptionKey
                    };

                    transEncRemovedValidations.Add(encRemoved);
                }
                catch (Exception) { }
            }

            return transEncRemovedValidations;
        }

        private async Task<IEnumerable<DecryptedValidationModel>> DecryptValidations(int pollId, IEnumerable<ValidationModel> validations)
        {
            List<DecryptedValidationModel> decVals = new List<DecryptedValidationModel>();
            var encoder = new UTF8Encoding();
            var votes = await _voteData.GetEncryptedBallotsByPoll(pollId);
            foreach (var v in validations)
            {
                if (votes.Any(x => x.PollId == v.PollId && new BigInteger(x.Ballot).ToString() == new BigInteger(v.EncryptedBallot).ToString()))
                {
                    try
                    {
                        RSA rsa = new RSACryptoServiceProvider();
                        rsa.ImportFromPem(v.EncryptionKey);
                        var decryptedBallotBytes = rsa.Decrypt(v.EncryptedBallot, RSAEncryptionPadding.Pkcs1);
                        if(int.TryParse(encoder.GetString(decryptedBallotBytes), out int decryptedBallot))
                        {
                            decVals.Add(new DecryptedValidationModel { PollId = v.PollId, EncryptedBallot = v.EncryptedBallot, EncryptionKey = v.EncryptionKey!, DecryptedBallot = decryptedBallot });
                        }
                        
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            return decVals;
        }
    }
}
