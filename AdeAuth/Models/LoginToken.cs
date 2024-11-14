using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdeAuth.Models
{
    public class LoginToken : Token
    {
        public LoginToken(string refreshToken, string accessToken, DateTime dateTime) : base(accessToken,dateTime)
        {
            RefreshToken = refreshToken;
        }
        public string RefreshToken { get; set; }
    }
}
