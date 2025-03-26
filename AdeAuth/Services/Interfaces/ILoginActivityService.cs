using AdeAuth.Models;
using AdeAuth.Services.Utility;

namespace AdeAuth.Services.Interfaces
{
    public interface ILoginActivityService
    {
        Task<AccessResult> CreateLoginActivityAsync(LoginActivity loginActivity);

        AccessResult CreateLoginActivity(LoginActivity loginActivity);

        Task<AccessResult> UpdateLoginActivityAsync(LoginActivity loginActivity);

        AccessResult UpdateLoginActivity(LoginActivity loginActivity);

        Task<AccessResult> DeleteLoginActivityAsync(LoginActivity loginActivity);

        AccessResult DeleteLoginActivity(LoginActivity loginActivity);

        Task<AccessResult<LoginActivity>> FetchLoginActivityAsync(Guid userId, string ipAddress);

        AccessResult<LoginActivity> FetchLoginActivity(Guid userId, string ipAddress);

        AccessResult<IEnumerable<LoginActivity>> FetchLoginActivity(Guid userId);
    }
}
