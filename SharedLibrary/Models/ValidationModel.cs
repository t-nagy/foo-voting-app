using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Models
{
    public class ValidationModel
    {
        public int PollId { get; set; }
        public required string EncryptedBallot { get; set; }
        public string? EncryptionKey { get; set; }
    }
}
