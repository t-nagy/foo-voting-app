using ClientLib.Authentication;
using SharedLibrary.Models;

namespace ClientLib.DataManagers
{
    public interface IVoteAdministrationManager : ILoginRequester, ILoggedInUserManager
    {
        Task<bool> TryRegisterKey(string key);
        Task<string?> GetAdminVerificationKey();
        Task<SignedBallotModel?> GetAdminSignature(SignedBallotModel bcb);
    }
}