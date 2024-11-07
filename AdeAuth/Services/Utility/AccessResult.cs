

using Microsoft.AspNetCore.Mvc;

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
        where TModel : class 
    {
        public static new AccessResult<TModel> Failed(params AccessError[] errors)
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
}
