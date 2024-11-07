using AdeAuth.Client.Db;
using AdeAuth.Client.Models;
using AdeAuth.Client.Services;
using AdeAuth.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

namespace AdeAuth.Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();

            var tokenSecret = "123456AccessForge@!2092000111@@wqrys";

            var connectionString = "Server=localhost\\SQLEXPRESS;Database=AccessForge;Trusted_Connection=True;TrustServerCertificate=true;";

            services.AddIdentityService<IdentityDbContext,User,Role>(options =>
            {
                options.RegisterServicesFromAssembly(typeof(Program).Assembly);
                options.UseSqlServer(connectionString);
            })
            .AddGoogleTwoFactorAuthenticator(s=>
            {
                    s.Issuer = "AdeNote";
                    s.AutheticatorKey = Encoding.ASCII.GetBytes(tokenSecret);
            })
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenSecret = tokenSecret;
            });

            services.AddScoped<Application>();

            using var provider = services.BuildServiceProvider();
            using var scope = provider.CreateScope();
            var scopedProvider = scope.ServiceProvider;

            Application = scopedProvider.GetRequiredService<Application>();

            _ = CreateRole("User").Result;
            _ = CreateRole("Admin",SubRole.Super).Result;

            // var response = SignUp("adeolaaderibigbe09@gmail.com", "Adeolaisthebest", "User").Result;

            var response = Application.AuthenticateByEmail("adeolaaderibigbe09@gmail.com", "Adeolaisthebest").Result;

            Console.WriteLine(response);
        }

        public static async Task<bool> CreateRole(string name, SubRole subRole = SubRole.None)
        {
            if (string.IsNullOrWhiteSpace(name))
            { 
                throw new ArgumentNullException("name");
            }

            var isExist = await Application.IsExist(name);
            if(isExist)
            {
                return false;
            }

            var role = new Role(name, subRole);
            return await Application.CreateRole(role);
        }

        public static async Task<string> SignUp(string email, string password, string role)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(role))
            {
                return "Invalid name or password or role";
            }

            return await Application.SignUp(email, password, role);
        }

        private static Application Application;

    }
}
