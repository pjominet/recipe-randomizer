using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        protected T FindFirstOrDefault<T>() where T : class
        {
            return FindAll<T>().FirstOrDefault();
        }

        protected T FindFirstOrDefault<T>(params string[] includes) where T : class
        {
            return FindAll<T>(includes).FirstOrDefault();
        }

        protected T FindFirstOrDefault<T>(Expression<Func<T, bool>> expression) where T : class
        {
            return FindAll(expression).FirstOrDefault();
        }

        protected T FindFirstOrDefault<T>(Expression<Func<T, bool>> expression, params string[] includes) where T : class
        {
            return FindAll(expression, includes).FirstOrDefault();
        }

        #endregion

        #region public

        public IEnumerable<T> GetAll<T>(Expression<Func<T, bool>> expression = null, params string[] includes) where T : class
        {
            return expression == null ? FindAll<T>(includes) : FindAll(expression, includes);
        }

        public T GetFirstOrDefault<T>(Expression<Func<T, bool>> expression = null, params string[] includes) where T : class
        {
            return expression == null ? FindFirstOrDefault<T>(includes) : FindFirstOrDefault(expression, includes);
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

        public bool SaveChanges()
        {
            try
            {
                Context.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
                return false;
            }
        }

        #endregion

    }
}
