using Dapper;
using Microsoft.Data.SqlClient;
using SharedLibrary.Models;
using System.Data;

namespace AdminAPI.DataAccess
{
    internal class PollOptionData
    {
        private readonly ConfigHelper _config;

        public PollOptionData()
        {
            _config = new ConfigHelper();
        }

        public async Task<List<OptionModel>> LoadOptionsByPoll(int pollId)
        {
            using (SqlConnection connection = new SqlConnection(_config.GetConnectionString(_config.VoteDbConnectionStringName)))
            {
                return (await connection.QueryAsync<OptionModel>("uspPollOption_GetByPollId", new { PollId = pollId }, commandType: CommandType.StoredProcedure)).ToList();
            }
        }

        public async Task<OptionModel> SaveOptionInPollCreation(OptionModel option, SqlDataAccess sql)
        {
            var p = new DynamicParameters();
            p.Add("@OptionText", option.OptionText);
            p.Add("@PollId", option.PollId);
            p.Add("Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await sql.Connection!.ExecuteAsync("uspPollOption_Insert", p, commandType: CommandType.StoredProcedure, transaction: sql.Transaction);
            option.Id = p.Get<int>("Id");
            return option;
        }
    }
}
