using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdeAuth.Models
{
    public class LocatorDto
    {
        public string LoginDevice { get;  set; }
        public string IpAddress { get;  set; }
        public string Country { get;  set; }
        public string City { get;  set; }
        public DateTime RecentLoginTime { get;  set; }
    }
}
