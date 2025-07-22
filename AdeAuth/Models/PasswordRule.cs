using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdeAuth.Models
{
    /// <summary>
    /// Manages password rules such as minimum length, complexity, etc.
    /// </summary>
    public class PasswordRule
    {
        /// <summary>
        /// Maanges if Number is required in password
        /// </summary>
        
        public bool HasNumber { get; set; }

        ///<summary>
        /// Manages if Capital letter is required in password
        /// </summary>
        public bool HasCapitalLetter { get; set; }

        ///<summary>
        /// Manages if Small letter is required in password
        /// </summary>
        public bool HasSmallLetter { get; set; }

        /// <summary>
        /// Manages if Special character is required in password
        /// </summary>
        public bool HasSpecialNumber { get; set; }

        /// <summary>
        /// Manages the minimum length of the password
        /// </summary>
        public int MinimumPasswordLength { get; set; } = 3;

        /// <summary>
        /// Manages the maximum length of the password
        /// </summary>
        public int MaximumPasswordLength { get; set; } = 5;
    }
}
