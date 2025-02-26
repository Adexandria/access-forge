using AdeAuth.Models;
using AdeAuth.Services.Utility;

namespace AdeAuth.Services.Interfaces
{
    /// <summary>
    /// Manages role services
    /// </summary>
    /// <typeparam name="TModel">Application role type</typeparam>
    public interface IRoleService<TModel> where TModel : ApplicationRole
    {
        /// <summary>
        /// Create roles asynchronous
        /// </summary>
        /// <param name="roles">New roles to add</param>
        /// <returns>boolean value</returns>
        Task<AccessResult> CreateRolesAsync(List<TModel> roles);

        /// <summary>
        /// Create roles
        /// </summary>
        /// <param name="roles">New roles to add</param>
        /// <returns>boolean value</returns>
        AccessResult CreateRoles(List<TModel> roles);

        /// <summary>
        /// Fetches all existing roles
        /// </summary>
        AccessResult<IEnumerable<TModel>> GetExistingRoles { get; }


        /// <summary>
        /// Fetches existing role by role name asynchronous
        /// </summary>
        /// <param name="role">Name of role</param>
        /// <returns>Role</returns>
        Task<AccessResult<TModel>> GetExistingRoleAsync(string role);


        /// <summary>
        /// Fetches existing role by role name
        /// </summary>
        /// <param name="role">Name of role</param>
        /// <returns>Role</returns>
        AccessResult<TModel> GetExistingRole(string role);

        /// <summary>
        /// Creates role asynchronous
        /// </summary>
        /// <param name="role">New role to add</param>
        /// <returns>boolean value</returns>
        Task<AccessResult> CreateRoleAsync(TModel role);

        /// <summary>
        /// Creates role 
        /// </summary>
        /// <param name="role">New role to add</param>
        /// <returns>boolean value</returns>
        AccessResult CreateRole(TModel role);

        /// <summary>
        /// Delete roles asynchronous
        /// </summary>
        /// <param name="roles">Roles to delete</param>
        /// <returns>Boolean value</returns>
        Task<AccessResult> DeleteRoleRangeAsync(IEnumerable<TModel> roles);


        /// <summary>
        /// Delete roles
        /// </summary>
        /// <param name="roles">Roles to delete</param>
        /// <returns>Boolean value</returns>
        AccessResult DeleteRoleRange(IEnumerable<TModel> roles);

        /// <summary>
        /// Delete role asynchronous
        /// </summary>
        /// <param name="role">Role to delete</param>
        /// <returns>Boolean value</returns>
        Task<AccessResult> DeleteRoleAsync(TModel role);


        /// <summary>
        /// Delete role
        /// </summary>
        /// <param name="role">Role to delete</param>
        /// <returns>Boolean value</returns>
        AccessResult DeleteRole(TModel role);
    }
}
