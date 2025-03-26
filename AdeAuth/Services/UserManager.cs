using AdeAuth.Models;
using AdeAuth.Services.Interfaces;
using AdeAuth.Services.Utility;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace AdeAuth.Services
{
    public class UserManager<TUser>
        where TUser : ApplicationUser
    {
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

            var claims = new List<UserClaim>()
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

            var claims = new List<UserClaim>()
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
        public async Task<AccessResult> UpdateUserAsync(TUser user)
        {
            new Validator()
               .IsValid(user, "Invalid user")
               .Validate();

            return await _userService.UpdateUserAsync(user);
        }

        public AccessResult UpdateUser(TUser user)
        {
            new Validator()
               .IsValid(user, "Invalid user")
               .Validate();

            return _userService.UpdateUser(user);
        }

        #endregion

        #region Set username
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

        public async Task<AccessResult> DeleteUserAsync(TUser user)
        {
            new Validator()
            .IsValid(user, "Invalid user")
            .Validate();
            return await _userService.DeleteUserAsync(user);
        }

        public AccessResult DeleteUser(TUser user)
        {
            new Validator()
        .IsValid(user, "Invalid user")
        .Validate();
            return _userService.DeleteUser(user);
        }

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
        public async Task<AccessResult<string>> GenerateResetPasswordTokenAsync(string email)
        {
            new Validator().IsValidText(email, "Invalid email")
              .Validate();
            return await GenerateTokenAsync(email);
        }

        public async Task<AccessResult<string>> GenerateResetPasswordTokenAsync(Guid userId)
        {
            new Validator().IsValidGuid(userId, "Invalid user id")
               .Validate();
            return await GenerateTokenAsync(userId);
        }

        public AccessResult<string> GenerateResetPasswordToken(string email)
        {
            new Validator().IsValidText(email, "Invalid email")
              .Validate();
            return GenerateToken(email);
        }

        public AccessResult<string> GenerateResetPasswordToken(Guid userId)
        {
            new Validator().IsValidGuid(userId, "Invalid user id")
              .Validate();
            return GenerateToken(userId);
        }

        public async Task<AccessResult<string>> GenerateEmailConfirmationTokenAsync(string email)
        {
            new Validator().IsValidText(email, "Invalid email")
              .Validate();
            return await GenerateTokenAsync(email);
        }

        public async Task<AccessResult<string>> GenerateEmailConfirmationTokenAsync(Guid userId)
        {
            new Validator().IsValidGuid(userId, "Invalid user id")
                .Validate();
            return await GenerateTokenAsync(userId);
        }

        public AccessResult<string> GenerateEmailConfirmationToken(string email)
        {
            new Validator().IsValidText(email, "Invalid email")
              .Validate();
            return GenerateToken(email);
        }

        public AccessResult<string> GenerateEmailConfirmationToken(Guid userId)
        {
            new Validator().IsValidGuid(userId, "Invalid user id")
                .Validate();
            return GenerateToken(userId);
        }

        public AccessResult<string> GeneratePhoneNumberConfirmationToken(string email)
        {
            new Validator().IsValidText(email, "Invalid email")
                .Validate();
            return GenerateToken(email);
        }

        public AccessResult<string> GeneratePhoneNumberConfirmationToken(Guid userId)
        {
            new Validator().IsValidGuid(userId, "Invalid user id")
               .Validate();
            return GenerateToken(userId);
        }

        public async Task<AccessResult<string>> GeneratePhoneNumberConfirmationTokenAsync(string email)
        {
            new Validator().IsValidText(email, "Invalid email")
               .Validate();
            return await GenerateTokenAsync(email);
        }

        public async Task<AccessResult<string>> GeneratePhoneNumberConfirmationTokenAsync(Guid userId)
        {
            new Validator().IsValidGuid(userId, "Invalid user id")
                .Validate();
            return await GenerateTokenAsync(userId);
        }


        #endregion

        #region fetch user

        public async Task<AccessResult<TUser>> FetchUserByEmailAsync(string email)
        {
            new Validator().IsValidText(email, "Invalid email")
                 .Validate();
            var response = await _userService.FetchUserByEmailAsync(email);

            return response;
        }

        public async Task<AccessResult<TUser>> FetchUserByUserNameAsync(string userName)
        {
            new Validator().IsValidText(userName, "Invalid username")
                  .Validate();
            var response = await _userService.FetchUserByUsernameAsync(userName);

            return response;
        }

        public AccessResult<TUser> FetchUserByUserName(string userName)
        {
            new Validator().IsValidText(userName, "Invalid username")
                 .Validate();
            return _userService.FetchUserByUsername(userName);
        }

        public AccessResult<TUser> FetchUserByEmail(string email)
        {
            new Validator().IsValidText(email, "Invalid email")
                  .Validate();
            return _userService.FetchUserByEmail(email);
        }


        public async Task<AccessResult<TUser>> FetchUserByIdAsync(Guid userId)
        {
            new Validator().IsValidGuid(userId, "Invalid user id")
            .Validate();
            var response = await _userService.FetchUserByIdAsync(userId);

            return response;
        }

        public AccessResult<TUser> FetchUserById(Guid userId)
        {
            new Validator().IsValidGuid(userId, "Invalid user id")
            .Validate();
            return _userService.FetchUserById(userId);
        }

        #endregion

        #region Private methods
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

        private string GenerateToken(string email, string id)
        {
            var claims = new Dictionary<string, object>()
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
