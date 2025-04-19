using SharedLibrary;
using SharedLibrary.Models;

namespace AdminAPI.DataAccess.DataModels
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
                PollId = PollId,
                Role = (PollRole)ParticipantRole,
                HasVoted = HasVoted
            };
        }
    }
}
