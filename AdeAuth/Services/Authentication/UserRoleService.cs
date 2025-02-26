using AdeAuth.Models;
using AdeAuth.Services.Interfaces;
using AdeAuth.Services.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AdeAuth.Services.Authentication
{
    public class UserRoleService<TDbContext, TUser, TRole> : Repository<TDbContext, UserRole>, IUserRoleService<TUser>
        where TUser : ApplicationUser
        where TDbContext : DbContext
        where TRole : ApplicationRole
    {
        public UserRoleService(TDbContext dbContext) : base(dbContext)
        {
            _userRoles = dbContext.Set<UserRole>();
            _users = dbContext.Set<TUser>();
            _roles = dbContext.Set<TRole>();
        }

        public AccessResult AddUserRole(TUser user, string role)
        {
            var currentRole = GetExistingRole(role);
            if (currentRole == null)
            {
                return AccessResult.Failed(new AccessError("Invalid role", StatusCodes.Status404NotFound));
            }

            if (!_users.Any(s => s == user)) 
            {
                return AccessResult.Failed(new AccessError("Failed to Add role to user", StatusCodes.Status400BadRequest));
            }

            _userRoles.Add(new UserRole()
            {
                RoleId = currentRole.Id,
                UserId = user.Id
            });

            if (!SaveChanges())
            {
                return AccessResult.Failed(new AccessError("Failed to Add role to user", StatusCodes.Status400BadRequest));
            }

            return AccessResult.Success();
        }

        public AccessResult AddUserRole(string email, string role)
        {
            var currentRole = GetExistingRole(role);
            if (currentRole == null)
            {
                return AccessResult.Failed(new AccessError("Invalid role", StatusCodes.Status404NotFound));
            }

            var currentUser = _users.Where(s => s.Email == email).FirstOrDefault();

            if (currentUser == null)
                return AccessResult.Failed(new AccessError("User does not exist", StatusCodes.Status404NotFound));

            _userRoles.Add(new UserRole()
            {
                RoleId = currentRole.Id,
                UserId = currentUser.Id
            });

            if (!SaveChanges())
            {
                return AccessResult.Failed(new AccessError("Failed to Add role to user", StatusCodes.Status400BadRequest));
            }

            return AccessResult.Success();
        }

        public async Task<AccessResult> AddUserRoleAsync(TUser user, string role)
        {
            var currentRole = await GetExistingRoleAsync(role);
            if (currentRole == null)
            {
                return AccessResult.Failed(new AccessError("Invalid role", StatusCodes.Status404NotFound));
            }

            if (!_users.Any(s => s == user))
            {
                return AccessResult.Failed(new AccessError("Failed to Add role to user", StatusCodes.Status400BadRequest));
            }

            _userRoles.Add(new UserRole()
            {
                RoleId = currentRole.Id,
                UserId = user.Id
            });

            var response = await SaveChangesAsync();

            if (!response)
            {
                return AccessResult.Failed(new AccessError("Failed to Add role to user", StatusCodes.Status400BadRequest));
            }

            return AccessResult.Success();
        }

        public async Task<AccessResult> AddUserRoleAsync(string email, string role)
        {
            var currentRole = await GetExistingRoleAsync(role);
            if (currentRole == null)
            {
                return AccessResult.Failed(new AccessError("Invalid role", StatusCodes.Status404NotFound));
            }

            var currentUser = await _users.Where(s => s.Email == email).FirstOrDefaultAsync();

            if (currentUser == null)
                return AccessResult.Failed(new AccessError("User does not exist", StatusCodes.Status404NotFound));

            _userRoles.Add(new UserRole()
            {
                RoleId = currentRole.Id,
                UserId = currentUser.Id
            });

            var response = await SaveChangesAsync();

            if (!response)
            {
                return AccessResult.Failed(new AccessError("Failed to Add role to user", StatusCodes.Status400BadRequest));
            }

            return AccessResult.Success();
        }

        public AccessResult RemoveUserRole(TUser user, string role)
        {
            var currentRole = GetExistingRole(role);
            if (currentRole == null)
            {
                return AccessResult.Failed(new AccessError("Invalid role", StatusCodes.Status404NotFound));
            }

            if (!_users.Any(s => s == user))
            {
                return AccessResult.Failed(new AccessError("Failed to Add role to user", StatusCodes.Status400BadRequest));
            }

            var userRole = _userRoles.Where(s => s.UserId == user.Id && s.RoleId == currentRole.Id).FirstOrDefault();

            if (userRole == null)
            {
                return AccessResult.Failed(new AccessError("Role isn't associated with this user", StatusCodes.Status404NotFound));
            }

            _userRoles.Remove(userRole);

            if (!SaveChanges())
            {
                return AccessResult.Failed(new AccessError("Failed to Add role to user", StatusCodes.Status400BadRequest));
            }

            return AccessResult.Success();
        }

        public async Task<AccessResult> RemoveUserRoleAsync(TUser user, string role)
        {
            var currentRole = await GetExistingRoleAsync(role);
            if (currentRole == null)
            {
                return AccessResult.Failed(new AccessError("Invalid role", StatusCodes.Status404NotFound));
            }

            if (!_users.Any(s => s == user))
            {
                return AccessResult.Failed(new AccessError("Failed to Add role to user", StatusCodes.Status400BadRequest));
            }

            var userRole = await _userRoles.Where(s => s.UserId == user.Id && s.RoleId == currentRole.Id).FirstOrDefaultAsync();

            if (userRole == null)
            {
                return AccessResult.Failed(new AccessError("Role isn't associated with this user", StatusCodes.Status404NotFound));
            }
            _userRoles.Remove(userRole);

            var response = await SaveChangesAsync();

            if (!response)
            {
                return AccessResult.Failed(new AccessError("Failed to Add role to user", StatusCodes.Status400BadRequest));
            }

            return AccessResult.Success();
        }

        /// <summary>
        /// Get existing role
        /// </summary>
        /// <param name="role">Role</param>
        /// <returns>role</returns>
        private async Task<TRole> GetExistingRoleAsync(string role)
        {
            return await _roles.Where(s => s.Name == role)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Get existing role
        /// </summary>
        /// <param name="role">Role</param>
        /// <returns>role</returns>
        private TRole GetExistingRole(string role)
        {
            return _roles.Where(s => s.Name == role).FirstOrDefault();
        }


        private readonly DbSet<TUser> _users;

        private readonly DbSet<TRole> _roles;

        private readonly DbSet<UserRole> _userRoles;
    }
}
