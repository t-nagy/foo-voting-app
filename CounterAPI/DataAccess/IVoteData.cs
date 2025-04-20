using CounterAPI.Models;
using SharedLibrary;
using SharedLibrary.Models;

namespace CounterAPI.DataAccess
{
    public interface IVoteData
    {
        Task<IEnumerable<SubmittedVoteModel>> GetVotesByPoll(int pollId);
        Task<IEnumerable<SubmittedVoteModel>> GetValidatedVotesByPoll(int pollId);
        Task<IEnumerable<SignedBallotModel>> GetEncryptedBallotsByPoll(int pollId);
        Task SaveVotes(IEnumerable<SignedBallotModel> votes);
        Task UpdateVotesWithValidation(IEnumerable<DecryptedValidationModel> validations);
    }
}