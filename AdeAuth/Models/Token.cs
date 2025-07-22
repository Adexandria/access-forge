using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdeAuth.Models
{
    /// <summary>
    /// Manages tokens such as access token and expiration time
    /// </summary>
    public class Token(string accessToken, DateTime dateTime)
    {
        /// <summary>
        /// Manages the access token
        /// </summary>
        public string AccessToken { get; set; } = accessToken;

        /// <summary>
        /// Gets or sets the expiration time for the current item.
        /// </summary>
        public DateTime ExpirationTime { get; set; } = dateTime;
    }
}
