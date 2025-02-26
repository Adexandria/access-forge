using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdeAuth.Models
{
    public class TwoAuthenticationConfiguration
    {
        public byte[] AutheticatorKey { get; set; }
        public string Issuer { get; set; }
    }
}
