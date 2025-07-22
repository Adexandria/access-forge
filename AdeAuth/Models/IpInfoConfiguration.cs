using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdeAuth.Models
{
    /// <summary>
    /// Manages IP information configuration
    /// </summary>
    public class IpInfoConfiguration
    {
        /// <summary>
        /// A constructor
        /// </summary>  
        public IpInfoConfiguration(string apiKey)
        {
            APIKey = apiKey; 
        }

        /// <summary>
        /// Manages API key for IP information service
        /// </summary>
        public string APIKey { get; set; }
    }
}
