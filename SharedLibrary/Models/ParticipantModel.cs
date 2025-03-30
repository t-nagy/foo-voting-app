using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Models
{
    public class ParticipantModel
    {
        public required string UserId { get; set; }
        public int PollId { get; set; }
        public PollRole Role { get; set; }
        public bool HasVoted { get; set; }
    }
}
