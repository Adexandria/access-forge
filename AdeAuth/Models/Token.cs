using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdeAuth.Models
{
    public class Token
    {
        public Token(string accessToken, DateTime dateTime)
        {
            AccessToken = accessToken;
            ExpirationTime = dateTime;
        }
        public string AccessToken { get; set; }

        public DateTime ExpirationTime { get; set; }
    }
}
