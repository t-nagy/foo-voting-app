using SharedLibrary;
using SharedLibrary.Models;

namespace AdminAPI.Controllers.DataAccess.DataModels
{
    internal class ParticipantDbModel
    {
        public required string UserId { get; set; }
        public int PollId { get; set; }
        public int ParticipantRole { get; set; }
        public bool HasVoted { get; set; }

        public ParticipantModel ToParticipantModel()
        {
            return new ParticipantModel
            {
                Username = UserId,
                PollId = this.PollId,
                Role = (PollRole)ParticipantRole,
                HasVoted = this.HasVoted
            };
        }
    }
}
