using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Repositories.BaseRepository
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private readonly DbContext _context;
        private readonly DbSet<T> _dbSet;

        public BaseRepository(DbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        public virtual async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public virtual void AddRange(IEnumerable<T> entities)
        {
            _context.Set<T>().AddRange(entities);
        }

        public virtual async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _context.Set<T>().AddRangeAsync(entities);
        }

        public virtual IEnumerable<T> Find(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate);
        }

        public virtual IEnumerable<T> FindWithInclude(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            var query = _dbSet.Where(predicate);
            return includes.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
        }

        public async Task<T> GetSingleByCondition(Expression<Func<T, bool>> expression, string[] includes = null)
        {
            if (includes != null && includes.Count() > 0)
            {
                var query = _dbSet.Include(includes.First());
                foreach (var include in includes.Skip(1))
                    query = query.Include(include);
                var buildQuery = await query.FirstOrDefaultAsync(expression);
                return buildQuery;
            }

            var buildQ = await _dbSet.FirstOrDefaultAsync(expression);
            return buildQ;
        }

        public virtual async Task<IEnumerable<T>> FindAsync(
            Expression<Func<T, bool>> predicate,
            bool tracking = false,
            params Expression<Func<T, object>>[] includes)
        {
            var query = _dbSet.Where(predicate);

            if (!tracking)
            {
                query = query.AsNoTracking();
            }

            if (includes == null || !includes.Any())
            {
                return await query.ToArrayAsync();
            }

            return await includes.Aggregate(query, (current, includeProperty) => current.Include(includeProperty)).ToArrayAsync();
        }

        public virtual T Get(int id)
        {
            return _dbSet.Find(id);
        }

        public virtual T Get(string id)
        {
            return _dbSet.Find(id);
        }

        public virtual IQueryable<T> GetAll()
        {
            return _dbSet;
        }

        public virtual IQueryable<T> Query(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate);
        }

        public async Task<List<T>> GetMulti(Expression<Func<T, bool>> predicate, string[] includes = null)
        {
            //HANDLE INCLUDES FOR ASSOCIATED OBJECTS IF APPLICABLE
            if (includes != null && includes.Count() > 0)
            {
                var query = _dbSet.Include(includes.First());
                foreach (var include in includes.Skip(1))
                    query = query.Include(include);
                var buildQuery = await query.Where<T>(predicate).AsQueryable<T>().ToListAsync();
                return buildQuery;
            }

            var buildQ = await _dbSet.Where<T>(predicate).AsQueryable<T>().ToListAsync();
            return buildQ;
        }
        public async Task<List<T>> GetMultiAsNoTracking(Expression<Func<T, bool>> predicate, string[] includes = null)
        {
            if (includes != null && includes.Count() > 0)
            {
                var query = _dbSet.Include(includes.First());
                foreach (var include in includes.Skip(1))
                    query = query.Include(include);
                var buildQuery = await query.Where<T>(predicate).AsNoTracking().ToListAsync();
                return buildQuery;
            }

            var buildQ = await _dbSet.Where<T>(predicate).AsNoTracking().ToListAsync();
            return buildQ;
        }
        public virtual void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        public virtual void RemoveRange(IEnumerable<T> entities)
        {
            foreach (var entity in entities.ToList())
            {
                var entry = _context.Entry(entity);
                entry.State = EntityState.Deleted;
                _dbSet.Remove(entity);
            }
        }

        public virtual T FirstOrDefault(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.FirstOrDefault(predicate);
        }

        public virtual Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            if (includes == null || !includes.Any())
            {
                return _dbSet.FirstOrDefaultAsync(predicate);
            }

            return includes
                .Aggregate(_dbSet.Where(predicate), (current, includeProperty) => current.Include(includeProperty))
                .FirstOrDefaultAsync();
        }

        public virtual void Update(T entity)
        {
            var entry = _context.Entry(entity);
            _dbSet.Attach(entity);
            entry.State = EntityState.Modified;
        }

        public virtual void UpdateRange(IEnumerable<T> entities)
        {
            foreach (var entity in entities.ToList())
            {
                var entry = _context.Entry(entity);
                _dbSet.Attach(entity);
                entry.State = EntityState.Modified;
            }
        }

        public virtual Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.AnyAsync(predicate);
        }

        public virtual Task SaveChanges()
        {
            return _context.SaveChangesAsync();
        }

        public virtual async Task<T> SaveChangesAsync(T entity)
        {
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}
