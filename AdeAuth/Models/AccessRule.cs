using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdeAuth.Models
{
    public class AccessRule
    {
        public bool IsRequireEmailConfirmation { get; set; }
        public PasswordRule Password {  get; set; } = new PasswordRule();
    }
}
