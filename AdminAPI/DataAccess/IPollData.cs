using SharedLibrary.Models;

namespace AdminAPI.DataAccess
{
    public interface IPollData
    {
        Task<PollModel?> GetPollByJoinCode(string code);
        Task<DateTime?> GetVoteEndDate(int pollId);
        Task<DateTime?> GetValidationEndDate(int pollId);
        Task<PollModel?> LoadPoll(int pollId);
        Task<IEnumerable<PollModel>> LoadPollsAllMinimal(string userId);
        Task<PollModel> SavePoll(PollModel poll);
    }
}