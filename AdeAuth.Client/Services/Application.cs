using AdeAuth.Client.Models;
using AdeAuth.Models;
using AdeAuth.Services.Interfaces;


namespace AdeAuth.Client.Services
{
    public class Application
    {
        public Application(IAuthService userService,
            IRoleService<Role> roleService, ITokenProvider tokenProvider, IPasswordManager passwordManager)
        {
            _userService = userService;
            _tokenProvider = tokenProvider;
            _roleService = roleService;
            _passwordManager = passwordManager;
        }

        public async Task<string> SignUp(string email, string password, string role)
        {
            var isUserExist = await _userService.IsUserExist(email);
            if (isUserExist)
                return "This account is already associated with this application.";

            var userRole = await _roleService.GetExistingRoleAsync(role);

            var hashedPassword = _passwordManager.HashPassword(password, out var salt);

            var user = new User(email).SetHashedPassword(hashedPassword, salt).SetRole(userRole.Id);

            var isCreated = await _userService.CreateUserAsync(user);

            if(!isCreated)
            {
                return "Failed to create account";
            }

            var claims = new Dictionary<string, object>()
            {
                {"id", user.Id.ToString() },
                {"email", email },
                {"role", userRole.Name }
            };

            var generatedToken = _tokenProvider.GenerateToken(claims, 30);

            return generatedToken;
        }

        public async Task<bool> AssignUserRoles(string email, string role, string token)
        {
            var claims = _tokenProvider.ReadToken(token, "id", "email","role");

            var userRole = claims.ContainsValue(role) ? claims[role].ToString() : null;

            if(userRole != "Admin")
            {
                return false;
            }

            var isDecoded = Guid.TryParse(claims["id"].ToString(),out Guid userId);

            if (!isDecoded)
            {
                return false;                
            }

           var isUserExist = await _userService.IsUserExist(userId);

           if (!isUserExist)
           {
                return false;
           }

           return await _roleService.AddUserRoleAsync(email, role);
        }



        public async Task<string> AuthenticateByEmail(string email, string password)
        {
            var authenticatedUser = await _userService.AuthenticateUsingEmailAsync(email, password);
            if(authenticatedUser == null)
            {
                return "Invalid email/password";
            }


            var claims = new Dictionary<string, object>()
            {
                {"id", authenticatedUser.Id.ToString() },
                {"email", email },
                {"role", authenticatedUser.Role.Name }
            };

            var generatedToken = _tokenProvider.GenerateToken(claims, 30);

            return generatedToken;
        }

        public async Task<bool> CreateRole(Role role)
        {
            var response = await _roleService.CreateRoleAsync(role);

            return response;
        }

        public async Task<bool> IsExist(string role)
        {
            var response = await _roleService.GetExistingRoleAsync(role);

            return response != null;
        }

        private readonly ITokenProvider _tokenProvider;
        private readonly IPasswordManager _passwordManager;
        private readonly IRoleService<Role> _roleService;
        private readonly IAuthService _userService;
    }
}
