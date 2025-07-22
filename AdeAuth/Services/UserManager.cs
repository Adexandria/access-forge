using AdeAuth.Models;
using AdeAuth.Services.Interfaces;
using AdeAuth.Services.Utility;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace AdeAuth.Services
{
    /// <summary>
    /// Provides user management functionalities, including user creation, updates, and authentication setup.
    /// </summary>
    /// <remarks>The <see cref="UserManager{TUser}"/> class offers a comprehensive set of methods for managing
    /// user accounts, including creating, updating, and deleting users, as well as handling authentication and
    /// authorization tasks. It supports operations such as setting user details, enabling two-factor authentication,
    /// and generating tokens for password resets and confirmations. This class is designed to work with a user service,
    /// token provider, and multi-factor authentication service to provide a robust user management solution.</remarks>
    /// <typeparam name="TUser">The type of user managed by this class, which must inherit from <see cref="ApplicationUser"/>.</typeparam>
    public class UserManager<TUser>
        where TUser : ApplicationUser
    {
        /// <summary>
        /// A constructor.
        /// </summary>
        /// <param name="userService">The service used to manage user data and operations.</param>
        /// <param name="tokenProvider">The provider responsible for generating and validating authentication tokens.</param>
        /// <param name="mfaService">The service used to handle multi-factor authentication processes.</param>
        /// <param name="accessOption">The configuration options that define access control settings.</param>
        /// <param name="userClaimService">The service used to manage user claims and associated operations.</param>
        /// <param name="authenticationConfiguration">The configuration settings for two-factor authentication.</param>
        public UserManager(IUserService<TUser> userService,
            ITokenProvider tokenProvider,
            IMfaService mfaService, AccessOption accessOption,IUserClaimService userClaimService,
            TwoAuthenticationConfiguration authenticationConfiguration)
        {
            _tokenProvider = tokenProvider;
            _userService = userService;
            _mfaService = mfaService;
            _userClaimService = userClaimService;
            _accessOption = accessOption;
            _twoAuthenticationConfiguration = authenticationConfiguration;
        }

        #region Create user
        /// <summary>
        /// Asynchronously creates a new user and assigns default claims.
        /// </summary>
        /// <remarks>This method validates the provided user information before attempting to create the
        /// user. If the user creation is successful, default claims such as email and name identifier are assigned to
        /// the user.</remarks>
        /// <param name="user">The user to be created. Must not be null and should contain valid user information.</param>
        /// <returns>An <see cref="AccessResult"/> indicating the success or failure of the user creation operation. If
        /// successful, the result contains the created user's details; otherwise, it contains error information.</returns>
        public async Task<AccessResult> CreateUserAsync(TUser user)
        {
            new Validator()
                .IsValid(user, "Invalid user")
                .Validate();
            var response = await _userService.CreateUserAsync(user);

            if (!response.IsSuccessful)
            {
                return AccessResult.Failed(response.Errors.Single());
            }

            var claims = new List<UserClaim>
            {
                new()
                {
                    UserId = user.Id,
                    ClaimType = ClaimTypes.Email,
                    ClaimValue = user.Email
                },
                new()
                {
                    UserId = user.Id,
                    ClaimType = ClaimTypes.NameIdentifier,
                    ClaimValue = user.Id.ToString()
                }
            };

            var claimResponse = await _userClaimService.CreateUserClaimsAsync(claims);

            if (!claimResponse.IsSuccessful)
            {
               return AccessResult.Failed(new AccessError("Failed to create user", StatusCodes.Status400BadRequest));
            }

            return response;
        }

        /// <summary>
        /// Creates a new user and assigns default claims.
        /// </summary>
        /// <remarks>This method validates the provided user information before attempting to create the
        /// user. If the user creation is successful, default claims such as email and name identifier are assigned to
        /// the user.</remarks>
        /// <param name="user">The user to be created. Must not be null and should contain valid user information.</param>
        /// <returns>An <see cref="AccessResult"/> indicating the success or failure of the user creation operation. If
        /// successful, the result contains the created user's details; otherwise, it contains error information.</returns>
        public AccessResult CreateUser(TUser user)
        {
            new Validator()
               .IsValid(user, "Invalid user")
               .Validate();

            var response =  _userService.CreateUser(user);

            if (!response.IsSuccessful)
            {
                return AccessResult.Failed(response.Errors.Single());
            }

            var claims = new List<UserClaim>
            {
                new()
                {
                    UserId = user.Id,
                    ClaimType = ClaimTypes.Email,
                    ClaimValue = user.Email
                },
                new()
                {
                    UserId = user.Id,
                    ClaimType = ClaimTypes.NameIdentifier,
                    ClaimValue = user.Id.ToString()
                }
            };

            var claimResponse =  _userClaimService.CreateUserClaims(claims);

            if (!claimResponse.IsSuccessful)
            {
                return AccessResult.Failed(new AccessError("Failed to create user", StatusCodes.Status400BadRequest));
            }

            return response;
        }

        #endregion

        #region Update user

        /// <summary>
        /// Updates the specified user asynchronously.
        /// </summary>
        /// <param name="user">The user object containing updated information. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="AccessResult"/>
        /// indicating the outcome of the update operation.</returns>
        public async Task<AccessResult> UpdateUserAsync(TUser user)
        {
            new Validator()
               .IsValid(user, "Invalid user")
               .Validate();

            return await _userService.UpdateUserAsync(user);
        }

        /// <summary>
        /// Updates the specified user in the system.
        /// </summary>
        /// <param name="user">The user object containing updated information. Cannot be null.</param>
        /// <returns>An <see cref="AccessResult"/> indicating the outcome of the update operation.</returns>
        public AccessResult UpdateUser(TUser user)
        {
            new Validator()
               .IsValid(user, "Invalid user")
               .Validate();

            return _userService.UpdateUser(user);
        }

        #endregion

        #region Set username
        /// <summary>
        /// Asynchronously sets the username for a specified user.
        /// </summary>
        /// <remarks>This method first validates the input parameters. It then attempts to fetch the user
        /// by the specified  <paramref name="userId"/>. If the user is found, the method updates the user's username
        /// and saves the changes.</remarks>
        /// <param name="userId">The unique identifier of the user whose username is to be set. Must be a valid GUID.</param>
        /// <param name="username">The new username to assign to the user. Must be a valid non-empty string.</param>
        /// <returns>An <see cref="AccessResult"/> indicating the success or failure of the operation.  Returns <see
        /// cref="AccessResult.Success"/> if the username is successfully set; otherwise,  returns <see
        /// cref="AccessResult.Failed"/> with an appropriate error message.</returns>
        public async Task<AccessResult> SetUsernameAsync(Guid userId, string username)
        {
            new Validator().IsValidGuid(userId, "Invalid user id")
                .IsValidText(username, "Invalid username")
                .Validate();

            var response = await _userService.FetchUserByIdAsync(userId);

            if (!response.IsSuccessful)
            {
                return AccessResult.Failed(new AccessError("Invalid user", StatusCodes.Status400BadRequest));
            }

            var user = response.Data;

            user.UserName = username;

            var updateResponse = await _userService.UpdateUserAsync(user);

            if (!updateResponse.IsSuccessful)
            {
                return AccessResult.Failed(new AccessError("Failed to set username", StatusCodes.Status400BadRequest));
            }

            return AccessResult.Success();
        }

        /// <summary>
        /// Asynchronously sets the username for the specified user.
        /// </summary>
        /// <remarks>This method updates the user's username and attempts to persist the change using the
        /// user service. If the update operation is unsuccessful, the method returns a failed <see
        /// cref="AccessResult"/> with an appropriate error message.</remarks>
        /// <param name="user">The user whose username is to be set. Cannot be null.</param>
        /// <param name="username">The new username to assign to the user. Must be a valid, non-empty string.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="AccessResult"/>
        /// indicating the success or failure of the operation.</returns>
        public async Task<AccessResult> SetUsernameAsync(TUser user, string username)
        {

            new Validator()
            .IsValid(user, "Invalid user")
             .IsValidText(username, "Invalid username")
                .Validate();


            user.UserName = username;

            var updateResponse = await _userService.UpdateUserAsync(user);

            if (!updateResponse.IsSuccessful)
            {
                return AccessResult.Failed(new AccessError("Failed to set username", StatusCodes.Status400BadRequest));
            }

            return AccessResult.Success();
        }

        /// <summary>
        /// Sets the username for a specified user.
        /// </summary>
        /// <remarks>This method attempts to update the username of an existing user identified by
        /// <paramref name="userId"/>.  It validates the input parameters and checks if the user exists before
        /// attempting the update.  If the user does not exist or the update fails, the method returns a failed <see
        /// cref="AccessResult"/>  with a relevant error message.</remarks>
        /// <param name="userId">The unique identifier of the user whose username is to be set. Must be a valid GUID.</param>
        /// <param name="username">The new username to assign to the user. Must be a valid non-empty string.</param>
        /// <returns>An <see cref="AccessResult"/> indicating the success or failure of the operation.  Returns <see
        /// cref="AccessResult.Success"/> if the username is successfully set; otherwise,  returns <see
        /// cref="AccessResult.Failed"/> with an appropriate error message.</returns>
        public AccessResult SetUsername(Guid userId, string username)
        {
            new Validator().IsValidGuid(userId, "Invalid user id")
               .IsValidText(username, "Invalid username")
               .Validate();


            var response = _userService.FetchUserById(userId);

            if (!response.IsSuccessful)
            {
                return AccessResult.Failed(new AccessError("Invalid user", StatusCodes.Status400BadRequest));
            }

            var user = response.Data;

            user.UserName = username;

            var updateResponse = _userService.UpdateUser(user);

            if (!updateResponse.IsSuccessful)
            {
                return AccessResult.Failed(new AccessError("Failed to set username", StatusCodes.Status400BadRequest));
            }

            return AccessResult.Success();
        }

        /// <summary>
        /// Sets the username for the specified user.
        /// </summary>
        /// <param name="user">The user whose username is to be set. Cannot be null.</param>
        /// <param name="username">The new username to assign to the user. Must be a valid, non-empty string.</param>
        /// <returns>An <see cref="AccessResult"/> indicating the success or failure of the operation. Returns <see
        /// cref="AccessResult.Success"/> if the username is set successfully; otherwise, returns <see
        /// cref="AccessResult.Failed"/> with an appropriate error message.</returns>
        public AccessResult SetUsername(TUser user, string username)
        {
            new Validator()
         .IsValid(user, "Invalid user")
          .IsValidText(username, "Invalid username")
             .Validate();

            user.UserName = username;

            var updateResponse = _userService.UpdateUser(user);

            if (!updateResponse.IsSuccessful)
            {
                return AccessResult.Failed(new AccessError("Failed to set username", StatusCodes.Status400BadRequest));
            }

            return AccessResult.Success();
        }

        #endregion

        #region Set email

        /// <summary>
        /// Asynchronously sets the email address for a specified user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose email is to be set. Must be a valid GUID.</param>
        /// <param name="email">The new email address to assign to the user. Must be a valid email format.</param>
        /// <returns>An <see cref="AccessResult"/> indicating the success or failure of the operation.  Returns <see
        /// cref="AccessResult.Success"/> if the email was successfully set; otherwise,  returns <see
        /// cref="AccessResult.Failed"/> with an appropriate error message.</returns>
        public async Task<AccessResult> SetEmailAsync(Guid userId, string email)
        {
            new Validator().IsValidGuid(userId, "Invalid user id")
               .IsValidText(email, "Invalid email")
               .Validate();

            var response = await _userService.FetchUserByIdAsync(userId);

            if (!response.IsSuccessful)
            {
                return AccessResult.Failed(new AccessError("Invalid user", StatusCodes.Status400BadRequest));
            }

            var user = response.Data;

            user.Email = email;

            var updateResponse = await _userService.UpdateUserAsync(user);

            if (!updateResponse.IsSuccessful)
            {
                return AccessResult.Failed(new AccessError("Failed to set email", StatusCodes.Status400BadRequest));
            }

            return AccessResult.Success();
        }

        /// <summary>
        /// Asynchronously sets the email address for the specified user.
        /// </summary>
        /// <remarks>This method updates the user's email address and persists the change using the user
        /// service.  Ensure that the user object is valid and the email is correctly formatted before calling this
        /// method.</remarks>
        /// <param name="user">The user whose email address is to be set. Cannot be null.</param>
        /// <param name="email">The new email address to assign to the user. Must be a valid email format.</param>
        /// <returns>An <see cref="AccessResult"/> indicating the success or failure of the operation. Returns <see
        /// cref="AccessResult.Success"/> if the email was set successfully; otherwise, returns <see
        /// cref="AccessResult.Failed"/> with an appropriate error message.</returns>
        public async Task<AccessResult> SetEmailAsync(TUser user, string email)
        {
            new Validator()
             .IsValid(user, "Invalid user")
                .IsValidText(email, "Invalid email")
                 .Validate();


            user.Email = email;

            var updateResponse = await _userService.UpdateUserAsync(user);

            if (!updateResponse.IsSuccessful)
            {
                return AccessResult.Failed(new AccessError("Failed to set email", StatusCodes.Status400BadRequest));
            }

            return AccessResult.Success();
        }

        /// <summary>
        /// Sets the email address for a specified user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose email is to be set. Must be a valid GUID.</param>
        /// <param name="email">The new email address to assign to the user. Must be a valid email format.</param>
        /// <returns>An <see cref="AccessResult"/> indicating the success or failure of the operation.  Returns <see
        /// cref="AccessResult.Success"/> if the email was successfully set; otherwise,  returns <see
        /// cref="AccessResult.Failed"/> with an appropriate error message.</returns>
        public AccessResult SetEmail(Guid userId, string email)
        {
            new Validator()
          .IsValidGuid(userId, "Invalid user id")
             .IsValidText(email, "Invalid email")
              .Validate();

            var response = _userService.FetchUserById(userId);

            if (!response.IsSuccessful)
            {
                return AccessResult.Failed(new AccessError("Invalid user", StatusCodes.Status400BadRequest));
            }

            var user = response.Data;

            user.Email = email;

            var updateResponse = _userService.UpdateUser(user);

            if (!updateResponse.IsSuccessful)
            {
                return AccessResult.Failed(new AccessError("Failed to set email", StatusCodes.Status400BadRequest));
            }

            return AccessResult.Success();
        }


        /// <summary>
        /// Sets the email address for the specified user.
        /// </summary>
        /// <param name="user">The user whose email address is to be set. Cannot be null.</param>
        /// <param name="email">The new email address to assign to the user. Must be a valid email format.</param>
        /// <returns>An <see cref="AccessResult"/> indicating the success or failure of the operation.  Returns <see
        /// cref="AccessResult.Success"/> if the email was set successfully; otherwise,  returns <see
        /// cref="AccessResult.Failed"/> with an appropriate error message.</returns>
        public AccessResult SetEmail(TUser user, string email)
        {
            new Validator()
          .IsValid(user, "Invalid user")
             .IsValidText(email, "Invalid email")
              .Validate();

            user.Email = email;

            var updateResponse = _userService.UpdateUser(user);

            if (!updateResponse.IsSuccessful)
            {
                return AccessResult.Failed(new AccessError("Failed to set email", StatusCodes.Status400BadRequest));
            }

            return AccessResult.Success();
        }

        #endregion

        #region Enable lock out

        /// <summary>
        /// Enables lockout for the specified user for a given duration.
        /// </summary>
        /// <remarks>This method updates the user's lockout status and sets the lockout expiration time. 
        /// If the update operation fails, the method returns a failed <see cref="AccessResult"/> with an appropriate
        /// error message.</remarks>
        /// <param name="user">The user for whom lockout is to be enabled. Cannot be null.</param>
        /// <param name="lockOutTimeInMinutes">The duration in minutes for which the user will be locked out. Must be a positive integer.</param>
        /// <returns>An <see cref="AccessResult"/> indicating the success or failure of the operation.</returns>
        public async Task<AccessResult> EnableLockoutAsync(TUser user, int lockOutTimeInMinutes)
        {
            new Validator()
            .IsValid(user, "Invalid user")
             .IsValidInteger(lockOutTimeInMinutes, 1)
              .Validate();

            user.LockoutEnabled = true;
            user.LockOutExpiration = DateTime.UtcNow.AddMinutes(lockOutTimeInMinutes);
            var updateResponse = await _userService.UpdateUserAsync(user);

            if (!updateResponse.IsSuccessful)
            {
                return AccessResult.Failed(new AccessError("Failed to set lock out", StatusCodes.Status400BadRequest));
            }

            return AccessResult.Success();
        }

        /// <summary>
        /// Enables lockout for the specified user for a given duration.
        /// </summary>
        /// <remarks>This method updates the user's lockout status and sets the lockout expiration time.
        /// If the update operation fails, the method returns a failed <see cref="AccessResult"/> with an appropriate
        /// error message.</remarks>
        /// <param name="user">The user for whom the lockout is to be enabled. Cannot be null.</param>
        /// <param name="lockOutTimeInMinutes">The duration, in minutes, for which the lockout is enabled. Must be a positive integer.</param>
        /// <returns>An <see cref="AccessResult"/> indicating the success or failure of the operation.</returns>
        public AccessResult EnableLockout(TUser user, int lockOutTimeInMinutes)
        {
            new Validator()
            .IsValid(user, "Invalid user")
                .IsValidInteger(lockOutTimeInMinutes, 1)
                 .Validate();

            user.LockoutEnabled = true;
            user.LockOutExpiration = DateTime.UtcNow.AddMinutes(lockOutTimeInMinutes);
            var updateResponse = _userService.UpdateUser(user);

            if (!updateResponse.IsSuccessful)
            {
                return AccessResult.Failed(new AccessError("Failed to set lock out", StatusCodes.Status400BadRequest));
            }

            return AccessResult.Success();
        }

        /// <summary>
        /// Enables lockout for a specified user for a given duration.
        /// </summary>
        /// <remarks>This method sets the user's lockout status to enabled and specifies the expiration
        /// time for the lockout. If the user cannot be found or the update operation fails, the method returns a failed
        /// <see cref="AccessResult"/>.</remarks>
        /// <param name="userId">The unique identifier of the user to enable lockout for. Must be a valid GUID.</param>
        /// <param name="lockOutTimeInMinutes">The duration, in minutes, for which the user will be locked out. Must be a positive integer.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="AccessResult"/>
        /// indicating the success or failure of the operation.</returns>
        public async Task<AccessResult> EnableLockoutAsync(Guid userId, int lockOutTimeInMinutes)
        {
            new Validator()
             .IsValidGuid(userId, "Invalid user id")
                .IsValidInteger(lockOutTimeInMinutes, 1)
                .Validate();


            var response = await _userService.FetchUserByIdAsync(userId);

            if (!response.IsSuccessful)
            {
                return AccessResult.Failed(new AccessError("Invalid user", StatusCodes.Status400BadRequest));
            }

            var user = response.Data;

            user.LockoutEnabled = true;
            user.LockOutExpiration = DateTime.UtcNow.AddMinutes(lockOutTimeInMinutes);
            var updateResponse = await _userService.UpdateUserAsync(user);

            if (!updateResponse.IsSuccessful)
            {
                return AccessResult.Failed(new AccessError("Failed to set lock out", StatusCodes.Status400BadRequest));
            }

            return AccessResult.Success();
        }

        /// <summary>
        /// Enables lockout for a specified user for a given duration.
        /// </summary>
        /// <remarks>This method updates the user's lockout status and sets the lockout expiration time.
        /// If the user cannot be found or the update fails, the method returns a failed result with an error
        /// message.</remarks>
        /// <param name="userId">The unique identifier of the user to enable lockout for. Must be a valid GUID.</param>
        /// <param name="lockOutTimeInMinutes">The duration, in minutes, for which the user will be locked out. Must be a positive integer.</param>
        /// <returns>An <see cref="AccessResult"/> indicating the success or failure of the operation. Returns <see
        /// cref="AccessResult.Success"/> if the lockout is successfully enabled; otherwise, returns <see
        /// cref="AccessResult.Failed"/> with an appropriate error message.</returns>
        public AccessResult EnableLockout(Guid userId, int lockOutTimeInMinutes)
        {
            new Validator()
           .IsValidGuid(userId, "Invalid user id")
              .IsValidInteger(lockOutTimeInMinutes, 1)
              .Validate();

            var response = _userService.FetchUserById(userId);

            if (!response.IsSuccessful)
            {
                return AccessResult.Failed(new AccessError("Invalid user", StatusCodes.Status400BadRequest));
            }

            var user = response.Data;
            user.LockoutEnabled = true;
            user.LockOutExpiration = DateTime.UtcNow.AddMinutes(lockOutTimeInMinutes);
            var updateResponse = _userService.UpdateUser(user);

            if (!updateResponse.IsSuccessful)
            {
                return AccessResult.Failed(new AccessError("Failed to set lock out", StatusCodes.Status400BadRequest));
            }

            return AccessResult.Success();
        }

        #endregion

        #region Set first name

        /// <summary>
        /// Asynchronously sets the first name of the specified user.
        /// </summary>
        /// <remarks>This method updates the user's first name and persists the change using the user
        /// service.  If the update operation is unsuccessful, the method returns a failed <see cref="AccessResult"/>
        /// with an appropriate error message.</remarks>
        /// <param name="user">The user whose first name is to be set. Cannot be null.</param>
        /// <param name="firstName">The new first name to assign to the user. Must be a valid text string.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="AccessResult"/>
        /// indicating the success or failure of the operation.</returns>
        public async Task<AccessResult> SetFirstNameAsync(TUser user, string firstName)
        {
            new Validator()
            .IsValid(user, "Invalid user")
            .IsValidText(firstName, "Invalid firstname")
             .Validate();

            user.FirstName = firstName;

            var updateResponse = await _userService.UpdateUserAsync(user);

            if (!updateResponse.IsSuccessful)
            {
                return AccessResult.Failed(new AccessError("Failed to set first name", StatusCodes.Status400BadRequest));
            }

            return AccessResult.Success();
        }

        /// <summary>
        /// Asynchronously sets the first name of a user identified by the specified user ID.
        /// </summary>
        /// <remarks>This method performs validation on the input parameters and attempts to update the
        /// user's first name in the system.  If the user cannot be found or the update operation fails, the method
        /// returns a failed <see cref="AccessResult"/>.</remarks>
        /// <param name="userId">The unique identifier of the user whose first name is to be set. Must be a valid GUID.</param>
        /// <param name="firstName">The new first name to assign to the user. Must be a valid non-empty string.</param>
        /// <returns>An <see cref="AccessResult"/> indicating the success or failure of the operation.  Returns <see
        /// cref="AccessResult.Success"/> if the first name is successfully updated;  otherwise, returns <see
        /// cref="AccessResult.Failed"/> with an appropriate error message.</returns>
        public async Task<AccessResult> SetFirstNameAsync(Guid userId, string firstName)
        {
            new Validator()
          .IsValidGuid(userId, "Invalid user id")
          .IsValidText(firstName, "Invalid firstname")
           .Validate();

            var response = await _userService.FetchUserByIdAsync(userId);

            if (!response.IsSuccessful)
            {
                return AccessResult.Failed(new AccessError("Invalid user", StatusCodes.Status400BadRequest));
            }

            var user = response.Data;

            user.FirstName = firstName;

            var updateResponse = await _userService.UpdateUserAsync(user);

            if (!updateResponse.IsSuccessful)
            {
                return AccessResult.Failed(new AccessError("Failed to set first name", StatusCodes.Status400BadRequest));
            }

            return AccessResult.Success();
        }

        /// <summary>
        /// Sets the first name of the specified user.
        /// </summary>
        /// <param name="user">The user whose first name is to be set. Cannot be null.</param>
        /// <param name="firstName">The new first name to assign to the user. Must be a valid, non-empty string.</param>
        /// <returns>An <see cref="AccessResult"/> indicating the success or failure of the operation. Returns <see
        /// cref="AccessResult.Success"/> if the update is successful; otherwise, returns <see
        /// cref="AccessResult.Failed"/> with an appropriate error message.</returns>
        public AccessResult SetFirstName(TUser user, string firstName)
        {
            new Validator()
            .IsValid(user, "Invalid user")
            .IsValidText(firstName, "Invalid firstname")
            .Validate();

            user.FirstName = firstName;

            var updateResponse = _userService.UpdateUser(user);

            if (!updateResponse.IsSuccessful)
            {
                return AccessResult.Failed(new AccessError("Failed to set first name", StatusCodes.Status400BadRequest));
            }

            return AccessResult.Success();
        }

        /// <summary>
        /// Sets the first name of a user identified by the specified user ID.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose first name is to be set. Must be a valid GUID.</param>
        /// <param name="firstName">The new first name to assign to the user. Cannot be null or empty.</param>
        /// <returns>An <see cref="AccessResult"/> indicating the success or failure of the operation. Returns <see
        /// cref="AccessResult.Success"/> if the first name is set successfully; otherwise, returns <see
        /// cref="AccessResult.Failed"/> with an appropriate error message.</returns>
        public AccessResult SetFirstName(Guid userId, string firstName)
        {
            new Validator()
                .IsValidGuid(userId, "Invalid user id")
                .IsValidText(firstName, "Invalid firstname")
                .Validate();

            var response = _userService.FetchUserById(userId);

            if (!response.IsSuccessful)
            {
                return AccessResult.Failed(new AccessError("Invalid user", StatusCodes.Status400BadRequest));
            }

            var user = response.Data;

            user.FirstName = firstName;

            var updateResponse = _userService.UpdateUser(user);

            if (!updateResponse.IsSuccessful)
            {
                return AccessResult.Failed(new AccessError("Failed to set first name", StatusCodes.Status400BadRequest));
            }

            return AccessResult.Success();
        }

        #endregion

        #region Set last name

        /// <summary>
        /// Asynchronously sets the last name of the specified user.
        /// </summary>
        /// <remarks>This method updates the user's last name and persists the change using the user
        /// service. If the update operation fails, the method returns an <see cref="AccessResult"/> with an
        /// error.</remarks>
        /// <param name="user">The user whose last name is to be set. Cannot be null.</param>
        /// <param name="lastName">The new last name to assign to the user. Must be a valid text string.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="AccessResult"/>
        /// indicating the success or failure of the operation.</returns>
        public async Task<AccessResult> SetLastNameAsync(TUser user, string lastName)
        {
            new Validator()
            .IsValid(user, "Invalid user")
            .IsValidText(lastName, "Invalid lastname")
            .Validate();
            user.LastName = lastName;

            var updateResponse = await _userService.UpdateUserAsync(user);

            if (!updateResponse.IsSuccessful)
            {
                return AccessResult.Failed(new AccessError("Failed to set first name", StatusCodes.Status400BadRequest));
            }

            return AccessResult.Success();
        }

        /// <summary>
        /// Asynchronously sets the last name for a specified user.
        /// </summary>
        /// <remarks>This method first validates the input parameters. If the user is not found or the
        /// operation fails, it returns an <see cref="AccessResult"/> with an appropriate error message.</remarks>
        /// <param name="userId">The unique identifier of the user whose last name is to be set. Must be a valid GUID.</param>
        /// <param name="lastName">The new last name to assign to the user. Cannot be null or empty.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="AccessResult"/>
        /// indicating the success or failure of the operation.</returns>
        public async Task<AccessResult> SetLastNameAsync(Guid userId, string lastName)
        {
            new Validator()
            .IsValidGuid(userId, "Invalid user id")
                .IsValidText(lastName, "Invalid lastname")
                .Validate();
            var response = await _userService.FetchUserByIdAsync(userId);

            if (!response.IsSuccessful)
            {
                return AccessResult.Failed(new AccessError("Invalid user", StatusCodes.Status400BadRequest));
            }

            var user = response.Data;

            return await SetLastNameAsync(user, lastName);
        }

        /// <summary>
        /// Sets the last name of the specified user.
        /// </summary>
        /// <param name="user">The user whose last name is to be set. Cannot be null.</param>
        /// <param name="lastName">The new last name to assign to the user. Must be a valid text string.</param>
        /// <returns>An <see cref="AccessResult"/> indicating the success or failure of the operation. Returns <see
        /// cref="AccessResult.Success"/> if the last name was set successfully; otherwise, returns <see
        /// cref="AccessResult.Failed"/> with an appropriate error message.</returns>
        public AccessResult SetLastName(TUser user, string lastName)
        {
            new Validator()
            .IsValid(user, "Invalid user")
            .IsValidText(lastName, "Invalid lastname")
            .Validate();

            user.LastName = lastName;

            var updateResponse = _userService.UpdateUser(user);

            if (!updateResponse.IsSuccessful)
            {
                return AccessResult.Failed(new AccessError("Failed to set first name", StatusCodes.Status400BadRequest));
            }

            return AccessResult.Success();
        }
        
        /// <summary>
        /// Updates the last name of a user identified by the specified user ID.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose last name is to be updated. Must be a valid GUID.</param>
        /// <param name="lastName">The new last name to assign to the user. Must be a non-empty string.</param>
        /// <returns>An <see cref="AccessResult"/> indicating the success or failure of the operation. Returns a failed result if
        /// the user is not found.</returns>
        public AccessResult SetLastName(Guid userId, string lastName)
        {
            new Validator()
            .IsValidGuid(userId, "Invalid user id")
                .IsValidText(lastName, "Invalid lastname")
                .Validate();

            var response = _userService.FetchUserById(userId);

            if (!response.IsSuccessful)
            {
                return AccessResult.Failed(new AccessError("Invalid user", StatusCodes.Status400BadRequest));
            }

            var user = response.Data;

            return SetLastName(user, lastName);
        }

        #endregion

        #region Delete user
        /// <summary>
        /// Asynchronously deletes a user identified by the specified user ID.
        /// </summary>
        /// <remarks>This method first validates the provided user ID and attempts to fetch the user.  If
        /// the user is found, it proceeds to delete the user asynchronously.</remarks>
        /// <param name="userId">The unique identifier of the user to be deleted. Must be a valid GUID.</param>
        /// <returns>An <see cref="AccessResult"/> indicating the success or failure of the operation.  If the user ID is invalid
        /// or the user cannot be found, the result will contain an error with a status code of 400.</returns>
        public async Task<AccessResult> DeleteUserAsync(Guid userId)
        {
            new Validator()
            .IsValidGuid(userId, "Invalid user id")
                .Validate();

            var response = await _userService.FetchUserByIdAsync(userId);

            if (!response.IsSuccessful)
            {
                return AccessResult.Failed(new AccessError("Invalid user", StatusCodes.Status400BadRequest));
            }

            var user = response.Data;

            return await DeleteUserAsync(user);
        }

        /// <summary>
        /// Asynchronously deletes the specified user from the system.
        /// </summary>
        /// <param name="user">The user to be deleted. Must not be null and must be a valid user object.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains an <see cref="AccessResult"/>
        /// indicating the success or failure of the deletion operation.</returns>
        public async Task<AccessResult> DeleteUserAsync(TUser user)
        {
            new Validator()
            .IsValid(user, "Invalid user")
            .Validate();
            return await _userService.DeleteUserAsync(user);
        }

        /// <summary>
        /// Deletes the specified user from the system.
        /// </summary>
        /// <param name="user">The user to be deleted. Must not be null.</param>
        /// <returns>An <see cref="AccessResult"/> indicating the success or failure of the operation.</returns>
        public AccessResult DeleteUser(TUser user)
        {
            new Validator()
        .IsValid(user, "Invalid user")
        .Validate();
            return _userService.DeleteUser(user);
        }

        /// <summary>
        /// Deletes a user identified by the specified user ID.
        /// </summary>
        /// <param name="userId">The unique identifier of the user to be deleted. Must be a valid GUID.</param>
        /// <returns>An <see cref="AccessResult"/> indicating the success or failure of the operation.  Returns a failed result
        /// with an error message if the user ID is invalid or the user cannot be found.</returns>
        public AccessResult DeleteUser(Guid userId)
        {
            new Validator()
            .IsValidGuid(userId, "Invalid user id")
            .Validate();

            var response = _userService.FetchUserById(userId);

            if (!response.IsSuccessful)
            {
                return AccessResult.Failed(new AccessError("Invalid user", StatusCodes.Status400BadRequest));
            }

            var user = response.Data;

            return DeleteUser(user);
        }

        #endregion 

        #region Add phone number
        /// <summary>
        /// Adds a phone number to the specified user.
        /// </summary>
        /// <remarks>This method updates the user's phone number and persists the change. If the update
        /// fails, an error result is returned.</remarks>
        /// <param name="user">The user to whom the phone number will be added. Cannot be null.</param>
        /// <param name="phoneNumber">The phone number to add. Must be a valid phone number format.</param>
        /// <returns>An <see cref="AccessResult"/> indicating the success or failure of the operation.</returns>
        public AccessResult AddPhoneNumber(TUser user, string phoneNumber)
        {
            new Validator()
                .IsValid(user, "Invalid user")
                .IsValidText(phoneNumber, "Invalid phonenumber")
                .Validate();
            user.PhoneNumber = phoneNumber;

            var result = _userService.UpdateUser(user);

            if (!result.IsSuccessful)
            {
                return AccessResult.Failed(new AccessError("Failed to add phonenumber", StatusCodes.Status400BadRequest));
            }

            return AccessResult.Success();
        }

        /// <summary>
        /// Asynchronously adds a phone number to the specified user.
        /// </summary>
        /// <remarks>This method updates the user's phone number and persists the change asynchronously. 
        /// Ensure that the user object is valid and the phone number is correctly formatted before calling this
        /// method.</remarks>
        /// <param name="user">The user to whom the phone number will be added. Cannot be null.</param>
        /// <param name="phoneNumber">The phone number to add. Must be a valid, non-empty string.</param>
        /// <returns>An <see cref="AccessResult"/> indicating the success or failure of the operation.  Returns <see
        /// cref="AccessResult.Success"/> if the phone number is successfully added;  otherwise, returns <see
        /// cref="AccessResult.Failed"/> with an appropriate error message.</returns>
        public async Task<AccessResult> AddPhoneNumberAsync(TUser user, string phoneNumber)
        {
            new Validator()
                .IsValid(user, "Invalid user")
                .IsValidText(phoneNumber, "Invalid phonenumber")
                .Validate();

            user.PhoneNumber = phoneNumber;

            var result = await _userService.UpdateUserAsync(user);

            if (!result.IsSuccessful)
            {
                return AccessResult.Failed(new AccessError("Failed to add phonenumber", StatusCodes.Status400BadRequest));
            }

            return AccessResult.Success();
        }

        /// <summary>
        /// Adds a phone number to the specified user's profile.
        /// </summary>
        /// <remarks>This method attempts to fetch the user by the provided <paramref name="userId"/>. If
        /// the user does not exist, the operation fails with a 404 status code. If the update operation fails, the
        /// method returns a failure result with a 400 status code.</remarks>
        /// <param name="userId">The unique identifier of the user to whom the phone number will be added. Must be a valid GUID.</param>
        /// <param name="phoneNumber">The phone number to add to the user's profile. Must be a valid non-empty string.</param>
        /// <returns>An <see cref="AccessResult"/> indicating the success or failure of the operation. Returns <see
        /// cref="AccessResult.Success"/> if the phone number is added successfully; otherwise, returns <see
        /// cref="AccessResult.Failed"/> with an appropriate error message and status code.</returns>
        public AccessResult AddPhoneNumber(Guid userId, string phoneNumber)
        {
            new Validator()
            .IsValidGuid(userId, "Invalid user id")
            .IsValidText(phoneNumber, "Invalid phonenumber")
            .Validate();

            var result = _userService.FetchUserById(userId);

            if (result.Data == null)
            {
                return AccessResult.Failed(new AccessError("Invalid user", StatusCodes.Status404NotFound));
            }

            var user = result.Data;

            user.PhoneNumber = phoneNumber;

            var updateResult = _userService.UpdateUser(user);

            if (!updateResult.IsSuccessful)
            {
                return AccessResult.Failed(new AccessError("Failed to add phonenumber", StatusCodes.Status400BadRequest));
            }

            return AccessResult.Success();
        }

        /// <summary>
        /// Asynchronously adds a phone number to the specified user's profile.
        /// </summary>
        /// <remarks>This method fetches the user by the provided <paramref name="userId"/> and updates
        /// their profile with the given <paramref name="phoneNumber"/>. If the user does not exist, the method returns
        /// a failed <see cref="AccessResult"/> with a 404 status code. If the update operation fails, it returns a
        /// failed <see cref="AccessResult"/> with a 400 status code.</remarks>
        /// <param name="userId">The unique identifier of the user to whom the phone number will be added. Must be a valid GUID.</param>
        /// <param name="phoneNumber">The phone number to add to the user's profile. Cannot be null or empty.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="AccessResult"/>
        /// indicating the success or failure of the operation.</returns>
        public async Task<AccessResult> AddPhoneNumberAsync(Guid userId, string phoneNumber)
        {
            new Validator()
            .IsValidGuid(userId, "Invalid user id")
            .IsValidText(phoneNumber, "Invalid phonenumber")
            .Validate();

            var result = await _userService.FetchUserByIdAsync(userId);

            if (result.Data == null)
            {
                return AccessResult.Failed(new AccessError("Invalid user", StatusCodes.Status404NotFound));
            }

            var user = result.Data;

            user.PhoneNumber = phoneNumber;

            var updateResult = await _userService.UpdateUserAsync(user);

            if (!updateResult.IsSuccessful)
            {
                return AccessResult.Failed(new AccessError("Failed to add phonenumber", StatusCodes.Status400BadRequest));
            }

            return AccessResult.Success();
        }

        #endregion

        #region Confirm phone number

        /// <summary>
        /// Confirms a user's phone number using a Google Authenticator time-based one-time password (TOTP).
        /// </summary>
        /// <remarks>This method verifies the provided TOTP against the configured authenticator key. If
        /// the verification is successful, it updates the user's phone number confirmation status.</remarks>
        /// <param name="userId">The unique identifier of the user whose phone number is being confirmed. Must be a valid GUID.</param>
        /// <param name="totp">The time-based one-time password generated by Google Authenticator. Must be a valid non-empty string.</param>
        /// <returns>An <see cref="AccessResult"/> indicating the success or failure of the phone number confirmation. Returns
        /// <see cref="AccessResult.Success"/> if the phone number is successfully confirmed; otherwise, returns <see
        /// cref="AccessResult.Failed"/> with an appropriate error message and status code.</returns>
        public AccessResult ConfirmPhoneNumberByGoogleAuthenticator(Guid userId, string totp)
        {
            new Validator()
            .IsValidGuid(userId, "Invalid user id")
                .IsValidText(totp, "Invalid time-based one time password")
                .Validate();

            var isVerified = _mfaService.VerifyGoogleAuthenticatorTotp(totp, _twoAuthenticationConfiguration.AutheticatorKey);

            if (!isVerified)
            {
                return AccessResult.Failed(new AccessError("Failed to confirm phone number", StatusCodes.Status400BadRequest));
            }

            var result = _userService.FetchUserById(userId);

            if (result.Data == null)
            {
                return AccessResult.Failed(new AccessError("Invalid user", StatusCodes.Status404NotFound));
            }

            var user = result.Data;

            user.PhoneNumberConfirmed = true;

            var updateResult = _userService.UpdateUser(user);

            if (!updateResult.IsSuccessful)
            {
                return AccessResult.Failed(new AccessError("Failed to add phonenumber", StatusCodes.Status400BadRequest));
            }

            return AccessResult.Success();
        }

        /// <summary>
        /// Confirms a user's phone number using a Google Authenticator time-based one-time password (TOTP).
        /// </summary>
        /// <remarks>This method verifies the provided TOTP against the user's Google Authenticator setup
        /// and updates the user's phone number confirmation status if successful.</remarks>
        /// <param name="userId">The unique identifier of the user whose phone number is being confirmed. Must be a valid GUID.</param>
        /// <param name="totp">The time-based one-time password generated by the user's Google Authenticator app. Must be a valid TOTP.</param>
        /// <returns>An <see cref="AccessResult"/> indicating the success or failure of the phone number confirmation. Returns a
        /// successful result if the phone number is confirmed; otherwise, returns a failed result with an appropriate
        /// error message.</returns>
        public async Task<AccessResult> ConfirmPhoneNumberByGoogleAuthenticatorAsync(Guid userId, string totp)
        {

            new Validator()
            .IsValidGuid(userId, "Invalid user id")
            .IsValidText(totp, "Invalid time-based one time password")
            .Validate();

            var isVerified = _mfaService.VerifyGoogleAuthenticatorTotp(totp, _twoAuthenticationConfiguration.AutheticatorKey);

            if (!isVerified)
            {
                return AccessResult.Failed(new AccessError("Failed to confirm phone number", StatusCodes.Status400BadRequest));
            }

            var result = await _userService.FetchUserByIdAsync(userId);

            if (result.Data == null)
            {
                return AccessResult.Failed(new AccessError("Invalid user", StatusCodes.Status404NotFound));
            }

            var user = result.Data;

            user.PhoneNumberConfirmed = true;

            var updateResult = await _userService.UpdateUserAsync(user);

            if (!updateResult.IsSuccessful)
            {
                return AccessResult.Failed(new AccessError("Failed to add phonenumber", StatusCodes.Status400BadRequest));
            }

            return AccessResult.Success();
        }

        /// <summary>
        /// Confirms the phone number of a user using a Google Authenticator time-based one-time password (TOTP).
        /// </summary>
        /// <remarks>This method verifies the provided TOTP against the user's Google Authenticator setup.
        /// If the verification is successful, the user's phone number is marked as confirmed.</remarks>
        /// <param name="user">The user whose phone number is to be confirmed. Cannot be null.</param>
        /// <param name="totp">The time-based one-time password provided by the Google Authenticator. Must be a valid TOTP string.</param>
        /// <returns>An <see cref="AccessResult"/> indicating the success or failure of the phone number confirmation. Returns
        /// <see cref="AccessResult.Success"/> if the phone number is successfully confirmed; otherwise, returns <see
        /// cref="AccessResult.Failed"/> with an appropriate error message.</returns>
        public AccessResult ConfirmPhoneNumberByGoogleAuthenticator(TUser user, string totp)
        {
            new Validator()
                .IsValid(user, "Invalid user")
                .IsValidText(totp, "Invalid time-based one time password")
                .Validate();

            var isVerified = _mfaService.VerifyGoogleAuthenticatorTotp(totp, _twoAuthenticationConfiguration.AutheticatorKey);

            if (!isVerified)
            {
                return AccessResult.Failed(new AccessError("Failed to confirm phone number", StatusCodes.Status400BadRequest));
            }

            user.PhoneNumberConfirmed = true;

            var updateResult = _userService.UpdateUser(user);

            if (!updateResult.IsSuccessful)
            {
                return AccessResult.Failed(new AccessError("Failed to add phonenumber", StatusCodes.Status400BadRequest));
            }

            return AccessResult.Success();
        }

        /// <summary>
        /// Confirms the phone number of a user using a Google Authenticator time-based one-time password (TOTP).
        /// </summary>
        /// <remarks>This method verifies the provided TOTP against the user's Google Authenticator setup.
        /// If the verification is successful, the user's phone number is marked as confirmed. The method then attempts
        /// to update the user's information in the data store. If the update fails, the method returns a failed <see
        /// cref="AccessResult"/>.</remarks>
        /// <param name="user">The user whose phone number is to be confirmed. Cannot be null.</param>
        /// <param name="totp">The time-based one-time password provided by Google Authenticator. Must be a valid TOTP.</param>
        /// <returns>An <see cref="AccessResult"/> indicating the success or failure of the phone number confirmation. Returns
        /// <see cref="AccessResult.Success"/> if the phone number is successfully confirmed; otherwise, returns <see
        /// cref="AccessResult.Failed"/> with an appropriate error message.</returns>
        public async Task<AccessResult> ConfirmPhoneNumberByGoogleAuthenticatorAsync(TUser user, string totp)
        {
            new Validator()
            .IsValid(user, "Invalid user")
            .IsValidText(totp, "Invalid time-based one time password")
            .Validate();

            var isVerified = _mfaService.VerifyGoogleAuthenticatorTotp(totp, _twoAuthenticationConfiguration.AutheticatorKey);

            if (!isVerified)
            {
                return AccessResult.Failed(new AccessError("Failed to confirm phone number", StatusCodes.Status400BadRequest));
            }

            user.PhoneNumberConfirmed = true;

            var updateResult = await _userService.UpdateUserAsync(user);

            if (!updateResult.IsSuccessful)
            {
                return AccessResult.Failed(new AccessError("Failed to add phonenumber", StatusCodes.Status400BadRequest));
            }

            return AccessResult.Success();
        }

        /// <summary>
        /// Confirms a user's phone number using a verification token.
        /// </summary>
        /// <remarks>This method validates the provided user ID and token, checks the token's claims to
        /// ensure it matches the user ID,  and updates the user's phone number confirmation status in the system.  It
        /// is important that the token is valid and corresponds to the user ID for the confirmation to
        /// succeed.</remarks>
        /// <param name="userId">The unique identifier of the user whose phone number is to be confirmed. Must be a valid GUID.</param>
        /// <param name="token">The verification token associated with the user's phone number. Cannot be null or empty.</param>
        /// <returns>An <see cref="AccessResult"/> indicating the success or failure of the operation.  Returns <see
        /// cref="AccessResult.Success"/> if the phone number is successfully confirmed;  otherwise, returns <see
        /// cref="AccessResult.Failed"/> with an appropriate error message and status code.</returns>
        public AccessResult ConfirmPhoneNumberByToken(Guid userId, string token)
        {
            new Validator()
            .IsValidGuid(userId, "Invalid user id")
            .IsValidText(token, "Invalid token")
            .Validate();

            var claims = _tokenProvider.ReadToken(token, ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(claims[ClaimTypes.NameIdentifier]?.ToString(), out var id))
            {
                return AccessResult.Failed(new AccessError("Failed to confirm phone number", StatusCodes.Status400BadRequest));
            }

            if (userId != id)
            {
                return AccessResult.Failed(new AccessError("Failed to confirm phone number", StatusCodes.Status400BadRequest));
            }

            var result = _userService.FetchUserById(userId);

            if (result.Data == null)
            {
                return AccessResult.Failed(new AccessError("Invalid user", StatusCodes.Status404NotFound));
            }

            var user = result.Data;

            user.PhoneNumberConfirmed = true;

            var updateResult = _userService.UpdateUser(user);

            if (!updateResult.IsSuccessful)
            {
                return AccessResult.Failed(new AccessError("Failed to add phonenumber", StatusCodes.Status400BadRequest));
            }

            return AccessResult.Success();
        }

        /// <summary>
        /// Confirms a user's phone number using a provided token.
        /// </summary>
        /// <remarks>This method validates the provided token against the user's identifier and updates
        /// the user's phone number confirmation status if valid.</remarks>
        /// <param name="userId">The unique identifier of the user whose phone number is to be confirmed.</param>
        /// <param name="token">The token used to confirm the phone number. Must be a valid, non-null string.</param>
        /// <returns>An <see cref="AccessResult"/> indicating the success or failure of the operation.  Returns <see
        /// cref="AccessResult.Success"/> if the phone number is successfully confirmed;  otherwise, returns <see
        /// cref="AccessResult.Failed"/> with an appropriate error message and status code.</returns>
        public async Task<AccessResult> ConfirmPhoneNumberByTokenAsync(Guid userId, string token)
        {
            new Validator()
            .IsValidGuid(userId, "Invalid user id")
            .IsValidText(token, "Invalid token")
            .Validate();

            var claims = _tokenProvider.ReadToken(token, ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(claims[ClaimTypes.NameIdentifier]?.ToString(), out var id))
            {
                return AccessResult.Failed(new AccessError("Failed to confirm phone number", StatusCodes.Status400BadRequest));
            }

            if (userId != id)
            {
                return AccessResult.Failed(new AccessError("Failed to confirm phone number", StatusCodes.Status400BadRequest));
            }

            var result = await _userService.FetchUserByIdAsync(userId);

            if (result.Data == null)
            {
                return AccessResult.Failed(new AccessError("Invalid user", StatusCodes.Status404NotFound));
            }

            var user = result.Data;

            user.PhoneNumberConfirmed = true;

            var updateResult = await _userService.UpdateUserAsync(user);

            if (!updateResult.IsSuccessful)
            {
                return AccessResult.Failed(new AccessError("Failed to add phonenumber", StatusCodes.Status400BadRequest));
            }

            return AccessResult.Success();
        }

        /// <summary>
        /// Confirms the phone number of a user using a provided token.
        /// </summary>
        /// <remarks>The method validates the provided user and token before attempting to confirm the
        /// phone number. If the token does not match the user's identifier, the confirmation will fail.</remarks>
        /// <param name="user">The user whose phone number is to be confirmed. Cannot be null.</param>
        /// <param name="token">The token used to confirm the phone number. Must be a valid, non-null string.</param>
        /// <returns>An <see cref="AccessResult"/> indicating the success or failure of the phone number confirmation. Returns
        /// <see cref="AccessResult.Success"/> if the phone number is successfully confirmed; otherwise, returns <see
        /// cref="AccessResult.Failed"/> with an appropriate error message.</returns>
        public AccessResult ConfirmPhoneNumberByToken(TUser user, string token)
        {

            new Validator()
            .IsValid(user, "Invalid user")
            .IsValidText(token, "Invalid token")
            .Validate();

            var claims = _tokenProvider.ReadToken(token, ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(claims[ClaimTypes.NameIdentifier]?.ToString(), out var id))
            {
                return AccessResult.Failed(new AccessError("Failed to confirm phone number", StatusCodes.Status400BadRequest));
            }

            if (user.Id == id)
            {
                return AccessResult.Failed(new AccessError("Failed to confirm phone number", StatusCodes.Status400BadRequest));
            }

            user.PhoneNumberConfirmed = true;

            var updateResult = _userService.UpdateUser(user);

            if (!updateResult.IsSuccessful)
            {
                return AccessResult.Failed(new AccessError("Failed to add phonenumber", StatusCodes.Status400BadRequest));
            }

            return AccessResult.Success();
        }

        /// <summary>
        /// Confirms the phone number of a user using a provided token.
        /// </summary>
        /// <remarks>This method validates the provided user and token, checks the token's claims to
        /// ensure it matches the user's identifier, and updates the user's phone number confirmation status. If the
        /// update fails, an error is returned.</remarks>
        /// <param name="user">The user whose phone number is to be confirmed. Cannot be null.</param>
        /// <param name="token">The token used to confirm the phone number. Must be a valid, non-empty string.</param>
        /// <returns>An <see cref="AccessResult"/> indicating the success or failure of the phone number confirmation. Returns
        /// <see cref="AccessResult.Success"/> if the phone number is successfully confirmed; otherwise, returns <see
        /// cref="AccessResult.Failed"/> with an appropriate error message.</returns>
        public async Task<AccessResult> ConfirmPhoneNumberByTokenAsync(TUser user, string token)
        {
            new Validator()
                .IsValid(user, "Invalid user")
                .IsValidText(token, "Invalid token")
                .Validate();

            var claims = _tokenProvider.ReadToken(token, ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(claims[ClaimTypes.NameIdentifier]?.ToString(), out var id))
            {
                return AccessResult.Failed(new AccessError("Failed to confirm phone number", StatusCodes.Status400BadRequest));
            }

            if (user.Id == id)
            {
                return AccessResult.Failed(new AccessError("Failed to confirm phone number", StatusCodes.Status400BadRequest));
            }

            user.PhoneNumberConfirmed = true;

            var updateResult = await _userService.UpdateUserAsync(user);

            if (!updateResult.IsSuccessful)
            {
                return AccessResult.Failed(new AccessError("Failed to add phonenumber", StatusCodes.Status400BadRequest));
            }

            return AccessResult.Success();
        }

        #endregion

        #region Confirm Email
        /// <summary>
        /// Confirms the email address of a user using a specified token.
        /// </summary>
        /// <remarks>This method validates the provided user and token before attempting to confirm the
        /// email. It reads the token to extract claims and checks if the token's identifier matches the user's
        /// identifier. If the identifiers do not match, the email confirmation fails.</remarks>
        /// <param name="user">The user whose email is to be confirmed. Cannot be null.</param>
        /// <param name="token">The token used to confirm the user's email. Cannot be null or empty.</param>
        /// <returns>An <see cref="AccessResult"/> indicating the success or failure of the email confirmation. If the
        /// confirmation is successful, the user's email is marked as confirmed and the user is updated.</returns>
        public AccessResult ConfirmEmailByToken(TUser user, string token)
        {
            new Validator()
                .IsValid(user, "Invalid user")
                .IsValidText(token, "Invalid token")
                .Validate();

            var claims = _tokenProvider.ReadToken(token, ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(claims[ClaimTypes.NameIdentifier]?.ToString(), out var id))
            {
                return AccessResult.Failed(new AccessError("Failed to confirm phone number", StatusCodes.Status400BadRequest));
            }

            if (user.Id == id)
            {
                return AccessResult.Failed(new AccessError("Failed to confirm phone number", StatusCodes.Status400BadRequest));
            }

            user.EmailConfirmed = true;

            return _userService.UpdateUser(user);
        }

        /// <summary>
        /// Confirms the email address of a user using a provided token.
        /// </summary>
        /// <remarks>The method validates the provided user and token before attempting to confirm the
        /// email. If the token is invalid or does not match the user's identifier, the email confirmation will
        /// fail.</remarks>
        /// <param name="user">The user whose email is to be confirmed. Cannot be null.</param>
        /// <param name="token">The token used to confirm the user's email. Must be a valid, non-null string.</param>
        /// <returns>An <see cref="AccessResult"/> indicating the success or failure of the email confirmation process.</returns>
        public async Task<AccessResult> ConfirmEmailByTokenAsync(TUser user, string token)
        {
            new Validator()
                .IsValid(user, "Invalid user")
                .IsValidText(token, "Invalid token")
                    .Validate();

            var claims = _tokenProvider.ReadToken(token, ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(claims[ClaimTypes.NameIdentifier]?.ToString(), out var id))
            {
                return AccessResult.Failed(new AccessError("Failed to confirm phone number", StatusCodes.Status400BadRequest));
            }

            if (user.Id == id)
            {
                return AccessResult.Failed(new AccessError("Failed to confirm phone number", StatusCodes.Status400BadRequest));
            }

            user.EmailConfirmed = true;

            return await _userService.UpdateUserAsync(user);
        }

        /// <summary>
        /// Confirms a user's email address using a provided token.
        /// </summary>
        /// <remarks>The method validates the provided user ID and token, checks if the token corresponds
        /// to the user ID, and updates the user's email confirmation status.</remarks>
        /// <param name="userId">The unique identifier of the user whose email is to be confirmed.</param>
        /// <param name="token">The token used to confirm the user's email address.</param>
        /// <returns>An <see cref="AccessResult"/> indicating the success or failure of the email confirmation process. Returns a
        /// successful result if the email is confirmed; otherwise, returns a failed result with an appropriate error
        /// message.</returns>
        public AccessResult ConfirmEmailByToken(Guid userId, string token)
        {
            new Validator()
            .IsValidGuid(userId, "Invalid user id")
            .IsValidText(token, "Invalid token")
            .Validate();

            var claims = _tokenProvider.ReadToken(token, ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(claims[ClaimTypes.NameIdentifier]?.ToString(), out var id))
            {
                return AccessResult.Failed(new AccessError("Failed to confirm phone number", StatusCodes.Status400BadRequest));
            }

            if (userId != id)
            {
                return AccessResult.Failed(new AccessError("Failed to confirm phone number", StatusCodes.Status400BadRequest));
            }

            var result = _userService.FetchUserById(userId);

            if (!result.IsSuccessful)
            {
                return AccessResult.Failed(new AccessError("Invalid user", StatusCodes.Status404NotFound));
            }

            result.Data.EmailConfirmed = true;

            return _userService.UpdateUser(result.Data);
        }

        /// <summary>
        /// Confirms a user's email address using a provided token.
        /// </summary>
        /// <remarks>This method validates the provided user ID and token, checks the token's claims to
        /// ensure it matches the user ID, and updates the user's email confirmation status if all checks pass. If the
        /// user ID or token is invalid, or if the token does not match the user ID, the method returns a failed <see
        /// cref="AccessResult"/> with an appropriate error message.</remarks>
        /// <param name="userId">The unique identifier of the user whose email is to be confirmed. Must be a valid GUID.</param>
        /// <param name="token">The token used to confirm the user's email. Must be a valid, non-empty string.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="AccessResult"/>
        /// indicating the success or failure of the email confirmation process.</returns>
        public async Task<AccessResult> ConfirmEmailByTokenAsync(Guid userId, string token)
        {
            new Validator()
                  .IsValidGuid(userId, "Invalid user id")
                .IsValidText(token, "Invalid token")
            .Validate();

            var claims = _tokenProvider.ReadToken(token, ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(claims[ClaimTypes.NameIdentifier]?.ToString(), out var id))
            {
                return AccessResult.Failed(new AccessError("Failed to confirm phone number", StatusCodes.Status400BadRequest));
            }

            if (userId != id)
            {
                return AccessResult.Failed(new AccessError("Failed to confirm phone number", StatusCodes.Status400BadRequest));
            }

            var result = _userService.FetchUserById(userId);

            if (!result.IsSuccessful)
            {
                return AccessResult.Failed(new AccessError("Invalid user", StatusCodes.Status404NotFound));
            }

            result.Data.EmailConfirmed = true;

            return await _userService.UpdateUserAsync(result.Data);
        }

        /// <summary>
        /// Confirms a user's email address using a provided token.
        /// </summary>
        /// <remarks>This method validates the provided email and token, checks the token's claims to
        /// ensure it matches the email, and updates the user's email confirmation status if all checks pass.</remarks>
        /// <param name="email">The email address of the user to confirm.</param>
        /// <param name="token">The token used to confirm the email address.</param>
        /// <returns>An <see cref="AccessResult"/> indicating the success or failure of the email confirmation. Returns a
        /// successful result if the email is confirmed; otherwise, returns a failed result with an appropriate error
        /// message.</returns>
        public AccessResult ConfirmEmailByToken(string email, string token)
        {
            new Validator()
            .IsValidText(email, "Invalid email")
            .IsValidText(token, "Invalid token")
            .Validate();

            var claims = _tokenProvider.ReadToken(token, ClaimTypes.NameIdentifier);

            if (!claims.TryGetValue(ClaimTypes.Email, out object userEmail))
            {
                return AccessResult.Failed(new AccessError("Failed to confirm phone number", StatusCodes.Status400BadRequest));
            }

            if (userEmail.ToString() != email)
            {
                return AccessResult.Failed(new AccessError("Failed to confirm phone number", StatusCodes.Status400BadRequest));
            }

            var result = _userService.FetchUserByEmail(email);

            if (!result.IsSuccessful)
            {
                return AccessResult.Failed(new AccessError("Invalid user", StatusCodes.Status404NotFound));
            }

            result.Data.EmailConfirmed = true;

            return _userService.UpdateUser(result.Data);
        }

        /// <summary>
        /// Confirms a user's email address using a provided token.
        /// </summary>
        /// <remarks>This method validates the provided email and token, checks the token's claims to
        /// ensure it matches the email, and updates the user's email confirmation status if all checks pass.</remarks>
        /// <param name="email">The email address of the user to confirm.</param>
        /// <param name="token">The token used to confirm the email address.</param>
        /// <returns>An <see cref="AccessResult"/> indicating the success or failure of the email confirmation. Returns a
        /// successful result if the email is confirmed; otherwise, returns a failure result with an appropriate error
        /// message.</returns>
        public async Task<AccessResult> ConfirmEmailByTokenAsync(string email, string token)
        {
            new Validator()
                  .IsValidText(email, "Invalid email")
                .IsValidText(token, "Invalid token")
            .Validate();

            var claims = _tokenProvider.ReadToken(token, ClaimTypes.NameIdentifier);

            if (!claims.TryGetValue(ClaimTypes.Email, out object userEmail))
            {
                return AccessResult.Failed(new AccessError("Failed to confirm phone number", StatusCodes.Status400BadRequest));
            }

            if (userEmail.ToString() != email)
            {
                return AccessResult.Failed(new AccessError("Failed to confirm phone number", StatusCodes.Status400BadRequest));
            }

            var result = await _userService.FetchUserByEmailAsync(email);

            if (!result.IsSuccessful)
            {
                return AccessResult.Failed(new AccessError("Invalid user", StatusCodes.Status404NotFound));
            }

            result.Data.EmailConfirmed = true;

            return await _userService.UpdateUserAsync(result.Data);
        }
        #endregion

        #region Set Google Authenticator
        /// <summary>
        /// Sets up Google Authenticator for a user identified by their email address.
        /// </summary>
        /// <remarks>This method enables two-factor authentication for the user by setting up Google
        /// Authenticator.  It updates the user's record with the authenticator key and enables two-factor
        /// authentication.</remarks>
        /// <param name="email">The email address of the user for whom the Google Authenticator is to be set up. Must be a valid email
        /// format.</param>
        /// <returns>An <see cref="AccessResult{Authenticator}"/> containing the authenticator details if successful; otherwise,
        /// an error indicating the failure reason.</returns>
        public AccessResult<Authenticator> SetGoogleAuthenticator(string email)
        {
            new Validator()
                     .IsValidText(email, "Invalid email")
                   .Validate();

            var result = _userService.FetchUserByEmail(email);
            if (result == null)
            {
                return AccessResult<Authenticator>.Failed(new AccessError("Invalid user", StatusCodes.Status404NotFound));
            }
            var authenticator = _mfaService.SetupGoogleAuthenticator(_twoAuthenticationConfiguration.Issuer, email, _twoAuthenticationConfiguration.AutheticatorKey);

            result.Data.AuthenticatorKey = authenticator.ManualKey;

            result.Data.TwoFactorAuthenticationEnabled = true;

            var updateResponse = _userService.UpdateUser(result.Data);

            if (!updateResponse.IsSuccessful)
            {
                return AccessResult<Authenticator>.Failed(new AccessError("Failed to set up google authenticator", StatusCodes.Status400BadRequest));
            }

            return AccessResult<Authenticator>.Success(authenticator);
        }

        /// <summary>
        /// Sets up Google Authenticator for a user identified by their email address.
        /// </summary>
        /// <remarks>This method enables two-factor authentication for the user by generating a Google
        /// Authenticator key. The user's two-factor authentication status is updated in the system.</remarks>
        /// <param name="email">The email address of the user for whom to set up Google Authenticator. Cannot be null or empty.</param>
        /// <returns>An <see cref="AccessResult{Authenticator}"/> containing the Google Authenticator setup details if
        /// successful; otherwise, an error result indicating the failure reason.</returns>
        public async Task<AccessResult<Authenticator>> SetGoogleAuthenticatorAsync(string email)
        {
            new Validator()
                .IsValidText(email, "Invalid email")
                .Validate();
            var result = await _userService.FetchUserByEmailAsync(email);
            if (result == null)
            {
                return AccessResult<Authenticator>.Failed(new AccessError("Invalid user", StatusCodes.Status404NotFound));
            }
            var authenticator = _mfaService.SetupGoogleAuthenticator(_twoAuthenticationConfiguration.Issuer, email, _twoAuthenticationConfiguration.AutheticatorKey);

            result.Data.AuthenticatorKey = authenticator.ManualKey;

            result.Data.TwoFactorAuthenticationEnabled = true;

            var updateResponse = await _userService.UpdateUserAsync(result.Data);

            if (!updateResponse.IsSuccessful)
            {
                return AccessResult<Authenticator>.Failed(new AccessError("Failed to set up google authenticator", StatusCodes.Status400BadRequest));
            }

            return AccessResult<Authenticator>.Success(authenticator);
        }
        /// <summary>
        /// Sets up Google Authenticator for the specified user, enabling two-factor authentication.
        /// </summary>
        /// <remarks>This method configures Google Authenticator for the user by generating an
        /// authenticator key and enabling two-factor authentication. It updates the user's information in the system
        /// and returns the authenticator details if successful. If the update fails, it returns an error
        /// result.</remarks>
        /// <param name="user">The user for whom the Google Authenticator is being set up. Cannot be null.</param>
        /// <returns>An <see cref="AccessResult{Authenticator}"/> containing the authenticator details if the setup is
        /// successful; otherwise, an error result indicating the failure.</returns>
        public AccessResult<Authenticator> SetGoogleAuthenticator(TUser user)
        {
            new Validator().IsValid(user, "Invalid user")
             .Validate();
            var authenticator = _mfaService.SetupGoogleAuthenticator(_twoAuthenticationConfiguration.Issuer, user.Email, _twoAuthenticationConfiguration.AutheticatorKey);

            user.AuthenticatorKey = authenticator.ManualKey;

            user.TwoFactorAuthenticationEnabled = true;

            var updateResponse = _userService.UpdateUser(user);

            if (!updateResponse.IsSuccessful)
            {
                return AccessResult<Authenticator>.Failed(new AccessError("Failed to set up google authenticator", StatusCodes.Status400BadRequest));
            }

            return AccessResult<Authenticator>.Success(authenticator);
        }

        /// <summary>
        /// Sets up Google Authenticator for the specified user asynchronously.
        /// </summary>
        /// <remarks>This method enables two-factor authentication for the user by generating a Google
        /// Authenticator key. The user's email and a configured issuer are used to create the authenticator.</remarks>
        /// <param name="user">The user for whom the Google Authenticator is being set up. Cannot be null.</param>
        /// <returns>An <see cref="AccessResult{Authenticator}"/> containing the authenticator details if the setup is
        /// successful; otherwise, an error result indicating the failure.</returns>
        public async Task<AccessResult<Authenticator>> SetGoogleAuthenticatorAsync(TUser user)
        {
            new Validator().IsValid(user, "Invalid user")
                .Validate();

            var authenticator = _mfaService.SetupGoogleAuthenticator(_twoAuthenticationConfiguration.Issuer, user.Email, _twoAuthenticationConfiguration.AutheticatorKey);

            user.AuthenticatorKey = authenticator.ManualKey;

            user.TwoFactorAuthenticationEnabled = true;

            var updateResponse = await _userService.UpdateUserAsync(user);

            if (!updateResponse.IsSuccessful)
            {
                return AccessResult<Authenticator>.Failed(new AccessError("Failed to set up google authenticator", StatusCodes.Status400BadRequest));
            }

            return AccessResult<Authenticator>.Success(authenticator);
        }

        #endregion

        #region Set Sms Authenticator
        /// <summary>
        /// Enables SMS-based two-factor authentication for a specified user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user for whom to enable SMS authentication. Must be a valid GUID.</param>
        /// <returns>An <see cref="AccessResult"/> indicating the success or failure of the operation. Returns a failed result if
        /// the user is not found or if the update operation is unsuccessful.</returns>
        public AccessResult SetSmsAuthenticator(Guid userId)
        {
            new Validator().IsValidGuid(userId, "Invalid user id")
               .Validate();

            var result = _userService.FetchUserById(userId);
            if (result == null)
            {
                return AccessResult.Failed(new AccessError("Invalid user", StatusCodes.Status404NotFound));
            }

            result.Data.TwoFactorAuthenticationEnabled = true;

            var response = _userService.UpdateUser(result.Data);

            if (!response.IsSuccessful)
            {
                return AccessResult<Authenticator>.Failed(new AccessError("Failed to set up google authenticator", StatusCodes.Status400BadRequest));
            }

            return AccessResult.Success();
        }

        /// <summary>
        /// Enables SMS-based two-factor authentication for the specified user.
        /// </summary>
        /// <remarks>This method updates the user's profile to enable two-factor authentication via SMS.
        /// It requires that the user exists in the system.</remarks>
        /// <param name="userId">The unique identifier of the user for whom to enable SMS authentication. Must be a valid GUID.</param>
        /// <returns>An <see cref="AccessResult"/> indicating the success or failure of the operation. Returns <see
        /// cref="AccessResult.Success"/> if the operation is successful; otherwise, returns <see
        /// cref="AccessResult.Failed"/> with an appropriate error message and status code.</returns>
        public async Task<AccessResult> SetSmsAuthenticatorAsync(Guid userId)
        {
            new Validator().IsValidGuid(userId, "Invalid user id")
               .Validate();
            var result = await _userService.FetchUserByIdAsync(userId);
            if (result == null)
            {
                return AccessResult.Failed(new AccessError("Invalid user", StatusCodes.Status404NotFound));
            }

            result.Data.TwoFactorAuthenticationEnabled = true;

            var updateResponse = await _userService.UpdateUserAsync(result.Data);

            if (!updateResponse.IsSuccessful)
            {
                return AccessResult.Failed(new AccessError("Failed to set up google authenticator", StatusCodes.Status400BadRequest));
            }
            return AccessResult.Success();
        }

        /// <summary>
        /// Enables SMS-based two-factor authentication for the specified user.
        /// </summary>
        /// <remarks>This method updates the user's profile to enable two-factor authentication and
        /// attempts to save the changes. If the update operation fails, the method returns a failed <see
        /// cref="AccessResult"/> with an appropriate error message.</remarks>
        /// <param name="user">The user for whom to enable SMS-based two-factor authentication. Cannot be null.</param>
        /// <returns>An <see cref="AccessResult"/> indicating the success or failure of the operation.</returns>
        public AccessResult SetSmsAuthenticator(TUser user)
        {
            new Validator().IsValid(user, "Invalid user")
               .Validate();
            user.TwoFactorAuthenticationEnabled = true;

            var response = _userService.UpdateUser(user);

            if (!response.IsSuccessful)
            {
                return AccessResult<Authenticator>.Failed(new AccessError("Failed to set up google authenticator", StatusCodes.Status400BadRequest));
            }

            return AccessResult.Success();
        }

        /// <summary>
        /// Enables SMS-based two-factor authentication for the specified user.
        /// </summary>
        /// <remarks>This method updates the user's account to enable SMS-based two-factor authentication.
        /// It requires the user to be valid and updates the user's information in the system. If the update operation
        /// fails, the method returns a failed access result with a relevant error message.</remarks>
        /// <param name="user">The user for whom to enable SMS-based two-factor authentication. Cannot be null.</param>
        /// <returns>An <see cref="AccessResult"/> indicating the success or failure of the operation. Returns <see
        /// cref="AccessResult.Success"/> if the operation is successful; otherwise, returns <see
        /// cref="AccessResult.Failed"/> with an appropriate error message.</returns>
        public async Task<AccessResult> SetSmsAuthenticatorAsync(TUser user)
        {
            new Validator().IsValid(user, "Invalid user")
                 .Validate();
            user.TwoFactorAuthenticationEnabled = true;

            var updateResponse = await _userService.UpdateUserAsync(user);

            if (!updateResponse.IsSuccessful)
            {
                return AccessResult.Failed(new AccessError("Failed to set up google authenticator", StatusCodes.Status400BadRequest));
            }
            return AccessResult.Success();
        }
        #endregion

        #region Remove two factor authentication
        
        /// <summary>
        /// Disables two-factor authentication for the specified user.
        /// </summary>
        /// <remarks>This method updates the user's account to disable two-factor authentication and
        /// clears the authenticator key. It returns a failed <see cref="AccessResult"/> if the update operation is
        /// unsuccessful.</remarks>
        /// <param name="user">The user for whom two-factor authentication will be disabled. Cannot be null.</param>
        /// <returns>An <see cref="AccessResult"/> indicating the success or failure of the operation.</returns>
        public AccessResult RemoveTwoFactorAuthentication(TUser user)
        {
            new Validator().IsValid(user, "Invalid user")
                  .Validate();
            user.TwoFactorAuthenticationEnabled = false;
            user.AuthenticatorKey = null;

            var response = _userService.UpdateUser(user);

            if (!response.IsSuccessful)
            {
                return AccessResult.Failed(new AccessError("Failed to set up google authenticator", StatusCodes.Status400BadRequest));
            }
            return AccessResult.Success();
        }

        /// <summary>
        /// Disables two-factor authentication for the specified user asynchronously.
        /// </summary>
        /// <remarks>This method updates the user's two-factor authentication settings by disabling it and
        /// clearing the authenticator key. It then attempts to update the user information in the underlying user
        /// service.</remarks>
        /// <param name="user">The user for whom two-factor authentication will be disabled. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="AccessResult"/>
        /// indicating the success or failure of the operation.</returns>
        public async Task<AccessResult> RemoveTwoFactorAuthenticationAsync(TUser user)
        {
            new Validator().IsValid(user, "Invalid user")
            .Validate();

            user.TwoFactorAuthenticationEnabled = false;
            user.AuthenticatorKey = null;

            var response = await _userService.UpdateUserAsync(user);

            if (!response.IsSuccessful)
            {
                return AccessResult.Failed(new AccessError("Failed to set up google authenticator", StatusCodes.Status400BadRequest));
            }
            return AccessResult.Success();
        }

        /// <summary>
        /// Disables two-factor authentication for the specified user.
        /// </summary>
        /// <remarks>This method updates the user's account to disable two-factor authentication and
        /// clears the authenticator key.</remarks>
        /// <param name="userId">The unique identifier of the user for whom two-factor authentication will be disabled. Must be a valid GUID.</param>
        /// <returns>An <see cref="AccessResult"/> indicating the success or failure of the operation. Returns <see
        /// cref="AccessResult.Success"/> if the operation is successful; otherwise, returns <see
        /// cref="AccessResult.Failed"/> with an appropriate error message and status code.</returns>
        public AccessResult RemoveTwoFactorAuthentication(Guid userId)
        {
            new Validator().IsValidGuid(userId, "Invalid user id")
                .Validate();
            var result = _userService.FetchUserById(userId);
            if (result == null)
            {
                return AccessResult.Failed(new AccessError("Invalid user", StatusCodes.Status404NotFound));
            }
            var user = result.Data;

            user.TwoFactorAuthenticationEnabled = false;
            user.AuthenticatorKey = null;

            var response = _userService.UpdateUser(user);

            if (!response.IsSuccessful)
            {
                return AccessResult.Failed(new AccessError("Failed to set up google authenticator", StatusCodes.Status400BadRequest));
            }
            return AccessResult.Success();
        }

        /// <summary>
        /// Disables two-factor authentication for the specified user.
        /// </summary>
        /// <remarks>This method fetches the user by the provided <paramref name="userId"/> and disables
        /// their two-factor authentication by setting the relevant properties to null or false. If the user is not
        /// found, the method returns a failure result with a 404 status code.</remarks>
        /// <param name="userId">The unique identifier of the user for whom two-factor authentication will be disabled. Must be a valid GUID.</param>
        /// <returns>An <see cref="AccessResult"/> indicating the success or failure of the operation. Returns <see
        /// cref="AccessResult.Success"/> if the operation is successful; otherwise, returns <see
        /// cref="AccessResult.Failed"/> with an appropriate error message.</returns>
        public async Task<AccessResult> RemoveTwoFactorAuthenticationAsync(Guid userId)
        {
            new Validator().IsValidGuid(userId, "Invalid user id")
            .Validate();
            var result = await _userService.FetchUserByIdAsync(userId);
            if (result == null)
            {
                return AccessResult.Failed(new AccessError("Invalid user", StatusCodes.Status404NotFound));
            }
            var user = result.Data;

            user.TwoFactorAuthenticationEnabled = false;
            user.AuthenticatorKey = null;

            var response = await _userService.UpdateUserAsync(user);

            if (!response.IsSuccessful)
            {
                return AccessResult.Failed(new AccessError("Failed to set up google authenticator", StatusCodes.Status400BadRequest));
            }
            return AccessResult.Success();
        }

        #endregion

        #region Reset password
        /// <summary>
        /// Resets the password for a user identified by the provided token.
        /// </summary>
        /// <remarks>This method validates the provided password and token, reads the user's email from
        /// the token, and updates the user's password in the system. Ensure that the token is valid and has not expired
        /// before calling this method.</remarks>
        /// <param name="password">The new password to set for the user. Must meet the password policy requirements.</param>
        /// <param name="token">A token that identifies the user and authorizes the password reset. Must be a valid token containing the
        /// user's email claim.</param>
        /// <returns>An <see cref="AccessResult"/> indicating the success or failure of the password reset operation. Returns
        /// <see cref="AccessResult.Success"/> if the password is successfully reset; otherwise, returns <see
        /// cref="AccessResult.Failed"/> with an appropriate error message.</returns>
        public async Task<AccessResult> ResetPasswordAsync(string password, string token)
        {
            new Validator(_accessOption)
                .IsValidPassword(password)
                .IsValidText(token, "Invalid password")
                .Validate();

            var claims = _tokenProvider.ReadToken(token, ClaimTypes.Email);

            if (!claims.TryGetValue(ClaimTypes.Email, out object claimValue))
            {
                return AccessResult.Failed(new AccessError("Failed reset password", StatusCodes.Status400BadRequest));
            }

            var value = claimValue.ToString();

            var result = await _userService.FetchUserByEmailAsync(value);

            if (!result.IsSuccessful)
            {
                return AccessResult.Failed(new AccessError("Failed reset password", StatusCodes.Status400BadRequest));
            }

            var user = result.Data;

            var hashedPassword = _passwordManager.HashPassword(password, out string salt);

            user.PasswordHash = hashedPassword;

            user.Salt = salt;

            var response = await _userService.UpdateUserAsync(user);

            if (!response.IsSuccessful)
            {
                return AccessResult.Failed(new AccessError("Failed reset password", StatusCodes.Status400BadRequest));
            }

            return AccessResult.Success();
        }

        /// <summary>
        /// Resets the password for a user identified by the provided token.
        /// </summary>
        /// <remarks>The method validates the provided password and token before attempting to reset the
        /// password. If the token is invalid or the user cannot be found, the operation will fail.</remarks>
        /// <param name="password">The new password to set for the user. Must meet the password policy requirements.</param>
        /// <param name="token">A token that identifies the user and authorizes the password reset. Must be a valid, non-null string.</param>
        /// <returns>An <see cref="AccessResult"/> indicating the success or failure of the password reset operation. Returns
        /// <see cref="AccessResult.Success"/> if the password is successfully reset; otherwise, returns <see
        /// cref="AccessResult.Failed"/> with an appropriate error message and status code.</returns>
        public AccessResult ResetPassword(string password, string token)
        {
            new Validator(_accessOption)
                .IsValidPassword(password)
             .IsValidText(token, "Invalid token")
                .Validate();
            var claims = _tokenProvider.ReadToken(token, ClaimTypes.Email);

            if (!claims.TryGetValue(ClaimTypes.Email, out object claimValue))
            {
                return AccessResult.Failed(new AccessError("Failed reset password", StatusCodes.Status400BadRequest));
            }

            var value = claimValue.ToString();

            var result = _userService.FetchUserByEmail(value);

            if (!result.IsSuccessful)
            {
                return AccessResult.Failed(new AccessError("Failed reset password", StatusCodes.Status400BadRequest));
            }

            var user = result.Data;

            var hashedPassword = _passwordManager.HashPassword(password, out string salt);

            user.PasswordHash = hashedPassword;

            user.Salt = salt;

            var response = _userService.UpdateUser(user);

            if (!response.IsSuccessful)
            {
                return AccessResult.Failed(new AccessError("Failed reset password", StatusCodes.Status400BadRequest));
            }

            return AccessResult.Success();
        }


        #endregion

        #region Generate token for reset password, email confirmation and phone number confirmation

        /// <summary>
        /// Generates a reset password token for the specified email address.
        /// </summary>
        /// <param name="email">The email address for which to generate the reset password token. Cannot be null or empty.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="AccessResult{T}"/>
        /// with the generated token as a string.</returns>
        public async Task<AccessResult<string>> GenerateResetPasswordTokenAsync(string email)
        {
            new Validator().IsValidText(email, "Invalid email")
              .Validate();
            return await GenerateTokenAsync(email);
        }

        /// <summary>
        /// Generates a reset password token for the specified user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user for whom the reset password token is generated. Must be a valid GUID.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="AccessResult{T}"/>
        /// with the reset password token as a string.</returns>
        public async Task<AccessResult<string>> GenerateResetPasswordTokenAsync(Guid userId)
        {
            new Validator().IsValidGuid(userId, "Invalid user id")
               .Validate();
            return await GenerateTokenAsync(userId);
        }

        /// <summary>
        /// Generates a reset password token for the specified email address.
        /// </summary>
        /// <param name="email">The email address for which to generate the reset password token. Must be a valid email format.</param>
        /// <returns>An <see cref="AccessResult{T}"/> containing the reset password token as a string.</returns>
        public AccessResult<string> GenerateResetPasswordToken(string email)
        {
            new Validator().IsValidText(email, "Invalid email")
              .Validate();
            return GenerateToken(email);
        }

        /// <summary>
        /// Generates a reset password token for the specified user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user for whom the reset password token is generated. Must be a valid GUID.</param>
        /// <returns>An <see cref="AccessResult{T}"/> containing the reset password token as a string.</returns>
        public AccessResult<string> GenerateResetPasswordToken(Guid userId)
        {
            new Validator().IsValidGuid(userId, "Invalid user id")
              .Validate();
            return GenerateToken(userId);
        }

        /// <summary>
        /// Generates an email confirmation token for the specified email address.
        /// </summary>
        /// <param name="email">The email address for which to generate the confirmation token. Cannot be null or empty.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="AccessResult{T}"/>
        /// object with the generated token as a string.</returns>
        public async Task<AccessResult<string>> GenerateEmailConfirmationTokenAsync(string email)
        {
            new Validator().IsValidText(email, "Invalid email")
              .Validate();
            return await GenerateTokenAsync(email);
        }

        /// <summary>
        /// Generates an email confirmation token for the specified user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user for whom the email confirmation token is generated. Must be a valid GUID.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="AccessResult{T}"/>
        /// object with the generated email confirmation token as a string.</returns>
        public async Task<AccessResult<string>> GenerateEmailConfirmationTokenAsync(Guid userId)
        {
            new Validator().IsValidGuid(userId, "Invalid user id")
                .Validate();
            return await GenerateTokenAsync(userId);
        }

        /// <summary>
        /// Generates a token for email confirmation.
        /// </summary>
        /// <param name="email">The email address for which to generate the confirmation token. Must be a valid email format.</param>
        /// <returns>An <see cref="AccessResult{T}"/> containing the generated token as a string.</returns>
        public AccessResult<string> GenerateEmailConfirmationToken(string email)
        {
            new Validator().IsValidText(email, "Invalid email")
              .Validate();
            return GenerateToken(email);
        }

        /// <summary>
        /// Generates an email confirmation token for the specified user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user for whom the email confirmation token is generated. Must be a valid GUID.</param>
        /// <returns>An <see cref="AccessResult{T}"/> containing the generated email confirmation token as a string.</returns>
        public AccessResult<string> GenerateEmailConfirmationToken(Guid userId)
        {
            new Validator().IsValidGuid(userId, "Invalid user id")
                .Validate();
            return GenerateToken(userId);
        }

        /// <summary>
        /// Generates a confirmation token for verifying a phone number associated with the specified email.
        /// </summary>
        /// <param name="email">The email address associated with the phone number to be verified. Cannot be null or empty.</param>
        /// <returns>An <see cref="AccessResult{T}"/> containing the generated confirmation token as a string.</returns>
        public AccessResult<string> GeneratePhoneNumberConfirmationToken(string email)
        {
            new Validator().IsValidText(email, "Invalid email")
                .Validate();
            return GenerateToken(email);
        }

        /// <summary>
        /// Generates a confirmation token for verifying a user's phone number.
        /// </summary>
        /// <param name="userId">The unique identifier of the user for whom the confirmation token is generated. Must be a valid GUID.</param>
        /// <returns>An <see cref="AccessResult{T}"/> containing the generated confirmation token as a string.</returns>
        public AccessResult<string> GeneratePhoneNumberConfirmationToken(Guid userId)
        {
            new Validator().IsValidGuid(userId, "Invalid user id")
               .Validate();
            return GenerateToken(userId);
        }

        /// <summary>
        /// Generates a confirmation token for verifying a user's phone number.
        /// </summary>
        /// <param name="email">The email address associated with the user's account. Cannot be null or empty.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="AccessResult{T}"/>
        /// object with the generated token as a string.</returns>
        public async Task<AccessResult<string>> GeneratePhoneNumberConfirmationTokenAsync(string email)
        {
            new Validator().IsValidText(email, "Invalid email")
               .Validate();
            return await GenerateTokenAsync(email);
        }
        /// <summary>
        /// Generates a confirmation token for verifying a user's phone number.
        /// </summary>
        /// <param name="userId">The unique identifier of the user for whom the token is generated. Must be a valid GUID.</param>
        /// <returns>An <see cref="AccessResult{T}"/> containing the generated token as a string.</returns>
        public async Task<AccessResult<string>> GeneratePhoneNumberConfirmationTokenAsync(Guid userId)
        {
            new Validator().IsValidGuid(userId, "Invalid user id")
                .Validate();
            return await GenerateTokenAsync(userId);
        }


        #endregion

        #region fetch user

        /// <summary>
        /// Asynchronously fetches a user by their email address.
        /// </summary>
        /// <param name="email">The email address of the user to fetch. Cannot be null or empty.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see
        /// cref="AccessResult{TUser}"/> object representing the user associated with the specified email.</returns>
        public async Task<AccessResult<TUser>> FetchUserByEmailAsync(string email)
        {
            new Validator().IsValidText(email, "Invalid email")
                 .Validate();
            var response = await _userService.FetchUserByEmailAsync(email);

            return response;
        }

        /// <summary>
        /// Asynchronously retrieves a user by their username.
        /// </summary>
        /// <param name="userName">The username of the user to retrieve. Cannot be null or empty.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains an <see
        /// cref="AccessResult{TUser}"/> object representing the user if found; otherwise, an appropriate error result.</returns>
        public async Task<AccessResult<TUser>> FetchUserByUserNameAsync(string userName)
        {
            new Validator().IsValidText(userName, "Invalid username")
                  .Validate();
            var response = await _userService.FetchUserByUsernameAsync(userName);

            return response;
        }

        /// <summary>
        /// Retrieves a user by their username.
        /// </summary>
        /// <param name="userName">The username of the user to retrieve. Cannot be null or empty.</param>
        /// <returns>An <see cref="AccessResult{TUser}"/> containing the user information if found; otherwise, an error result.</returns>
        public AccessResult<TUser> FetchUserByUserName(string userName)
        {
            new Validator().IsValidText(userName, "Invalid username")
                 .Validate();
            return _userService.FetchUserByUsername(userName);
        }

        /// <summary>
        /// Retrieves a user by their email address.
        /// </summary>
        /// <param name="email">The email address of the user to retrieve. Cannot be null or empty.</param>
        /// <returns>An <see cref="AccessResult{TUser}"/> containing the user information if found; otherwise, an error result.</returns>

        public AccessResult<TUser> FetchUserByEmail(string email)
        {
            new Validator().IsValidText(email, "Invalid email")
                  .Validate();
            return _userService.FetchUserByEmail(email);
        }

        /// <summary>
        /// Asynchronously retrieves a user by their unique identifier.
        /// </summary>
        /// <param name="userId">The unique identifier of the user to retrieve. Must be a valid GUID.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see
        /// cref="AccessResult{TUser}"/> object that includes the user data if found, or an appropriate access result
        /// indicating the outcome.</returns>

        public async Task<AccessResult<TUser>> FetchUserByIdAsync(Guid userId)
        {
            new Validator().IsValidGuid(userId, "Invalid user id")
            .Validate();
            var response = await _userService.FetchUserByIdAsync(userId);

            return response;
        }

        /// <summary>
        /// Retrieves a user by their unique identifier.
        /// </summary>
        /// <param name="userId">The unique identifier of the user to retrieve. Must be a valid GUID.</param>
        /// <returns>An <see cref="AccessResult{TUser}"/> containing the user information if found; otherwise, an error result.</returns>
        public AccessResult<TUser> FetchUserById(Guid userId)
        {
            new Validator().IsValidGuid(userId, "Invalid user id")
            .Validate();
            return _userService.FetchUserById(userId);
        }

        #endregion

        #region Private methods
        /// <summary>
        /// Asynchronously generates an access token for a user identified by their email address.
        /// </summary>
        /// <remarks>This method first attempts to fetch the user by their email address. If the user is
        /// found and valid, a token is generated and returned. If the user is not found or invalid, the method returns
        /// an error indicating the failure.</remarks>
        /// <param name="email">The email address of the user for whom the token is to be generated. Cannot be null or empty.</param>
        /// <returns>An <see cref="AccessResult{T}"/> containing the generated token as a string if the operation is successful;
        /// otherwise, an <see cref="AccessResult{T}"/> containing an error indicating the failure reason.</returns>
        private async Task<AccessResult<string>> GenerateTokenAsync(string email)
        {
            var result = await _userService.FetchUserByEmailAsync(email);
            if (!result.IsSuccessful)
            {
                return AccessResult<string>.Failed(new AccessError("Invalid user", StatusCodes.Status400BadRequest));
            }

            var token = GenerateToken(email, result.Data.Id.ToString());

            return AccessResult<string>.Success(token);
        }

        /// <summary>
        /// Asynchronously generates an access token for a specified user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user for whom the token is generated.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an  <see
        /// cref="AccessResult{T}"/> object with the generated token as a string if successful;  otherwise, an error
        /// indicating the failure reason.</returns>
        private async Task<AccessResult<string>> GenerateTokenAsync(Guid userId)
        {
            var result = await _userService.FetchUserByIdAsync(userId);
            if (!result.IsSuccessful)
            {
                return AccessResult<string>.Failed(new AccessError("Invalid user", StatusCodes.Status400BadRequest));
            }

            var token = GenerateToken(result.Data.Email, userId.ToString());

            return AccessResult<string>.Success(token);
        }

        /// <summary>
        /// Generates an access token for the specified user email.
        /// </summary>
        /// <remarks>This method attempts to fetch the user associated with the provided email address. If
        /// the user is found, it generates a token for that user. If the user is not found, the method returns a
        /// failure result with an error message indicating an invalid user.</remarks>
        /// <param name="email">The email address of the user for whom the token is generated. Cannot be null or empty.</param>
        /// <returns>An <see cref="AccessResult{T}"/> containing the generated token as a string if the operation is successful;
        /// otherwise, an <see cref="AccessResult{T}"/> indicating failure with an appropriate error message.</returns>
        private AccessResult<string> GenerateToken(string email)
        {
            var result = _userService.FetchUserByEmail(email);
            if (!result.IsSuccessful)
            {
                return AccessResult<string>.Failed(new AccessError("Invalid user", StatusCodes.Status400BadRequest));
            }

            var token = GenerateToken(email, result.Data.Id.ToString());

            return AccessResult<string>.Success(token);
        }

        /// <summary>
        /// Generates an access token for the specified user.
        /// </summary>
        /// <remarks>This method retrieves the user information using the provided <paramref
        /// name="userId"/> and generates a token based on the user's email and ID. If the user cannot be found or
        /// another error occurs during retrieval, the method returns a failure result with a descriptive error
        /// message.</remarks>
        /// <param name="userId">The unique identifier of the user for whom the token is generated.</param>
        /// <returns>An <see cref="AccessResult{T}"/> containing the generated token as a string if the operation is successful;
        /// otherwise, an <see cref="AccessResult{T}"/> indicating failure with an appropriate error message.</returns>
        private AccessResult<string> GenerateToken(Guid userId)
        {

            var result = _userService.FetchUserById(userId);
            if (!result.IsSuccessful)
            {
                return AccessResult<string>.Failed(new AccessError("Invalid user", StatusCodes.Status400BadRequest));
            }

            var token = GenerateToken(result.Data.Email, userId.ToString());

            return AccessResult<string>.Success(token);
        }

        /// <summary>
        /// Generates a token containing the specified email and identifier claims.
        /// </summary>
        /// <param name="email">The email address to include in the token claims. Cannot be null or empty.</param>
        /// <param name="id">The unique identifier to include in the token claims. Cannot be null or empty.</param>
        /// <returns>A string representing the generated token, valid for 30 minutes.</returns>
        private string GenerateToken(string email, string id)
        {
            var claims = new Dictionary<string, object>
            {
                {ClaimTypes.Email, email},
                {ClaimTypes.NameIdentifier, id}
            };

            var token = _tokenProvider.GenerateToken(claims, 30);

            return token;
        }

        #endregion

        private readonly IUserService<TUser> _userService;

        private readonly IPasswordManager _passwordManager;

        private readonly ITokenProvider _tokenProvider;

        private readonly IMfaService _mfaService;

        private readonly IUserClaimService _userClaimService;

        private readonly TwoAuthenticationConfiguration _twoAuthenticationConfiguration;

        private readonly AccessOption _accessOption;    
    }
}
