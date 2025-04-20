using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Models
{
    public class SubmittedVoteModel
    {
        public int PollId { get; set; }
        public required byte[] EncryptedBallot { get; set; }
        public string? EncryptionKey { get; set; }
        public int DecryptedBallot { get; set; }

        public bool CanBeDecrypted
        {
            get { return EncryptionKey != null; }
        }
    }
}
