using SharedLibrary.Models;
using System.Numerics;

namespace CounterAPI.DataAccess.DbModels
{
    public class ValidationDbModel
    {
        public int PollId { get; set; }
        public string? EncryptedBallot { get; set; }
        public string? EncryptionKey { get; set; }

        public ValidationDbModel()
        {

        }

        public ValidationDbModel(ValidationModel validation)
        {
            PollId = validation.PollId;
            EncryptedBallot = new BigInteger(validation.EncryptedBallot).ToString();
            EncryptionKey = validation.EncryptionKey;
        }

        public ValidationModel ToValidationModel()
        {
            return new ValidationModel
            {
                PollId = PollId,
                EncryptedBallot = BigInteger.Parse(EncryptedBallot!).ToByteArray(),
                EncryptionKey = EncryptionKey
            };
        }
    }
}
