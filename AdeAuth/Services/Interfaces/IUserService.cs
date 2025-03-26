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

        Task<AccessResult<TModel>> FetchUserByEmailAsync(string email);

        AccessResult<TModel> FetchUserByEmail(string email);

        Task<AccessResult<TModel>> FetchUserByUsernameAsync(string username);

        AccessResult<TModel> FetchUserByUsername(string username);

        Task<AccessResult<TModel>> FetchUserByIdAsync(Guid userId);
        
        AccessResult<TModel> FetchUserById(Guid userId);
    }
}
