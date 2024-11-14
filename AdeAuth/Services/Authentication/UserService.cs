using AdeAuth.Models;
using AdeAuth.Services.Interfaces;
using AdeAuth.Services.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AdeAuth.Services.Authentication
{
    /// <summary>
    /// Manages user service
    /// </summary>
    /// <typeparam name="TDbContext">Context to manage operation</typeparam>
    /// <typeparam name="TModel">Application user</typeparam>
    public class UserService<TDbContext, TModel> : Repository<TDbContext, TModel>, IUserService<TModel>
        where TDbContext : DbContext
        where TModel : ApplicationUser
    {
        /// <summary>
        /// A constructor
        /// </summary>
        /// <param name="dbContext">Context to manage operation</param>
        public UserService(TDbContext dbContext) : base(dbContext)
        {
            _users = dbContext.Set<TModel>();
        }


        /// <summary>
        /// Create user
        /// </summary>
        /// <param name="user">New user to create</param>
        /// <returns>Boolean value</returns>
        public async Task<AccessResult> CreateUserAsync(TModel user)
        {
            await _users.AddAsync(user);

            var response = await SaveChangesAsync();

            if (!response)
            {
                return AccessResult.Failed(new AccessError("Failed to create user", StatusCodes.Status400BadRequest));
            }

            return AccessResult.Success();
        }


        public AccessResult CreateUser(TModel user)
        {
            _users.Add(user);

            var response = SaveChanges();

            if (!response)
            {
                return AccessResult.Failed(new AccessError("Failed to create user", StatusCodes.Status400BadRequest));
            }

            return AccessResult.Success();
        }

        public async Task<AccessResult> UpdateUserAsync(TModel user)
        {
            _users.Update(user);

            bool isTracked = Db.Entry(user).State != EntityState.Detached;
            if (isTracked)
            {
                _users.Update(user);
            }
            else
            {
                var currentUser = _users.FirstOrDefault(s => s.Id == user.Id);
                if (currentUser == null)
                {
                    return AccessResult.Failed(new AccessError("Failed to update user", StatusCodes.Status400BadRequest));
                }
                Db.Entry(currentUser).CurrentValues.SetValues(user);
            }

            var response = await SaveChangesAsync();
            if (!response)
            {
                return AccessResult.Failed(new AccessError("Failed to update user", StatusCodes.Status400BadRequest));
            }

            return AccessResult.Success();
        }

        public AccessResult UpdateUser(TModel user)
        {
            bool isTracked = Db.Entry(user).State != EntityState.Detached;
            if (isTracked)
            {
                _users.Update(user);
            }
            else
            {
                var currentUser = _users.FirstOrDefault(s => s.Id == user.Id);
                if (currentUser == null)
                {
                    return AccessResult.Failed(new AccessError("Failed to update user", StatusCodes.Status400BadRequest));
                }
                Db.Entry(currentUser).CurrentValues.SetValues(user);
            }

            var response = SaveChanges();

            if (!response)
            {
                return AccessResult.Failed(new AccessError("Failed to update user", StatusCodes.Status400BadRequest));
            }

            return AccessResult.Success();
        }

        public async Task<AccessResult> DeleteUserAsync(TModel user)
        {
            _users.Remove(user);
            var response = await SaveChangesAsync();
            if (!response)
            {
                return AccessResult.Failed(new AccessError("Failed to delete user", StatusCodes.Status400BadRequest));
            }

            return AccessResult.Success();
        }

        public AccessResult DeleteUser(TModel user)
        {
            _users.Remove(user);
            var response = SaveChanges();
            if (!response)
            {
                return AccessResult.Failed(new AccessError("Failed to delete user", StatusCodes.Status400BadRequest));
            }

            return AccessResult.Success();
        }

        public async Task<AccessResult<TModel>> FetchUserByEmailAsync(string email)
        {
            var response = await _users.Where(s => s.Email == email).FirstOrDefaultAsync();

            if (response == null)
            {
                return AccessResult<TModel>.Failed(new AccessError("Invalid user", StatusCodes.Status404NotFound));
            }

            return AccessResult<TModel>.Success(response);
        }

        public AccessResult<TModel> FetchUserByEmail(string email)
        {
            var response = _users.Where(s => s.Email == email).FirstOrDefault();

            if (response == null)
            {
                return AccessResult<TModel>.Failed(new AccessError("Invalid user", StatusCodes.Status404NotFound));
            }

            return AccessResult<TModel>.Success(response);
        }

        public async Task<AccessResult<TModel>> FetchUserByIdAsync(Guid userId)
        {
            var response = await _users.Where(s => s.Id == userId).FirstOrDefaultAsync();

            if (response == null)
            {
                return AccessResult<TModel>.Failed(new AccessError("Invalid user", StatusCodes.Status404NotFound));
            }

            return AccessResult<TModel>.Success(response);
        }

        public AccessResult<TModel> FetchUserById(Guid userId)
        {
            var response = _users.Where(s => s.Id == userId).FirstOrDefault();

            if (response == null)
            {
                return AccessResult<TModel>.Failed(new AccessError("Invalid user", StatusCodes.Status404NotFound));
            }

            return AccessResult<TModel>.Success(response);
        }

        public async Task<AccessResult<TModel>> FetchUserByUsernameAsync(string username)
        {
            var response = await _users.Where(s => s.UserName == username).FirstOrDefaultAsync();

            if (response == null)
            {
                return AccessResult<TModel>.Failed(new AccessError("Invalid user", StatusCodes.Status404NotFound));
            }

            return AccessResult<TModel>.Success(response);
        }

        public AccessResult<TModel> FetchUserByUsername(string username)
        {
            var response = _users.Where(s => s.UserName == username).FirstOrDefault();

            if (response == null)
            {
                return AccessResult<TModel>.Failed(new AccessError("Invalid user", StatusCodes.Status404NotFound));
            }

            return AccessResult<TModel>.Success(response);
        }

        private readonly DbSet<TModel> _users;
    }
}
