using AdeAuth.Models;
using AdeAuth.Services.Extensions;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;

namespace AdeAuth.Services.Utility
{
    public class Validator
    {
        public Validator(AccessOption _accessOption)
        {
            errors = new List<string>();
            accessOption = _accessOption;
        }

        public Validator()
        {
            errors = new List<string>();
        }


        public Validator IsValidText(string text, string errorMessage)
        {
            if (text.IsEmpty())
            {
                errors.Add(errorMessage);
            }

            return this;
        }
        public Validator IsValidPassword(string password)
        {
            if(password.IsEmpty())
            {
                errors.Add("Password cannot be empty");
            }
            errors.AddRange(accessOption.Validate(password));

            return this;
        }

        public Validator IsValidEmail(string email, string errorMessage)
        {
            if(string.IsNullOrEmpty(email))
            {
                errors.Add(errorMessage);
                return this;
            }

            var emailAddress = new MailAddress(email);
            if(emailAddress.Address != email)
            {
                errors.Add(errorMessage);
            }

            return this;
        }

        public Validator IsValidGuid(Guid id, string errorMessage)
        {
            if (id.IsEmpty())
            {
                errors.Add(errorMessage);
            }
            return this;
        }


        public Validator IsValid<TUser>(TUser user, string errorMessage)
            where TUser: ApplicationUser
        {
            if(user == null)
            {
                errors.Add(errorMessage);
            }

            return this;
        }

        public Validator IsValidInteger(int value, int minimumValue = 0)
        {
            if(value < minimumValue)
            {
                errors.Add($"Value can not be lower than {minimumValue}");
            }

            return this;
        }

        public void Validate()
        {
            if(errors.Count > 0)
            {
                throw new ValidationException(string.Join("\n", errors));
            }
           
        }

        private readonly List<string> errors;
        private readonly AccessOption accessOption;
    }
}
