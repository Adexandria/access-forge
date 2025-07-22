using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdeAuth.Models
{
    /// <summary>
    /// Manages login tokens such as refresh token and access token
    /// </summary>
    public class LoginToken(string refreshToken, string accessToken, DateTime dateTime) : Token(accessToken,dateTime)
    {
        /// <summary>
        /// Manages the refresh token
        /// </summary>
        public string RefreshToken { get; set; } = refreshToken;
    }
}
