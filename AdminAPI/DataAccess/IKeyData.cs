
namespace AdminAPI.DataAccess
{
    public interface IKeyData
    {
        Task<IEnumerable<string>?> GetKeyByUser(string userId);
        Task SaveKey(string userId, string key);
    }
}