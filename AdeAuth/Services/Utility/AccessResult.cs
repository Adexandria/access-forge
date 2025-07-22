using AdeAuth.Models;

namespace AdeAuth.Services.Utility
{

    /// <summary>
    /// Represents the result of an access operation, indicating success or failure and providing error details if
    /// applicable.
    /// </summary>
    /// <remarks>Use the static methods <see cref="Failed"/> and <see cref="Success"/> to create instances of
    /// this class.</remarks>
    public class AccessResult
    {
        /// <summary>
        /// Creates an <see cref="AccessResult"/> indicating a failed access attempt.
        /// </summary>
        /// <param name="errors">An array of <see cref="AccessError"/> objects representing the errors encountered during the access attempt.</param>
        /// <returns>An <see cref="AccessResult"/> with <see cref="AccessResult.IsSuccessful"/> set to <see langword="false"/>
        /// and the specified errors.</returns>
        public static AccessResult Failed(params AccessError[] errors)
        {
            return new AccessResult
            {
                IsSuccessful = false,
                Errors = errors
            };
        }

        /// <summary>
        /// Creates a successful <see cref="AccessResult"/> instance.
        /// </summary>
        /// <returns>An <see cref="AccessResult"/> object with <see cref="AccessResult.IsSuccessful"/> set to <see
        /// langword="true"/>.</returns>
        public static AccessResult Success()
        {
            return new AccessResult
            {
                IsSuccessful = true
            };
        }

       /// <summary>
       /// Gets a collection of access errors encountered during the operation.
       /// </summary>
       public IEnumerable<AccessError> Errors { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether the access operation was successful.
        /// </summary>
        /// <remarks>If <see cref="IsSuccessful"/> is <see langword="false"/>, the <see cref="Errors"/> property will
        /// contain details about the errors encountered.</remarks>
        /// </summary>
        public bool IsSuccessful { get; protected set; }
    }

    /// <summary>
    /// Represents the result of an access operation, including success status, data, and errors.
    /// </summary>
    /// <remarks>This class extends <see cref="AccessResult"/> to include a data payload of type <typeparamref
    /// name="TModel"/>. It provides static methods to create instances representing either success or failure of an
    /// access operation.</remarks>
    /// <typeparam name="TModel">The type of the data returned on a successful access operation.</typeparam>
    public class AccessResult<TModel> : AccessResult
    {
        /// <summary>
        /// Creates a new <see cref="AccessResult{TModel}"/> instance representing a failed access attempt.
        /// </summary>
        /// <param name="errors">An array of <see cref="AccessError"/> objects detailing the reasons for the failure.</param>
        /// <returns>An <see cref="AccessResult{TModel}"/> with <see cref="AccessResult{TModel}.IsSuccessful"/> set to <see
        /// langword="false"/> and the specified errors.</returns>
        public static new AccessResult<TModel> Failed(params AccessError[] errors)
        {
            return new AccessResult<TModel>
            {
                IsSuccessful = false,
                Errors = errors
            };
        }

        /// <summary>
        /// Creates an <see cref="AccessResult{TModel}"/> indicating a failed access attempt.
        /// </summary>
        /// <param name="errors">A collection of <see cref="AccessError"/> objects that describe the reasons for the failure.</param>
        /// <returns>An <see cref="AccessResult{TModel}"/> with <see cref="AccessResult{TModel}.IsSuccessful"/> set to <see
        /// langword="false"/> and the specified errors.</returns>
        public static AccessResult<TModel> Failed(IEnumerable<AccessError> errors)
        {
            return new AccessResult<TModel>
            {
                IsSuccessful = false,
                Errors = errors
            };
        }

        /// <summary>
        /// Creates a successful <see cref="AccessResult{TModel}"/> instance with the specified response data.
        /// </summary>
        /// <param name="response">The data to be included in the successful result. Cannot be null.</param>
        /// <returns>An <see cref="AccessResult{TModel}"/> object with <see cref="AccessResult{TModel}.IsSuccessful"/> set to
        /// <see langword="true"/> and <see cref="AccessResult{TModel}.Data"/> containing the specified response.</returns>

        public static AccessResult<TModel> Success(TModel response)
        {
            return new AccessResult<TModel>
            {
                IsSuccessful = true,
                Data = response
            };
        }

        /// <summary>
        /// Gets the data model associated with this instance.
        /// </summary>
        public TModel Data { get; protected set; }
    }

    /// <summary>
    /// Represents the result of a sign-in attempt, indicating the outcome and any additional actions required.
    /// </summary>
    /// <remarks>This class provides static methods to create instances representing different sign-in
    /// outcomes, such as success, failure, locked out, or requiring two-factor authentication. It also allows setting
    /// additional login activity information.</remarks>
    public class SignInResult
    {
        /// <summary>
        /// A constructor that initializes a new instance of the <see cref="SignInResult"/> class with default
        /// </summary>

        public SignInResult()
        {
            IsSuccessful = false;
            IsTwoFactorRequired = false;
            IsLockedOut = false;
        }

        /// <summary>
        /// Creates a <see cref="SignInResult"/> indicating a successful sign-in operation.
        /// </summary>
        /// <returns>A <see cref="SignInResult"/> object with <see cref="SignInResult.IsSuccessful"/> set to <see
        /// langword="true"/>.</returns>
        public static SignInResult Success()
        {
            return new SignInResult
            {
                IsSuccessful = true
            };
        }

        /// <summary>
        /// Creates a <see cref="SignInResult"/> indicating that the user is locked out.
        /// </summary>
        /// <returns>A <see cref="SignInResult"/> with <see cref="SignInResult.IsLockedOut"/> set to <see langword="true"/>  and
        /// <see cref="SignInResult.IsSuccessful"/> set to <see langword="true"/>.</returns>
        public static SignInResult LockedOut()
        {
            return new SignInResult
            {
                IsLockedOut = true,
                IsSuccessful = true
            };
        }

        /// <summary>
        /// Creates a <see cref="SignInResult"/> indicating that two-factor authentication is required.
        /// </summary>
        /// <returns>A <see cref="SignInResult"/> with <see cref="SignInResult.IsTwoFactorRequired"/> set to <see
        /// langword="true"/> and <see cref="SignInResult.IsSuccessful"/> set to <see langword="true"/>.</returns>
        public static SignInResult RequireTwoFactor()
        {
            return new SignInResult
            {
                IsTwoFactorRequired = true,
                IsSuccessful = true
            };
        }

        /// <summary>
        /// Creates a <see cref="SignInResult"/> that represents a failed sign-in attempt.
        /// </summary>
        /// <returns>A <see cref="SignInResult"/> indicating that the sign-in operation was unsuccessful.</returns>
        public static SignInResult Failed()
        {
            return new SignInResult();
        }

        /// <summary>
        /// Updates the login activity details for the current session.
        /// </summary>
        /// <param name="locator">An instance of <see cref="LoginActivity"/> containing the login details to be set, such as device, city,
        /// country, IP address, and recent login time.</param>
        /// <returns>The updated <see cref="SignInResult"/> instance with the new login activity details.</returns>
        public SignInResult SetLoginActivity(LoginActivity locator)
        {
            LoginActivity = new LocatorDto()
            {
                LoginDevice = locator.Device,
                City = locator.City,
                Country = locator.Country,
                IpAddress = locator.IpAddress,
                RecentLoginTime = locator.RecentLoginTime
            };

            return this;
        }

        /// <summary>
        /// Gets a value indicating whether the operation was successful.
        /// </summary>
        public bool IsSuccessful { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether the user is currently locked out.
        /// </summary>
        public bool IsLockedOut { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether two-factor authentication is required.
        /// </summary>
        public bool IsTwoFactorRequired { get; protected set; }

        /// <summary>
        /// Manages the login activity details for the user, such as device, city, country, IP address, and recent login time.
        /// </summary>
        public LocatorDto LoginActivity {  get; protected set; }
    }


    /// <summary>
    /// Represents the result of a sign-in attempt, including the outcome and any associated data.
    /// </summary>
    /// <remarks>This class provides methods to create different types of sign-in results, such as successful
    /// sign-ins, failed attempts, locked-out states, and two-factor authentication requirements. It also allows setting
    /// additional login activity information.</remarks>
    /// <typeparam name="TModel">The type of the data associated with a successful sign-in.</typeparam>
    public class SignInResult<TModel> : SignInResult
    {

        /// <summary>
        /// A constructor that initializes a new instance of the <see cref="SignInResult{TModel}"/> class with default values.
        /// </summary>
        public SignInResult()
        {
            IsSuccessful = false;
            IsTwoFactorRequired = false;
            IsLockedOut = false;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="SignInResult{TModel}"/> class representing a failed sign-in
        /// attempt.
        /// </summary>
        /// <returns>A new <see cref="SignInResult{TModel}"/> object indicating that the sign-in operation was unsuccessful.</returns>
        public static new SignInResult<TModel> Failed()
        {
            return new SignInResult<TModel>();
        }

        /// <summary>
        /// Creates a successful sign-in result with the specified response data.
        /// </summary>
        /// <param name="response">The response data to include in the sign-in result. Cannot be null.</param>
        /// <returns>A <see cref="SignInResult{TModel}"/> indicating a successful sign-in operation, containing the specified
        /// response data.</returns>
        public static SignInResult<TModel> Success(TModel response)
        {
            return new SignInResult<TModel>
            {
                Data = response,
                IsSuccessful = true
            };
        }

        /// <summary>
        /// Creates a new <see cref="SignInResult{TModel}"/> indicating that the sign-in attempt resulted in a lockout.
        /// </summary>
        /// <remarks>This method is used to represent a scenario where a user is locked out during the
        /// sign-in process.  Despite the lockout, the operation is considered successful in terms of processing the
        /// sign-in attempt.</remarks>
        /// <returns>A <see cref="SignInResult{TModel}"/> with <see cref="SignInResult{TModel}.IsLockedOut"/> set to <see
        /// langword="true"/>  and <see cref="SignInResult{TModel}.IsSuccessful"/> set to <see langword="true"/>.</returns>
        public static new SignInResult<TModel> LockedOut()
        {
            return new SignInResult<TModel>
            {
                IsLockedOut = true,
                IsSuccessful = true
            };
        }

        /// <summary>
        /// Creates a sign-in result indicating that two-factor authentication is required.
        /// </summary>
        /// <returns>A <see cref="SignInResult{TModel}"/> object with <see cref="SignInResult{TModel}.IsTwoFactorRequired"/> set
        /// to <see langword="true"/> and <see cref="SignInResult{TModel}.IsSuccessful"/> set to <see langword="true"/>.</returns>
        public static new SignInResult<TModel> RequireTwoFactor()
        {
            return new SignInResult<TModel>
            {
                IsTwoFactorRequired = true,
                IsSuccessful = true
            };
        }

        /// <summary>
        /// Updates the login activity details for the current session.
        /// </summary>
        /// <param name="locator">An instance of <see cref="LoginActivity"/> containing the login details such as device, city, country, IP
        /// address, and recent login time.</param>
        /// <returns>The updated <see cref="SignInResult{TModel}"/> instance with the new login activity details.</returns>
        public new SignInResult<TModel> SetLoginActivity(LoginActivity locator)
        {
            LoginActivity = new LocatorDto()
            {
                LoginDevice = locator.Device,
                City = locator.City,
                Country = locator.Country,
                IpAddress = locator.IpAddress,
                RecentLoginTime = locator.RecentLoginTime
            };

            return this;
        }

        /// <summary>
        /// Gets the data model associated with this instance.
        /// </summary>
        public TModel Data { get; protected set; }
    }

}
