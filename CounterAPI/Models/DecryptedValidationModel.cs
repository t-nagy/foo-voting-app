namespace CounterAPI.Models
{
    public class DecryptedValidationModel
    {
        public int PollId { get; set; }
        public required byte[] EncryptedBallot { get; set; }
        public required string EncryptionKey { get; set; }
        public int DecryptedBallot { get; set; }
    }
}
