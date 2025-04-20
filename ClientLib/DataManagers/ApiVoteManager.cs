using ClientLib.Persistance;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using SharedLibrary;
using SharedLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace ClientLib.DataManagers
{
    public class ApiVoteManager : ShufflerApiCaller, IVoteManager
    {
        private readonly IVoteAdministrationManager _adminManager;
        private readonly IKeyManager _keyManager;
        private readonly ILocalVoteDataAccess _localDataAccess;

        private RSACryptoServiceProvider? _adminRSA;
        private RsaBlindingParameters? _blindingParams;
        private RSA? _counterRSA;
        private RsaKeyParameters AdminPublicKey
        {
            get { return DotNetUtilities.GetRsaPublicKey(_adminRSA); }
        }

        public ApiVoteManager(IVoteAdministrationManager adminManager, IKeyManager keyManager, ILocalVoteDataAccess localDataAccess)
        {
            _adminManager = adminManager;
            _keyManager = keyManager;
            _localDataAccess = localDataAccess;
        }

        public bool TryGetVotedOptionId(int pollId, out int optionId)
        {
            optionId = 0;
            if (_localDataAccess.TryGetVotedOption(_adminManager.LoggedInEmail!, pollId, out int optId))
            {
                optionId = optId;
                return true;
            }

            return false;
        }

        public bool TryGetBallot(int pollId, out SignedBallotModel? ballot)
        {
            ballot = null;
            if (_localDataAccess.TryGetBallot(_adminManager.LoggedInEmail!, pollId, out ballot))
            {
                return true;
            }

            return false;
        }

        public ValidatedState GetValidatedState(int pollId)
        {
            return _localDataAccess.GetValidatedState(_adminManager.LoggedInEmail!, pollId);
        }

        public async Task<VoteSubmitResult> SubmitVote(int pollId, int voteOptionId)
        {
            if (!await _adminManager.TryRegisterKey(_keyManager.GetUserSigningKey().ExportRSAPublicKeyPem()))
            {
                return VoteSubmitResult.KeyRegistrationFailed;
            }

            var userPollRSA = new RSACryptoServiceProvider();
            var commitedBallot = EncryptBallot(voteOptionId, userPollRSA);

            SignedBallotModel? blindedCommitedBallot = await PrepareForBlindSignature(commitedBallot);
            if (blindedCommitedBallot == null)
            {
                return VoteSubmitResult.NoAdminVerificationKey;
            }
            blindedCommitedBallot.PollId = pollId;

            var adminSignedBlindedBallot = await _adminManager.GetAdminSignature(blindedCommitedBallot);
            if (adminSignedBlindedBallot == null)
            {
                return VoteSubmitResult.AdminRefusedToSign;
            }

            SignedBallotModel adminSignedUnblindedBallot = new SignedBallotModel { Ballot = commitedBallot, PollId = adminSignedBlindedBallot.PollId };
            adminSignedUnblindedBallot.Signature = UnblindSignature(adminSignedBlindedBallot.Signature!);
            if (!VerifyAdminSignature(adminSignedUnblindedBallot.Ballot, adminSignedUnblindedBallot.Signature))
            {
                return VoteSubmitResult.AdminSignatureInvalid;
            }

            var transportReadyBallot = await EncryptForTransport(adminSignedUnblindedBallot.Ballot);
            if (transportReadyBallot.Length == 0)
            {
                return VoteSubmitResult.TrasportEncryptionFailed;
            }
            var transportReadySignature = await EncryptForTransport(adminSignedUnblindedBallot.Signature);
            if (transportReadySignature.Length == 0)
            {
                return VoteSubmitResult.TrasportEncryptionFailed;
            }

            SignedBallotModel transportReadyVote = new SignedBallotModel { PollId = adminSignedUnblindedBallot.PollId, Ballot = transportReadyBallot, Signature = transportReadySignature };


            _localDataAccess.StoreVote(_adminManager.LoggedInEmail!, adminSignedUnblindedBallot, new BigInteger(transportReadyBallot), userPollRSA, voteOptionId);

            if (!await PostVoteToShuffler(transportReadyVote))
            {
                return VoteSubmitResult.ShufflerPostFailed;
            }

            return VoteSubmitResult.Success;
        }

        public async Task<VoteValidationResult> ValidateVote(int pollId)
        {
            SignedBallotModel ballot;
            RSA key;
            if (!_localDataAccess.TryGetBallot(_adminManager.LoggedInEmail!, pollId, out ballot!))
            {
                throw new InvalidOperationException();
            }
            if (!_localDataAccess.TryGetKey(_adminManager.LoggedInEmail!, pollId, out key!))
            {
                throw new InvalidOperationException();
            }
            ValidationModel validation = new ValidationModel {PollId = ballot.PollId, EncryptedBallot = ballot.Ballot, EncryptionKey = key.ExportRSAPrivateKeyPem() };

            BigInteger transportEncryptedBallot;
            if (!_localDataAccess.TryGetTransportEncryptedBallot(_adminManager.LoggedInEmail!, pollId, out transportEncryptedBallot))
            {
                return VoteValidationResult.LocalDataCorrupted;
            }

            var test = transportEncryptedBallot.ToString();

            ValidationModel transportReadyValidation = new ValidationModel { PollId = validation.PollId, EncryptedBallot = transportEncryptedBallot.ToByteArray(), EncryptionKey = validation.EncryptionKey };

            if (!await PostValidationToShuffler(transportReadyValidation))
            {
                return VoteValidationResult.ShufflerPostFailed;
            }

            _localDataAccess.UpdateValidatedAttribute(_adminManager.LoggedInEmail!, pollId);

            return VoteValidationResult.Success;
        }

        public async Task<Dictionary<int, int>?> GetResults(int pollId)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(AddressService.CounterAddress);
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response;
            try
            {
                response = await client.GetAsync($"/Vote/?pollId={pollId}");
            }
            catch (ServerUnreachableException)
            {
                throw;
            }

            IEnumerable<SubmittedVoteModel>? submittedVotes;
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    submittedVotes = await response.Content.ReadFromJsonAsync<IEnumerable<SubmittedVoteModel>>();
                }
                catch (Exception)
                {
                    return null;
                }

                if (submittedVotes == null)
                {
                    return null;
                }
            }
            else
            {
                return null;
            }

            try
            {
                response = await client.GetAsync($"/Validate/?pollId={pollId}");
            }
            catch (ServerUnreachableException)
            {
                throw;
            }

            if (response.IsSuccessStatusCode)
            {
                IEnumerable<SubmittedVoteModel>? validatedVotes;
                try
                {
                    validatedVotes = await response.Content.ReadFromJsonAsync<IEnumerable<SubmittedVoteModel>>();
                }
                catch (Exception)
                {
                    return null;
                }

                if (validatedVotes == null)
                {
                    return null;
                }

                if (!CheckVotesValidity(submittedVotes, validatedVotes))
                {
                    throw new VoteInvalidException("Some of the votes in validated vote are not found within the submitted votes!");
                }
                return ProcessResults(pollId, validatedVotes);
            }

            return null;
        }

        private bool CheckVotesValidity(IEnumerable<SubmittedVoteModel> submittedVotes, IEnumerable<SubmittedVoteModel> validatedVotes)
        {
            bool invalid = validatedVotes.Any((x) =>
            {
                return submittedVotes.All(y => new BigInteger(y.EncryptedBallot).ToString() != new BigInteger(x.EncryptedBallot).ToString());
            });

            return !invalid;
        }

        private Dictionary<int, int> ProcessResults(int pollId, IEnumerable<SubmittedVoteModel> votes)
        {
            bool shouldCheckValidity = true;
            ValidatedState validatedState = _localDataAccess.GetValidatedState(_adminManager.LoggedInEmail!, pollId);
            if (!_localDataAccess.TryGetBallot(_adminManager.LoggedInEmail!, pollId, out SignedBallotModel? ballot))
            {
                shouldCheckValidity = false;
            }
            if (validatedState != ValidatedState.Yes)
            {
                shouldCheckValidity = false;
            }
            if (shouldCheckValidity)
            {
                if (!votes.Any(x => new BigInteger(x.EncryptedBallot).ToString() == new BigInteger(ballot!.Ballot).ToString()))
                {
                    throw new VoteInvalidException("The user's ballot was not found in the results!");
                }
            }

            Dictionary<int, int> results = new Dictionary<int, int>();
            foreach (var v in votes)
            {
                var decryptedResult = DecryptResult(v);

                if (results.ContainsKey(decryptedResult))
                {
                    results[decryptedResult]++;
                }
                else
                {
                    results.Add(decryptedResult, 1);
                }
            }

            return results;
        }

        private int DecryptResult(SubmittedVoteModel vote)
        {
            RSA rsa = new RSACryptoServiceProvider();
            rsa.ImportFromPem(vote.EncryptionKey);
            var encoder = new UTF8Encoding();
            var decryptedResultString = encoder.GetString(rsa.Decrypt(vote.EncryptedBallot, RSAEncryptionPadding.Pkcs1));
            return int.Parse(decryptedResultString);
        }

        private byte[] EncryptBallot(int voteOptionId, RSA userPollRSA)
        {
            string optionIdString = voteOptionId.ToString();
            var encoder = new UTF8Encoding();
            byte[] data = encoder.GetBytes(optionIdString);
            return userPollRSA.Encrypt(data, RSAEncryptionPadding.Pkcs1);
        }

        private async Task<SignedBallotModel?> PrepareForBlindSignature(byte[] cb)
        {
            string? adminKeyPem = await _adminManager.GetAdminVerificationKey();
            if (adminKeyPem == null)
            {
                return null;
            }

            _adminRSA = new RSACryptoServiceProvider();
            _adminRSA.ImportFromPem(adminKeyPem);

            var blindingFactorGenerator = new RsaBlindingFactorGenerator();
            blindingFactorGenerator.Init(AdminPublicKey);
            var blindingFactor = blindingFactorGenerator.GenerateBlindingFactor();
            _blindingParams = new RsaBlindingParameters(AdminPublicKey, blindingFactor);

            PssSigner signer = new PssSigner(new RsaBlindingEngine(), new Sha256Digest(), 20);
            signer.Init(true, _blindingParams);
            signer.BlockUpdate(cb, 0, cb.Length);
            byte[] bcb = signer.GenerateSignature();

            RSA userRSA = _keyManager.GetUserSigningKey();
            byte[] sv = userRSA.SignData(bcb, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

            return new SignedBallotModel { Ballot = bcb, Signature = sv };
        }

        private byte[] UnblindSignature(byte[] sa)
        {
            RsaBlindingEngine blindingEngine = new RsaBlindingEngine();
            blindingEngine.Init(false, _blindingParams);
            return blindingEngine.ProcessBlock(sa, 0, sa.Length);
        }

        private bool VerifyAdminSignature(byte[] cb, byte[] scb)
        {
            var signer = new PssSigner(new RsaEngine(), new Sha256Digest(), 20);
            signer.Init(false, AdminPublicKey);
            signer.BlockUpdate(cb, 0, cb.Length);
            return signer.VerifySignature(scb);
        }

        private async Task<bool> PostVoteToShuffler(SignedBallotModel vote)
        {
            HttpResponseMessage response;
            try
            {
                var content = new HttpRequestMessage();
                response = await _client.PostAsJsonAsync("/vote", vote);
            }
            catch (HttpRequestException ex)
            {
                throw new ServerUnreachableException(DefaultServerUnreachableExceptionMessage, ex);
            }

            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            return false;
        }

        private async Task<bool> PostValidationToShuffler(ValidationModel validation)
        {
            HttpResponseMessage response;
            try
            {
                var content = new HttpRequestMessage();
                response = await _client.PostAsJsonAsync("/validate", validation);
            }
            catch (HttpRequestException ex)
            {
                throw new ServerUnreachableException(DefaultServerUnreachableExceptionMessage, ex);
            }

            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            return false;
        }

        private async Task<byte[]> EncryptForTransport(byte[] data)
        {
            if (_counterRSA == null)
            {
                if (!await AcquireCounterRSA())
                {
                    return new byte[0];
                }
            }

            return _counterRSA!.Encrypt(data, RSAEncryptionPadding.Pkcs1);
        }

        private async Task<bool> AcquireCounterRSA()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(AddressService.CounterAddress);
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response;
            try
            {
                response = await client.GetAsync($"/PublicKey");
            }
            catch (Exception)
            {
                return false;
            }

            if (response.IsSuccessStatusCode)
            {
                RSAKeyWrapper? wrapper = null;
                try
                {
                    wrapper = await response.Content.ReadFromJsonAsync<RSAKeyWrapper?>();
                }
                catch (Exception)
                {
                }
                if (wrapper == null)
                {
                    return false;
                }

                _counterRSA = new RSACryptoServiceProvider(2048);
                _counterRSA.ImportFromPem(wrapper.Key);
                return true;
            }

            return false;
        }

    }


}
