using AdeAuth.Models;
using AdeAuth.Services.Interfaces;
using AdeAuth.Services.Utility;
using Microsoft.AspNetCore.Http;

namespace AdeAuth.Services
{
    /// <summary>
    /// Manages user sign-in operations, including authentication and account status checks.
    /// </summary>
    /// <remarks>The <see cref="SignInManager{TUser}"/> class provides methods for signing in users using
    /// various credentials, handling two-factor authentication, and managing account lockout states. It integrates with
    /// services for password management, user retrieval, token generation, and multi-factor authentication.</remarks>
    /// <typeparam name="TUser">The type of user object, which must inherit from <see cref="ApplicationUser"/>.</typeparam>
    /// <param name="passwordManager">Manages password</param>
    /// <param name="userService">Manages user service</param>
    /// <param name="tokenProvider">Manages token provider</param>
    /// <param name="mfaService">Manages MFA services</param>
    /// <param name="tokenConfiguration">Manages authentication configuration</param>
    /// <param name="accessOption">Provides options for validating access rules and requirements.</param>
    /// <param name="userClaimService">Provides methods for managing user claims, including creation, update, deletion, and retrieval operations.
    /// </param>
    /// <param name="loginActivityService">Provides methods for managing login activities, including creation, update, deletion, and retrieval of login
    /// activity records.</param>
    /// <param name="locatorService">Provides methods to retrieve user location and device information.</param>
    /// <param name="authenticationConfiguration">Represents the configuration settings for two-factor authentication.</param>
    public class SignInManager<TUser>(IPasswordManager passwordManager,
        IUserService<TUser> userService,
        ITokenProvider tokenProvider,
        IMfaService mfaService,
        TokenConfiguration tokenConfiguration,
        AccessOption accessOption, IUserClaimService userClaimService,
        ILoginActivityService loginActivityService, ILocatorService locatorService,
        TwoAuthenticationConfiguration authenticationConfiguration)
        where TUser : ApplicationUser
    {

        #region Login

        /// <summary>
        /// Attempts to sign in a user using their email and password Asynchronously.
        /// </summary>
        /// <remarks>This method validates the provided email and password, checks the user's account
        /// status, and generates a login token if successful. It handles various sign-in scenarios, including account
        /// lockout and two-factor authentication requirements.</remarks>
        /// <param name="email">The email address of the user attempting to sign in. Cannot be null or empty.</param>
        /// <param name="password">The password associated with the user's email. Cannot be null or empty.</param>
        /// <returns>A <see cref="SignInResult{LoginToken}"/> indicating the result of the sign-in attempt.  The result can
        /// indicate success, failure, a locked-out account, or a requirement for two-factor authentication.</returns>
        public async Task<SignInResult<LoginToken>> SignInByEmailAsync(string email, string password)
        {
            new Validator(_accessOption)
           .IsValidText(email, "Invalid email")
            .IsValidText(password, "Invalid password")
           .Validate();

            var response = await _userService.FetchUserByEmailAsync(email);

            if (!response.IsSuccessful)
            {
                return SignInResult<LoginToken>.Failed();
            }

            var user = response.Data;

            var isPasswordCorrect = _passwordManager.VerifyPassword(password, user.PasswordHash, user.Salt);

            if (!isPasswordCorrect)
            {
                return SignInResult<LoginToken>.Failed();
            }

            _accessOption.ValidateEmailRequirement(user);

            if (user.LockoutEnabled)
            {
                return SignInResult<LoginToken>.LockedOut();
            }

            if (user.TwoFactorAuthenticationEnabled)
            {
                return SignInResult<LoginToken>.RequireTwoFactor();
            }

            var claimResponse = await _userClaimService.FetchUserClaimsAsync(user.Id);

            Dictionary<string,object> claims = claimResponse.Data?.ToDictionary(s => s.ClaimType, s => (object)s.ClaimValue);

            var loginActivity = await UpdateLoginActivityWithDeviceLocatorAsync(user.Id);

            var accessToken = _tokenProvider.GenerateToken(claims, _tokenConfiguration.ExpirationTime);

            var refreshToken = _tokenProvider.GenerateToken(10);

            var token = new LoginToken(refreshToken, accessToken , DateTime.UtcNow.AddMinutes(_tokenConfiguration.ExpirationTime));

            return SignInResult<LoginToken>.Success(token).SetLoginActivity(loginActivity);
        }

        /// <summary>
        /// Attempts to sign in a user using their email and password Asynchronously.
        /// </summary>
        /// <remarks>This method validates the provided email and password, checks the user's account
        /// status, and generates a login token if successful. It handles various sign-in scenarios, including account
        /// lockout and two-factor authentication requirements.</remarks>
        /// <param name="email">The email address of the user attempting to sign in. Cannot be null or empty.</param>
        /// <param name="password">The password associated with the user's email. Cannot be null or empty.</param>
        /// <returns>A <see cref="SignInResult{LoginToken}"/> indicating the result of the sign-in attempt.  The result can
        /// indicate success, failure, a locked-out account, or a requirement for two-factor authentication.</returns>
        public SignInResult<LoginToken> SignInByEmail(string email, string password)
        {
            new Validator(_accessOption)
           .IsValidText(email, "Invalid email")
           .IsValidText(password,"Invalid password")
           .Validate();

            var response =  _userService.FetchUserByEmail(email);

            if (!response.IsSuccessful)
            {
                return SignInResult<LoginToken>.Failed();
            }

            var user = response.Data;


            var isPasswordCorrect = _passwordManager.VerifyPassword(password, user.PasswordHash, user.Salt);

            if (!isPasswordCorrect)
            {
                return SignInResult<LoginToken>.Failed();
            }

            _accessOption.ValidateEmailRequirement(user);

            if (user.LockoutEnabled)
            {
                return SignInResult<LoginToken>.LockedOut();
            }

            if (user.TwoFactorAuthenticationEnabled)
            {
                return SignInResult<LoginToken>.RequireTwoFactor();
            }

            var claimResponse =  _userClaimService.FetchUserClaims(user.Id);

            Dictionary<string, object> claims = claimResponse.Data?.ToDictionary(s => s.ClaimType, s => (object)s.ClaimValue);

            var loginActivity =  UpdateLoginActivityWithDeviceLocator(user.Id);

            var accessToken = _tokenProvider.GenerateToken(claims, _tokenConfiguration.ExpirationTime);

            var refreshToken = _tokenProvider.GenerateToken(10);

            var token = new LoginToken(refreshToken, accessToken, DateTime.UtcNow.AddMinutes(_tokenConfiguration.ExpirationTime));

            return SignInResult<LoginToken>.Success(token).SetLoginActivity(loginActivity);
        }

        /// <summary>
        /// Attempts to sign in a user using their username and password asynchronously.
        /// </summary>
        /// <remarks>This method validates the provided username and password, checks the user's account
        /// status, and generates a login token if successful. It handles scenarios such as account lockout and
        /// two-factor authentication requirements.</remarks>
        /// <param name="username">The username of the user attempting to sign in. Cannot be null or empty.</param>
        /// <param name="password">The password associated with the username. Cannot be null or empty.</param>
        /// <returns>A <see cref="SignInResult{T}"/> containing a <see cref="LoginToken"/> if the sign-in is successful. Returns
        /// a failed result if the username or password is incorrect, the account is locked out, or two-factor
        /// authentication is required.</returns>
        public async Task<SignInResult<LoginToken>> SignInByUsernameAsync(string username, string password)
        {
            new Validator(_accessOption)
           .IsValidText(username, "Invalid username")
            .IsValidText(password, "Invalid password")
           .Validate();

            var response = await _userService.FetchUserByUsernameAsync(username);

            if (!response.IsSuccessful)
            {
                return SignInResult<LoginToken>.Failed();
            }

            var user = response.Data;

            var isPasswordCorrect = _passwordManager.VerifyPassword(password, user.PasswordHash, user.Salt);

            if (!isPasswordCorrect)
            {
                return SignInResult<LoginToken>.Failed();
            }

            _accessOption.ValidateEmailRequirement(user);

            if (user.LockoutEnabled)
            {
                return SignInResult<LoginToken>.LockedOut();
            }

            if (user.TwoFactorAuthenticationEnabled)
            {
                return SignInResult<LoginToken>.RequireTwoFactor();
            }

            var claimResponse = await _userClaimService.FetchUserClaimsAsync(user.Id);

            Dictionary<string, object> claims = claimResponse.Data?.ToDictionary(s => s.ClaimType, s => (object)s.ClaimValue);

            var loginActivity = await UpdateLoginActivityWithDeviceLocatorAsync(user.Id);

            var accessToken = _tokenProvider.GenerateToken(claims, _tokenConfiguration.ExpirationTime);

            var refreshToken = _tokenProvider.GenerateToken(10);

            var token = new LoginToken(refreshToken, accessToken, DateTime.UtcNow.AddMinutes(_tokenConfiguration.ExpirationTime));

            return SignInResult<LoginToken>.Success(token).SetLoginActivity(loginActivity);
        }

        /// <summary>
        /// Attempts to sign in a user using their username and password.
        /// </summary>
        /// <remarks>This method validates the provided username and password, checks the user's account
        /// status, and generates a login token if successful. It handles scenarios such as account lockout and
        /// two-factor authentication requirements.</remarks>
        /// <param name="username">The username of the user attempting to sign in. Cannot be null or empty.</param>
        /// <param name="password">The password associated with the username. Cannot be null or empty.</param>
        /// <returns>A <see cref="SignInResult{T}"/> containing a <see cref="LoginToken"/> if the sign-in is successful. Returns
        /// a failed result if the username or password is incorrect, the account is locked out, or two-factor
        /// authentication is required.</returns>
        public SignInResult<LoginToken> SignInByUsername(string username, string password)
        {
            new Validator(_accessOption)
           .IsValidText(username, "Invalid email")
            .IsValidText(password, "Invalid password")
           .Validate();

            var response = _userService.FetchUserByUsername(username);

            if (!response.IsSuccessful)
            {
                return SignInResult<LoginToken>.Failed();
            }

            var user = response.Data;

            var isPasswordCorrect = _passwordManager.VerifyPassword(password, user.PasswordHash, user.Salt);

            if (!isPasswordCorrect)
            {
                return SignInResult<LoginToken>.Failed();
            }


            if (user.LockoutEnabled)
            {
                return SignInResult<LoginToken>.LockedOut();
            }

            if (user.TwoFactorAuthenticationEnabled)
            {
                return SignInResult<LoginToken>.RequireTwoFactor();
            }
            var claimResponse = _userClaimService.FetchUserClaims(user.Id);

            Dictionary<string, object> claims = claimResponse.Data?.ToDictionary(s => s.ClaimType, s => (object)s.ClaimValue);

            var loginActivity =  UpdateLoginActivityWithDeviceLocator(user.Id);

            var accessToken = _tokenProvider.GenerateToken(claims, _tokenConfiguration.ExpirationTime);

            var refreshToken = _tokenProvider.GenerateToken(10);

            var token = new LoginToken(refreshToken, accessToken, DateTime.UtcNow.AddMinutes(_tokenConfiguration.ExpirationTime));

            return SignInResult<LoginToken>.Success(token).SetLoginActivity(loginActivity);
        }

        #endregion
        // manages exceptions

        #region Authenticate using two factor authenticator/sms

        /// <summary>
        /// Authenticates a user using two-factor authentication with an authenticator code asynchronously.
        /// </summary>
        /// <param name="code">TOTP cod from google authenticator</param>
        /// <returns>A <see cref="SignInResult"/> indicating the success or failure of the unlock operation.</returns>
        public SignInResult TwoFactorAuthenticatorSignInAsync(string code)
        {
            new Validator()
           .IsValidText(code, "Invalid authenticator code")
           .Validate();

            var response = _mfaService.VerifyGoogleAuthenticatorTotp(code, _twoAuthenticationConfiguration.AutheticatorKey);

            return response ? SignInResult.Success(): SignInResult.Failed();
        }

        #endregion

        #region Unlock user


       /// <summary>
       /// Unlocks a user account by disabling the lockout feature for the specified user asynchronously.
       /// </summary>
       /// <remarks>This method attempts to unlock the user account by setting the <c>LockoutEnabled</c>
       /// property to <see langword="false"/>. It first validates the user ID and then fetches the user details. If the
       /// user is found and updated successfully, the method returns a successful sign-in result; otherwise, it returns
       /// a failed result.</remarks>
       /// <param name="userId">The unique identifier of the user to unlock. Must be a valid GUID.</param>
       /// <returns>A <see cref="SignInResult"/> indicating the success or failure of the unlock operation.</returns>
        public async Task<SignInResult> UnlockUserAsync(Guid userId)
        {
            new Validator().IsValidGuid(userId, "Invalid user id")
            .Validate();

            var response = await _userService.FetchUserByIdAsync(userId);

            if(!response.IsSuccessful)
            {
                return SignInResult.Failed();
            }

            var user = response.Data;

            user.LockoutEnabled = false;

            var updateResponse =  await _userService.UpdateUserAsync(user);

            if(updateResponse.IsSuccessful)
            {
                return SignInResult.Success();
            }

            return SignInResult.Failed();
        }

        /// <summary>
        /// Unlocks a user account by disabling the lockout feature for the specified user.
        /// </summary>
        /// <remarks>This method attempts to unlock the user account by setting the <c>LockoutEnabled</c>
        /// property to <see langword="false"/>. It first validates the user ID and then fetches the user details. If the
        /// user is found and updated successfully, the method returns a successful sign-in result; otherwise, it returns
        /// a failed result.</remarks>
        /// <param name="userId">The unique identifier of the user to unlock. Must be a valid GUID.</param>
        /// <returns>A <see cref="SignInResult"/> indicating the success or failure of the unlock operation.</returns>
        public SignInResult UnlockUser(Guid userId)
        {
            new Validator().IsValidGuid(userId, "Invalid user id")
            .Validate();

            var response = _userService.FetchUserById(userId);

            if (!response.IsSuccessful)
            {
                return SignInResult.Failed();
            }

            var user = response.Data;

            user.LockoutEnabled = false;

            var updateResponse =  _userService.UpdateUser(user);

            if (updateResponse.IsSuccessful)
            {
                return SignInResult.Success();
            }

            return SignInResult.Failed();
        }

        /// <summary>
        /// Unlocks a user account by disabling the lockout feature for the specified user.
        /// </summary>
        /// <remarks>This method attempts to unlock the user account by setting the <c>LockoutEnabled</c>
        /// property to <see langword="false"/>. It first validates the user ID and then fetches the user details. If the
        /// user is found and updated successfully, the method returns a successful sign-in result; otherwise, it returns
        /// a failed result.</remarks>
        /// <param name="user">The user to unlock</param>
        /// <returns>A <see cref="SignInResult"/> indicating the success or failure of the unlock operation.</returns>
        public async Task<SignInResult> UnlockUserAsync(TUser user)
        {
            new Validator().IsValid(user, "Invalid user")
            .Validate();

            user.LockoutEnabled = false;

            var updateResponse = await _userService.UpdateUserAsync(user);

            if (updateResponse.IsSuccessful)
            {
                return SignInResult.Success();
            }

            return SignInResult.Failed();
        }

        /// <summary>
        /// Unlocks the specified user by disabling their lockout status.
        /// </summary>
        /// <remarks>This method updates the user's lockout status to allow sign-in attempts.  Ensure that
        /// the user object is valid before calling this method.</remarks>
        /// <param name="user">The user to be unlocked. Must be a valid user object.</param>
        /// <returns>A <see cref="SignInResult"/> indicating the success or failure of the unlock operation.</returns>
        public SignInResult UnlockUser(TUser user)
        {
            new Validator().IsValid(user, "Invalid user")
          .Validate();

            user.LockoutEnabled = false;

            var updateResponse =  _userService.UpdateUser(user);

            if (updateResponse.IsSuccessful)
            {
                return SignInResult.Success();
            }

            return SignInResult.Failed();
        }

        #endregion

        #region Check if two factor is enabled

        /// <summary>
        /// Determines whether two-factor authentication is enabled for the specified user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user to check for two-factor authentication status.</param>
        /// <returns>A <see cref="SignInResult"/> indicating whether two-factor authentication is required.  Returns <see
        /// cref="SignInResult.Failed"/> if the user retrieval is unsuccessful; otherwise, returns <see
        /// cref="SignInResult.RequireTwoFactor"/>.</returns>
        public async Task<SignInResult>  IsTwoFactorEnabledAsync(Guid userId)
        {
            new Validator().IsValidGuid(userId, "Invalid user id")
            .Validate();
            var response = await _userService.FetchUserByIdAsync(userId);

            if (!response.IsSuccessful)
            {
                return SignInResult.Failed();
            }

            return SignInResult.RequireTwoFactor();
        }

        /// <summary>
        /// Determines whether two-factor authentication is required for the specified user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user to check for two-factor authentication requirement.</param>
        /// <returns>A <see cref="SignInResult"/> indicating whether two-factor authentication is required.  Returns <see
        /// cref="SignInResult.Failed"/> if the user retrieval is unsuccessful; otherwise, returns <see
        /// cref="SignInResult.RequireTwoFactor"/>.</returns>
        public SignInResult IsTwoFactorEnabled(Guid userId)
        {
            new Validator().IsValidGuid(userId, "Invalid user id")
            .Validate();

            var response = _userService.FetchUserById(userId);

            if (!response.IsSuccessful)
            {
                return SignInResult.Failed();
            }

            return SignInResult.RequireTwoFactor();
        }

        #endregion

        #region Check if account is locked out

        /// <summary>
        /// Determines whether the user account associated with the specified user ID is locked out.
        /// </summary>
        /// <param name="userId">The unique identifier of the user to check for lockout status.</param>
        /// <returns>A <see cref="SignInResult"/> indicating the lockout status of the user.  Returns <see
        /// cref="SignInResult.LockedOut"/> if the account is locked out;  otherwise, returns <see
        /// cref="SignInResult.Failed"/> if the user retrieval is unsuccessful.</returns>
        public async Task<SignInResult> IsLockedOutAsync(Guid userId)
        {
            new Validator().IsValidGuid(userId, "Invalid user id")
            .Validate();

            var response = await _userService.FetchUserByIdAsync(userId);

            if (!response.IsSuccessful)
            {
                return SignInResult.Failed();
            }

            return SignInResult.LockedOut();
        }

        /// <summary>
        /// Determines whether the specified user is currently locked out.
        /// </summary>
        /// <param name="userId">The unique identifier of the user to check for lockout status. Must be a valid GUID.</param>
        /// <returns>A <see cref="SignInResult"/> indicating the lockout status of the user. Returns <see
        /// cref="SignInResult.Failed"/> if the user cannot be fetched; otherwise, returns <see
        /// cref="SignInResult.LockedOut"/>.</returns>
        public SignInResult IsLockedOut(Guid userId)
        {
            new Validator().IsValidGuid(userId, "Invalid user id")
            .Validate();

            var response = _userService.FetchUserById(userId);

            if (!response.IsSuccessful)
            {
                return SignInResult.Failed();
            }

            return SignInResult.LockedOut();
        }
        #endregion

      
        #region Generate otp using google authenticator

        /// <summary>
        /// Generates a one-time password (OTP) using Google Authenticator.
        /// </summary>
        /// <remarks>This method utilizes the Google Authenticator algorithm to generate a time-based OTP.
        /// The OTP is valid for the specified duration in minutes.</remarks>
        /// <param name="expirationTimeInMinutes">The time in minutes after which the generated OTP will expire. Must be a positive integer.</param>
        /// <returns>A <see cref="SignInResult{T}"/> containing the generated OTP as a string if successful.</returns>
        public SignInResult<string> GenerateOtpUsingGoogleAuthenticator(int expirationTimeInMinutes)
        {
            new Validator()
            .IsValidInteger(expirationTimeInMinutes, 1)
             .Validate();

            var otp = _mfaService
                .GenerateGoogleAuthenticatorPin(_twoAuthenticationConfiguration.AutheticatorKey,
                DateTime.UtcNow.AddMinutes(expirationTimeInMinutes));

            return SignInResult<string>.Success(otp);
        }

        /// <summary>
        /// Verifies a one-time password (OTP) using Google Authenticator.
        /// </summary>
        /// <param name="otp">The OTP to be verified. Must be a valid non-null string.</param>
        /// <returns>An <see cref="AccessResult"/> indicating the result of the verification. Returns <see
        /// cref="AccessResult.Success"/> if the OTP is verified successfully; otherwise, returns <see
        /// cref="AccessResult.Failed"/> with an appropriate error message.</returns>
        public AccessResult VerifyOtpUsingGoogleAuthenticator(string otp)
        {
            new Validator()
            .IsValidText(otp, "Invalid otp")
            .Validate();

            var isVerified = _mfaService.VerifyGoogleAuthenticatorTotp(otp, _twoAuthenticationConfiguration.AutheticatorKey);

            if (!isVerified)
            {
                return AccessResult.Failed(new AccessError("Failed to verify otp", StatusCodes.Status400BadRequest));
            }
            return AccessResult.Success();
        }

        #endregion

      

        /// <summary>
        /// Updates the login activity for a specified user with device and location information.
        /// </summary>
        /// <remarks>This method fetches the user's device configuration and IP address to determine the
        /// login activity.  If the login activity does not exist, it creates a new one with the user's location
        /// details.</remarks>
        /// <param name="userId">The unique identifier of the user whose login activity is to be updated.</param>
        /// <returns>A <see cref="LoginActivity"/> object representing the updated login activity.  Returns <see
        /// langword="default"/> if the update operation is unsuccessful.</returns>
        private async Task<LoginActivity> UpdateLoginActivityWithDeviceLocatorAsync(Guid userId)
        {
            // handle exceptions because a device might not exist and you are making it configurable
            var deviceConfiguration = _locatorService.FetchUserDevice();

            var ipAddress = _locatorService.FetchIpAddress();

            var loginActivityResponse = await _loginActivityService.FetchLoginActivityAsync(userId, ipAddress);

            LoginActivity loginActivity;
            if (!loginActivityResponse.IsSuccessful)
            {
                var deviceLocation = _locatorService.FetchUserLocation(ipAddress);

                loginActivity = new LoginActivity(deviceConfiguration.Device, userId)
                    .AddLocation(ipAddress, deviceLocation.City, deviceLocation.Country);
            }
            else
            {
                loginActivity = loginActivityResponse.Data.UpdateLoginActivity();
            }

            var updateLoginActivity = await _loginActivityService.UpdateLoginActivityAsync(loginActivity);

            return updateLoginActivity.IsSuccessful ? loginActivity : default;
        }


        /// <summary>
        /// Updates the login activity for a specified user by utilizing device locator services.
        /// </summary>
        /// <remarks>This method fetches the user's device configuration and IP address to determine the
        /// login activity. If no existing login activity is found, it creates a new one with the user's location
        /// details.</remarks>
        /// <param name="userId">The unique identifier of the user whose login activity is being updated.</param>
        /// <returns>A <see cref="LoginActivity"/> object representing the updated login activity if the operation is successful;
        /// otherwise, <see langword="default"/>.</returns>
        private LoginActivity UpdateLoginActivityWithDeviceLocator(Guid userId)
        {
            var deviceConfiguration = _locatorService.FetchUserDevice();

            var ipAddress = _locatorService.FetchIpAddress();

            var loginActivityResponse =  _loginActivityService.FetchLoginActivity(userId, ipAddress);

            LoginActivity loginActivity;

            if (!loginActivityResponse.IsSuccessful)
            {
                var deviceLocation = _locatorService.FetchUserLocation(ipAddress);

                loginActivity = new LoginActivity(deviceConfiguration?.Device, userId)
                    .AddLocation(ipAddress, deviceLocation?.City, deviceLocation?.Country);

                var logicActivityResponse = _loginActivityService.CreateLoginActivity(loginActivity);

                return logicActivityResponse.IsSuccessful ? loginActivity: default;
            }
            else
            {
                loginActivity = loginActivityResponse.Data.UpdateLoginActivity();

                var updateLoginActivity = _loginActivityService.UpdateLoginActivity(loginActivity);

                return updateLoginActivity.IsSuccessful ? loginActivity : default;
            }
            
        }


        private readonly IUserService<TUser> _userService = userService;

        private readonly IPasswordManager _passwordManager = passwordManager;

        private readonly ITokenProvider _tokenProvider = tokenProvider;

        private readonly IMfaService _mfaService = mfaService;

        private readonly TokenConfiguration _tokenConfiguration = tokenConfiguration;

        private readonly AccessOption _accessOption = accessOption;

        private readonly IUserClaimService _userClaimService = userClaimService;

        private readonly ILoginActivityService _loginActivityService = loginActivityService;

        private readonly ILocatorService _locatorService = locatorService;

        private readonly TwoAuthenticationConfiguration _twoAuthenticationConfiguration = authenticationConfiguration;
    }
}
