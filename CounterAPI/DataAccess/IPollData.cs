
using SharedLibrary;

namespace CounterAPI.DataAccess
{
    public interface IPollData
    {
        Task<DateTime?> GetValidationEndDate(int pollId);
        Task<DateTime?> GetVoteEndDate(int pollId);
        Task UpdatePollStatus(int pollId, PollStatus newStatus);
        Task<PollStatus> GetPollStatus(int pollId);
    }
}