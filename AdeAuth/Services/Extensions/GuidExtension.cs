using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdeAuth.Services.Extensions
{
    /// <summary>
    /// Provides extension methods for the <see cref="Guid"/> structure.
    /// </summary>
    internal static class GuidExtension
    {
        /// <summary>
        /// Determines whether the specified <see cref="Guid"/> is empty.
        /// </summary>
        /// <param name="guid">The <see cref="Guid"/> to check.</param>
        /// <returns><see langword="true"/> if the <paramref name="guid"/> is equal to <see cref="Guid.Empty"/>; otherwise, <see
        /// langword="false"/>.</returns>
        public static bool IsEmpty(this Guid guid)
        {
            return guid == Guid.Empty;

        }
    }
}
