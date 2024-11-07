using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdeAuth.Services.Utility
{
    public class DependencyType
    {
        public DependencyType(Type interfaceType, Type implementationType)
        {
            InterfaceType = interfaceType;
            ImplementationType = implementationType;
        }

        public Type InterfaceType { get; set; }
        public Type ImplementationType { get; set; }
    }
}
