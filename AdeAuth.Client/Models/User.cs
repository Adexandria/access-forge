using AdeAuth.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdeAuth.Client.Models
{
    public class User : ApplicationUser
    {
        public User(string email)
        {
            Id = Guid.NewGuid();
            Email = email;
        }

        public User SetHashedPassword(string password, string salt)
        {
            PasswordHash = password;
            Salt = salt;
            return this;
        }

        public User SetRole(Guid roleId)
        {
            RoleId = roleId;
            return this;
        }
        public string? Address { get; set; }
        public Guid RoleId { get; set; }
        public Role Role { get; set; }
    }
}
