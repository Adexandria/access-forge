using AdeAuth.Db;
using AdeAuth.Models;
using AdeAuth.Services;
using AdeAuth.Services.Authentication;
using AdeAuth.Services.Extensions;
using AdeAuth.Services.Interfaces;
using AdeAuth.Services.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace AdeAuth.Infrastructure
{
    public static class AuthContainerBuilder
    {
        /// <summary>
        /// Sets up identity services using identity context
        /// </summary>
        /// <typeparam name="TDbContext">Identity context</typeparam>
        /// <param name="serviceCollection">Manages dependencies of services</param>
        /// <param name="configurationBuilder">Build identity service configuration</param>
        public static IServiceCollection AddIdentityService<TDbContext>(this IServiceCollection serviceCollection,
            Action<AuthConfiguration> configurationBuilder)
            where TDbContext : IdentityContext
        {
            var authConfiguration = new AuthConfiguration(serviceCollection);

            configurationBuilder(authConfiguration);

            RegisterDbContext<TDbContext>(serviceCollection, authConfiguration.ConnectionString);

            serviceCollection.RegisterDependencies(authConfiguration.dependencyTypes);

            RegisterDependencies<TDbContext, ApplicationUser, ApplicationRole>(serviceCollection);

            return serviceCollection;
        }



        /// <summary>
        /// Sets up identity services using identity context
        /// </summary>
        /// <typeparam name="TUser">User model</typeparam>
        /// <typeparam name="TDbContext">Identity context</typeparam>
        /// <param name="serviceCollection">Manages dependencies of services</param>
        /// <param name="configurationBuilder">Build identity service configuration</param>
        public static IServiceCollection AddIdentityService<TDbContext, TUser>(this IServiceCollection serviceCollection,
            Action<AuthConfiguration> configurationBuilder)
            where TDbContext : IdentityContext<TUser>
            where TUser : ApplicationUser
        {
            var authConfiguration = new AuthConfiguration(serviceCollection);

            configurationBuilder(authConfiguration);

            RegisterDbContext<TDbContext>(serviceCollection, authConfiguration.ConnectionString);

            serviceCollection.RegisterDependencies(authConfiguration.dependencyTypes);

            RegisterDependencies<TDbContext, TUser, ApplicationRole>(serviceCollection);


            return serviceCollection;
        }

        /// <summary>
        /// Sets up identity services using identity context
        /// </summary>
        /// <typeparam name="TRole">Role model</typeparam>
        /// <typeparam name="TUser">User model</typeparam>
        /// <typeparam name="TDbContext">Identity context</typeparam>
        /// <param name="serviceCollection">Manages dependencies of services</param>
        /// <param name="configurationBuilder">Build identity service configuration</param>
        public static IServiceCollection AddIdentityService<TDbContext, TUser, TRole>(this IServiceCollection serviceCollection,
            Action<AuthConfiguration> configurationBuilder)
         where TDbContext : IdentityContext<TUser, TRole>
         where TUser : ApplicationUser
         where TRole : ApplicationRole
        {
            var authConfiguration = new AuthConfiguration(serviceCollection);

            configurationBuilder(authConfiguration);

            RegisterDbContext<TDbContext>(serviceCollection, authConfiguration.ConnectionString);

            serviceCollection.RegisterDependencies(authConfiguration.dependencyTypes);

            RegisterDependencies<TDbContext, TUser, TRole>(serviceCollection);

            return serviceCollection;
        }

        public static IServiceCollection AddIdentityRule(this IServiceCollection serviceCollection,Action<AccessRule> action)
        {
            var rule = new AccessRule();

            action(rule);

            serviceCollection.AddSingleton(rule);

            return serviceCollection;
        }

        /// <summary>
        /// Register two factor authentication configuration
        /// </summary>
        /// <param name="serviceCollection">Manages dependencies of services</param>
        /// <param name="actionBuilder">Manages two factor authentication configuration</param>
        /// <returns>Manages dependencies of services</returns>
        public static IServiceCollection AddGoogleAuthenticator
            (this IServiceCollection serviceCollection, Action<TwoAuthenticationConfiguration> actionBuilder)
        {
            var twoAuthenticationConfiguration = new TwoAuthenticationConfiguration();

            actionBuilder(twoAuthenticationConfiguration);

            serviceCollection.AddSingleton(twoAuthenticationConfiguration);

            return serviceCollection;
        }

        /// <summary>
        /// Sets up jwt bearer authentication scheme
        /// </summary>
        /// <param name="authenticationBuilder">Configuration authentication</param>
        /// <param name="actionBuilder">Build token configuration</param>
        /// <returns>Configuration authentication</returns>
        public static IServiceCollection AddJwtBearer(this IServiceCollection serviceCollection, Action<TokenConfiguration> actionBuilder)
        {
            var tokenConfiguration = new TokenConfiguration();

            actionBuilder(tokenConfiguration);

            serviceCollection.AddAuthentication(tokenConfiguration.AuthenticationScheme)
                .AddJwtBearer(tokenConfiguration.AuthenticationScheme ?? JwtBearerDefaults.AuthenticationScheme, option =>
                {
                    option.SaveToken = true;
                    option.TokenValidationParameters = new TokenValidationParameters()
                    {

                        ValidAudience = tokenConfiguration.Audience ?? null,
                        ValidIssuer = tokenConfiguration.Issuer ?? null,
                        ValidateIssuer = !string.IsNullOrEmpty(tokenConfiguration.Issuer),
                        ValidateAudience = !string.IsNullOrEmpty(tokenConfiguration.Audience),
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenConfiguration.TokenSecret)),
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true
                    };
                });

            serviceCollection.AddSingleton(tokenConfiguration);

            return serviceCollection;
        }

        /// <summary>
        /// Sets up microsoft account for single sign on
        /// </summary>
        /// <param name="authenticationBuilder">Configuration authentication</param>
        /// <param name="actionBuilder">Builds microsoft single sign on</param>
        /// <returns>Configuration authentication</returns>
        public static IServiceCollection AddMicrosoftAccount(this AuthenticationBuilder authenticationBuilder, Action<AzureConfiguration> actionBuilder)
        {
            var azureConfiguration = new AzureConfiguration();

            actionBuilder(azureConfiguration);

            authenticationBuilder.AddJwtBearer(azureConfiguration.AuthenticationScheme ?? JwtBearerDefaults.AuthenticationScheme, options =>
                  {
                      options.SaveToken = true;
                      options.MetadataAddress = $"{azureConfiguration.Instance}{azureConfiguration.Type}/v2.0/.well-known/openid-configuration";
                      options.TokenValidationParameters = new TokenValidationParameters()
                      {
                          NameClaimType = azureConfiguration.NameClaimType ?? ClaimsIdentity.DefaultNameClaimType,
                          ValidAudience = azureConfiguration.Audience,
                          ValidIssuer = $"{azureConfiguration.Instance}{azureConfiguration.TenantId}/v2.0",
                          ValidateIssuer = true,
                          ValidateAudience = true,
                          ValidateIssuerSigningKey = true,
                          ValidateLifetime = true
                      };
                  });

            return authenticationBuilder.Services;
        }

        public static IServiceCollection AddIPInfoConfiguration(this IServiceCollection servicesCollection,string apiKey)
        {
            var ipConfiguration = new IpInfoConfiguration(apiKey);

            servicesCollection.AddSingleton(ipConfiguration);

            return servicesCollection;
        }

        /// <summary>
        /// Creates table if they don't exist
        /// </summary>
        /// <param name="dbContext">Db context</param>
        /// <param name="entityName">Entity name</param>
        private static void CheckTableExistsAndCreateIfMissing(DbContext dbContext, string entityName)
        {
            var defaultSchema = "dbo";
            var tableName = string.IsNullOrWhiteSpace(defaultSchema) ? $"[{entityName}]" : $"[{defaultSchema}].[{entityName}]";

            try
            {
                _ = dbContext.Database.ExecuteSqlRaw($"SELECT TOP(1) * FROM {tableName}"); //Throws on missing table
            }
            catch (Exception)
            {
                var scriptStart = $"CREATE TABLE [{entityName}]";
                const string scriptEnd = "GO";
                var script = dbContext.Database.GenerateCreateScript();

                var tableScript = script.Split(scriptStart).Last().Split(scriptEnd);
                var first = $"{scriptStart} {tableScript.First()}";

                dbContext.Database.ExecuteSqlRaw(first);
            }
        }

        /// <summary>
        /// Registers dbcontext
        /// </summary>
        /// <typeparam name="TDbContext">Identity context</typeparam>
        /// <param name="services">Manages dependencies of services</param>
        /// <param name="connectionString">Connection string of sql server</param>
        private static void RegisterDbContext<TDbContext>(IServiceCollection services, string connectionString)
            where TDbContext : DbContext
        {
            services.AddDbContext<TDbContext>((s) => s.UseSqlServer(connectionString));

            services.RunMigration<TDbContext>();

            RegisterDependencies(services);
        }

        /// <summary>
        /// Run migrations
        /// </summary>
        /// <typeparam name="TDbContext">Identity context</typeparam>
        /// <param name="services">Manages dependencies of services</param>
        /// <param name="action">Registers db context dependencies</param>
        private static void RunMigration<TDbContext>(this IServiceCollection services)
            where TDbContext : DbContext
        {
            var provider = services.BuildServiceProvider();

            var scope = provider.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();

            var databaseCreator = dbContext.GetService<IRelationalDatabaseCreator>();

            databaseCreator.EnsureCreated();

            if (databaseCreator.CanConnect())
            {
                CheckTableExistsAndCreateIfMissing(dbContext, "Users");
                CheckTableExistsAndCreateIfMissing(dbContext, "Roles");
                CheckTableExistsAndCreateIfMissing(dbContext, "UserRoles");
                CheckTableExistsAndCreateIfMissing(dbContext, "LoginActivities");
                CheckTableExistsAndCreateIfMissing(dbContext, "UserClaims");
            }
        }


        //revist
        /// <summary>
        /// Register dependencies
        /// </summary>
        /// <typeparam name="TDbContext">Identity context</typeparam>
        /// <param name="services">Manages dependencies of services</param>
        private static void RegisterDependencies<TDbContext, TUser, TRole>(IServiceCollection services)
           where TDbContext : DbContext
            where TUser : ApplicationUser
            where TRole : ApplicationRole
        {
            services.AddScoped<IRoleService<TRole>, RoleService<TDbContext, TRole>>();

            services.AddScoped<IUserService<TUser>, UserService<TDbContext, TUser>>();

            services.AddScoped<IUserRoleService<TUser>, UserRoleService<TDbContext, TUser, TRole>>();

            services.AddScoped<SignInManager<TUser>>();

            services.AddScoped<UserManager<TUser>>();

            services.AddScoped<IUserClaimService, UserClaimService<TDbContext>>();

            services.AddScoped<ILoginActivityService, LoginActivityService<TDbContext>>();
        }

        /// <summary>
        /// Register dependencies
        /// </summary>
        /// <param name="services">Manages dependencies of services</param>
        private static void RegisterDependencies(IServiceCollection services)
        {
            services.AddSingleton<IPasswordManager, PasswordManager>();

            services.AddSingleton<ITokenProvider, TokenProvider>();

            services.AddSingleton<IMfaService, MfaService>();

            services.AddScoped<ILocatorService,LocatorService>();

            services.AddSingleton<AccessOption>();

            services.AddHttpContextAccessor();
       
            services.AddLogging(c => c.AddConsole()); 
        }

        /// <summary>
        /// Register dependencies
        /// </summary>
        /// <typeparam name="TDbContext">Identity context</typeparam>
        /// <param name="services">Manages dependencies of services</param>
        private static void RegisterDependencies(this IServiceCollection services, List<DependencyType> dependencyTypes)
        {
            dependencyTypes?.Foreach(service =>
            {
                services.AddScoped(service.InterfaceType, service.ImplementationType);
            });
        }
    }
}
