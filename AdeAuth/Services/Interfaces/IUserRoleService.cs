using AdeAuth.Models;
using AdeAuth.Services.Utility;


namespace AdeAuth.Services.Interfaces
{
    /// <summary>
    /// Defines methods for managing user roles within an application.
    /// </summary>
    /// <remarks>This service provides both synchronous and asynchronous methods for adding and removing roles
    /// from users. It supports operations using either a user object or an email identifier.</remarks>
    /// <typeparam name="TUser">The type of user, which must inherit from <see cref="ApplicationUser"/>.</typeparam>
    public interface IUserRoleService<TUser>
        where TUser : ApplicationUser
    {
        /// <summary>
        /// Asynchronously adds a specified role to a user.
        /// </summary>
        /// <param name="user">The user to whom the role will be added. Cannot be null.</param>
        /// <param name="role">The name of the role to add. Cannot be null or empty.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="AccessResult"/>
        /// indicating the outcome of the operation.</returns>
        Task<AccessResult> AddUserRoleAsync(TUser user, string role);

        /// <summary>
        /// Adds a specified role to a user.
        /// </summary>
        /// <remarks>This method checks if the specified role exists and if the user is already present in
        /// the system before attempting to add the role. It also ensures that changes are saved successfully.</remarks>
        /// <param name="user">The user to whom the role will be added. Cannot be null.</param>
        /// <param name="role">The name of the role to add. Must be a valid, existing role.</param>
        /// <returns>An <see cref="AccessResult"/> indicating the success or failure of the operation. Returns <see
        /// cref="AccessResult.Success"/> if the role is successfully added. Returns <see cref="AccessResult.Failed"/>
        /// with an appropriate error message if the role is invalid, the user does not exist, or if saving changes
        /// fails.</returns>
        AccessResult AddUserRole(TUser user, string role);

        /// <summary>
        /// Removes a specified role from a user.
        /// </summary>
        /// <param name="user">The user from whom the role will be removed. Cannot be null.</param>
        /// <param name="role">The role to remove</param>
        /// <returns>An <see cref="AccessResult"/> indicating the success or failure of the operation.  Returns <see
        /// cref="AccessResult.Success"/> if the role is successfully removed;  otherwise, returns <see
        /// cref="AccessResult.Failed"/> with an appropriate error message.</returns>
        AccessResult RemoveUserRole(TUser user, string role);

        /// <summary>
        /// Asynchronously removes a specified role from a user.
        /// </summary>
        /// <param name="user">The user from whom the role will be removed. Cannot be null.</param>
        /// <param name="role">The name of the role to be removed. Cannot be null or empty.</param>
        /// <returns>An <see cref="AccessResult"/> indicating the success or failure of the operation. Returns <see
        /// cref="AccessResult.Success"/> if the role is successfully removed. Returns <see cref="AccessResult.Failed"/>
        /// with an appropriate error message if the role does not exist, the user is not found, the role is not
        /// associated with the user, or if the operation fails to persist changes.</returns>
        Task<AccessResult> RemoveUserRoleAsync(TUser user, string role);

        /// <summary>
        /// Add role to user.
        /// </summary>
        /// <param name="email">Email of the user</param>
        /// <param name="role">The role to assigned to the user</param>
        /// <returns>An <see cref="AccessResult"/> indicating the success or failure of the operation. Returns <see
        /// cref="AccessResult.Success"/> if the role is successfully added to the user. Returns <see cref="AccessResult.Failed"/>
        /// with an appropriate error message if the role is invalid, the user does not exist, or if saving changes
        /// fails</returns>
        AccessResult AddUserRole(string email, string role);

        /// <summary>
        /// Asynchronously adds a specified role to a user identified by their email address.
        /// </summary>
        /// <remarks>This method checks if the specified role exists and if the user with the given email
        /// exists.  If either does not exist, the operation fails with a corresponding error message.</remarks>
        /// <param name="email">The email address of the user to whom the role will be added. Cannot be null or empty.</param>
        /// <param name="role">The name of the role to be added to the user. Cannot be null or empty.</param>
        /// <returns>An <see cref="AccessResult"/> indicating the success or failure of the operation.  Returns <see
        /// cref="AccessResult.Success"/> if the role is successfully added;  otherwise, returns <see
        /// cref="AccessResult.Failed"/> with an appropriate error message.</returns>
        Task<AccessResult> AddUserRoleAsync(string email, string role);
    }
}
