using AdeAuth.Models;
using System.ComponentModel.DataAnnotations;

namespace AdeAuth.Services.Utility
{
    public class AccessOption(AccessRule accessRule = null)
    {
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


        public void ValidateEmailRequirement<TUser>(TUser user)
            where TUser: ApplicationUser
        {
           var errors = _validate.ValidateEmailConfirmation(user.EmailConfirmed)
                .Validate();

           if(errors.Any())
           {
                throw new ValidationException(string.Join("\n", errors));
           }
        }

        private readonly ValidateRule _validate = accessRule == null
                ? new ValidateRule(new AccessRule()) : new ValidateRule(accessRule);
    }
}
