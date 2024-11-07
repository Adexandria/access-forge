using AdeAuth.Models;

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
        Task<bool> CreateRolesAsync(List<TModel> roles);

        /// <summary>
        /// Create roles
        /// </summary>
        /// <param name="roles">New roles to add</param>
        /// <returns>boolean value</returns>
        bool CreateRoles(List<TModel> roles);

        /// <summary>
        /// Fetches all existing roles
        /// </summary>
        IEnumerable<TModel> GetExistingRoles { get; }


        /// <summary>
        /// Fetches existing role by role name asynchronous
        /// </summary>
        /// <param name="role">Name of role</param>
        /// <returns>Role</returns>
        Task<TModel> GetExistingRoleAsync(string role);


        /// <summary>
        /// Fetches existing role by role name
        /// </summary>
        /// <param name="role">Name of role</param>
        /// <returns>Role</returns>
        TModel GetExistingRole(string role);

        /// <summary>
        /// Creates role asynchronous
        /// </summary>
        /// <param name="role">New role to add</param>
        /// <returns>boolean value</returns>
        Task<bool> CreateRoleAsync(TModel role);

        /// <summary>
        /// Creates role 
        /// </summary>
        /// <param name="role">New role to add</param>
        /// <returns>boolean value</returns>
        bool CreateRole(TModel role);

        /// <summary>
        /// Delete roles asynchronous
        /// </summary>
        /// <param name="roles">Roles to delete</param>
        /// <returns>Boolean value</returns>
        Task<bool> DeleteRoleRangeAsync(IEnumerable<TModel> roles);


        /// <summary>
        /// Delete roles
        /// </summary>
        /// <param name="roles">Roles to delete</param>
        /// <returns>Boolean value</returns>
        bool DeleteRoleRange(IEnumerable<TModel> roles);

        /// <summary>
        /// Delete role asynchronous
        /// </summary>
        /// <param name="role">Role to delete</param>
        /// <returns>Boolean value</returns>
        Task<bool> DeleteRoleAsync(TModel role);


        /// <summary>
        /// Delete role
        /// </summary>
        /// <param name="role">Role to delete</param>
        /// <returns>Boolean value</returns>
        bool DeleteRole(TModel role);
    }
}
