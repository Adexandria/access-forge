using AdeAuth.Models;
using AdeAuth.Services.Exceptions;

namespace AdeAuth.Services.Utility
{
    public class AccessOption
    {
        public AccessOption(AccessRule accessRule = null)
        { 
            _validate =  accessRule == null 
                ? new ValidateRule(new AccessRule()) : new ValidateRule(accessRule);
        }

        public List<string> Validate(string password)
        {
          var errors = _validate.CheckNumber(password)
                .CheckCapitalLetter(password)
               .CheckSpecialLetter(password)
               .CheckPasswordLength(password)
               .CheckSmallLetter(password)
               .Validate();

            return errors;
        }


        public List<string> ValidateEmailRequirement<TUser>(TUser user)
            where TUser: ApplicationUser
        {
           return _validate.ValidateEmailConfirmation(user.EmailConfirmed)
                .Validate();
        }

        private readonly ValidateRule _validate;
    }
}
