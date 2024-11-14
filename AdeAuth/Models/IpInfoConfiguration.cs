using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdeAuth.Models
{
    public class IpInfoConfiguration
    {
        public IpInfoConfiguration(string apiKey)
        {
            APIKey = apiKey; 
        }
        public string APIKey { get; set; }
    }
}
