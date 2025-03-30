using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Models
{
    public class PollModel
    {
        public int Id { get; set; }
        public required string OwnerName { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime VoteCollectionEndDate { get; set; }
        public DateTime VoteValidationEndDate { get; set; }
        public bool IsPublic { get; set; }
        public string? JoinCode { get; set; }
    }
}
