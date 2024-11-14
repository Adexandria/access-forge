using AdeAuth.Client.Db;
using AdeAuth.Client.Models;
using AdeAuth.Client.Services;
using AdeAuth.Infrastructure;
using AdeAuth.Models;
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

            services.AddIdentityService<IdentityDbContext, User, Role>(options =>
            {
                options.RegisterServicesFromAssembly(typeof(Program).Assembly);
                options.UseSqlServer(connectionString);
            })
            .AddGoogleAuthenticator(s =>
            {
                s.Issuer = "AdeNote";
                s.AutheticatorKey = Encoding.ASCII.GetBytes(tokenSecret);
            })
            .AddJwtBearer(s =>
            {
                s.TokenSecret = tokenSecret;
                s.AuthenticationScheme = JwtBearerDefaults.AuthenticationScheme;
                s.ExpirationTime = 30;
            })
            .AddIPInfoConfiguration("a9e6713beb5988")
            .AddIdentityRule(s =>
            {
                s.IsRequireEmailConfirmation = true;
                s.Password = new PasswordRule
                {
                    MaximumPasswordLength = 10,
                    MinimumPasswordLength = 3,
                    HasCapitalLetter = true,
                    HasNumber = true,
                    HasSmallLetter = true,
                    HasSpecialNumber = true,
                };
            });



            services.AddScoped<Application>();

            using var provider = services.BuildServiceProvider();
            using var scope = provider.CreateScope();
            var scopedProvider = scope.ServiceProvider;

            Application = scopedProvider.GetRequiredService<Application>();

           /* var response = Application.SignUp("adeolaaderibigbe09@gmail.com", "Adeol");*/

            var token = Application.Authenticate("adeolaaderibigbe09@gmail.com", "Adeol");

            Console.WriteLine(token);
        }


        private static Application Application;

    }
}
