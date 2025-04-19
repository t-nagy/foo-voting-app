using ClientLib.Persistance;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Security;
using SharedLibrary;
using SharedLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ClientLib.DataManagers
{
    public class ApiVoteManager : ShufflerApiCaller, IVoteManager
    {
        private readonly IVoteAdministrationManager _adminManager;
        private readonly IKeyManager _keyManager;
        private readonly ILocalVoteDataAccess _localDataAccess;

        private RSACryptoServiceProvider? adminRSA;
        private RsaBlindingParameters? blindingParams;
        private RsaKeyParameters AdminPublicKey
        {
            get { return DotNetUtilities.GetRsaPublicKey(adminRSA); }
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

            _localDataAccess.StoreVote(_adminManager.LoggedInEmail!, adminSignedUnblindedBallot, userPollRSA, voteOptionId);

            if (!await PostVoteToShuffler(adminSignedUnblindedBallot))
            {
                return VoteSubmitResult.ShufflerPostFailed;
            }

            var decrypted = userPollRSA.Decrypt(adminSignedUnblindedBallot.Ballot, RSAEncryptionPadding.Pkcs1);
            var encoder = new UTF8Encoding();
            var dstring = encoder.GetString(decrypted);

            return VoteSubmitResult.Success;
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

            adminRSA = new RSACryptoServiceProvider();
            adminRSA.ImportFromPem(adminKeyPem);

            var blindingFactorGenerator = new RsaBlindingFactorGenerator();
            blindingFactorGenerator.Init(AdminPublicKey);
            var blindingFactor = blindingFactorGenerator.GenerateBlindingFactor();
            blindingParams = new RsaBlindingParameters(AdminPublicKey, blindingFactor);

            PssSigner signer = new PssSigner(new RsaBlindingEngine(), new Sha256Digest(), 20);
            signer.Init(true, blindingParams);
            signer.BlockUpdate(cb, 0, cb.Length);
            byte[] bcb = signer.GenerateSignature();

            RSA userRSA = _keyManager.GetUserSigningKey();
            byte[] sv = userRSA.SignData(bcb, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

            return new SignedBallotModel { Ballot = bcb, Signature = sv };
        }

        private byte[] UnblindSignature(byte[] sa)
        {
            RsaBlindingEngine blindingEngine = new RsaBlindingEngine();
            blindingEngine.Init(false, blindingParams);
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
    }


}
