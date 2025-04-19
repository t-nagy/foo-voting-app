using AdminAPI.DataAccess.DataModels;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace AdminAPI.DataAccess
{
    public class KeyData
    {
        private readonly ConfigHelper _config;

        public KeyData()
        {
            _config = new ConfigHelper();
        }

        public async Task<IEnumerable<string>?> GetKeyByUser(string userId)
        {
            using (SqlConnection connection = new SqlConnection(_config.GetConnectionString(_config.IdentityDbConnectionStringName)))
            {
                var result = await connection.QueryAsync<SignKeyDbModel>("uspVerification_GetByUser", new { UserId = userId }, commandType: CommandType.StoredProcedure);
                if (result == null)
                {
                    return null;
                }

                List<string> keys = new List<string>();
                foreach (var key in result)
                {
                    keys.Add(key.VerifyKey!);
                }

                return keys;
            }
        }

        public async Task SaveKey(string userId, string key)
        {
            using (SqlConnection connection = new SqlConnection(_config.GetConnectionString(_config.IdentityDbConnectionStringName)))
            {
                var p = new DynamicParameters();
                var result = await connection.ExecuteAsync("uspVerification_Insert", new { UserId = userId, Key = key }, commandType: CommandType.StoredProcedure);

            }
        }
    }
}
