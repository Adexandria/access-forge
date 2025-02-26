using AdeAuth.Models;

namespace AdeAuth.Services.Utility
{
    public class AccessResult
    {
        public static AccessResult Failed(params AccessError[] errors)
        {
            return new AccessResult
            {
                IsSuccessful = false,
                Errors = errors
            };
        }

        public static AccessResult Success()
        {
            return new AccessResult
            {
                IsSuccessful = true
            };
        }

       public IEnumerable<AccessError> Errors { get; protected set; }
       public bool IsSuccessful { get; protected set; }
    }

    public class AccessResult<TModel> : AccessResult
    {
        public static new AccessResult<TModel> Failed(params AccessError[] errors)
        {
            return new AccessResult<TModel>
            {
                IsSuccessful = false,
                Errors = errors
            };
        }

        public static AccessResult<TModel> Failed(IEnumerable<AccessError> errors)
        {
            return new AccessResult<TModel>
            {
                IsSuccessful = false,
                Errors = errors
            };
        }


        public static AccessResult<TModel> Success(TModel response)
        {
            return new AccessResult<TModel>
            {
                IsSuccessful = true,
                Data = response
            };
        }

        public TModel Data { get; protected set; }
    }

    public class SignInResult
    {

        public SignInResult()
        {
            IsSuccessful = false;
            IsTwoFactorRequired = false;
            IsLockedOut = false;
        }

        public static SignInResult Success()
        {
            return new SignInResult
            {
                IsSuccessful = true
            };
        }

        public static SignInResult LockedOut()
        {
            return new SignInResult
            {
                IsLockedOut = true,
                IsSuccessful = true
            };
        }

        public static SignInResult RequireTwoFactor()
        {
            return new SignInResult
            {
                IsTwoFactorRequired = true,
                IsSuccessful = true
            };
        }

        public static SignInResult Failed()
        {
            return new SignInResult();
        }

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

        public bool IsSuccessful { get; protected set; }
        public bool IsLockedOut { get; protected set; }
        public bool IsTwoFactorRequired { get; protected set; }
        public LocatorDto LoginActivity {  get; protected set; }
    }


    public class SignInResult<TModel> : SignInResult
    {

        public SignInResult()
        {
            IsSuccessful = false;
            IsTwoFactorRequired = false;
            IsLockedOut = false;
        }
        public static new SignInResult<TModel> Failed()
        {
            return new SignInResult<TModel>();
        }

        public static SignInResult<TModel> Success(TModel response)
        {
            return new SignInResult<TModel>
            {
                Data = response,
                IsSuccessful = true
            };
        }


        public static new SignInResult<TModel> LockedOut()
        {
            return new SignInResult<TModel>
            {
                IsLockedOut = true,
                IsSuccessful = true
            };
        }

        public static new SignInResult<TModel> RequireTwoFactor()
        {
            return new SignInResult<TModel>
            {
                IsTwoFactorRequired = true,
                IsSuccessful = true
            };
        }

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

        public TModel Data { get; protected set; }
    }

}
