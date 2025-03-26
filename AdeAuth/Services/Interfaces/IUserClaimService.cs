using AdeAuth.Models;
using AdeAuth.Services.Utility;

namespace AdeAuth.Services.Interfaces
{
    public interface IUserClaimService
    {
        AccessResult CreateUserClaim(UserClaim claim);
        AccessResult CreateUserClaims(List<UserClaim> claims);
        AccessResult UpdateUserClaim(UserClaim claim);
        AccessResult DeleteUserClaim(UserClaim claim);
        AccessResult<UserClaim> GetUserClaim(string claimType, Guid userId);
        AccessResult<List<UserClaim>> FetchUserClaims(Guid userId);
       Task<AccessResult> CreateUserClaimsAsync(List<UserClaim> claims);
       Task<AccessResult> CreateUserClaimAsync(UserClaim claim);
        Task<AccessResult> UpdateUserClaimAsync(UserClaim claim);
        Task<AccessResult> DeleteUserClaimAsync(UserClaim claim);
        Task<AccessResult<UserClaim>> GetUserClaimAsync(string claimType, Guid userId);
        Task<AccessResult<List<UserClaim>>> FetchUserClaimsAsync(Guid userId);
    }
}
