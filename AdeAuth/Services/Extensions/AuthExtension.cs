using AdeAuth.Services.Utility;

namespace AdeAuth.Services.Extensions
{
    public static class AuthExtension
    {
        /// <summary>
        /// Searches for the type
        /// </summary>
        /// <param name="types">Type to register</param>
        /// <param name="type">Type to search</param>
        /// <returns>dependency type</returns>
        public static DependencyType GetServiceType(this Type[] serviceTypes, Type type)
        {
            var implementationType = serviceTypes.Where(s => s.GetInterfaces()
            .Any(p => p.Name == type.Name) && !s.IsAbstract).FirstOrDefault();

            if (implementationType == null)
            {
                return default;
            }

            var interfaceType = implementationType.GetInterfaces().FirstOrDefault();

            if (interfaceType != null)
            {
                return new DependencyType(interfaceType, implementationType);
            }

            return default;
        }
    }
}
