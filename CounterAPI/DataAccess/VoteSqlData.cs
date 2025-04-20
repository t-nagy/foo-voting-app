using CounterAPI.DataAccess.DbModels;
using CounterAPI.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Utilities;
using SharedLibrary;
using SharedLibrary.Models;
using System.Numerics;

namespace CounterAPI.DataAccess
{
    public class VoteSqlData : IVoteData
    {
        private readonly ConfigHelper _config;

        public VoteSqlData(ConfigHelper config)
        {
            _config = config;
        }

        public async Task<IEnumerable<SubmittedVoteModel>> GetVotesByPoll(int pollId)
        {
            using (SqlConnection connection = new SqlConnection(_config.GetConnectionString(_config.VoteDataConnectionStringName)))
            {
                var result = await connection.QueryAsync<SubmittedVoteDbModel>("uspPublishedVote_GetVotesByPoll", new { PollId = pollId }, commandType: System.Data.CommandType.StoredProcedure);
                List<SubmittedVoteModel> votes = new List<SubmittedVoteModel>();
                foreach (var v in result)
                {
                    votes.Add(v.ToSubmittedVoteModel());
                }

                return votes;
            }
        }

        public async Task<IEnumerable<SubmittedVoteModel>> GetValidatedVotesByPoll(int pollId)
        {
            using (SqlConnection connection = new SqlConnection(_config.GetConnectionString(_config.VoteDataConnectionStringName)))
            {
                var result = await connection.QueryAsync<SubmittedVoteDbModel>("uspPublishedVote_GetValidatedVotesByPoll", new { PollId = pollId }, commandType: System.Data.CommandType.StoredProcedure);
                List<SubmittedVoteModel> votes = new List<SubmittedVoteModel>();
                foreach (var v in result)
                {
                    votes.Add(v.ToSubmittedVoteModel());
                }

                return votes;
            }
        }

        public async Task<IEnumerable<SignedBallotModel>> GetEncryptedBallotsByPoll(int pollId)
        {
            using (SqlConnection connection = new SqlConnection(_config.GetConnectionString(_config.VoteDataConnectionStringName)))
            {
                var result = await connection.QueryAsync<EncryptedBallotDbModel>("uspPublishedVote_GetEncryptedBallotByPoll", new { PollId = pollId }, commandType: System.Data.CommandType.StoredProcedure);
                List<SignedBallotModel> votes = new List<SignedBallotModel>();
                foreach (var v in result)
                {
                    votes.Add(v.ToSignedBallotModel()!);
                }

                return votes;
            }
        }

        public async Task SaveVotes(IEnumerable<SignedBallotModel> votes)
        {
            using (SqlConnection connection = new SqlConnection(_config.GetConnectionString(_config.VoteDataConnectionStringName)))
            {
                foreach (var vote in votes)
                {
                    var dbModel = new EncryptedBallotDbModel(vote);
                    await connection.ExecuteAsync("uspPublishedVote_Insert", new { PollId = dbModel.PollId, EncryptedBallot = dbModel.EncryptedBallot }, commandType: System.Data.CommandType.StoredProcedure);
                }
            }
        }

        public async Task UpdateVotesWithValidation(IEnumerable<DecryptedValidationModel> validations)
        {
            using (SqlConnection connection = new SqlConnection(_config.GetConnectionString(_config.VoteDataConnectionStringName)))
            {
                foreach (var v in validations)
                {
                    await connection.ExecuteAsync("uspPublishedVote_UpdateValidation", new { PollId = v.PollId, EncryptedBallot = new BigInteger(v.EncryptedBallot).ToString(), EncryptionKey = v.EncryptionKey, DecryptedBallot = v.DecryptedBallot }, commandType: System.Data.CommandType.StoredProcedure);
                }
            }
        }
    }
}
