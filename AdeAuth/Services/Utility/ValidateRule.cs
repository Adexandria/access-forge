using AdeAuth.Models;
using System.Text.RegularExpressions;

namespace AdeAuth.Services.Utility
{
    internal class ValidateRule
    {
        public ValidateRule(AccessRule _accessRule)
        {
            accessRule = _accessRule;
            errors = new List<string>();
        }

        public ValidateRule CheckSpecialLetter(string password)
        {
            if (accessRule.Password.HasSpecialNumber)
            {
                ValidatePassword(password, "[0-9]",
                 "Password must have at least one number");
            }
            return this;
        }

        public ValidateRule CheckNumber(string password)
        {
            if (accessRule.Password.HasNumber)
            {
                ValidatePassword(password, "[0-9]",
                   "Password must have at least one number");
            }
            return this;
        }

        public ValidateRule CheckSmallLetter(string password)
        {
            if (accessRule.Password.HasSmallLetter)
            {
                ValidatePassword(password, "[a-z]",
                   "Password must have at least one small letter");
            }
            return this;
        }

        public ValidateRule CheckCapitalLetter(string password)
        {
            if (accessRule.Password.HasCapitalLetter)
            {
                ValidatePassword(password, "[A-Z]",
                   "Password must have at least one capital letter");
            }
            return this;
        }

        public ValidateRule CheckPasswordLength(string password)
        {
            if (accessRule.Password.MinimumPasswordLength > password.Length
                || accessRule.Password.MaximumPasswordLength < password.Length)
            {
                errors.Add($"Invalid password length, " +
                    $"Password length from {accessRule.Password.MinimumPasswordLength} - {accessRule.Password.MaximumPasswordLength} is acceptable");
            }
            return this;
        }

        public List<string> Validate()
        {
            var validationErrors = errors.ToList();

            errors.Clear();

            return validationErrors;
        }

        public ValidateRule  ValidateEmailConfirmation(bool isEmailConfirmed)
        {
            if (accessRule.IsRequireEmailConfirmation)
            {
                if (!isEmailConfirmed)
                {
                    errors.Add("User's email confirmation is required");
                }
            }
            return this;
        }

        private void ValidatePassword(string password, string pattern, string errorMessage)
        {
            var isMatch = Regex.IsMatch(password, pattern);

            if (isMatch)
            {
                errors.Add(errorMessage);
            }

        }

        private readonly AccessRule accessRule;

        private readonly List<string> errors;
    }
}
