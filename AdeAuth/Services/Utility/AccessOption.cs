using AdeAuth.Models;
using System.ComponentModel.DataAnnotations;

namespace AdeAuth.Services.Utility
{
    /// <summary>
    /// Provides options for validating access rules and requirements.
    /// </summary>
    /// <remarks>The <see cref="AccessOption"/> class offers methods to validate passwords and email
    /// confirmation requirements based on specified access rules. It utilizes a set of validation rules to ensure
    /// compliance with security standards.</remarks>
    /// <param name="accessRule"></param>
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

        /// <summary>
        /// Validates whether the specified user's email confirmation requirement is met.
        /// </summary>
        /// <typeparam name="TUser">The type of the user, which must inherit from <see cref="ApplicationUser"/>.</typeparam>
        /// <param name="user">The user whose email confirmation status is to be validated. Cannot be null.</param>
        /// <exception cref="ValidationException">Thrown if the user's email confirmation requirement is not met.</exception>
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
