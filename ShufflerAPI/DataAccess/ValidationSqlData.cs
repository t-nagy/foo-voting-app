using Dapper;
using Microsoft.Data.SqlClient;
using SharedLibrary.Models;
using ShufflerAPI.DataAccess.DbModels;

namespace ShufflerAPI.DataAccess
{
    internal class ValidationSqlData
    {
        private readonly ConfigHelper _config;

        public ValidationSqlData(ConfigHelper config)
        {
            _config = config;
        }

        public async Task GetValidationsByPoll(int pollId)
        {
            using (SqlConnection connection = new SqlConnection(_config.GetConnectionString(_config.ShufflerDbConnectionStringName)))
            {
                var result = await connection.QueryAsync<ValidationModel>("uspValidation_GetByPoll", new { PollId = pollId }, commandType: System.Data.CommandType.StoredProcedure);

            }
        }
    }
}
