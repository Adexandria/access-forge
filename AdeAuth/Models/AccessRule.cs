using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdeAuth.Models
{
    /// <summary>
    /// Manages access rules for password policy and email confirmation
    /// </summary> 
    public class AccessRule
    {
        /// <summary>
        /// Manages if email confirmation is required
        /// </summary>
        public bool IsRequireEmailConfirmation { get; set; }

        /// <summary>
        /// Manages password rules such as minimum length, complexity, etc.
        /// </summary>
        public PasswordRule Password {  get; set; } = new PasswordRule();
    }
}
