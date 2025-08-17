using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.BaseRepository
{
    public interface IBaseRepository<T> where T : class
    {
        IQueryable<T> GetAll();
        // biding data        
        IEnumerable<T> Find(Expression<Func<T, bool>> predicate);
        Task<T> GetSingleByCondition(Expression<Func<T, bool>> expression, string[] includes = null);
        IQueryable<T> Query(Expression<Func<T, bool>> predicate);
        Task<List<T>> GetMulti(Expression<Func<T, bool>> predicate, string[] includes = null);
        Task<List<T>> GetMultiAsNoTracking(Expression<Func<T, bool>> predicate, string[] includes = null);
        IEnumerable<T> FindWithInclude(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, bool tracking = false, params Expression<Func<T, object>>[] includes);
        T FirstOrDefault(Expression<Func<T, bool>> predicate);
        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
        void Add(T entity);
        Task AddAsync(T entity);
        void AddRange(IEnumerable<T> entities);
        Task AddRangeAsync(IEnumerable<T> entities);
        void Update(T entity);
        void UpdateRange(IEnumerable<T> entities);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
        Task SaveChanges();
        Task<T> SaveChangesAsync(T entity);
    }
}
