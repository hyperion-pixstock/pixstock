using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Hyperion.Pf.Entity.Repository
{
    /// <summary>
    /// エンティティを取得するリソース
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IGenericRepository<T> where T : class
    {

        DbContext Entities { get; }

        T Add(T entity);

        T Delete(T entity);

        void Edit(T entity);

        IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate);

        IEnumerable<T> GetAll();

        void Save();

    }
}