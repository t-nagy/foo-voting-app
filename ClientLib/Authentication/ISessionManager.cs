
namespace ClientLib.Authentication
{
    public interface ISessionManager
    {
        event EventHandler<EventArgs>? LoginRequiredEvent;

        public bool IsLoggedIn { get; }

        Task<string?> GetAuthenticationToken();
        Task<bool> StartNewSession(string email, string password);
    }
}