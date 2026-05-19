using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace E_Shopping.Domain.Interfaces.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<int> SaveChangesAsync();
        Task<T> GetByIdAsync(int id);
        Task<List<T>> GetAllAsync();
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        void DeleteRange(IEnumerable<T> entities);
        IQueryable<T> GetQueryable();

        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

        Task<List<T>> GetWhereAsync(Expression<Func<T, bool>> predicate);

        Task<List<T>> GetWhereWithIncludeAsync(
            Expression<Func<T, bool>> predicate,
            params Expression<Func<T, object>>[] includes
        );
        Task<T> GetSingleWithIncludeAsync(
            Expression<Func<T, bool>> predicate,
            params Expression<Func<T, object>>[] includes
        );
        Task<List<T>> GetAllWithIncludeAsync(params Expression<Func<T, object>>[] includes);
    }
}