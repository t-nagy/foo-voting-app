using Dapper;
using Microsoft.Data.SqlClient;
using SharedLibrary.Models;
using ShufflerAPI.DataAccess.DbModels;
using ShufflerAPI.Models;

namespace ShufflerAPI.DataAccess
{
    public class VoteSqlData : IVoteData
    {
        private readonly ConfigHelper _config;

        public VoteSqlData(ConfigHelper config)
        {
            _config = config;
        }

        public async Task<IEnumerable<SignedBallotModel>> GetVotesByPoll(int pollId)
        {
            using (SqlConnection connection = new SqlConnection(_config.GetConnectionString(_config.ShufflerDbConnectionStringName)))
            {
                var result = await connection.QueryAsync<VoteDbModel>("uspVote_GetSignedBallotsByPoll", new { Pollid = pollId }, commandType: System.Data.CommandType.StoredProcedure);
                List<SignedBallotModel> votes = new List<SignedBallotModel>();
                foreach (var v in result)
                {
                    SignedBallotModel? converted = v.ToSignedBallotModel();
                    if (converted != null)
                    {
                        votes.Add(converted);
                    }
                }

                return votes;
            }
        }

        public async Task<IEnumerable<ValidationModel>> GetValidationsByPoll(int pollId)
        {
            using (SqlConnection connection = new SqlConnection(_config.GetConnectionString(_config.ShufflerDbConnectionStringName)))
            {
                var result = await connection.QueryAsync<ValidationDbModel>("uspVote_GetValidationsByPoll", new { PollId = pollId }, commandType: System.Data.CommandType.StoredProcedure);
                List<ValidationModel> validations = new List<ValidationModel>();
                foreach (var v in result)
                {
                    validations.Add(v.ToValidationModel());
                }

                return validations;
            }
        }

        public async Task SaveVote(SignedBallotModel vote)
        {
            using (SqlConnection connection = new SqlConnection(_config.GetConnectionString(_config.ShufflerDbConnectionStringName)))
            {
                var dbModel = new VoteDbModel(vote);
                await connection.ExecuteAsync("uspVote_Insert", new { PollId = dbModel.PollId, EncryptedBallot = dbModel.EncryptedBallot, AdminSignature = dbModel.AdminSignature }, commandType: System.Data.CommandType.StoredProcedure);
            }
        }

        public async Task UpdateValidation(ValidationModel validation)
        {
            using (SqlConnection connection = new SqlConnection(_config.GetConnectionString(_config.ShufflerDbConnectionStringName)))
            {
                var dbModel = new ValidationDbModel(validation);
                await connection.ExecuteAsync("uspVote_UpdateValidation", new { PollId = dbModel.PollId, EncryptedBallot = dbModel.EncryptedBallot, EncryptionKey = dbModel.EncryptionKey }, commandType: System.Data.CommandType.StoredProcedure);
            }
        }

        public async Task UpdateIsSubmitted(int pollId)
        {
            using (SqlConnection connection = new SqlConnection(_config.GetConnectionString(_config.ShufflerDbConnectionStringName)))
            {
                await connection.ExecuteAsync("uspVote_UpdateIsSubmitted", new { PollId = pollId }, commandType: System.Data.CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<WaitingPollModel>> GetPollIdsAndSubmittedState()
        {
            using (SqlConnection connection = new SqlConnection(_config.GetConnectionString(_config.ShufflerDbConnectionStringName)))
            {
                return await connection.QueryAsync<WaitingPollModel>("uspVote_GetPollIdsAndSubmittedState", commandType: System.Data.CommandType.StoredProcedure);
            }
        }

        public async Task DeleteValidatedVotes(int pollId)
        {
            using (SqlConnection connection = new SqlConnection(_config.GetConnectionString(_config.ShufflerDbConnectionStringName)))
            {
                await connection.ExecuteAsync("uspVote_DeleteByPoll", new { PollId = pollId }, commandType: System.Data.CommandType.StoredProcedure);
            }
        }
    }
}
