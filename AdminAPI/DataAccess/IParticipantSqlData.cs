using SharedLibrary.Models;

namespace AdminAPI.DataAccess
{
    public interface IParticipantData
    {
        Task<ParticipantModel?> GetParticipantByIdAndPoll(string participantId, int pollId);
        Task<List<ParticipantModel>> GetParticipantsByPoll(int pollId);
        Task ParticipantSetVoted(string participantId, int pollid);
        Task SaveParticipant(ParticipantModel participant, SqlDataAccess? sql = null);
    }
}