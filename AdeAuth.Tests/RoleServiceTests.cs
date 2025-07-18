using AdeAuth.Db;
using AdeAuth.Models;
using AdeAuth.Services.Authentication;
using AdeAuth.Services.Interfaces;

namespace AdeAuth.Tests
{
    public class RoleServiceTests : DbContextTestHelper
    {
        [SetUp]
        public override void Setup()
        {
            base.Setup();
            identityContext = new(dbOptions);
            roleService = new RoleService<IdentityContext,ApplicationRole>(identityContext);
        }

        [Test]
        public async Task ShouldAddRoleAsyncSuccessfully()
        {
            ApplicationRole role = new()
            {
                Id = Guid.NewGuid(),
                Name = "User"
            };

            var response = await roleService.CreateRoleAsync(role);
            
            Assert.That(response.IsSuccessful);
        }

        [Test]
        public void ShouldAddRoleSuccessfully()
        {
            ApplicationRole role = new()
            {
                Id = Guid.NewGuid(),
                Name = "User"
            };

            var response = roleService.CreateRole(role);

            Assert.That(response.IsSuccessful);
        }

        [Test]
        public void ShouldAddRolesSuccessfully()
        {
            List<ApplicationRole> roles = new()
            {  new()
                {
                    Id = Guid.NewGuid(),
                    Name = "User"
                }

            };

            var response = roleService.CreateRoles(roles);

            Assert.That(response.IsSuccessful);
        }


        // Replace all instances of Assert.True(...) and Assert.False(...) with Assert.That(..., Is.True) and Assert.That(..., Is.False)

        [Test]
        public async Task ShouldAddRolesAsyncSuccessfully()
        {
            List<ApplicationRole> roles = new()
            {  new()
                {
                    Id = Guid.NewGuid(),
                    Name = "User"
                }

            };

            var response = await roleService.CreateRolesAsync(roles);

            Assert.That(response.IsSuccessful);
        }

        [Test]
        public void ShouldDeleteRoleSuccessfully()
        {
            ApplicationRole role = new()
            {
                Id = Guid.NewGuid(),
                Name = "User"
            };

            _ = roleService.CreateRole(role);

            var response = roleService.DeleteRole(role);

            Assert.That(response.IsSuccessful);
        }

        [Test]
        public async Task ShouldDeleteRoleAsyncSuccessfully()
        {
            ApplicationRole role = new()
            {
                Id = Guid.NewGuid(),
                Name = "User"
            };

            _ = await roleService.CreateRoleAsync(role);

            var response = await roleService.DeleteRoleAsync(role);

            Assert.That(response.IsSuccessful);
        }

        [Test]
        public async Task ShouldFailDeleteRoleAsyncSuccessfully()
        {
            var response = await roleService.DeleteRoleAsync(new ApplicationRole());

            Assert.That(!response.IsSuccessful);
        }

        [Test]
        public void ShouldFailDeleteRoleSuccessfully()
        {
            var response = roleService.DeleteRole(new ApplicationRole()
            {
                Id = Guid.NewGuid()
            });


            Assert.That(!response.IsSuccessful);
        }

        [Test]
        public void ShouldDeleteRolesSuccessfully()
        {
            ApplicationRole role = new()
            {
                Id = Guid.NewGuid(),
                Name = "User"
            };

            _ = roleService.CreateRole(role);

            var response = roleService.DeleteRoleRange(new[] { role });

            Assert.That(response.IsSuccessful);
        }

        [Test]
        public async Task ShouldDeleteRolesAsyncSuccessfully()
        {
            ApplicationRole role = new()
            {
                Id = Guid.NewGuid(),
                Name = "User"
            };

            _ = await roleService.CreateRoleAsync(role);

            var response = await roleService.DeleteRoleRangeAsync(new[] { role });

            Assert.That(response.IsSuccessful);
        }

        [TearDown]
        public void TearDown()
        {
            identityContext.Database.EnsureDeleted();
        }

        private IdentityContext identityContext;
        private IRoleService<ApplicationRole> roleService;
    }
}
