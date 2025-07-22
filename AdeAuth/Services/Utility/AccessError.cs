using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdeAuth.Services.Utility
{
    /// <summary>
    /// Represents an error that occurs during access operations, providing details about the error description and
    /// code.
    /// </summary>
    /// <param name="description">The description of the error</param>
    /// <param name="code">The status code</param>
    public class AccessError(string description, int code)
    {
        public string Description { get; set; } = description;
        public int Code { get; set; } = code;
    }
}
