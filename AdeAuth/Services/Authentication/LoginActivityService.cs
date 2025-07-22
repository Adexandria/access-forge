using AdeAuth.Models;
using AdeAuth.Services.Interfaces;
using AdeAuth.Services.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AdeAuth.Services.Authentication
{
    /// <summary>
    /// Provides services for managing login activities within a specified database context.
    /// </summary>
    /// <remarks>This service allows for creating, updating, deleting, and fetching login activity records. It
    /// operates on a <see cref="DbSet{TEntity}"/> of <see cref="LoginActivity"/> entities within the provided database
    /// context.</remarks>
    /// <typeparam name="TDbContext">The type of the database context used by this service. Must be a subclass of <see cref="DbContext"/>.</typeparam>
    /// <param name="dbContext"></param>
    internal class LoginActivityService<TDbContext>(TDbContext dbContext) : Repository<TDbContext,LoginActivity>(dbContext),ILoginActivityService
        where TDbContext : DbContext
    {

        /// <summary>
        /// Creates login activity for a user.
        /// </summary>
        /// <param name="loginActivity">Manages the login activity of the user</param>
        /// <returns>An <see cref="AccessResult"/> </returns>
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

        /// <summary>
        /// Creates login activity for a user asynchronously.
        /// </summary>
        /// <param name="loginActivity">Manages the login actiuvity of the user</param>
        /// <returns>An <see cref="AccessResult"/> </returns>
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

        /// <summary>
        /// Deletes a login activity for a user.
        /// </summary>
        /// <param name="loginActivity">Manages the login activity of the user</param>
        /// <returns>An <see cref="AccessResult"/> </returns> 
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


        /// <summary>
        /// Delete a login activity for a user asynchronously.
        /// </summary>
        /// <param name="loginActivity">Manages the login activity of the user</param>
        /// <returns>An <see cref="AccessResult"/> </returns> 
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

        /// <summary>
        /// fetches the login activity for a user based on the user id and ip address.
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="ipAddress">Ip address of the user</param>
        /// <returns>An <see cref="AccessResult{T}"/> containing an instance of <see cref="LoginActivity"/> object
        /// associated with the specified user.</returns>
        public AccessResult<LoginActivity> FetchLoginActivity(Guid userId, string ipAddress)
        {
            var loginActivity = _activities.Where(s => s.IpAddress == ipAddress && s.UserId == userId).FirstOrDefault();

            if(loginActivity == null)
            {
                return AccessResult<LoginActivity>.Failed(new AccessError("Invalid ip address for a user", StatusCodes.Status400BadRequest));
            }
            return AccessResult<LoginActivity>.Success(loginActivity);
        }

        /// <summary>
        /// Retrieves the login activities for a specified user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose login activities are to be fetched.</param>
        /// <returns>An <see cref="AccessResult{T}"/> containing an enumerable collection of <see cref="LoginActivity"/> objects
        /// associated with the specified user.</returns>
        public AccessResult<IEnumerable<LoginActivity>> FetchLoginActivity(Guid userId)
        {
            var loginActivities = _activities.Where(s => s.UserId == userId).AsNoTracking();

            return AccessResult<IEnumerable<LoginActivity>>.Success(loginActivities);
        }

        /// <summary>
        /// Retrieves the login activities for a specified user asynchronously.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose login activities are to be fetched.</param>
        /// <returns>An <see cref="AccessResult{T}"/> containing an enumerable collection of <see cref="LoginActivity"/> objects
        /// associated with the specified user.</returns>
        public async Task<AccessResult<LoginActivity>> FetchLoginActivityAsync(Guid userId, string ipAddress)
        {
            var loginActivity = await _activities.Where(s => s.IpAddress == ipAddress && s.UserId == userId).FirstOrDefaultAsync();

            if (loginActivity == null)
            {
                return AccessResult<LoginActivity>.Failed(new AccessError("Invalid ip address for a user", StatusCodes.Status400BadRequest));
            }

            return AccessResult<LoginActivity>.Success(loginActivity);
        }

        /// <summary>
        /// Updates the login activity for a user.
        /// </summary>
        /// <param name="loginActivity">Manages logfin activity of a user</param>
        /// <returns>An <see cref="AccessResult"/></returns>
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

        /// <summary>
        /// Updates the login activity for a user asynchronously.
        /// </summary>
        /// <param name="loginActivity">Manages logfin activity of a user</param>
        /// <returns>An <see cref="AccessResult"/></returns>
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


        private readonly DbSet<LoginActivity> _activities = dbContext.Set<LoginActivity>();
    }
}
