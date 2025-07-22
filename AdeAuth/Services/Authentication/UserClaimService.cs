using AdeAuth.Models;
using AdeAuth.Services.Interfaces;
using AdeAuth.Services.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;


namespace AdeAuth.Services.Authentication
{
    /// <summary>
    /// Manages the user claim
    /// </summary>
    /// <remarks>This service allows for creating, updating, deleting, and fetching user claims. It
    /// operates on a <see cref="DbSet{TEntity}"/> of <see cref="UserClaim"/> entities within the provided database
    /// context.</remarks>
    /// <typeparam name="TDbContext">The type of the database context used by this service. Must be a subclass of <see cref="DbContext"/>.</typeparam>
    public class UserClaimService<TDbContext> : Repository<TDbContext,UserClaim>,IUserClaimService
        where TDbContext: DbContext
    {
        /// <summary>
        /// A constructor
        /// </summary>
        /// <param name="dbContext"></param>
        public UserClaimService(TDbContext dbContext):base(dbContext)
        {
            _userClaims = Db.Set<UserClaim>();
        }

        /// <summary>
        /// Creates a new user claim.
        /// </summary>
        /// <param name="claim">Manages user claim</param>
        /// <returns>An <see cref="AccessResult"/> indicating the outcome of the operation.  Returns <see
        /// cref="AccessResult.Success"/> if the claim was successfully created;  otherwise, returns <see 
        /// cref="AccessResult.Failed" /> with an error message and status code</returns>
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

        /// <summary>
        /// Deletes a specified user claim from the system.
        /// </summary>
        /// <param name="claim">The user claim to be deleted. Cannot be null.</param>
        /// <returns>An <see cref="AccessResult"/> indicating the outcome of the operation.  Returns <see
        /// cref="AccessResult.Success"/> if the claim was successfully deleted;  otherwise, returns <see
        /// cref="AccessResult.Failed"/> with an error message and status code.</returns>
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

        /// <summary>
        /// Retrieves the list of claims associated with a specified user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose claims are to be fetched.</param>
        /// <returns>An <see cref="AccessResult{T}"/> containing a list of <see cref="UserClaim"/> objects associated with the
        /// specified user. The list will be empty if the user has no claims.</returns>
        public AccessResult<List<UserClaim>> FetchUserClaims(Guid userId)
        {
            var claims = _userClaims.Where(s => s.UserId == userId).ToList();

            return AccessResult<List<UserClaim>>.Success(claims);
        }

        /// <summary>
        /// Retrieves a specific user claim based on the claim type and user ID. 
        /// </summary>
        /// <param name="claimType">Claim type to extract</param>
        /// <param name="userId">The unique identifier of the user whose claims are to be fetched.</param>
        /// <returns>An <see cref="AccessResult{T}"/> containing <see cref="UserClaim"/> object associated with the
        /// specified user.</returns>
        public AccessResult<UserClaim> GetUserClaim(string claimType, Guid userId)
        {
            var claim = _userClaims.Where(s => s.ClaimType == claimType && s.UserId == userId).FirstOrDefault();

            if(claim == null)
            {
                return AccessResult<UserClaim>.Failed(new AccessError("User claim not found", StatusCodes.Status404NotFound));
            }

            return AccessResult<UserClaim>.Success(claim);
        }

        /// <summary>
        /// Updates an existing user claim in the system.
        /// </summary>
        /// <param name="claim">The <see cref="UserClaim"/> object representing the claim to be updated. Cannot be null.</param>
        /// <returns>An <see cref="AccessResult"/> indicating the outcome of the operation.  Returns <see
        /// cref="AccessResult.Success"/> if the claim was successfully created;  otherwise, returns <see 
        /// cref="AccessResult.Failed" /> with an error message and status code</returns>
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

        /// <summary>
        /// Asynchronously creates a new user claim and saves it to the data store.
        /// </summary>
        /// <param name="claim">The <see cref="UserClaim"/> object representing the claim to be added. Cannot be null.</param>
        /// <returns>An <see cref="AccessResult"/> indicating the outcome of the operation.  Returns <see
        /// cref="AccessResult.Success"/> if the claim is successfully created;  otherwise, returns <see
        /// cref="AccessResult.Failed"/> with an error message and status code.</returns>
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

        /// <summary>
        /// Updates the specified user claim asynchronously.
        /// </summary>
        /// <param name="claim">The user claim to update. Cannot be null.</param>
        /// <returns>An <see cref="AccessResult"/> indicating the outcome of the update operation. Returns <see
        /// cref="AccessResult.Success"/> if the update is successful; otherwise, returns <see
        /// cref="AccessResult.Failed"/> with an appropriate error message.</returns>
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

        /// <summary>
        /// Deletes a specified user claim asynchronously.
        /// </summary>
        /// <param name="claim">The user claim to be deleted. Cannot be null.</param>
        /// <returns>An <see cref="AccessResult"/> indicating the outcome of the operation.  Returns <see
        /// cref="AccessResult.Success"/> if the claim was successfully deleted;  otherwise, returns <see
        /// cref="AccessResult.Failed"/> with an error message.</returns>
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

        /// <summary>
        /// Asynchronously retrieves a user claim based on the specified claim type and user identifier.
        /// </summary>
        /// <param name="claimType">The type of the claim to retrieve. This parameter cannot be null or empty.</param>
        /// <param name="userId">The unique identifier of the user whose claim is to be retrieved.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="AccessResult{T}"/>
        /// of <see cref="UserClaim"/>. If the claim is found, the result is successful and contains the user claim;
        /// otherwise, the result indicates failure with an appropriate error message.</returns>
        public async Task<AccessResult<UserClaim>> GetUserClaimAsync(string claimType, Guid userId)
        {
            var claim = await _userClaims.Where(s => s.ClaimType == claimType && s.UserId == userId).FirstOrDefaultAsync();

            if (claim == null)
            {
                return AccessResult<UserClaim>.Failed(new AccessError("User claim not found", StatusCodes.Status404NotFound));
            }

            return AccessResult<UserClaim>.Success(claim);
        }

        /// <summary>
        /// Asynchronously retrieves a list of claims associated with a specified user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose claims are to be fetched.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an  <see cref="AccessResult{T}"/> object with
        /// a list of <see cref="UserClaim"/> objects  associated with the specified user.</returns>
        public async Task<AccessResult<List<UserClaim>>> FetchUserClaimsAsync(Guid userId)
        {
            var claims = await _userClaims.Where(s => s.UserId == userId).ToListAsync();

            return AccessResult<List<UserClaim>>.Success(claims);
        }

        /// <summary>
        /// Adds a list of user claims to the current collection and persists the changes.
        /// </summary>
        /// <param name="claims">A list of <see cref="UserClaim"/> objects to be added. Cannot be null or empty.</param>
        /// <returns>An <see cref="AccessResult"/> indicating the success or failure of the operation.  Returns <see
        /// cref="AccessResult.Success"/> if the claims are successfully added and saved;  otherwise, returns <see
        /// cref="AccessResult.Failed"/> with an error message and status code.</returns>
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

        /// <summary>
        /// Adds a list of user claims to the current collection and persists the changes asynchronously.
        /// </summary>
        /// <param name="claims">A list of <see cref="UserClaim"/> objects to be added. Cannot be null or empty.</param>
        /// <returns>An <see cref="AccessResult"/> indicating the success or failure of the operation.  Returns <see
        /// cref="AccessResult.Success"/> if the claims are successfully added and saved;  otherwise, returns <see
        /// cref="AccessResult.Failed"/> with an error message and status code.</returns>
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
