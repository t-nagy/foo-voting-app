using Microsoft.AspNetCore.Mvc.Formatters;
using SharedLibrary.Models;
using System.Numerics;

namespace CounterAPI.DataAccess.DbModels
{
    public class SubmittedVoteDbModel
    {
        public int PollId { get; set; }
        public required string EncryptedBallot { get; set; }
        public string? EncryptionKey { get; set; }
        public int DecryptedBallot { get; set; }

        public SubmittedVoteModel ToSubmittedVoteModel()
        {
            return new SubmittedVoteModel
            {
                PollId = PollId,
                EncryptedBallot = BigInteger.Parse(EncryptedBallot).ToByteArray(),
                EncryptionKey = EncryptionKey,
                DecryptedBallot = DecryptedBallot
            };
        }
    }
}
