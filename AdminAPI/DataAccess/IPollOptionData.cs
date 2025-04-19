using SharedLibrary.Models;

namespace AdminAPI.DataAccess
{
    internal interface IPollOptionData
    {
        Task<List<OptionModel>> LoadOptionsByPoll(int pollId);
        Task<OptionModel> SaveOptionInPollCreation(OptionModel option, SqlDataAccess sql);
    }
}