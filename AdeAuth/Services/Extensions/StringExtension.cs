using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdeAuth.Services.Extensions
{
    /// <summary>
    /// Provides extension methods for string manipulation.
    /// </summary>
    internal static class StringExtension
    {
        /// <summary>
        /// Determines if a string is empty or null.
        /// </summary>
        /// <param name="value">The value to check </param>
        /// <returns><see langword="true"/> if the <paramref name="guid"/> is equal to <see cref="Guid.Empty"/>; otherwise, <see
        /// langword="false"/>.</returns>
        public static bool IsEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

    }
}
