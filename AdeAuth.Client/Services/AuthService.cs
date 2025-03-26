using AdeAuth.Client.Db;
using AdeAuth.Client.Models;
using AdeAuth.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AdeAuth.Client.Services
{
    public class AuthService : IAuthService
    {
        public AuthService(IdentityDbContext dbContext, IPasswordManager passwordManager)
        {
            _db = dbContext;
            _passwordManager = passwordManager;
        }

        public async Task<User> AuthenticateUsingEmailAsync(string email, string password)
        {
            var currentUser = await _db.Users.Where(s => s.Email == email)
                .Include(s=>s.Role).FirstOrDefaultAsync();
            if (currentUser == null)
            {
                return default;
            }

            var isPasswordCorrect = _passwordManager.VerifyPassword(password, currentUser.PasswordHash, currentUser.Salt);

            if (isPasswordCorrect)
            {
                return currentUser;
            }

            return default;
        }

        public async Task<User> AuthenticateUsingUsernameAsync(string username, string password)
        {
            var currentUser = await _db.Users.Where(s => s.UserName == username)
                .Include(s => s.Role).FirstOrDefaultAsync();
            if (currentUser == null)
            {
                return default;
            }

            var isPasswordCorrect = _passwordManager.VerifyPassword(password, currentUser.PasswordHash, currentUser.Salt);

            if (isPasswordCorrect)
            {
                return currentUser;
            }
            return default;
        }

        public async Task<bool> CreateUserAsync(User user)
        {
            _db.Users.Add(user);

            return await SaveChangesAsync();
        }

        public async Task<bool> IsUserExist(string email)
        {
            return  await _db.Users.AnyAsync(s=>s.Email == email);
        }

        public async Task<bool> IsUserExist(Guid id)
        {
            return await _db.Users.AnyAsync(s => s.Id == id);
        }


        private async Task<bool> SaveChangesAsync()
        {

            var saved = false;
            while (!saved)
            {
                try
                {
                    int commitedResult = await _db.SaveChangesAsync();
                    if (commitedResult == 0)
                    {
                        saved = false;
                        break;
                    }
                    saved = true;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    foreach (var entry in ex.Entries)
                    {
                        if (entry.Entity is User)
                        {
                            var proposedValues = entry.CurrentValues;
                            var databaseValues = entry.GetDatabaseValues();

                            foreach (var property in proposedValues.Properties)
                            {
                                var databaseValue = databaseValues[property];
                            }

                            entry.OriginalValues.SetValues(databaseValues);
                        }
                        else
                        {
                            throw new NotSupportedException(
                                "Don't know how to handle concurrency conflicts for "
                                + entry.Metadata.Name);
                        }
                    }
                }
            }
            return saved;

        }



        private readonly IdentityDbContext _db;

        private readonly IPasswordManager _passwordManager;
    }
}
