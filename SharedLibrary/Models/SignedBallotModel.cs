using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Models
{
    public class SignedBallotModel
    {
        public int PollId { get; set; }
        public required byte[] Ballot { get; set; }
        public byte[]? Signature { get; set; }
    }
}
