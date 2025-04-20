using SharedLibrary.Models;
using System.Numerics;

namespace CounterAPI.DataAccess.DbModels
{
    public class EncryptedBallotDbModel
    {
        public int PollId { get; set; }
        public string? EncryptedBallot { get; set; }
        public string? AdminSignature { get; set; }

        public EncryptedBallotDbModel()
        {

        }

        public EncryptedBallotDbModel(SignedBallotModel vote)
        {
            PollId = vote.PollId;
            EncryptedBallot = new BigInteger(vote.Ballot).ToString();
            AdminSignature = new BigInteger(vote.Signature!).ToString();
        }

        public SignedBallotModel? ToSignedBallotModel()
        {
            if (EncryptedBallot == null)
            {
                return null;
            }
            byte[] encBallot = BigInteger.Parse(EncryptedBallot).ToByteArray();
            byte[]? signature = AdminSignature != null ? BigInteger.Parse(AdminSignature).ToByteArray() : null;

            return new SignedBallotModel { PollId = PollId, Ballot = encBallot, Signature = signature };
        }
    }
}
