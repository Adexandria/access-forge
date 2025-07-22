using AdeAuth.Models;
using AdeAuth.Services.Interfaces;
using AdeAuth.Services.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AdeAuth.Services.Authentication
{
    /// <summary>
    /// Manages user service
    /// </summary>
    /// <typeparam name="TDbContext">Context to manage operation</typeparam>
    /// <typeparam name="TModel">Application user</typeparam>
    /// <remarks>
    /// A constructor
    /// </remarks>
    /// <param name="dbContext">Context to manage operation</param>
    public class UserService<TDbContext, TModel>(TDbContext dbContext) : Repository<TDbContext, TModel>(dbContext), IUserService<TModel>
        where TDbContext : DbContext
        where TModel : ApplicationUser
    {

        /// create a method to fetch user with no tracking

        /// <summary>
        /// Create user asynchronously
        /// </summary>
        /// <param name="user">New user to create</param>
        /// <returns>Boolean value</returns>
        public async Task<AccessResult> CreateUserAsync(TModel user)
        {
            await _users.AddAsync(user);

            var response = await SaveChangesAsync();

            if (!response)
            {
                return AccessResult.Failed(new AccessError("Failed to create user", StatusCodes.Status400BadRequest));
            }

            return AccessResult.Success();
        }

        /// <summary>
        /// Creates a new user and saves the changes to the data store.
        /// </summary>
        /// <param name="user">The user model to be added. Cannot be null.</param>
        /// <returns>An <see cref="AccessResult"/> indicating the success or failure of the operation. Returns <see
        /// cref="AccessResult.Success"/> if the user is created successfully; otherwise, returns <see
        /// cref="AccessResult.Failed"/> with an error message and status code.</returns>
        public AccessResult CreateUser(TModel user)
        {
            _users.Add(user);

            var response = SaveChanges();

            if (!response)
            {
                return AccessResult.Failed(new AccessError("Failed to create user", StatusCodes.Status400BadRequest));
            }

            return AccessResult.Success();
        }

        /// <summary>
        /// Asynchronously updates the specified user in the database.
        /// </summary>
        /// <remarks>The method checks if the user entity is being tracked by the context. If not, it
        /// attempts to find the current user in the database and update its values. The operation will fail if the user
        /// does not exist in the database.</remarks>
        /// <param name="user">The user entity to update. The entity must have a valid identifier.</param>
        /// <returns>An <see cref="AccessResult"/> indicating the success or failure of the update operation. Returns <see
        /// cref="AccessResult.Success"/> if the update is successful; otherwise, returns <see
        /// cref="AccessResult.Failed"/> with an appropriate error message.</returns>
        public async Task<AccessResult> UpdateUserAsync(TModel user)
        {
            _users.Update(user);

            bool isTracked = Db.Entry(user).State != EntityState.Detached;
            if (isTracked)
            {
                _users.Update(user);
            }
            else
            {
                var currentUser = _users.FirstOrDefault(s => s.Id == user.Id);
                if (currentUser == null)
                {
                    return AccessResult.Failed(new AccessError("Failed to update user", StatusCodes.Status400BadRequest));
                }
                Db.Entry(currentUser).CurrentValues.SetValues(user);
            }

            var response = await SaveChangesAsync();
            if (!response)
            {
                return AccessResult.Failed(new AccessError("Failed to update user", StatusCodes.Status400BadRequest));
            }

            return AccessResult.Success();
        }

        /// <summary>
        /// Updates the specified user in the database.
        /// </summary>
        /// <remarks>The method checks if the user entity is being tracked by the context. If not, it
        /// attempts to find the current user in the database and update its values. The operation will fail if the user
        /// does not exist in the database.</remarks>
        /// <param name="user">The user entity to update. The entity must have a valid identifier.</param>
        /// <returns>An <see cref="AccessResult"/> indicating the success or failure of the update operation. Returns <see
        /// cref="AccessResult.Success"/> if the update is successful; otherwise, returns <see
        /// cref="AccessResult.Failed"/> with an appropriate error message.</returns>
        public AccessResult UpdateUser(TModel user)
        {
            bool isTracked = Db.Entry(user).State != EntityState.Detached;
            if (isTracked)
            {
                _users.Update(user);
            }
            else
            {
                var currentUser = _users.FirstOrDefault(s => s.Id == user.Id);
                if (currentUser == null)
                {
                    return AccessResult.Failed(new AccessError("Failed to update user", StatusCodes.Status400BadRequest));
                }
                Db.Entry(currentUser).CurrentValues.SetValues(user);
            }

            var response = SaveChanges();

            if (!response)
            {
                return AccessResult.Failed(new AccessError("Failed to update user", StatusCodes.Status400BadRequest));
            }

            return AccessResult.Success();
        }

        /// <summary>
        /// Asynchronously deletes the specified user from the data store.
        /// </summary>
        /// <param name="user">The user to be deleted. Cannot be null.</param>
        /// <returns>An <see cref="AccessResult"/> indicating the success or failure of the operation. Returns <see
        /// cref="AccessResult.Success"/> if the user is successfully deleted; otherwise, returns <see
        /// cref="AccessResult.Failed"/> with an appropriate error message.</returns>
        public async Task<AccessResult> DeleteUserAsync(TModel user)
        {
            _users.Remove(user);
            var response = await SaveChangesAsync();
            if (!response)
            {
                return AccessResult.Failed(new AccessError("Failed to delete user", StatusCodes.Status400BadRequest));
            }

            return AccessResult.Success();
        }

        /// <summary>
        /// Deletes the specified user from the system.
        /// </summary>
        /// <param name="user">The user to be deleted. Cannot be null.</param>
        /// <returns>An <see cref="AccessResult"/> indicating the success or failure of the operation. Returns <see
        /// cref="AccessResult.Success"/> if the user is successfully deleted; otherwise, returns <see
        /// cref="AccessResult.Failed"/> with an error message.</returns>
        public AccessResult DeleteUser(TModel user)
        {
            _users.Remove(user);
            var response = SaveChanges();
            if (!response)
            {
                return AccessResult.Failed(new AccessError("Failed to delete user", StatusCodes.Status400BadRequest));
            }

            return AccessResult.Success();
        }

        /// <summary>
        /// Retrieves a user by their email address asynchronously.
        /// </summary>
        /// <param name="email">The user's email</param>
        /// <returns>An <see cref="AccessResult{TModel}"/> containing the user if found; otherwise, an error result indicating
        /// the user was not found.</returns>
        public async Task<AccessResult<TModel>> FetchUserByEmailAsync(string email)
        {
            var response = await _users.Where(s => s.Email == email).FirstOrDefaultAsync();

            if (response == null)
            {
                return AccessResult<TModel>.Failed(new AccessError("Invalid user", StatusCodes.Status404NotFound));
            }

            return AccessResult<TModel>.Success(response);
        }

        /// <summary>
        /// Retrieves a user by their email address.
        /// </summary>
        /// <param name="email">The email address of the user to retrieve. Cannot be null or empty.</param>
        /// <returns>An <see cref="AccessResult{TModel}"/> containing the user if found; otherwise, an error result indicating
        /// the user was not found.</returns>
        public AccessResult<TModel> FetchUserByEmail(string email)
        {
            var response = _users.Where(s => s.Email == email).FirstOrDefault();

            if (response == null)
            {
                return AccessResult<TModel>.Failed(new AccessError("Invalid user", StatusCodes.Status404NotFound));
            }

            return AccessResult<TModel>.Success(response);
        }

        /// <summary>
        /// Asynchronously retrieves a user by their unique identifier.
        /// </summary>
        /// <param name="userId">The unique identifier of the user to retrieve.</param>
        /// <returns>An <see cref="AccessResult{TModel}"/> containing the user data if found;  otherwise, an error result
        /// indicating the user was not found.</returns>
        public async Task<AccessResult<TModel>> FetchUserByIdAsync(Guid userId)
        {
            var response = await _users.Where(s => s.Id == userId).FirstOrDefaultAsync();

            if (response == null)
            {
                return AccessResult<TModel>.Failed(new AccessError("Invalid user", StatusCodes.Status404NotFound));
            }

            return AccessResult<TModel>.Success(response);
        }

        /// <summary>
        /// Retrieves a user by their unique identifier.
        /// </summary>
        /// <param name="userId">The unique identifier of the user to retrieve.</param>
        /// <returns>An <see cref="AccessResult{TModel}"/> containing the user data if found;  otherwise, an error result
        /// indicating the user was not found.</returns>
        public AccessResult<TModel> FetchUserById(Guid userId)
        {
            var response = _users.Where(s => s.Id == userId).FirstOrDefault();

            if (response == null)
            {
                return AccessResult<TModel>.Failed(new AccessError("Invalid user", StatusCodes.Status404NotFound));
            }

            return AccessResult<TModel>.Success(response);
        }

        /// <summary>
        /// Retrieves a user by their username asynchronously.
        ///</summary>
        ///<param name="username">The username of the user to retrieve.</param>
        /// <returns>An <see cref="AccessResult{TModel}"/> containing the user data if found;  otherwise, an error result
        /// indicating the user was not found.</returns>
        public async Task<AccessResult<TModel>> FetchUserByUsernameAsync(string username)
        {
            var response = await _users.Where(s => s.UserName == username).FirstOrDefaultAsync();

            if (response == null)
            {
                return AccessResult<TModel>.Failed(new AccessError("Invalid user", StatusCodes.Status404NotFound));
            }

            return AccessResult<TModel>.Success(response);
        }

        /// <summary>
        /// Retrieves a user by their username asynchronously.
        ///</summary>
        ///<param name="username">The username of the user to retrieve.</param>
        /// <returns>An <see cref="AccessResult{TModel}"/> containing the user data if found;  otherwise, an error result
        /// indicating the user was not found.</returns>
        public AccessResult<TModel> FetchUserByUsername(string username)
        {
            var response = _users.Where(s => s.UserName == username).FirstOrDefault();

            if (response == null)
            {
                return AccessResult<TModel>.Failed(new AccessError("Invalid user", StatusCodes.Status404NotFound));
            }

            return AccessResult<TModel>.Success(response);
        }

        private readonly DbSet<TModel> _users = dbContext.Set<TModel>();
    }
}
