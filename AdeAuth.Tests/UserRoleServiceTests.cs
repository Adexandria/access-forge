using AdeAuth.Db;
using AdeAuth.Models;
using AdeAuth.Services.Authentication;
using AdeAuth.Services.Interfaces;


namespace AdeAuth.Tests
{
    public class UserRoleServiceTests : DbContextTestHelper
    {
        [SetUp]
        public override void Setup()
        {
            base.Setup();
            identityContext = new(dbOptions);
            userService = new UserService<IdentityContext, ApplicationUser>
                (identityContext);
            userRoleService = new UserRoleService<IdentityContext, ApplicationUser,ApplicationRole>
                (identityContext);
            roleService = new RoleService<IdentityContext, ApplicationRole>(identityContext);
        }
        [Test]
        public void ShouldAddUserRoleSuccessfully()
        {
            ApplicationUser user = new()
            {
                Id = new Guid("a8903f84-94ea-484e-b71f-79396fd85fbf"),
                FirstName = "Adeola",
                LastName = "Aderibigbe",
                UserName = "Addie",
                AuthenticatorKey = string.Empty,
                Email = "adeolaaderibigbe09@gmail.com",
                PasswordHash = "1234567",
                PhoneNumber = "1234567890",
                Salt = "1234567890"
            };

            ApplicationRole role = new()
            {
                Id = Guid.NewGuid(),
                Name = "User"
            };


            _ = userService.CreateUser(user);

            _ =  roleService.CreateRole(role);

            var response =  userRoleService.AddUserRole(user, "User");

            Assert.True(response.IsSuccessful);
        }

        [Test]
        public async Task ShouldAddUserRoleAsyncSuccessfully()
        {
            ApplicationUser user = new()
            {
                Id = new Guid("a8903f84-94ea-484e-b71f-79396fd85fbf"),
                FirstName = "Adeola",
                LastName = "Aderibigbe",
                UserName = "Addie",
                AuthenticatorKey = string.Empty,
                Email = "adeolaaderibigbe09@gmail.com",
                PasswordHash = "1234567",
                PhoneNumber = "1234567890",
                Salt = "1234567890"
            };

            ApplicationRole role = new()
            {
                Id = Guid.NewGuid(),
                Name = "User"
            };


            _ = await userService.CreateUserAsync(user);

            _ = await roleService.CreateRoleAsync(role);

            var response = await userRoleService.AddUserRoleAsync(user, "User");

            Assert.True(response.IsSuccessful);
        }

        [Test]
        public void ShouldFailToAddUserRoleIfUserDoesNotExist()
        {
            ApplicationRole role = new()
            {
                Id = Guid.NewGuid(),
                Name = "User"
            };

            ApplicationUser user = new()
            {
                Id = new Guid("a8903f84-94ea-484e-b71f-79396fd85fbf"),
                FirstName = "Adeola",
                LastName = "Aderibigbe",
                UserName = "Addie",
                AuthenticatorKey = string.Empty,
                Email = "adeolaaderibigbe09@gmail.com",
                PasswordHash = "1234567",
                PhoneNumber = "1234567890",
                Salt = "1234567890"
            };

            _ =  roleService.CreateRole(role);

            var response =  userRoleService.AddUserRole(user, "User");

            Assert.False(response.IsSuccessful);
        }

        [Test]
        public async Task ShouldFailToAddUserRoleIfUserDoesNotExistAsync()
        {
            ApplicationRole role = new()
            {
                Id = Guid.NewGuid(),
                Name = "User"
            };

            ApplicationUser user = new()
            {
                Id = new Guid("a8903f84-94ea-484e-b71f-79396fd85fbf"),
                FirstName = "Adeola",
                LastName = "Aderibigbe",
                UserName = "Addie",
                AuthenticatorKey = string.Empty,
                Email = "adeolaaderibigbe09@gmail.com",
                PasswordHash = "1234567",
                PhoneNumber = "1234567890",
                Salt = "1234567890"
            };

            _ = await roleService.CreateRoleAsync(role);

            var response = await userRoleService.AddUserRoleAsync(user, "User");

            Assert.False(response.IsSuccessful);
        }

        [Test]
        public void ShouldFailToAddUserRoleIfRoleDoesNotExist()
        {
            ApplicationUser user = new()
            {
                Id = new Guid("a8903f84-94ea-484e-b71f-79396fd85fbf"),
                FirstName = "Adeola",
                LastName = "Aderibigbe",
                UserName = "Addie",
                AuthenticatorKey = string.Empty,
                Email = "adeolaaderibigbe09@gmail.com",
                PasswordHash = "1234567",
                PhoneNumber = "1234567890",
                Salt = "1234567890"
            };

            _ =  userService.CreateUser(user);

            var response = userRoleService.AddUserRole(user, "User");

            Assert.False(response.IsSuccessful);
        }

        [Test]
        public async Task ShouldFailToAddUserRoleIfRoleDoesNotExistAsync()
        {
            ApplicationUser user = new()
            {
                Id = new Guid("a8903f84-94ea-484e-b71f-79396fd85fbf"),
                FirstName = "Adeola",
                LastName = "Aderibigbe",
                UserName = "Addie",
                AuthenticatorKey = string.Empty,
                Email = "adeolaaderibigbe09@gmail.com",
                PasswordHash = "1234567",
                PhoneNumber = "1234567890",
                Salt = "1234567890"
            };

            _ = await userService.CreateUserAsync(user);

            var response = await userRoleService.AddUserRoleAsync(user, "User");

            Assert.False(response.IsSuccessful);
        }

        [Test]
        public void ShouldRemoveUserRoleSuccessfully()
        {
            ApplicationRole role = new()
            {
                Id = Guid.NewGuid(),
                Name = "User"
            };
            ApplicationUser user = new()
            {
                Id = new Guid("a8903f84-94ea-484e-b71f-79396fd85fbf"),
                FirstName = "Adeola",
                LastName = "Aderibigbe",
                UserName = "Addie",
                AuthenticatorKey = string.Empty,
                Email = "adeolaaderibigbe09@gmail.com",
                PasswordHash = "1234567",
                PhoneNumber = "1234567890",
                Salt = "1234567890"
            };

            _ =  userService.CreateUser(user);
            _ =  roleService.CreateRole(role);
            _ =  userRoleService.AddUserRole(user, "User");

            var response =  userRoleService.RemoveUserRole(user, "User");

            Assert.True(response.IsSuccessful);
        }

        [Test]
        public async Task ShouldRemoveUserRoleAsyncSuccessfully()
        {
            ApplicationRole role = new()
            {
                Id = Guid.NewGuid(),
                Name = "User"
            };
            ApplicationUser user = new()
            {
                Id = new Guid("a8903f84-94ea-484e-b71f-79396fd85fbf"),
                FirstName = "Adeola",
                LastName = "Aderibigbe",
                UserName = "Addie",
                AuthenticatorKey = string.Empty,
                Email = "adeolaaderibigbe09@gmail.com",
                PasswordHash = "1234567",
                PhoneNumber = "1234567890",
                Salt = "1234567890"
            };

            _ = await userService.CreateUserAsync(user);
            _ = await roleService.CreateRoleAsync(role);
            _ = await userRoleService.AddUserRoleAsync(user, "User");

            var response = await userRoleService.RemoveUserRoleAsync(user, "User");

            Assert.True(response.IsSuccessful);
        }


        [Test]
        public void ShouldFailToRemoveUserRoleIfUserDoesNotExist()
        {
            ApplicationRole role = new()
            {
                Id = Guid.NewGuid(),
                Name = "User"
            };

            ApplicationUser user = new()
            {
                Id = new Guid("a8903f84-94ea-484e-b71f-79396fd85fbf"),
                FirstName = "Adeola",
                LastName = "Aderibigbe",
                UserName = "Addie",
                AuthenticatorKey = string.Empty,
                Email = "adeolaaderibigbe09@gmail.com",
                PasswordHash = "1234567",
                PhoneNumber = "1234567890",
                Salt = "1234567890"
            };

            _ = roleService.CreateRole(role);

            var response = userRoleService.RemoveUserRole(user, "User");

            Assert.False(response.IsSuccessful);
        }

        [Test]
        public async Task ShouldFailToRemoveUserRoleIfUserDoesNotExistAsync()
        {
            ApplicationRole role = new()
            {
                Id = Guid.NewGuid(),
                Name = "User"
            };

            ApplicationUser user = new()
            {
                Id = new Guid("a8903f84-94ea-484e-b71f-79396fd85fbf"),
                FirstName = "Adeola",
                LastName = "Aderibigbe",
                UserName = "Addie",
                AuthenticatorKey = string.Empty,
                Email = "adeolaaderibigbe09@gmail.com",
                PasswordHash = "1234567",
                PhoneNumber = "1234567890",
                Salt = "1234567890"
            };

            _ = await roleService.CreateRoleAsync(role);

            var response = await userRoleService.RemoveUserRoleAsync(user, "User");

            Assert.False(response.IsSuccessful);
        }


        [Test]
        public void ShouldFailToRemoveUserRoleIfRoleDoesNotExist()
        {
            ApplicationUser user = new()
            {
                Id = new Guid("a8903f84-94ea-484e-b71f-79396fd85fbf"),
                FirstName = "Adeola",
                LastName = "Aderibigbe",
                UserName = "Addie",
                AuthenticatorKey = string.Empty,
                Email = "adeolaaderibigbe09@gmail.com",
                PasswordHash = "1234567",
                PhoneNumber = "1234567890",
                Salt = "1234567890"
            };

            _ =  userService.CreateUser(user);

            var response = userRoleService.RemoveUserRole(user, "User");

            Assert.False(response.IsSuccessful);
        }


        [Test]
        public async Task ShouldFailToRemoveUserRoleIfRoleDoesNotExistAsync()
        {
            ApplicationUser user = new()
            {
                Id = new Guid("a8903f84-94ea-484e-b71f-79396fd85fbf"),
                FirstName = "Adeola",
                LastName = "Aderibigbe",
                UserName = "Addie",
                AuthenticatorKey = string.Empty,
                Email = "adeolaaderibigbe09@gmail.com",
                PasswordHash = "1234567",
                PhoneNumber = "1234567890",
                Salt = "1234567890"
            };

            _ = await userService.CreateUserAsync(user);

            var response = await userRoleService.RemoveUserRoleAsync(user, "User");

            Assert.False(response.IsSuccessful);
        }
        [Test]
        public void ShouldFailToRemoveUserRoleIfUserRoleDoesNotExist()
        {
            ApplicationRole role = new()
            {
                Id = Guid.NewGuid(),
                Name = "User"
            };
            ApplicationUser user = new()
            {
                Id = new Guid("a8903f84-94ea-484e-b71f-79396fd85fbf"),
                FirstName = "Adeola",
                LastName = "Aderibigbe",
                UserName = "Addie",
                AuthenticatorKey = string.Empty,
                Email = "adeolaaderibigbe09@gmail.com",
                PasswordHash = "1234567",
                PhoneNumber = "1234567890",
                Salt = "1234567890"
            };

            _ =  userService.CreateUser(user);
            _ =  roleService.CreateRole(role);

            var response =  userRoleService.RemoveUserRole(user, "User");

            Assert.False(response.IsSuccessful);
        }

        [Test]
        public async Task ShouldFailToRemoveUserRoleIfUserRoleDoesNotExistAsync()
        {
            ApplicationRole role = new()
            {
                Id = Guid.NewGuid(),
                Name = "User"
            };
            ApplicationUser user = new()
            {
                Id = new Guid("a8903f84-94ea-484e-b71f-79396fd85fbf"),
                FirstName = "Adeola",
                LastName = "Aderibigbe",
                UserName = "Addie",
                AuthenticatorKey = string.Empty,
                Email = "adeolaaderibigbe09@gmail.com",
                PasswordHash = "1234567",
                PhoneNumber = "1234567890",
                Salt = "1234567890"
            };

            _ = await userService.CreateUserAsync(user);
            _ = await roleService.CreateRoleAsync(role);

            var response = await userRoleService.RemoveUserRoleAsync(user, "User");

            Assert.False(response.IsSuccessful);
        }

        [Test]
        public void ShouldAddUserRoleByEmailSuccessfully()
        {
            ApplicationUser user = new()
            {
                Id = new Guid("a8903f84-94ea-484e-b71f-79396fd85fbf"),
                FirstName = "Adeola",
                LastName = "Aderibigbe",
                UserName = "Addie",
                AuthenticatorKey = string.Empty,
                Email = "adeolaaderibigbe09@gmail.com",
                PasswordHash = "1234567",
                PhoneNumber = "1234567890",
                Salt = "1234567890"
            };

            ApplicationRole role = new()
            {
                Id = Guid.NewGuid(),
                Name = "User"
            };


            _ =  userService.CreateUser(user);

            _ =  roleService.CreateRole(role);

            var response =  userRoleService.AddUserRole(user.Email, "User");

            Assert.True(response.IsSuccessful);
        }

        [Test]
        public async Task ShouldAddUserRoleByEmailSuccessfullyAsync()
        {
            ApplicationUser user = new()
            {
                Id = new Guid("a8903f84-94ea-484e-b71f-79396fd85fbf"),
                FirstName = "Adeola",
                LastName = "Aderibigbe",
                UserName = "Addie",
                AuthenticatorKey = string.Empty,
                Email = "adeolaaderibigbe09@gmail.com",
                PasswordHash = "1234567",
                PhoneNumber = "1234567890",
                Salt = "1234567890"
            };

            ApplicationRole role = new()
            {
                Id = Guid.NewGuid(),
                Name = "User"
            };


            _ = await userService.CreateUserAsync(user);

            _ = await roleService.CreateRoleAsync(role);

            var response = await userRoleService.AddUserRoleAsync(user.Email, "User");

            Assert.True(response.IsSuccessful);
        }


        [Test]
        public void ShouldFailToAddUserRoleByEmailIfUserDoesNotExist()
        {
            ApplicationRole role = new()
            {
                Id = Guid.NewGuid(),
                Name = "User"
            };

            _ =  roleService.CreateRole(role);

            var response =  userRoleService.AddUserRole("adeolaaderibigbe09@gmail.com", "User");

            Assert.False(response.IsSuccessful);
        }

        [Test]
        public async Task ShouldFailToAddUserRoleByEmailIfUserDoesNotExistAsync()
        {
            ApplicationRole role = new()
            {
                Id = Guid.NewGuid(),
                Name = "User"
            };

            _ = await roleService.CreateRoleAsync(role);

            var response = await userRoleService.AddUserRoleAsync("adeolaaderibigbe09@gmail.com", "User");

            Assert.False(response.IsSuccessful);
        }

        [Test]
        public void ShouldFailToAddUserRoleByEmailIfRoleDoesNotExist()
        {
            ApplicationUser user = new()
            {
                Id = new Guid("a8903f84-94ea-484e-b71f-79396fd85fbf"),
                FirstName = "Adeola",
                LastName = "Aderibigbe",
                UserName = "Addie",
                AuthenticatorKey = string.Empty,
                Email = "adeolaaderibigbe09@gmail.com",
                PasswordHash = "1234567",
                PhoneNumber = "1234567890",
                Salt = "1234567890"
            };

            _ =  userService.CreateUser(user);

            var response =  userRoleService.AddUserRole(user.Email, "User");

            Assert.False(response.IsSuccessful);
        }

        [Test]
        public async Task ShouldFailToAddUserRoleByEmailIfRoleDoesNotExistAsync()
        {
            ApplicationUser user = new()
            {
                Id = new Guid("a8903f84-94ea-484e-b71f-79396fd85fbf"),
                FirstName = "Adeola",
                LastName = "Aderibigbe",
                UserName = "Addie",
                AuthenticatorKey = string.Empty,
                Email = "adeolaaderibigbe09@gmail.com",
                PasswordHash = "1234567",
                PhoneNumber = "1234567890",
                Salt = "1234567890"
            };

            _ = await userService.CreateUserAsync(user);

            var response = await userRoleService.AddUserRoleAsync(user.Email, "User");

            Assert.False(response.IsSuccessful);
        }

        [TearDown]
        public void TearDown()
        {
            identityContext.Database.EnsureDeleted();
        }

        private IdentityContext identityContext;
        private IUserService<ApplicationUser> userService;
        private IRoleService<ApplicationRole> roleService;
        private IUserRoleService<ApplicationUser> userRoleService;
    }
}
