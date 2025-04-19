using Dapper;
using Microsoft.Data.SqlClient;
using SharedLibrary.Models;
using ShufflerAPI.DataAccess.DbModels;

namespace ShufflerAPI.DataAccess
{
    public class VoteSqlData : IVoteData
    {
        private readonly ConfigHelper _config;

        public VoteSqlData()
        {
            _config = new ConfigHelper();
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
                return await connection.QueryAsync<ValidationModel>("uspVote_GetValidationsByPoll", new { PollId = pollId }, commandType: System.Data.CommandType.StoredProcedure);

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
                await connection.ExecuteAsync("uspVote_UpdateValidation", new { PollId = validation.PollId, EncryptedBallot = validation.EncryptedBallot, EncryptionKey = validation.EncryptionKey }, commandType: System.Data.CommandType.StoredProcedure);
            }
        }
    }
}
