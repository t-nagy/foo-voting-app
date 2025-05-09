﻿using SharedLibrary.Models;
using ShufflerAPI.Models;

namespace ShufflerAPI.DataAccess
{
    public interface IVoteData
    {
        Task<IEnumerable<SignedBallotModel>> GetVotesByPoll(int pollId);
        Task<IEnumerable<ValidationModel>> GetValidationsByPoll(int pollId);
        Task SaveVote(SignedBallotModel vote);
        Task UpdateValidation(ValidationModel validation);
        Task UpdateIsSubmitted(int pollId);
        Task<IEnumerable<WaitingPollModel>> GetPollIdsAndSubmittedState();
        Task DeleteValidatedVotes(int pollId);
    }
}