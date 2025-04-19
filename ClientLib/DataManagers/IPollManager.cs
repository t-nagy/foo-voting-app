using ClientLib.Authentication;
using SharedLibrary.Models;

namespace ClientLib.DataManagers
{
    public interface IPollManager : ILoginRequester, ILoggedInUserManager
    {
        Task<IEnumerable<PollModel>?> GetAllPollsMinimal();
        Task<PollModel?> GetPollById(int pollId);
        Task<PollModel?> CreatePoll(PollModel poll);
    }
}