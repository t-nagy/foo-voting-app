using ClientLib.Authentication;
using SharedLibrary.Models;

namespace ClientLib.DataManagers
{
    public interface IParticipantManager : ILoginRequester, ILoggedInUserManager
    {
        Task<bool> AddParticipant(ParticipantModel participant);
        Task<PollModel?> JoinPollWithCode(string code);
    }
}