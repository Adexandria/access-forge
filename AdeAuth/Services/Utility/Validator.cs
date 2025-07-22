using AdeAuth.Models;
using AdeAuth.Services.Extensions;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;

namespace AdeAuth.Services.Utility
{
    /// <summary>
    /// Provides methods to validate various types of input data, such as text, passwords, emails, GUIDs, and integers.
    /// </summary>
    /// <remarks>The <see cref="Validator"/> class allows chaining of validation methods to accumulate errors,
    /// which can be checked by calling the <see cref="Validate"/> method. If any validation errors are present, a <see
    /// cref="ValidationException"/> is thrown. This class supports both default and custom validation logic through the
    /// use of an <see cref="AccessOption"/>.</remarks>
    public class Validator
    {
        /// <summary>
        /// A constructor that initializes the validator with a specific access option.
        /// </summary>
        /// <param name="_accessOption">Provides options for validating access rules and requirements.</param>
        public Validator(AccessOption _accessOption)
        {
            errors = [];
            accessOption = _accessOption;
        }

        /// <summary>
        /// A constructor that initializes the validator with default access options.
        /// </summary>
        public Validator()
        {
            errors = [];
        }

        /// <summary>
        /// Validates whether the specified text is not empty and adds an error message if it is.
        /// </summary>
        /// <param name="text">The text to validate. Cannot be null.</param>
        /// <param name="errorMessage">The error message to add if the text is empty.</param>
        /// <returns>The current <see cref="Validator"/> instance, allowing for method chaining.</returns>
        public Validator IsValidText(string text, string errorMessage)
        {
            if (text.IsEmpty())
            {
                errors.Add(errorMessage);
            }

            return this;
        }

        /// <summary>
        /// Validates the specified password according to predefined rules.
        /// </summary>
        /// <remarks>This method checks if the password meets certain criteria and collects any validation
        /// errors. The password must not be empty, and additional validation rules are applied through the
        /// <c>accessOption</c> object.</remarks>
        /// <param name="password">The password to validate. Cannot be null or empty.</param>
        /// <returns>A <see cref="Validator"/> instance containing any validation errors.</returns>
        public Validator IsValidPassword(string password)
        {
            if(password.IsEmpty())
            {
                errors.Add("Password cannot be empty");
            }
            errors.AddRange(accessOption.Validate(password));

            return this;
        }

        /// <summary>
        /// Validates whether the specified email address is in a valid format and adds an error message if it is not.
        /// </summary>
        /// <param name="email">Email to validate</param>
        /// <param name="errorMessage">The error message to display iof it fails</param>
        /// <returns>A <see cref="Validator"/> instance containing any validation errors.</returns>

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

        /// <summary>
        /// Validates whether the specified <see cref="Guid"/> is not empty and adds an error message if it is.
        /// </summary>
        /// <param name="id">The <see cref="Guid"/> to validate. Must not be empty.</param>
        /// <param name="errorMessage">The error message to add if the <paramref name="id"/> is empty.</param>
        /// <returns>The current <see cref="Validator"/> instance, allowing for method chaining.</returns>
        public Validator IsValidGuid(Guid id, string errorMessage)
        {
            if (id.IsEmpty())
            {
                errors.Add(errorMessage);
            }
            return this;
        }

        /// <summary>
        /// Validates the specified user and adds an error message if the user is null.
        /// </summary>
        /// <typeparam name="TUser">The type of the user, which must inherit from <see cref="ApplicationUser"/>.</typeparam>
        /// <param name="user">The user to validate. If null, an error message is added.</param>
        /// <param name="errorMessage">The error message to add if the user is null.</param>
        /// <returns>The current <see cref="Validator"/> instance, allowing for method chaining.</returns>
        public Validator IsValid<TUser>(TUser user, string errorMessage)
            where TUser: ApplicationUser
        {
            if(user == null)
            {
                errors.Add(errorMessage);
            }

            return this;
        }

        /// <summary>
        /// Determines whether the specified integer value is valid based on a minimum threshold.
        /// </summary>
        /// <remarks>If the <paramref name="value"/> is less than <paramref name="minimumValue"/>, an
        /// error message is added to the internal error list.</remarks>
        /// <param name="value">The integer value to validate.</param>
        /// <param name="minimumValue">The minimum allowable value. Defaults to 0.</param>
        /// <returns>The current <see cref="Validator"/> instance, allowing for method chaining.</returns>
        public Validator IsValidInteger(int value, int minimumValue = 0)
        {
            if(value < minimumValue)
            {
                errors.Add($"Value can not be lower than {minimumValue}");
            }

            return this;
        }

        /// <summary>
        /// Validates the current state and throws an exception if any validation errors are present.
        /// </summary>
        /// <remarks>This method checks for validation errors accumulated in the current context. If any
        /// errors are found, a <see cref="ValidationException"/> is thrown containing all error messages.</remarks>
        /// <exception cref="ValidationException">Thrown if one or more validation errors are present.</exception>
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
