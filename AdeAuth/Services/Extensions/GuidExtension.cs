using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdeAuth.Services.Extensions
{
    internal static class GuidExtension
    {
        public static bool IsEmpty(this Guid guid)
        {
            return guid == Guid.Empty;

        }
    }
}
