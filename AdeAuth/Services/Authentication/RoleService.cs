using AdeAuth.Models;
using AdeAuth.Services.Interfaces;
using AdeAuth.Services.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AdeAuth.Services.Authentication
{
    /// <summary>
    /// Manages role services
    /// </summary>
    /// <typeparam name="TRole">Application role</typeparam>
    public class RoleService<TDbContext, TModel> : Repository<TDbContext, TModel>, IRoleService<TModel>
        where TDbContext : DbContext
        where TModel : ApplicationRole
    {
        public RoleService(TDbContext dbContext) : base(dbContext)
        {
            _roles = dbContext.Set<TModel>();
        }

        /// <summary>
        /// Creates role
        /// </summary>
        /// <param name="role">New role to add</param>
        /// <returns>boolean value</returns>
        public async Task<AccessResult> CreateRoleAsync(TModel role)
        {
            await _roles.AddAsync(role);

            var response =  await SaveChangesAsync();

            if (response)
            {
                return AccessResult.Success();
            }

            return AccessResult.Failed(new AccessError("Failed to create role",
                StatusCodes.Status400BadRequest));
        }

        /// <summary>
        /// Creates role
        /// </summary>
        /// <param name="role">New role to add</param>
        /// <returns>boolean value</returns>
        public AccessResult CreateRole(TModel role)
        {
            _roles.Add(role);

            var response =  SaveChanges();

            if (response)
            {
                return AccessResult.Success();
            }

            return AccessResult.Failed(new AccessError("Failed to create role",
               StatusCodes.Status400BadRequest));
        }


        /// <summary>
        /// Create roles
        /// </summary>
        /// <param name="roles">New roles to add</param>
        /// <returns>boolean value</returns>
        public async Task<AccessResult> CreateRolesAsync(List<TModel> roles)
        {
            foreach (var role in roles)
            {
                await _roles.AddAsync(role);
            }

            var response = await SaveChangesAsync();

            if (response)
            {
                return AccessResult.Success();
            }

            return AccessResult.Failed(new AccessError("Failed to create roles",
                StatusCodes.Status400BadRequest));
        }

        /// <summary>
        /// Create roles
        /// </summary>
        /// <param name="roles">New roles to add</param>
        /// <returns>boolean value</returns>
        public AccessResult CreateRoles(List<TModel> roles)
        {
            foreach (var role in roles)
            {
                _roles.Add(role);
            }

            var response = SaveChanges();

            if (response)
            {
                return AccessResult.Success();
            }

            return AccessResult.Failed(new AccessError("Failed to create roles",
                StatusCodes.Status400BadRequest));
        }


        /// <summary>
        /// Delete role
        /// </summary>
        /// <param name="role">Role to delete</param>
        /// <returns>Boolean value</returns>
        public async Task<AccessResult> DeleteRoleAsync(TModel role)
        {
            _roles.Remove(role);

            var response = await SaveChangesAsync();

            if (response)
            {
                return AccessResult.Success();
            }

            return AccessResult.Failed(new AccessError("Failed to delete role",
                StatusCodes.Status400BadRequest));
        }

        /// <summary>
        /// Delete role
        /// </summary>
        /// <param name="role">Role to delete</param>
        /// <returns>Boolean value</returns>
        public AccessResult DeleteRole(TModel role)
        {
            _roles.Remove(role);

            var response = SaveChanges();

            if (response)
            {
                return AccessResult.Success();
            }

            return AccessResult
                .Failed(new AccessError("Failed to delete role", 
                StatusCodes.Status400BadRequest));
        }

        /// <summary>
        /// Delete roles
        /// </summary>
        /// <param name="roles">Roles to delete</param>
        /// <returns>Boolean value</returns>
        public async Task<AccessResult> DeleteRoleRangeAsync(IEnumerable<TModel> roles)
        {
            foreach (var role in roles)
            {
                _roles.Remove(role);
            }
            var response = await SaveChangesAsync();

            if (response)
            {
                return AccessResult.Success();
            }

            return AccessResult.Failed(new AccessError("Failed to delete roles", 
                StatusCodes.Status400BadRequest));
        }

        /// <summary>
        /// Delete roles
        /// </summary>
        /// <param name="roles">Roles to delete</param>
        /// <returns>Boolean value</returns>
        public AccessResult DeleteRoleRange(IEnumerable<TModel> roles)
        {
            foreach (var role in roles)
            {
                _roles.Remove(role);
            }
            var response = SaveChanges();
            if (response)
            {
                return AccessResult.Success();
            }
            return AccessResult.Failed(new AccessError("Failed to delete roles",
                StatusCodes.Status400BadRequest));
        }


        /// <summary>
        /// Get existing role
        /// </summary>
        /// <param name="role">Role</param>
        /// <returns>role</returns>
        public async Task<AccessResult<TModel>> GetExistingRoleAsync(string role)
        {
            var currentRole =  await _roles.Where(s => s.Name == role)
                .FirstOrDefaultAsync();

            if(currentRole == null)
            {
                return AccessResult<TModel>.Failed(new AccessError("Role does not exist",
                    StatusCodes.Status404NotFound));
            }

            return AccessResult<TModel>.Success(currentRole);
        }

        /// <summary>
        /// Get existing role
        /// </summary>
        /// <param name="role">Role</param>
        /// <returns>role</returns>
        public AccessResult<TModel> GetExistingRole(string role)
        {
            var currentRole = _roles.Where(s => s.Name == role).FirstOrDefault();

            if (currentRole == null)
            {
                return AccessResult<TModel>.Failed(new AccessError("Role does not exist",
                    StatusCodes.Status404NotFound));
            }

            return AccessResult<TModel>.Success(currentRole);
        }


        public AccessResult<IEnumerable<TModel>> GetExistingRoles
        {
            get
            {
                return AccessResult<IEnumerable<TModel>>.Success(_roles.ToList());
            }
        }
        private readonly DbSet<TModel> _roles;
    }

}
