using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdeAuth.Services.Utility
{
    public class AccessError
    {
        public AccessError(string description, int code) 
        {
            Description = description;
            Code = code;
        }

        public string Description { get; set; }
        public int Code { get; set; }
    }
}
