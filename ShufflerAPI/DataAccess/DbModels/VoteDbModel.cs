using SharedLibrary.Models;
using System.Numerics;

namespace ShufflerAPI.DataAccess.DbModels
{
    public class VoteDbModel
    {
        public int PollId { get; set; }
        public string? EncryptedBallot { get; set; }
        public string? AdminSignature { get; set; }

        public VoteDbModel()
        {
            
        }

        public VoteDbModel(SignedBallotModel vote)
        {
            PollId = vote.PollId;
            EncryptedBallot = new BigInteger(vote.Ballot).ToString();
            AdminSignature = new BigInteger(vote.Signature!).ToString();
        }

        public SignedBallotModel? ToSignedBallotModel()
        {
            if (EncryptedBallot == null || AdminSignature == null)
            {
                return null;
            }
            byte[] encBallot = BigInteger.Parse(EncryptedBallot).ToByteArray();
            byte[] signature = BigInteger.Parse(AdminSignature).ToByteArray();

            return new SignedBallotModel { PollId = PollId, Ballot = encBallot, Signature = signature };
        }
    }
}
