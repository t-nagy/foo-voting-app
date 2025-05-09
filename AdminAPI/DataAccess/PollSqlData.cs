﻿using AdminAPI.DataAccess.DataModels;
using Dapper;
using Microsoft.Data.SqlClient;
using SharedLibrary;
using SharedLibrary.Models;
using System.Data;
using System.Threading.Tasks;

namespace AdminAPI.DataAccess
{
    internal class PollSqlData : IPollData
    {
        private readonly ConfigHelper _config;
        private readonly IPollOptionData _optionData;
        private readonly IParticipantData _participantData;

        public PollSqlData(ConfigHelper config, IPollOptionData optionData, IParticipantData participantData)
        {
            _config = config;
            _optionData = optionData;
            _participantData = participantData;
        }

        public async Task<PollModel?> LoadPoll(int pollId)
        {
            using (SqlConnection connection = new SqlConnection(_config.GetConnectionString(_config.VoteDbConnectionStringName)))
            {
                PollDbModel? poll = (await connection.QueryAsync<PollDbModel>("uspPoll_GetById", new { Id = pollId }, commandType: CommandType.StoredProcedure)).FirstOrDefault();

                if (poll == null)
                {
                    return null;
                }

                poll.PollOptions = await _optionData.LoadOptionsByPoll(pollId);

                return poll.ToPollModel();
            }
        }

        public async Task<PollModel?> GetPollByJoinCode(string code)
        {
            using (SqlConnection connection = new SqlConnection(_config.GetConnectionString(_config.VoteDbConnectionStringName)))
            {
                PollDbModel? poll = (await connection.QueryAsync<PollDbModel>("uspPoll_GetByJoinCode", new { JoinCode = code }, commandType: CommandType.StoredProcedure)).FirstOrDefault();

                if (poll == null)
                {
                    return null;
                }

                poll.PollOptions = await _optionData.LoadOptionsByPoll(poll.Id);

                return poll.ToPollModel();
            }
        }

        public async Task<IEnumerable<PollModel>> LoadPollsAllMinimal(string userId)
        {
            using (SqlConnection connection = new SqlConnection(_config.GetConnectionString(_config.VoteDbConnectionStringName)))
            {
                var result = await connection.QueryAsync<PollDbModel>("uspPoll_GetMinimal", new { UserId = userId }, commandType: CommandType.StoredProcedure);
                List<PollModel> polls = new List<PollModel>();
                foreach (var r in result)
                {
                    polls.Add(r.ToPollModel());
                }

                return polls;
            }
        }


        public async Task<PollModel> SavePoll(PollModel poll)
        {
            using (SqlDataAccess sql = new SqlDataAccess(_config))
            {
                try
                {
                    sql.StartTransaction(_config.VoteDbConnectionStringName);

                    poll = await SavePollInTransaction(poll, sql);

                    poll.Participants!.ForEach(x => x.PollId = poll.Id);
                    foreach (var participant in poll.Participants)
                    {
                        await _participantData.SaveParticipant(participant, sql);
                    }

                    foreach (var opt in poll.PollOptions!)
                    {
                        opt.PollId = poll.Id;
                        opt.Id = (await _optionData.SaveOptionInPollCreation(opt, sql)).Id;
                    }

                    sql.CommitTransaction();
                }
                catch (Exception)
                {
                    sql.RollbackTransaction();
                }
            }

            return poll;
        }

        public async Task<DateTime?> GetVoteEndDate(int pollId)
        {
            using (SqlConnection connection = new SqlConnection(_config.GetConnectionString(_config.VoteDbConnectionStringName)))
            {
                return (await connection.QueryAsync<DateTime>("uspPoll_GetVotingEndByPoll", new { PollId = pollId }, commandType: CommandType.StoredProcedure)).FirstOrDefault();
            }
        }

        public async Task<DateTime?> GetValidationEndDate(int pollId)
        {
            using (SqlConnection connection = new SqlConnection(_config.GetConnectionString(_config.VoteDbConnectionStringName)))
            {
                return (await connection.QueryAsync<DateTime>("uspPoll_GetValidationEndByPoll", new { PollId = pollId }, commandType: CommandType.StoredProcedure)).FirstOrDefault();
            }
        }

        private async Task<PollModel> SavePollInTransaction(PollModel poll, SqlDataAccess sql)
        {
            var p = new DynamicParameters();
            p.Add("OwnerName", poll.OwnerName);
            p.Add("Title", poll.Title);
            p.Add("Description", poll.Description);
            p.Add("VoteCollectionEndDate", poll.VoteCollectionEndDate);
            p.Add("VoteValidationEndDate", poll.VoteValidationEndDate);
            if (!poll.IsPublic)
            {
                p.Add("IsPublic", poll.IsPublic);
                p.Add("JoinCode", poll.JoinCode);
            }
            p.Add("Id", dbType: DbType.Int32, direction: ParameterDirection.Output);
            p.Add("CreatedDate", dbType: DbType.DateTime2, direction: ParameterDirection.Output);

            await sql.Connection!.ExecuteAsync("uspPoll_Insert", p, commandType: CommandType.StoredProcedure, transaction: sql.Transaction);

            poll.Id = p.Get<int>("Id");
            poll.CreatedDate = p.Get<DateTime>("CreatedDate");

            return poll;
        }
    }
}
