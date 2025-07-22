using AdeAuth.Models;
using System.Text.RegularExpressions;

namespace AdeAuth.Services.Utility
{
    /// <summary>
    /// Provides methods to validate password and email confirmation rules based on specified access rules.
    /// </summary>
    /// <remarks>The <see cref="ValidateRule"/> class offers a fluent interface for validating various
    /// password criteria such as the presence of special characters, numbers, and letter cases, as well as password
    /// length. It also includes validation for email confirmation requirements. The validation results are collected
    /// and can be retrieved using the <see cref="Validate"/> method.</remarks>
    /// <param name="_accessRule"></param>
    internal class ValidateRule(AccessRule _accessRule)
    {
        /// <summary>
        /// Validates whether the specified password contains a special number based on the current access rule.
        /// </summary>
        /// <remarks>This method checks if the password contains at least one special number when the
        /// access rule requires it. If the password does not meet this requirement, an appropriate validation message
        /// is generated.</remarks>
        /// <param name="password">The password to validate.</param>
        /// <returns>The current <see cref="ValidateRule"/> instance, allowing for method chaining.</returns>
        public ValidateRule CheckSpecialLetter(string password)
        {
            if (accessRule.Password.HasSpecialNumber)
            {
                ValidatePassword(password, "[0-9]",
                 "Password must have at least one special number");
            }
            return this;
        }

        /// <summary>
        /// Checks if the specified password contains at least one numeric character.
        /// </summary>
        /// <remarks>This method verifies that the password includes at least one digit. If the password
        /// does not meet this requirement, an appropriate validation message is generated.</remarks>
        /// <param name="password">The password to validate.</param>
        /// <returns>The current <see cref="ValidateRule"/> instance, allowing for method chaining.</returns>
        public ValidateRule CheckNumber(string password)
        {
            if (accessRule.Password.HasNumber)
            {
                ValidatePassword(password, "[0-9]",
                   "Password must have at least one number");
            }
            return this;
        }

        /// <summary>
        /// Validates that the specified password contains at least one lowercase letter.
        /// </summary>
        /// <remarks>This method checks the password against a regular expression to ensure it includes at
        /// least one lowercase letter. If the password does not meet this requirement, an appropriate validation
        /// message is generated.</remarks>
        /// <param name="password">The password to validate.</param>
        /// <returns>The current <see cref="ValidateRule"/> instance, allowing for method chaining.</returns>
        public ValidateRule CheckSmallLetter(string password)
        {
            if (accessRule.Password.HasSmallLetter)
            {
                ValidatePassword(password, "[a-z]",
                   "Password must have at least one small letter");
            }
            return this;
        }

        /// <summary>
        /// Validates whether the specified password contains at least one capital letter.
        /// </summary>
        /// <remarks>This method checks if the password meets the requirement of having at least one
        /// uppercase letter. If the requirement is enabled, it performs the validation and returns the current instance
        /// of <see cref="ValidateRule"/>.</remarks>
        /// <param name="password">The password to validate.</param>
        /// <returns>A <see cref="ValidateRule"/> instance representing the result of the validation.</returns>

        public ValidateRule CheckCapitalLetter(string password)
        {
            if (accessRule.Password.HasCapitalLetter)
            {
                ValidatePassword(password, "[A-Z]",
                   "Password must have at least one capital letter");
            }
            return this;
        }

        /// <summary>
        /// Validates the length of the specified password against the configured minimum and maximum length
        /// requirements.
        /// </summary>
        /// <remarks>If the password length is outside the acceptable range, an error message is added to
        /// the validation errors.</remarks>
        /// <param name="password">The password to validate.</param>
        /// <returns>The current <see cref="ValidateRule"/> instance, allowing for method chaining.</returns>
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

        /// <summary>
        /// Validates the current state and returns a list of validation errors.
        /// </summary>
        /// <returns>A list of strings containing validation error messages. The list will be empty if no errors are found.</returns>
        public List<string> Validate()
        {
            var validationErrors = errors.ToList();

            errors.Clear();

            return validationErrors;
        }

        /// <summary>
        /// Validates whether the user's email has been confirmed based on the current access rules.
        /// </summary>
        /// <remarks>If email confirmation is required by the access rules and the email is not confirmed,
        /// an error message is added to the validation errors.</remarks>
        /// <param name="isEmailConfirmed">A boolean value indicating whether the user's email is confirmed.</param>
        /// <returns>The current instance of <see cref="ValidateRule"/> to allow method chaining.</returns>
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

        /// <summary>
        /// Validates the specified password against a given pattern.
        /// </summary>
        /// <remarks>If the password does not match the specified pattern, the provided error message is
        /// added to the list of errors.</remarks>
        /// <param name="password">The password to validate.</param>
        /// <param name="pattern">The regular expression pattern to match against the password.</param>
        /// <param name="errorMessage">The error message to add to the error list if the password does not match the pattern.</param>
        private void ValidatePassword(string password, string pattern, string errorMessage)
        {
            var isMatch = Regex.IsMatch(password, pattern);

            if (!isMatch)
            {
                errors.Add(errorMessage);
            }

        }

        private readonly AccessRule accessRule = _accessRule;

        private readonly List<string> errors = [];
    }
}
