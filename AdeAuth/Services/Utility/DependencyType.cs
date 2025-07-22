using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdeAuth.Services.Utility
{
    /// <summary>
    /// Represents a mapping between an interface type and its corresponding implementation type.
    /// </summary>
    /// <remarks>This class is typically used in dependency injection scenarios to specify which
    /// implementation should be used for a given interface.</remarks>
    /// <param name="interfaceType"></param>
    /// <param name="implementationType"></param>
    public class DependencyType(Type interfaceType, Type implementationType)
    {
        /// <summary>
        /// Gets or sets the type of the interface that this instance represents.
        /// </summary>
        public Type InterfaceType { get; set; } = interfaceType;

        /// <summary>
        /// Gets or sets the type that implements the current interface or abstract class.
        /// </summary>
        public Type ImplementationType { get; set; } = implementationType;
    }
}
