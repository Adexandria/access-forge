using Microsoft.EntityFrameworkCore;

namespace AdeAuth.Services
{
    public class Repository<TDbContext,TModel>
        where TDbContext : DbContext
    {
        public Repository(TDbContext dbContext)
        {
            Db = dbContext;
        }
        /// <summary>
        /// Save changes that support concurrency exception asynchronous
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        protected async Task<bool> SaveChangesAsync()
        {

            var saved = false;
            while (!saved)
            {
                try
                {
                    int commitedResult = await Db.SaveChangesAsync();
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
                        if (entry.Entity is TModel)
                        {
                            var proposedValues = entry.CurrentValues;

                            var databaseValues = entry.GetDatabaseValues();

                            if (databaseValues == null)
                                return saved;

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


        /// <summary>
        /// Save changes that support concurrency exception
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        protected bool SaveChanges()
        {

            var saved = false;
            while (!saved)
            {
                try
                {
                    int commitedResult =  Db.SaveChanges();
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
                        if (entry.Entity is TModel)
                        {
                            var proposedValues = entry.CurrentValues;

                            var databaseValues = entry.GetDatabaseValues();

                            if (databaseValues == null)
                                return saved;

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

        public readonly TDbContext Db;
    }
}