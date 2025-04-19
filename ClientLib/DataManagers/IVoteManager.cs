
using SharedLibrary.Models;

namespace ClientLib.DataManagers
{
    public interface IVoteManager
    {
        Task<VoteSubmitResult> SubmitVote(int pollId, int voteOptionId);
        bool TryGetVotedOptionId(int pollId, out int optionId);
        bool TryGetBallot(int pollId, out SignedBallotModel? ballot);
    }
}