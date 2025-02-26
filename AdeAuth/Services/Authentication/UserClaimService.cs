using AdeAuth.Models;
using AdeAuth.Services.Interfaces;
using AdeAuth.Services.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;


namespace AdeAuth.Services.Authentication
{
    public class UserClaimService<TDbContext> : Repository<TDbContext,UserClaim>,IUserClaimService
        where TDbContext: DbContext
    {
        public UserClaimService(TDbContext dbContext):base(dbContext)
        {
            _userClaims = Db.Set<UserClaim>();
        }
        public AccessResult CreateUserClaim(UserClaim claim)
        {
            _userClaims.Add(claim);

            var response = SaveChanges();

            if (response)
            {
                return AccessResult.Success();
            }

            return AccessResult.Failed(new AccessError("Failed to create user", StatusCodes.Status400BadRequest));
        }

        public AccessResult DeleteUserClaim(UserClaim claim)
        {
            _userClaims.Remove(claim);
            var response = SaveChanges();

            if (response)
            {
                return AccessResult.Success();
            }

            return AccessResult.Failed(new AccessError("Failed to delete user", StatusCodes.Status400BadRequest));
        }

        public AccessResult<List<UserClaim>> FetchUserClaims(Guid userId)
        {
            var claims = _userClaims.Where(s => s.UserId == userId).ToList();

            return AccessResult<List<UserClaim>>.Success(claims);
        }

        public AccessResult<UserClaim> GetUserClaim(string claimType, Guid userId)
        {
            var claim = _userClaims.Where(s => s.ClaimType == claimType && s.UserId == userId).FirstOrDefault();

            if(claim == null)
            {
                return AccessResult<UserClaim>.Failed(new AccessError("User claim not found", StatusCodes.Status404NotFound));
            }

            return AccessResult<UserClaim>.Success(claim);
        }

        public AccessResult UpdateUserClaim(UserClaim claim)
        {
            _userClaims.Update(claim);

            var response = SaveChanges();

            if (response)
            {
                return AccessResult.Success();
            }

            return AccessResult.Failed(new AccessError("Failed to update user", StatusCodes.Status400BadRequest));
        }

        public async Task<AccessResult> CreateUserClaimAsync(UserClaim claim)
        {
            _userClaims.Add(claim);

            var response = await SaveChangesAsync();

            if (response)
            {
                return AccessResult.Success();
            }

            return AccessResult.Failed(new AccessError("Failed to create user", StatusCodes.Status400BadRequest));
        }

        public async Task<AccessResult> UpdateUserClaimAsync(UserClaim claim)
        {
            _userClaims.Update(claim);

            var response = await SaveChangesAsync();

            if (response)
            {
                return AccessResult.Success();
            }

            return AccessResult.Failed(new AccessError("Failed to update user", StatusCodes.Status400BadRequest));
        }

        public async Task<AccessResult> DeleteUserClaimAsync(UserClaim claim)
        {
            _userClaims.Remove(claim);
            var response = await SaveChangesAsync();

            if (response)
            {
                return AccessResult.Success();
            }

            return AccessResult.Failed(new AccessError("Failed to delete user", StatusCodes.Status400BadRequest));
        }

        public async Task<AccessResult<UserClaim>> GetUserClaimAsync(string claimType, Guid userId)
        {
            var claim = await _userClaims.Where(s => s.ClaimType == claimType && s.UserId == userId).FirstOrDefaultAsync();

            if (claim == null)
            {
                return AccessResult<UserClaim>.Failed(new AccessError("User claim not found", StatusCodes.Status404NotFound));
            }

            return AccessResult<UserClaim>.Success(claim);
        }

        public async Task<AccessResult<List<UserClaim>>> FetchUserClaimsAsync(Guid userId)
        {
            var claims = await _userClaims.Where(s => s.UserId == userId).ToListAsync();

            return AccessResult<List<UserClaim>>.Success(claims);
        }

        public AccessResult CreateUserClaims(List<UserClaim> claims)
        {
            _userClaims.AddRange(claims); 
            var response = SaveChanges();

            if (response)
            {
                return AccessResult.Success();
            }

            return AccessResult.Failed(new AccessError("Failed to create user", StatusCodes.Status400BadRequest));
        }

        public async Task<AccessResult> CreateUserClaimsAsync(List<UserClaim> claims)
        {
            await _userClaims.AddRangeAsync(claims);

            var response = await SaveChangesAsync();

            if (response)
            {
                return AccessResult.Success();
            }

            return AccessResult.Failed(new AccessError("Failed to create user", StatusCodes.Status400BadRequest));
        }

        private readonly DbSet<UserClaim> _userClaims;
    }
}
