using AdeAuth.Models;
using AdeAuth.Services.Interfaces;
using AdeAuth.Services.Utility;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace AdeAuth.Services
{
    public class SignInManager<TUser> 
        where TUser : ApplicationUser
    {
        public SignInManager(IPasswordManager passwordManager,
            IUserService<TUser> userService,
            ITokenProvider tokenProvider,
            IMfaService mfaService, TwoAuthenticationConfiguration authenticationConfiguration)
        {
            _passwordManager = passwordManager;
            _userService = userService;
            _tokenProvider = tokenProvider;
            _mfaService = mfaService;
            _twoAuthenticationConfiguration = authenticationConfiguration;
        }


        #region Login

        public async Task<AccessResult> AuthenticateByUsernameAsync(string username, string password, Dictionary<string,object> claims)
        {
            var response = await _userService.FetchUserByUsernameAsync(username);

            if(!response.IsSuccessful)
            {
              return  AccessResult.Failed(new AccessError("Invalid username/password", StatusCodes.Status400BadRequest));
            }

            var user = response.Data;

            var isPasswordCorrect = _passwordManager.VerifyPassword(password, user.PasswordHash, user.Salt);

            if(!isPasswordCorrect)
            {
                return AccessResult.Failed(new AccessError("Invalid username/password", StatusCodes.Status400BadRequest));
            }
            // set time at the settings
            // do i want to set claims at the settings?? looking good

            var token = _tokenProvider.GenerateToken(claims,)
            return AccessResult.Success();
        }

        public AccessResult AuthenticateByUsername(string username, string password)
        {
            var response =  _userService.FetchUserByUsername(username);

            if (!response.IsSuccessful)
            {
                return AccessResult.Failed(new AccessError("Invalid username/password", StatusCodes.Status400BadRequest));
            }

            var user = response.Data;

            var isPasswordCorrect = _passwordManager.VerifyPassword(password, user.PasswordHash, user.Salt);

            if (!isPasswordCorrect)
            {
                return AccessResult.Failed(new AccessError("Invalid username/password", StatusCodes.Status400BadRequest));
            }

            return AccessResult.Success();
        }


        public async Task<AccessResult> AuthenticateByEmailAsync(string email, string password)
        {
            var response = await _userService.FetchUserByEmailAsync(email);

            if (!response.IsSuccessful)
            {
                return AccessResult.Failed(new AccessError("Invalid username/password", StatusCodes.Status400BadRequest));
            }

            var user = response.Data;

            var isPasswordCorrect = _passwordManager.VerifyPassword(password, user.PasswordHash, user.Salt);

            if (!isPasswordCorrect)
            {
                return AccessResult.Failed(new AccessError("Invalid username/password", StatusCodes.Status400BadRequest));
            }

            return AccessResult.Success();
        }

        public AccessResult AuthenticateByEmail(string email, string password)
        {
            var response = _userService.FetchUserByEmail(email);

            if (!response.IsSuccessful)
            {
                return AccessResult.Failed(new AccessError("Invalid username/password", StatusCodes.Status400BadRequest));
            }

            var user = response.Data;

            var isPasswordCorrect = _passwordManager.VerifyPassword(password, user.PasswordHash, user.Salt);

            if (!isPasswordCorrect)
            {
                return AccessResult.Failed(new AccessError("Invalid username/password", StatusCodes.Status400BadRequest));
            }

            return AccessResult.Success();
        }

        #endregion


        #region GenerateToken
        #endregion


        #region Generate otp using google authenticator
        public AccessResult<string> GenerateOtpUsingGoogleAuthenticator(int expirationTimeInMinutes)
        {
            var otp = _mfaService
                .GenerateGoogleAuthenticatorPin(_twoAuthenticationConfiguration.AutheticatorKey,
                DateTime.UtcNow.AddMinutes(expirationTimeInMinutes));

            return AccessResult<string>.Success(otp);
        }

        public AccessResult VerifyOtpUsingGoogleAuthenticator(string otp)
        {
            var isVerified = _mfaService.VerifyGoogleAuthenticatorTotp(otp, _twoAuthenticationConfiguration.AutheticatorKey);

            if (!isVerified)
            {
                return AccessResult.Failed(new AccessError("Failed to verify otp", StatusCodes.Status400BadRequest));
            }
            return AccessResult.Success();
        }

        #endregion

        private readonly IUserService<TUser> _userService;

        private readonly IPasswordManager _passwordManager;

        private readonly ITokenProvider _tokenProvider;

        private readonly IMfaService _mfaService;

        private readonly TwoAuthenticationConfiguration _twoAuthenticationConfiguration;
    }
}
