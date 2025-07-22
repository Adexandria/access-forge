using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdeAuth.Services.Utility
{
    public class AccessError(string description, int code)
    {
        public string Description { get; set; } = description;
        public int Code { get; set; } = code;
    }
}
