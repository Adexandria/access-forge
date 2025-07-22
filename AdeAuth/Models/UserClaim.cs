using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdeAuth.Models
{
    /// <summary>
    /// Manages claims for a user
    /// </summary>
    public class UserClaim
    {
        /// <summary>
        /// A constructor
        /// </summary>
        public UserClaim()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Manages Id of the user claim
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Manages claim type
        /// </summary>
        public string ClaimType { get; set; }

        /// <summary>
        /// Manages claim value
        /// </summary>
        public string ClaimValue { get; set;}

        /// <summary>
        /// Manages the user id to which this claim belongs.
        /// </summary>
        public Guid UserId { get; set; }
    }
}
