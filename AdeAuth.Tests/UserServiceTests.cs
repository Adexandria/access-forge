using AdeAuth.Db;
using AdeAuth.Models;
using AdeAuth.Services;
using AdeAuth.Services.Authentication;
using AdeAuth.Services.Interfaces;
using Moq;

namespace AdeAuth.Tests
{
    public class UserServiceTests : DbContextTestHelper
    {
        [SetUp]
        public override void Setup()
        {
            base.Setup();
            identityContext = new(dbOptions);
            userService = new UserService<IdentityContext,ApplicationUser>
                (identityContext);
        }
        [Test]
        public async Task ShouldCreateUsersAsyncSuccessfully()
        {
            ApplicationUser user = new()
            {Id = new Guid("a8903f84-94ea-484e-b71f-79396fd85fbf"),
                FirstName = "Adeola",
                LastName = "Aderibigbe",
                UserName = "Addie",
                AuthenticatorKey = string.Empty,
                Email = "adeolaaderibigbe09@gmail.com",
                PasswordHash = "1234567",
                PhoneNumber = "1234567890",
                Salt = "1234567890"
            };

            var response = await userService.CreateUserAsync(user);

            Assert.IsTrue(response.IsSuccessful);
        }

        [Test]
        public void ShouldCreateUsersSuccessfully()
        {
            ApplicationUser user = new()
            {Id = new Guid("a8903f84-94ea-484e-b71f-79396fd85fbf"),
                FirstName = "Adeola",
                LastName = "Aderibigbe",
                UserName = "Addie",
                AuthenticatorKey = string.Empty,
                Email = "adeolaaderibigbe09@gmail.com",
                PasswordHash = "1234567",
                PhoneNumber = "1234567890",
                Salt = "1234567890"
            };

            var response = userService.CreateUser(user);

            Assert.IsTrue(response.IsSuccessful);
        }


        [Test]
        public async Task ShouldUpdateUserAsyncSuccessfully()
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

            var response = await userService.UpdateUserAsync(user);

            Assert.IsTrue(response.IsSuccessful);
        }

        [Test]
        public void ShouldUpdateUserSuccessfully()
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

            var response = userService.UpdateUser(user);

            Assert.IsTrue(response.IsSuccessful);
        }


        [Test]
        public async Task ShouldDeleteUserAsyncSuccessfully()
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

            var response = await userService.DeleteUserAsync(user);

            Assert.IsTrue(response.IsSuccessful);
        }

        [Test]
        public void ShouldDeleteUserSuccessfully()
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

            var response = userService.DeleteUser(user);

            Assert.IsTrue(response.IsSuccessful);
        }



        [Test]
        public async Task ShouldFetchUserByEmailAsyncSuccessfully()
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

            var response = await userService.FetchUserByEmailAsync("adeolaaderibigbe09@gmail.com");

            Assert.IsTrue(response.IsSuccessful);
        }

        [Test]
        public void ShouldFetchUserByEmailSuccessfully()
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

            _ = userService.CreateUser(user);

            var response = userService.FetchUserByEmail("adeolaaderibigbe09@gmail.com");

            Assert.IsTrue(response.IsSuccessful);
        }

        [Test]
        public async Task ShouldFetchUserByIdAsyncSuccessfully()
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

            var response = await userService.FetchUserByIdAsync(new Guid("a8903f84-94ea-484e-b71f-79396fd85fbf"));

            Assert.IsTrue(response.IsSuccessful);
        }

        [Test]
        public void ShouldFetchUserByIdSuccessfully()
        {
            ApplicationUser user = new()
            {Id = new Guid("a8903f84-94ea-484e-b71f-79396fd85fbf"),
                FirstName = "Adeola",
                LastName = "Aderibigbe",
                UserName = "Addie",
                AuthenticatorKey = string.Empty,
                Email = "adeolaaderibigbe09@gmail.com",
                PasswordHash = "1234567",
                PhoneNumber = "1234567890",
                Salt = "1234567890"
            };

            _ =  userService.CreateUserAsync(user);

            var response =  userService.FetchUserById(new Guid("a8903f84-94ea-484e-b71f-79396fd85fbf"));

            Assert.IsTrue(response.IsSuccessful);
        }

        [Test]
        public async Task ShouldFetchUserByUsernameAsyncSuccessfully()
        {
            ApplicationUser user = new()
            {
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

            var response = await userService.FetchUserByUsernameAsync("Addie");

            Assert.IsTrue(response.IsSuccessful);
        }

        [Test]
        public void ShouldFetchUserByUsernameSuccessfully()
        {
            ApplicationUser user = new()
            {
                FirstName = "Adeola",
                LastName = "Aderibigbe",
                UserName = "Addie",
                AuthenticatorKey = string.Empty,
                Email = "adeolaaderibigbe09@gmail.com",
                PasswordHash = "1234567",
                PhoneNumber = "1234567890",
                Salt = "1234567890"
            };

            _ = userService.CreateUser(user);

            var response = userService.FetchUserByUsername("Addie");

            Assert.IsTrue(response.IsSuccessful);
        }
        [TearDown]
        public void TearDown()
        {
            identityContext.Database.EnsureDeleted();
        }

        private IdentityContext identityContext;

        private IUserService<ApplicationUser> userService;
    }
}