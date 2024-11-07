using AdeAuth.Client.Models;
using AdeAuth.Services.Interfaces;


namespace AdeAuth.Client.Services
{
    public interface IAuthService : IUserService<User>
    {
        Task<bool> IsUserExist(string email);

        Task<bool> IsUserExist(Guid id);
    }
}
