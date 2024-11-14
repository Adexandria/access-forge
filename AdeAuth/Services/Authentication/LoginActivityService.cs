using AdeAuth.Models;
using AdeAuth.Services.Interfaces;
using AdeAuth.Services.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AdeAuth.Services.Authentication
{
    internal class LoginActivityService<TDbContext> : Repository<TDbContext,LoginActivity>,ILoginActivityService
        where TDbContext : DbContext
    {
        public LoginActivityService(TDbContext dbContext): base(dbContext)
        {
            _activities = dbContext.Set<LoginActivity>();
        }

        // add as no tracking for all services
        public AccessResult CreateLoginActivity(LoginActivity loginActivity)
        {
            _activities.Add(loginActivity);

            var response = SaveChanges();

            if (response)
            {
                return AccessResult.Success();
            }

            return AccessResult.Failed(new AccessError("Failed to create login activity", StatusCodes.Status400BadRequest));
        }

        public async Task<AccessResult> CreateLoginActivityAsync(LoginActivity loginActivity)
        {
            await _activities.AddAsync(loginActivity);

            var response = await SaveChangesAsync();

            if (response)
            {
                return AccessResult.Success();
            }

            return AccessResult.Failed(new AccessError("Failed to create login activity", StatusCodes.Status400BadRequest));
        }

        public AccessResult DeleteLoginActivity(LoginActivity loginActivity)
        {
            _activities.Remove(loginActivity);

            var response = SaveChanges();

            if (response)
            {
                return AccessResult.Success();
            }

            return AccessResult.Failed(new AccessError("Failed to delete login activity", StatusCodes.Status400BadRequest));
        }

        public async Task<AccessResult> DeleteLoginActivityAsync(LoginActivity loginActivity)
        {
            _activities.Remove(loginActivity);

            var response = await SaveChangesAsync();

            if (response)
            {
                return AccessResult.Success();
            }

            return AccessResult.Failed(new AccessError("Failed to delete login activity", StatusCodes.Status400BadRequest));
        }

        public AccessResult<LoginActivity> FetchLoginActivity(Guid userId, string ipAddress)
        {
            var loginActivity = _activities.Where(s => s.IpAddress == ipAddress && s.UserId == userId).FirstOrDefault();

            if(loginActivity == null)
            {
                return AccessResult<LoginActivity>.Failed(new AccessError("Invalid ip address for a user", StatusCodes.Status400BadRequest));
            }
            return AccessResult<LoginActivity>.Success(loginActivity);
        }

        public AccessResult<IEnumerable<LoginActivity>> FetchLoginActivity(Guid userId)
        {
            var loginActivities = _activities.Where(s => s.UserId == userId).AsNoTracking();

            return AccessResult<IEnumerable<LoginActivity>>.Success(loginActivities);
        }

        public async Task<AccessResult<LoginActivity>> FetchLoginActivityAsync(Guid userId, string ipAddress)
        {
            var loginActivity = await _activities.Where(s => s.IpAddress == ipAddress && s.UserId == userId).FirstOrDefaultAsync();

            if (loginActivity == null)
            {
                return AccessResult<LoginActivity>.Failed(new AccessError("Invalid ip address for a user", StatusCodes.Status400BadRequest));
            }

            return AccessResult<LoginActivity>.Success(loginActivity);
        }

        public AccessResult UpdateLoginActivity(LoginActivity loginActivity)
        {
            _activities.Update(loginActivity);

            var response = SaveChanges();

            if (response)
            {
                return AccessResult.Success();
            }

            return AccessResult.Failed(new AccessError("Failed to update login activity", StatusCodes.Status400BadRequest));
        }

        public async Task<AccessResult> UpdateLoginActivityAsync(LoginActivity loginActivity)
        {
            _activities.Update(loginActivity);

            var response = await SaveChangesAsync();

            if (response)
            {
                return AccessResult.Success();
            }

            return AccessResult.Failed(new AccessError("Failed to update login activity", StatusCodes.Status400BadRequest));
        }


        private readonly DbSet<LoginActivity> _activities;
    }
}
