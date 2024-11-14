using AdeAuth.Models;
using AdeAuth.Services.Interfaces;
using AdeAuth.Services.Utility;
using Microsoft.AspNetCore.Http;

namespace AdeAuth.Services
{
    public class SignInManager<TUser> 
        where TUser : ApplicationUser
    {
        public SignInManager(IPasswordManager passwordManager,
            IUserService<TUser> userService,
            ITokenProvider tokenProvider,
            IMfaService mfaService,
            TokenConfiguration tokenConfiguration,
            AccessOption accessOption,IUserClaimService userClaimService,
            ILoginActivityService loginActivityService,ILocatorService locatorService,
            TwoAuthenticationConfiguration authenticationConfiguration)
        {
            _passwordManager = passwordManager;
            _userService = userService;
            _tokenProvider = tokenProvider;
            _mfaService = mfaService;
            _twoAuthenticationConfiguration = authenticationConfiguration;
            _tokenConfiguration = tokenConfiguration;
            _accessOption = accessOption;
            _userClaimService = userClaimService;
            _loginActivityService = loginActivityService;
            _locatorService = locatorService;
        }


        #region Login
        public async Task<SignInResult<LoginToken>> SignInByEmailAsync(string email, string password)
        {
            new Validator(_accessOption)
           .IsValidText(email, "Invalid email")
           .IsValidPassword(password)
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

            var loginActivity = await UpdateLoginActivityAsync(user.Id);

            var accessToken = _tokenProvider.GenerateToken(claims, _tokenConfiguration.ExpirationTime);

            var refreshToken = _tokenProvider.GenerateToken(10);

            var token = new LoginToken(refreshToken, accessToken , DateTime.UtcNow.AddMinutes(_tokenConfiguration.ExpirationTime));

            return SignInResult<LoginToken>.Success(token).SetLoginActivity(loginActivity);
        }

        public SignInResult<LoginToken> SignInByEmail(string email, string password)
        {
            new Validator(_accessOption)
           .IsValidText(email, "Invalid email")
           .IsValidPassword(password)
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

            var loginActivity =  UpdateLoginActivity(user.Id);

            var accessToken = _tokenProvider.GenerateToken(claims, _tokenConfiguration.ExpirationTime);

            var refreshToken = _tokenProvider.GenerateToken(10);

            var token = new LoginToken(refreshToken, accessToken, DateTime.UtcNow.AddMinutes(_tokenConfiguration.ExpirationTime));

            return SignInResult<LoginToken>.Success(token).SetLoginActivity(loginActivity);
        }

        public async Task<SignInResult<LoginToken>> SignInByUsernameAsync(string username, string password)
        {
            new Validator(_accessOption)
           .IsValidText(username, "Invalid username")
           .IsValidPassword(password)
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

            var loginActivity = await UpdateLoginActivityAsync(user.Id);

            var accessToken = _tokenProvider.GenerateToken(claims, _tokenConfiguration.ExpirationTime);

            var refreshToken = _tokenProvider.GenerateToken(10);

            var token = new LoginToken(refreshToken, accessToken, DateTime.UtcNow.AddMinutes(_tokenConfiguration.ExpirationTime));

            return SignInResult<LoginToken>.Success(token).SetLoginActivity(loginActivity);
        }

        public SignInResult<LoginToken> SignInByUsername(string username, string password)
        {
            new Validator(_accessOption)
           .IsValidText(username, "Invalid email")
           .IsValidPassword(password)
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

            var loginActivity =  UpdateLoginActivity(user.Id);

            var accessToken = _tokenProvider.GenerateToken(claims, _tokenConfiguration.ExpirationTime);

            var refreshToken = _tokenProvider.GenerateToken(10);

            var token = new LoginToken(refreshToken, accessToken, DateTime.UtcNow.AddMinutes(_tokenConfiguration.ExpirationTime));

            return SignInResult<LoginToken>.Success(token).SetLoginActivity(loginActivity);
        }

        #endregion
        // manages exceptions

        #region Authenticate using two factor authenticator/sms

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
        // store update external authetication
        #region Generate otp using google authenticator
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

        // sign out to delete all activities including refresh token and login activities

        private async Task<LoginActivity> UpdateLoginActivityAsync(Guid userId)
        {
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

        private LoginActivity UpdateLoginActivity(Guid userId)
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

                return logicActivityResponse.IsSuccessful ? loginActivity: default  ;
            }
            else
            {
                loginActivity = loginActivityResponse.Data.UpdateLoginActivity();

                var updateLoginActivity = _loginActivityService.UpdateLoginActivity(loginActivity);

                return updateLoginActivity.IsSuccessful ? loginActivity : default;
            }
            
        }


        private readonly IUserService<TUser> _userService;

        private readonly IPasswordManager _passwordManager;

        private readonly ITokenProvider _tokenProvider;

        private readonly IMfaService _mfaService;

        private readonly TokenConfiguration _tokenConfiguration;
        private readonly AccessOption _accessOption;
        private readonly IUserClaimService _userClaimService;
        private readonly ILoginActivityService _loginActivityService;
        private readonly ILocatorService _locatorService;
        private readonly TwoAuthenticationConfiguration _twoAuthenticationConfiguration;
    }
}
