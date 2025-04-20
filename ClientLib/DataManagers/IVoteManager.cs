
using SharedLibrary.Models;

namespace ClientLib.DataManagers
{
    public interface IVoteManager
    {
        Task<VoteSubmitResult> SubmitVote(int pollId, int voteOptionId);
        Task<VoteValidationResult> ValidateVote(int pollId);
        bool TryGetVotedOptionId(int pollId, out int optionId);
        bool TryGetBallot(int pollId, out SignedBallotModel? ballot);
        ValidatedState GetValidatedState(int pollId);
        Task<Dictionary<int, int>?> GetResults(int pollId);
    }
}