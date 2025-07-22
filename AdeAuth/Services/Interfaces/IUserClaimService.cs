using AdeAuth.Models;
using AdeAuth.Services.Utility;

namespace AdeAuth.Services.Interfaces
{
    /// <summary>
    /// Provides methods for managing user claims, including creation, update, deletion, and retrieval operations.
    /// </summary>
    /// <remarks>This service interface defines both synchronous and asynchronous methods for handling user
    /// claims. It allows for operations on individual claims as well as batch operations on lists of claims.</remarks>
    public interface IUserClaimService
    {
        /// <summary>
        /// Creates a new user claim in the system.
        /// </summary>
        /// <param name="claim">The <see cref="UserClaim"/> object representing the claim to be created. Cannot be null.</param>
        /// <returns>An <see cref="AccessResult"/> indicating the success or failure of the operation.</returns>
        AccessResult CreateUserClaim(UserClaim claim);

        /// <summary>
        /// Creates user claims and returns the result of the operation.
        /// </summary>
        /// <param name="claims">A list of <see cref="UserClaim"/> objects representing the claims to be created. Cannot be null or empty.</param>
        /// <returns>An <see cref="AccessResult"/> indicating the success or failure of the claim creation process.</returns>
        AccessResult CreateUserClaims(List<UserClaim> claims);

        /// <summary>
        /// Updates the specified user claim in the system.
        /// </summary>
        /// <param name="claim">The <see cref="UserClaim"/> object containing the updated claim information. Cannot be null.</param>
        /// <returns>An <see cref="AccessResult"/> indicating the success or failure of the update operation.</returns>
        AccessResult UpdateUserClaim(UserClaim claim);

        /// <summary>
        /// Deletes the specified user claim from the system.
        /// </summary>
        /// <param name="claim">The user claim to be deleted. Cannot be null.</param>
        /// <returns>An <see cref="AccessResult"/> indicating the success or failure of the operation.</returns>
        AccessResult DeleteUserClaim(UserClaim claim);

        /// <summary>
        /// Retrieves a specific user claim based on the claim type and user identifier.
        /// </summary>
        /// <param name="claimType">The type of claim to retrieve, specified as a string. This parameter cannot be null or empty.</param>
        /// <param name="userId">The unique identifier of the user whose claim is being requested.</param>
        /// <returns>An <see cref="AccessResult{UserClaim}"/> containing the user claim if found; otherwise, an appropriate error
        /// result.</returns>
        AccessResult<UserClaim> GetUserClaim(string claimType, Guid userId);

        /// <summary>
        /// Retrieves a list of claims associated with a specified user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose claims are to be fetched. Must not be <see langword="null"/> or an
        /// empty GUID.</param>
        /// <returns>An <see cref="AccessResult{T}"/> containing a list of <see cref="UserClaim"/> objects associated with the
        /// user. If the user has no claims, the list will be empty.</returns>
        AccessResult<List<UserClaim>> FetchUserClaims(Guid userId);

        /// <summary>
        /// Asynchronously creates user claims and returns the result of the operation.
        /// </summary>
        /// <param name="claims">A list of <see cref="UserClaim"/> objects representing the claims to be created. Cannot be null or empty.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="AccessResult"/>
        /// indicating the success or failure of the claim creation process.</returns>
        Task<AccessResult> CreateUserClaimsAsync(List<UserClaim> claims);

        /// <summary>
        /// Asynchronously creates a user claim and returns the result of the operation.
        /// </summary>
        /// <param name="claim">The <see cref="UserClaim"/> object representing the claim to be created. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="AccessResult"/>
        /// indicating the success or failure of the claim creation.</returns>
        Task<AccessResult> CreateUserClaimAsync(UserClaim claim);

        /// <summary>
        /// Updates the specified user claim asynchronously.
        /// </summary>
        /// <param name="claim">The user claim to be updated. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="AccessResult"/>
        /// indicating the success or failure of the update operation.</returns>
        Task<AccessResult> UpdateUserClaimAsync(UserClaim claim);

        /// <summary>
        /// Asynchronously deletes a specified user claim.
        /// </summary>
        /// <param name="claim">The user claim to be deleted. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="AccessResult"/>
        /// indicating the success or failure of the operation.</returns>
        Task<AccessResult> DeleteUserClaimAsync(UserClaim claim);

        /// <summary>
        /// Asynchronously retrieves a user claim of the specified type for a given user.
        /// </summary>
        /// <param name="claimType">The type of the claim to retrieve. This must be a valid claim type string.</param>
        /// <param name="userId">The unique identifier of the user whose claim is to be retrieved.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="AccessResult{T}"/>
        /// object that holds the user claim of the specified type. If the claim is not found, the result may indicate
        /// an unsuccessful access attempt.</returns>
        Task<AccessResult<UserClaim>> GetUserClaimAsync(string claimType, Guid userId);

        /// <summary>
        /// Asynchronously retrieves a list of claims associated with a specified user.
        /// </summary>
        /// <remarks>This method performs an asynchronous operation to fetch user claims, which may
        /// involve network or database access. Ensure that the calling code handles potential exceptions and awaits the
        /// task appropriately.</remarks>
        /// <param name="userId">The unique identifier of the user whose claims are to be fetched.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="AccessResult{T}"/>
        /// object with a list of <see cref="UserClaim"/> objects representing the user's claims. If the user has no
        /// claims, the list will be empty.</returns>
        Task<AccessResult<List<UserClaim>>> FetchUserClaimsAsync(Guid userId);
    }
}
