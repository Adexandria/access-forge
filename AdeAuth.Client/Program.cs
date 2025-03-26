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

            //   var response = Application.SignUp("adeolaaderibigbe09@gmail.com", "Kiki6*");

            //var response = Application.VerifyEmail("adeolaaderibigbe09@gmail.com", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6ImFkZW9sYWFkZXJpYmlnYmUwOUBnbWFpbC5jb20iLCJuYW1laWQiOiIyYWZjYjliZS00ODdkLTRlNTQtOTE3NS01N2QwMmVkNDFmN2EiLCJuYmYiOjE3NDA0ODAzMzEsImV4cCI6MTc0MDQ4MjEzMSwiaWF0IjoxNzQwNDgwMzMxfQ.ny1YyVmnwGG0iW2lokO6xuODniqyb6r2JU0p0N-zNa8");

            var response = Application.Authenticate("adeolaaderibigbe09@gmail.com", "Kiki6*");

            Console.WriteLine(response);
        }


        private static Application Application;

    }
}
