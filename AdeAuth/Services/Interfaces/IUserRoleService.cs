using AdeAuth.Models;
using AdeAuth.Services.Utility;


namespace AdeAuth.Services.Interfaces
{
    public interface IUserRoleService<TUser>
        where TUser : ApplicationUser
    {
        Task<AccessResult> AddUserRoleAsync(TUser user, string role);

        AccessResult AddUserRole(TUser user, string role);

        AccessResult RemoveUserRole(TUser user, string role);

        Task<AccessResult> RemoveUserRoleAsync(TUser user, string role);

        AccessResult AddUserRole(string email, string role);

        Task<AccessResult> AddUserRoleAsync(string email, string role);
    }
}
