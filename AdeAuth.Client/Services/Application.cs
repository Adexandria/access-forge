using AdeAuth.Client.Models;
using AdeAuth.Services;
using AdeAuth.Services.Interfaces;

namespace AdeAuth.Client.Services
{
    public class Application
    {
        public Application(SignInManager<User> _signInManager, 
            IPasswordManager _passwordManager,
            UserManager<User> _userManager)
        {
            signInManager = _signInManager;
            userManager = _userManager;
            passwordManager = _passwordManager;
        }


        public string SignUp(string email, string password)
        {
            var user = new User(email);

            var hashedPassword = passwordManager.HashPassword(password, out string salt);

            user.SetHashedPassword(hashedPassword, salt);

            var result = userManager.CreateUser(user);

            if(!result.IsSuccessful) 
            {
                return "Failed to create user";
            }

            var tokenResponse = userManager.GenerateEmailConfirmationToken(user.Id);

            return tokenResponse.Data;
        }


        public string Authenticate(string email, string password)
        {
            var result = signInManager.SignInByEmail(email, password);

            if (result.IsTwoFactorRequired)
            {
                return "Proceed to two factor authenticator";
            }

            if (result.IsLockedOut)
            {
                return "This user has been locked out";
            }

            if (!result.IsSuccessful)
            {
                return "Invalid email or password";
            }

            return result.Data.AccessToken;
        }

        public string VerifyEmail(string email, string token)
        {
            var response = userManager.ConfirmEmailByToken(email, token);

            if(response.IsSuccessful)
            {
                return "Successful";
            }
            return "Failed to verify email";
        }

        private readonly SignInManager<User> signInManager;
        private readonly IPasswordManager passwordManager;
        private readonly UserManager<User> userManager;
    }
}
