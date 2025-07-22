using AdeAuth.Models;
using AdeAuth.Services.Interfaces;
using AdeAuth.Services.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AdeAuth.Services.Authentication
{
    /// <summary>
    /// Provides services for managing user roles within a specified database context.
    /// </summary>
    /// <remarks>This service allows adding and removing roles for users, both synchronously and
    /// asynchronously. It ensures that roles are valid and users exist before performing operations.</remarks>
    /// <typeparam name="TDbContext">The type of the database context used for data operations.</typeparam>
    /// <typeparam name="TUser">The type representing a user in the application.</typeparam>
    /// <typeparam name="TRole">The type representing a role in the application.</typeparam>
    /// <param name="dbContext">Manages DbContext</param>
    public class UserRoleService<TDbContext, TUser, TRole>(TDbContext dbContext) : Repository<TDbContext, UserRole>(dbContext), IUserRoleService<TUser>
        where TUser : ApplicationUser
        where TDbContext : DbContext
        where TRole : ApplicationRole
    {
        /// <summary>
        /// Adds a specified role to a user.
        /// </summary>
        /// <remarks>This method checks if the specified role exists and if the user is already present in
        /// the system before attempting to add the role. It also ensures that changes are saved successfully.</remarks>
        /// <param name="user">The user to whom the role will be added. Cannot be null.</param>
        /// <param name="role">The name of the role to add. Must be a valid, existing role.</param>
        /// <returns>An <see cref="AccessResult"/> indicating the success or failure of the operation. Returns <see
        /// cref="AccessResult.Success"/> if the role is successfully added. Returns <see cref="AccessResult.Failed"/>
        /// with an appropriate error message if the role is invalid, the user does not exist, or if saving changes
        /// fails.</returns>
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

        /// <summary>
        /// Add role to user.
        /// </summary>
        /// <param name="email">Email of the user</param>
        /// <param name="role">The role to assigned to the user</param>
        /// <returns>An <see cref="AccessResult"/> indicating the success or failure of the operation. Returns <see
        /// cref="AccessResult.Success"/> if the role is successfully added to the user. Returns <see cref="AccessResult.Failed"/>
        /// with an appropriate error message if the role is invalid, the user does not exist, or if saving changes
        /// fails</returns>
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

        /// <summary>
        /// Add role to user asynchronously. 
        /// </summary>
        /// <param name="email">Email of the user</param>
        /// <param name="role">The role to assigned to the user</param>
        /// <returns>An <see cref="AccessResult"/> indicating the success or failure of the operation. Returns <see
        /// cref="AccessResult.Success"/> if the role is successfully added to the user. Returns <see cref="AccessResult.Failed"/>
        /// with an appropriate error message if the role is invalid, the user does not exist, or if saving changes
        /// fails</returns>
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

        /// <summary>
        /// Asynchronously adds a specified role to a user identified by their email address.
        /// </summary>
        /// <remarks>This method checks if the specified role exists and if the user with the given email
        /// exists.  If either does not exist, the operation fails with a corresponding error message.</remarks>
        /// <param name="email">The email address of the user to whom the role will be added. Cannot be null or empty.</param>
        /// <param name="role">The name of the role to be added to the user. Cannot be null or empty.</param>
        /// <returns>An <see cref="AccessResult"/> indicating the success or failure of the operation.  Returns <see
        /// cref="AccessResult.Success"/> if the role is successfully added;  otherwise, returns <see
        /// cref="AccessResult.Failed"/> with an appropriate error message.</returns>
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

        /// <summary>
        /// Removes a specified role from a user.
        /// </summary>
        /// <param name="user">The user from whom the role will be removed. Cannot be null.</param>
        /// <param name="role">The role to remove</param>
        /// <returns>An <see cref="AccessResult"/> indicating the success or failure of the operation.  Returns <see
        /// cref="AccessResult.Success"/> if the role is successfully removed;  otherwise, returns <see
        /// cref="AccessResult.Failed"/> with an appropriate error message.</returns>
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


        /// <summary>
        /// Asynchronously removes a specified role from a user.
        /// </summary>
        /// <param name="user">The user from whom the role will be removed. Cannot be null.</param>
        /// <param name="role">The name of the role to be removed. Cannot be null or empty.</param>
        /// <returns>An <see cref="AccessResult"/> indicating the success or failure of the operation. Returns <see
        /// cref="AccessResult.Success"/> if the role is successfully removed. Returns <see cref="AccessResult.Failed"/>
        /// with an appropriate error message if the role does not exist, the user is not found, the role is not
        /// associated with the user, or if the operation fails to persist changes.</returns>
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


        private readonly DbSet<TUser> _users = dbContext.Set<TUser>();

        private readonly DbSet<TRole> _roles = dbContext.Set<TRole>();

        private readonly DbSet<UserRole> _userRoles = dbContext.Set<UserRole>();
    }
}
