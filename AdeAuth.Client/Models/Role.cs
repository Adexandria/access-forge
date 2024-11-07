using AdeAuth.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdeAuth.Client.Models
{
    public class Role: ApplicationRole
    {
        public Role(string name, SubRole subRole)
        {
            Name = name;
            RoleType = subRole;
        }

        public Role(string name)
        {
            Name=name;
            RoleType = SubRole.None;
        }
        public SubRole RoleType { get; set; }
        public List<User> Users { get; set; }
    }
}
