using Microsoft.Data.SqlClient;
using static Org.BouncyCastle.Math.EC.ECCurve;
using System.Data;
using Dapper;
using SharedLibrary;

namespace CounterAPI.DataAccess
{
    public class PollSqlData : IPollData
    {
        private readonly ConfigHelper _config;

        public PollSqlData(ConfigHelper config)
        {
            _config = config;
        }

        public async Task<DateTime?> GetVoteEndDate(int pollId)
        {
            using (SqlConnection connection = new SqlConnection(_config.GetConnectionString(_config.VoteDataConnectionStringName)))
            {
                return (await connection.QueryAsync<DateTime>("uspPoll_GetVotingEndByPoll", new { PollId = pollId }, commandType: CommandType.StoredProcedure)).FirstOrDefault();
            }
        }

        public async Task<DateTime?> GetValidationEndDate(int pollId)
        {
            using (SqlConnection connection = new SqlConnection(_config.GetConnectionString(_config.VoteDataConnectionStringName)))
            {
                return (await connection.QueryAsync<DateTime>("uspPoll_GetValidationEndByPoll", new { PollId = pollId }, commandType: CommandType.StoredProcedure)).FirstOrDefault();
            }
        }

        public async Task UpdatePollStatus(int pollId, PollStatus newStatus)
        {
            using (SqlConnection connection = new SqlConnection(_config.GetConnectionString(_config.VoteDataConnectionStringName)))
            {
                await connection.ExecuteAsync("uspPoll_UpdateStatus", new { PollId = pollId, NewStatus = (int)newStatus }, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<PollStatus> GetPollStatus(int pollId)
        {
            using (SqlConnection connection = new SqlConnection(_config.GetConnectionString(_config.VoteDataConnectionStringName)))
            {
                var result = await connection.QueryAsync<int>("uspPoll_GetStatusByPoll", new { PollId = pollId }, commandType: System.Data.CommandType.StoredProcedure);
                return (PollStatus)result.First();
            }
        }
    }
}
