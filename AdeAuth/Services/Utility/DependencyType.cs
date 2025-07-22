using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdeAuth.Services.Utility
{
    public class DependencyType(Type interfaceType, Type implementationType)
    {
        public Type InterfaceType { get; set; } = interfaceType;
        public Type ImplementationType { get; set; } = implementationType;
    }
}
