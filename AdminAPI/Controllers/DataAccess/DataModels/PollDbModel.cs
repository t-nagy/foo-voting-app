using SharedLibrary.Models;
using SharedLibrary;

namespace AdminAPI.Controllers.DataAccess.DataModels
{
    public class PollDbModel
    {
        public int Id { get; set; }
        public string? OwnerName { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime VoteCollectionEndDate { get; set; }
        public DateTime VoteValidationEndDate { get; set; }
        public bool IsPublic { get; set; }
        public int Status { get; set; }
        public string? JoinCode { get; set; }
        public List<OptionModel>? PollOptions { get; set; }
        public List<ParticipantModel>? Participants { get; set; }

        public PollModel ToPollModel()
        {
            return new PollModel
            {
                Id = Id,
                OwnerName = OwnerName,
                Title = Title,
                Description = Description,
                CreatedDate = CreatedDate,
                VoteCollectionEndDate = VoteCollectionEndDate,
                VoteValidationEndDate = VoteValidationEndDate,
                IsPublic = IsPublic,
                Status = (PollStatus)Status,
                JoinCode = JoinCode,
                PollOptions = PollOptions,
                Participants = Participants
            };
        }
    }
}
