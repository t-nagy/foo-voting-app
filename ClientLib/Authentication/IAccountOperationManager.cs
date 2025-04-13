
using System.Globalization;

namespace ClientLib.Authentication
{
    public interface IAccountOperationManager : ILoginRequester, ILoggedInUserManager
    {
        Task<LoginResponse> Login(string email, string password);
        void Logout();
        Task<string?> Register(string email, string password);
        Task<bool> ResendEmailConfirmation(string email);
        Task<bool> ForgotPassword(string email);
        Task<string?> ResetPassword(string email, string resetCode, string newPassword);
        Task<string?> ChangePassword(string oldPassword, string newPassword);
    }
}