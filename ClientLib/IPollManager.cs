using ClientLib.Authentication;
using SharedLibrary.Models;

namespace ClientLib
{
    public interface IPollManager : ILoginRequester
    {
        Task<PollModel?> CreatePoll(PollModel poll);
    }
}