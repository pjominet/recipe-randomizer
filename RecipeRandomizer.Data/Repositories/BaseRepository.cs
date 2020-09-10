using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace RecipeRandomizer.Data.Repositories
{
    public class BaseRepository<TContext> where TContext : DbContext
    {
        protected TContext Context { get; }

        protected BaseRepository(TContext context)
        {
            Context = context;
        }

        #region protected

        protected IQueryable<T> FindAll<T>() where T : class
        {
            return Context.Set<T>();
        }

        protected IQueryable<T> FindAll<T>(Expression<Func<T, bool>> expression) where T : class
        {
            return Context.Set<T>().Where(expression);
        }

        protected IQueryable<T> FindAll<T>(params string[] includes) where T : class
        {
            IQueryable<T> dbQuery = Context.Set<T>();
            return includes.Aggregate(dbQuery, (current, include) => current.Include(include));
        }

        protected IQueryable<T> FindAll<T>(Expression<Func<T, bool>> expression, params string[] includes) where T : class
        {
            IQueryable<T> dbQuery = Context.Set<T>();

            dbQuery = includes.Aggregate(dbQuery, (current, include) => current.Include(include));
            dbQuery = dbQuery.Where(expression);

            return dbQuery;
        }

        protected async Task<T> FindFirstOrDefaultAsync<T>() where T : class
        {
            return await FindAll<T>().FirstOrDefaultAsync();
        }

        protected async Task<T> FindFirstOrDefaultAsync<T>(params string[] includes) where T : class
        {
            return await FindAll<T>(includes).FirstOrDefaultAsync();
        }

        protected async Task<T> FindFirstOrDefaultAsync<T>(Expression<Func<T, bool>> expression) where T : class
        {
            return await FindAll(expression).FirstOrDefaultAsync();
        }

        protected async Task<T> FindFirstOrDefaultAsync<T>(Expression<Func<T, bool>> expression, params string[] includes) where T : class
        {
            return await FindAll(expression, includes).FirstOrDefaultAsync();
        }

        #endregion

        #region public

        public async Task<IEnumerable<T>> GetAllAsync<T>(Expression<Func<T, bool>> expression = null, params string[] includes) where T : class
        {
            return expression == null
                ? await FindAll<T>(includes).ToListAsync()
                : await FindAll(expression, includes).ToListAsync();
        }

        public async Task<T> GetFirstOrDefaultAsync<T>(Expression<Func<T, bool>> expression = null, params string[] includes) where T : class
        {
            return expression == null
                ? await FindFirstOrDefaultAsync<T>(includes)
                : await FindFirstOrDefaultAsync(expression, includes);
        }

        public bool Exists<T>(Func<T, bool> expression) where T : class
        {
            return Context.Set<T>().Local.Any(expression);
        }

        public void Insert<T>(T entity) where T : class
        {
            Context.Set<T>().Add(entity);
        }

        public void Update<T>(T entity) where T : class
        {
            Context.Entry(entity).State = EntityState.Modified;
        }

        public void Delete<T>(T entity) where T : class
        {
            Context.Set<T>().Remove(entity);
        }

        public async Task<bool> SaveChanges()
        {
            try
            {
                await Context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                await Console.Error.WriteLineAsync(e.Message);
                return false;
            }
        }

        #endregion

    }
}
