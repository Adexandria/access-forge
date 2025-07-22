using AdeAuth.Models;
using AdeAuth.Services.Utility;


namespace AdeAuth.Services.Interfaces
{
    /// <summary>
    /// Manages user service
    /// </summary>
    /// <typeparam name="TModel">Application user</typeparam>
    public interface IUserService<TModel> where TModel : ApplicationUser
    {
        /// <summary>
        /// Create user asynchronous
        /// </summary>
        /// <param name="user">New user to create</param>
        /// <returns>Response</returns>
        public Task<AccessResult> CreateUserAsync(TModel user);

        /// <summary>
        /// Create user
        /// </summary>
        /// <param name="user">New user to create</param>
        /// <returns>Response</returns>
        public AccessResult CreateUser(TModel user);

        /// <summary>
        /// Update user asynchronous
        /// </summary>
        /// <param name="user">New user to update</param>
        /// <returns>Response</returns>
        public Task<AccessResult> UpdateUserAsync(TModel user);

        /// <summary>
        /// Update user
        /// </summary>
        /// <param name="user">New user to update</param>
        /// <returns>Response</returns>
        public AccessResult UpdateUser(TModel user);


        /// <summary>
        /// Delete user asynchronous
        /// </summary>
        /// <param name="user">New user to delete</param>
        /// <returns>Response</returns>
        public Task<AccessResult> DeleteUserAsync(TModel user);

        /// <summary>
        /// Delete user 
        /// </summary>
        /// <param name="user">New user to delete</param>
        /// <returns>Response</returns>
        public AccessResult DeleteUser(TModel user);

        /// <summary>
        /// Retrieves a user by their email address asynchronously.
        /// </summary>
        /// <param name="email">The user's email</param>
        /// <returns>An <see cref="AccessResult{TModel}"/> containing the user if found; otherwise, an error result indicating
        /// the user was not found.</returns>
        Task<AccessResult<TModel>> FetchUserByEmailAsync(string email);

        /// <summary>
        /// Retrieves a user by their email address.
        /// </summary>
        /// <param name="email">The email address of the user to retrieve. Cannot be null or empty.</param>
        /// <returns>An <see cref="AccessResult{TModel}"/> containing the user if found; otherwise, an error result indicating
        /// the user was not found.</returns>
        AccessResult<TModel> FetchUserByEmail(string email);

        /// <summary>
        /// Retrieves a user by their username asynchronously.
        ///</summary>
        ///<param name="username">The username of the user to retrieve.</param>
        /// <returns>An <see cref="AccessResult{TModel}"/> containing the user data if found;  otherwise, an error result
        /// indicating the user was not found.</returns>
        Task<AccessResult<TModel>> FetchUserByUsernameAsync(string username);
        
        /// <summary>
        /// Retrieves a user by their username asynchronously.
        ///</summary>
        ///<param name="username">The username of the user to retrieve.</param>
        /// <returns>An <see cref="AccessResult{TModel}"/> containing the user data if found;  otherwise, an error result
        /// indicating the user was not found.</returns>
        AccessResult<TModel> FetchUserByUsername(string username);

        /// <summary>
        /// Asynchronously retrieves a user by their unique identifier.
        /// </summary>
        /// <param name="userId">The unique identifier of the user to retrieve.</param>
        /// <returns>An <see cref="AccessResult{TModel}"/> containing the user data if found;  otherwise, an error result
        /// indicating the user was not found.</returns>
        Task<AccessResult<TModel>> FetchUserByIdAsync(Guid userId);

        /// <summary>
        /// Retrieves a user by their unique identifier.
        /// </summary>
        /// <param name="userId">The unique identifier of the user to retrieve.</param>
        /// <returns>An <see cref="AccessResult{TModel}"/> containing the user data if found;  otherwise, an error result
        /// indicating the user was not found.</returns>
        AccessResult<TModel> FetchUserById(Guid userId);
    }
}
