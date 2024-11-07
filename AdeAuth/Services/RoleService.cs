using AdeAuth.Models;
using AdeAuth.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AdeAuth.Services
{
    /// <summary>
    /// Manages role services
    /// </summary>
    /// <typeparam name="TRole">Application role</typeparam>
    public class RoleService<TDbContext, TModel> : Repository<TDbContext,TModel>,IRoleService<TModel>
        where TDbContext : DbContext
        where TModel : ApplicationRole
    {
        public RoleService(TDbContext dbContext):base(dbContext) 
        {
            _roles = dbContext.Set<TModel>();
        }

        /// <summary>
        /// Creates role
        /// </summary>
        /// <param name="role">New role to add</param>
        /// <returns>boolean value</returns>
        public async Task<bool> CreateRoleAsync(TModel role)
        {
            await _roles.AddAsync(role);

            return await SaveChangesAsync();
        }

        public bool CreateRole(TModel role)
        {
            _roles.Add(role);

            return SaveChanges();
        }      

        
        /// <summary>
        /// Create roles
        /// </summary>
        /// <param name="roles">New roles to add</param>
        /// <returns>boolean value</returns>
        public async Task<bool> CreateRolesAsync(List<TModel> roles)
        {
            foreach (var role in roles)
            {
                await _roles.AddAsync(role);
            }

            return await SaveChangesAsync();
        }

        public bool CreateRoles(List<TModel> roles)
        {
            foreach(var role in roles)
            {
                _roles.Add(role);
            }

            return SaveChanges();
        }


        /// <summary>
        /// Delete role
        /// </summary>
        /// <param name="role">Role to delete</param>
        /// <returns>Boolean value</returns>
        public async Task<bool> DeleteRoleAsync(TModel role)
        {
            _roles.Remove(role);
            return await SaveChangesAsync();
        }

        public bool DeleteRole(TModel role)
        {
           _roles.Remove(role);
           return  SaveChanges();
        }

        /// <summary>
        /// Delete roles
        /// </summary>
        /// <param name="roles">Roles to delete</param>
        /// <returns>Boolean value</returns>
        public async Task<bool> DeleteRoleRangeAsync(IEnumerable<TModel> roles)
        {
            foreach (var role in roles)
            {
                _roles.Remove(role);
            }
            return await SaveChangesAsync();
        }


        public bool DeleteRoleRange(IEnumerable<TModel> roles)
        {
            foreach (var role in roles)
            {
                _roles.Remove(role);
            }
            return SaveChanges();
        }


        /// <summary>
        /// Get existing role
        /// </summary>
        /// <param name="role">Role</param>
        /// <returns>role</returns>
        public async Task<TModel> GetExistingRoleAsync(string role)
        {
            return await _roles.Where(s => s.Name == role)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Get existing role
        /// </summary>
        /// <param name="role">Role</param>
        /// <returns>role</returns>
        public TModel GetExistingRole(string role)
        {
            return _roles.Where(s => s.Name == role).FirstOrDefault();
        }


        public IEnumerable<TModel> GetExistingRoles  
        {
            get
            {
                return _roles.ToList();
            }
        }
        private readonly DbSet<TModel> _roles;
    }

}
