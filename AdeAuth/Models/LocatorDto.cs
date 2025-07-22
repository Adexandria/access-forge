using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdeAuth.Models
{
    /// <summary>
    /// Represents the DTO for user login activity location details.
    /// </summary>
    public class LocatorDto
    {
        /// <summary>
        /// Manages ecent login activity for a user
        /// </summary>
        public string LoginDevice { get;  set; }

        /// <summary>
        /// Manages the IP address of the user
        /// </summary>
        public string IpAddress { get;  set; }

        /// <summary>
        /// Manages the country of the user
        /// </summary>
        public string Country { get;  set; }

        /// <summary>
        /// Manages the city of the user
        /// </summary>
        public string City { get;  set; }

        /// <summary>
        /// Manages the recent login time of the user
        /// </summary>
        public DateTime RecentLoginTime { get;  set; }
    }
}
