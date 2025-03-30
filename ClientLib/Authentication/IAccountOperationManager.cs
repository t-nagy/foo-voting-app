
using System.Globalization;

namespace ClientLib.Authentication
{
    public interface IAccountOperationManager
    {
        Task<string?> Register(string email, string password);
        Task<bool> ResendEmailConfirmation(string email);
        Task<bool> ForgotPassword(string email);
        Task<string?> ResetPassword(string email, string resetCode, string newPassword);
    }
}