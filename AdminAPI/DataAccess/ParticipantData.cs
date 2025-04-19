using AdminAPI.DataAccess.DataModels;
using Dapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using SharedLibrary;
using SharedLibrary.Models;
using System.Data;

namespace AdminAPI.DataAccess
{
    internal class ParticipantData
    {
        private readonly ConfigHelper _config;

        public ParticipantData()
        {
            _config = new ConfigHelper();
        }

        public async Task<ParticipantModel?> GetParticipantByIdAndPoll(string participantId, int pollId)
        {
            using (SqlConnection connection = new SqlConnection(_config.GetConnectionString(_config.VoteDbConnectionStringName)))
            {
                var p = new DynamicParameters();
                p.Add("UserId", participantId);
                p.Add("PollId", pollId);
                var result = await connection.QueryAsync<ParticipantDbModel>("uspParticipant_GetByPollAndUserId", p, commandType: CommandType.StoredProcedure);
                return result.FirstOrDefault()?.ToParticipantModel();
            }
        }

        public async Task<List<ParticipantModel>> GetParticipantsByPoll(int pollId)
        {
            using (SqlConnection connection = new SqlConnection(_config.GetConnectionString(_config.VoteDbConnectionStringName)))
            {
                var result = await connection.QueryAsync<ParticipantDbModel>("uspParticipant_GetByPoll", new { PollId = pollId }, commandType: CommandType.StoredProcedure);
                List<ParticipantModel> output = new List<ParticipantModel>();
                foreach (var p in result)
                {
                    output.Add(p.ToParticipantModel());
                }

                return output;
            }
        }

        public async Task SaveParticipant(ParticipantModel participant, SqlDataAccess? sql = null)
        {
            var p = new DynamicParameters();
            p.Add("UserId", participant.Username);
            p.Add("PollId", participant.PollId);
            p.Add("Role", (int)participant.Role);

            if (sql != null)
            {
                await sql.Connection!.ExecuteAsync("uspParticipant_Insert", p, commandType: CommandType.StoredProcedure, transaction: sql.Transaction);
                return;
            }

            using (SqlConnection connection = new SqlConnection(_config.GetConnectionString(_config.VoteDbConnectionStringName)))
            {
                await connection.ExecuteAsync("uspParticipant_Insert", p, commandType: CommandType.StoredProcedure);
                return;
            }
        }

        public async Task ParticipantSetVoted(string participantId, int pollid)
        {
            using (SqlConnection connection = new SqlConnection(_config.GetConnectionString(_config.VoteDbConnectionStringName)))
            {
                await connection.ExecuteAsync("uspParticipant_UpdateVoted", new { UserId = participantId, PollId = pollid }, commandType: CommandType.StoredProcedure);
            }
        }
    }
}
