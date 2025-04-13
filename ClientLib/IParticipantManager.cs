using SharedLibrary.Models;

namespace ClientLib
{
    public interface IParticipantManager
    {
        Task<bool> AddParticipant(ParticipantModel participant);
        Task<PollModel?> JoinPollWithCode(string code);
    }
}