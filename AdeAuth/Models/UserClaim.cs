using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdeAuth.Models
{
    public class UserClaim
    {
        public UserClaim()
        {
            Id = Guid.NewGuid();
        }
        public Guid Id { get; set; }

        public string ClaimType { get; set; }

        public string ClaimValue { get; set;}

        public Guid UserId { get; set; }
    }
}
