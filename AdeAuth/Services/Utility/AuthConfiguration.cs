using AdeAuth.Models;
using AdeAuth.Services.Extensions;
using AdeAuth.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace AdeAuth.Services.Utility
{
    /// <summary>
    /// Builds configuration for authentication library
    /// </summary>
    public class AuthConfiguration
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public AuthConfiguration(IServiceCollection services)
        {
            dependencyTypes = [];
        }

        /// <summary>
        /// Register dependencies using assembly searching
        /// </summary>
        /// <param name="assembly"></param>
        public void RegisterServicesFromAssembly(Assembly assembly)
        {
            var types = assembly.GetTypes();

            if (types.GetServiceType(typeof(IUserService<>)) is DependencyType serviceType)
            {
                dependencyTypes.Add(serviceType); 
            }

            if (types.GetServiceType(typeof(IRoleService<>)) is DependencyType roleType)
            {
                dependencyTypes.Add(roleType);    
            }
        }

        /// <summary>
        /// Register only services using type
        /// </summary>
        /// <param name="interfaceType">interface type</param>
        /// <param name="implementationType">Implementation type</param>
        public void RegisterService(Type interfaceType, Type implementationType)
        {
            dependencyTypes.Add(new DependencyType(interfaceType, implementationType));
        }

        public void RegisterService<TInterfaceType, TImplementationType>()
        {
            dependencyTypes.Add(new DependencyType(typeof(TInterfaceType), typeof(TImplementationType)));
        }

        /// <summary>
        /// Sets up sql server connection string
        /// </summary>
        /// <param name="connectionString"></param>
        public void UseSqlServer(string connectionString)
        {
            ConnectionString = connectionString;
        }


        /// <summary>
        /// Connection string
        /// </summary>
        internal string ConnectionString { get; set; }

        /// <summary>
        /// Manages services registered
        /// </summary>

        internal List<DependencyType> dependencyTypes;
    }
}
