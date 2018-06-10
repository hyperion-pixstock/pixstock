using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Hyperion.Pf.Entity.Repository
{
    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class GenericRepository<T> : IGenericRepository<T>
        where T : class
    {

        protected readonly DbSet<T> _dbset;

        protected DbContext _entities;

        public GenericRepository(DbContext context)
        {
            _entities = context;
            _dbset = context.Set<T>();
        }

        public DbContext Entities { get { return _entities; } }

        public virtual T Add(T entity)
        {
            return _dbset.Add(entity).Entity;
        }

        public virtual T Delete(T entity)
        {
            return _dbset.Remove(entity).Entity;
        }

        public virtual void Edit(T entity)
        {
            _entities.Entry(entity).State = EntityState.Modified;
        }

        public IEnumerable<T> FindBy(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
        {
            IEnumerable<T> query = _dbset.Where(predicate).AsEnumerable();
            return query;
        }

        public virtual IEnumerable<T> GetAll()
        {
            return _dbset.AsEnumerable<T>();
        }

        public virtual void Save()
        {
            _entities.SaveChanges();
        }

    }
}