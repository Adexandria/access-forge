using AdeAuth.Models;
using AdeAuth.Services.Utility;

namespace AdeAuth.Services.Interfaces
{
    /// <summary>
    /// Provides methods for managing login activities, including creation, update, deletion, and retrieval of login
    /// activity records.
    /// </summary>
    /// <remarks>This service interface defines both asynchronous and synchronous methods for handling login
    /// activities.  It supports operations to create, update, delete, and fetch login activity records based on user
    /// identifiers and IP addresses.</remarks>
    public interface ILoginActivityService
    {
        /// <summary>
        /// Asynchronously creates a new login activity record.
        /// </summary>
        /// <param name="loginActivity">The login activity details to be recorded. Cannot be null.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains an <see cref="AccessResult"/>
        /// indicating the outcome of the operation.</returns>
        Task<AccessResult> CreateLoginActivityAsync(LoginActivity loginActivity);

        /// <summary>
        /// Creates a new login activity record in the system.
        /// </summary>
        /// <param name="loginActivity">The login activity details to be recorded. Cannot be null.</param>
        /// <returns>An <see cref="AccessResult"/> indicating the success or failure of the operation.</returns>
        AccessResult CreateLoginActivity(LoginActivity loginActivity);

        /// <summary>
        /// Updates the login activity asynchronously for a specified user.
        /// </summary>
        /// <remarks>This method updates the login activity record for a user, which may include details
        /// such as login time, IP address, and device information. Ensure that the <paramref name="loginActivity"/>
        /// object is properly populated before calling this method.</remarks>
        /// <param name="loginActivity">The login activity details to be updated. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="AccessResult"/>
        /// indicating the outcome of the update operation.</returns>
        Task<AccessResult> UpdateLoginActivityAsync(LoginActivity loginActivity);

        /// <summary>
        /// Updates the login activity record with the specified details.
        /// </summary>
        /// <param name="loginActivity">The login activity details to update. Cannot be null.</param>
        /// <returns>An <see cref="AccessResult"/> indicating the outcome of the update operation.</returns>
        AccessResult UpdateLoginActivity(LoginActivity loginActivity);

        /// <summary>
        /// Asynchronously deletes the specified login activity from the system.
        /// </summary>
        /// <remarks>This method removes the specified login activity, which may affect audit logs or user
        /// activity tracking. Ensure that the login activity is no longer needed before deletion.</remarks>
        /// <param name="loginActivity">The login activity to be deleted. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="AccessResult"/>
        /// indicating the success or failure of the operation.</returns>
        Task<AccessResult> DeleteLoginActivityAsync(LoginActivity loginActivity);

        /// <summary>
        /// Deletes the specified login activity from the system.
        /// </summary>
        /// <param name="loginActivity">The login activity to be deleted. Cannot be null.</param>
        /// <returns>An <see cref="AccessResult"/> indicating the success or failure of the deletion operation.</returns>
        AccessResult DeleteLoginActivity(LoginActivity loginActivity);

        /// <summary>
        /// Asynchronously retrieves the login activity for a specified user and IP address.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose login activity is to be retrieved.</param>
        /// <param name="ipAddress">The IP address associated with the login activity.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="AccessResult{T}"/>
        /// object with the login activity details for the specified user and IP address.</returns>
        Task<AccessResult<LoginActivity>> FetchLoginActivityAsync(Guid userId, string ipAddress);

        /// <summary>
        /// Retrieves the login activity for a specified user and IP address.
        /// </summary>
        /// <remarks>This method provides information about the login attempts made by a user from a
        /// specific IP address. It is useful for auditing and security purposes to track user access
        /// patterns.</remarks>
        /// <param name="userId">The unique identifier of the user whose login activity is being requested.</param>
        /// <param name="ipAddress">The IP address from which the login activity is being queried.</param>
        /// <returns>An <see cref="AccessResult{T}"/> containing the login activity details for the specified user and IP
        /// address.</returns>
        AccessResult<LoginActivity> FetchLoginActivity(Guid userId, string ipAddress);

        /// <summary>
        /// Retrieves the login activity records for a specified user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose login activity is to be fetched. Cannot be <see langword="null"/>.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="LoginActivity"/> objects representing the user's login
        /// activity. The collection will be empty if no activity is found.</returns>
        AccessResult<IEnumerable<LoginActivity>> FetchLoginActivity(Guid userId);
    }
}
