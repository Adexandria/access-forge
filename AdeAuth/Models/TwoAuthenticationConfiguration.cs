using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdeAuth.Models
{
    /// <summary>
    /// Represents the configuration settings for two-factor authentication.
    /// </summary>
    public class TwoAuthenticationConfiguration
    {
        /// <summary>
        /// Manages authenticator key for two-factor authentication.
        /// </summary>
        public byte[] AutheticatorKey { get; set; }

        /// <summary>
        /// Manages the issuer of the two-factor authentication.
        /// </summary>
        public string Issuer { get; set; }
    }
}
