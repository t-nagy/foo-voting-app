
namespace ClientLib.Authentication
{
    public interface ISessionManager
    {
        event EventHandler<EventArgs>? LoginRequiredEvent;

        public bool IsLoggedIn { get; }
        public string? LoggedInEmail { get; }
        Task<string?> GetAuthenticationToken();
        Task<LoginResponse> StartNewSession(string email, string password);
    }
}