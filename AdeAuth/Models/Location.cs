using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdeAuth.Models
{
    /// <summary>
    /// Manages location information for a user
    /// </summary>
    public class Location
    {
        /// <summary>
        /// Gets or sets the name of the city.
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Manages the coountry of the user
        /// </summary>
        public string Country { get; set; }
    }
}
