using Microsoft.EntityFrameworkCore;
using PersonsManager.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PersonsManager.Repository.Implementation
{
    public class BaseRepository : IBaseRepository
    {
        protected readonly DbContext _context;

        public BaseRepository(DbContext context)
        {
            _context = context;
        }

        public List<T> GetAll<T>(bool withTracking = true) where T : class
        {
            IQueryable<T> query = _context.Set<T>();
            if (!withTracking)
                query = query.AsNoTracking();
            return query.ToList();
        }

        public async Task<List<T>> GetAllAsync<T>(bool withTracking = true) where T : class
        {
            IQueryable<T> query = _context.Set<T>();
            if (!withTracking)
                query = query.AsNoTracking();
            return await query.ToListAsync();
        }

        public List<T> GetAllWhere<T>(Expression<Func<T, bool>> predicate, bool withTracking = true) where T : class
        {
            IQueryable<T> query = _context.Set<T>();
            if (!withTracking)
                query = query.AsNoTracking();
            return query.Where(predicate).ToList();
        }

        public async Task<List<T>> GetAllWhereAsync<T>(Expression<Func<T, bool>> predicate, bool withTracking = true) where T : class
        {
            IQueryable<T> query = _context.Set<T>();
            if (!withTracking)
                query = query.AsNoTracking();
            return await query.Where(predicate).ToListAsync();
        }

        public T FirstOrDefault<T>(Expression<Func<T, bool>> predicate, bool withTracking = true) where T : class
        {
            IQueryable<T> query = _context.Set<T>();
            if (!withTracking)
                query = query.AsNoTracking();
            return query.FirstOrDefault(predicate);
        }

        public async Task<T> FirstOrDefaultAsync<T>(Expression<Func<T, bool>> predicate, bool withTracking = true) where T : class
        {
            IQueryable<T> query = _context.Set<T>();
            if (!withTracking)
                query = query.AsNoTracking();
            return await query.FirstOrDefaultAsync(predicate);
        }

        public void Add<T>(T entity) where T : class
        {
            _context.Set<T>().Add(entity);
        }

        public async Task AddAsync<T>(T entity) where T : class
        {
            await _context.Set<T>().AddAsync(entity);
        }

        public async Task AddRangeAsync<T>(IEnumerable<T> entities) where T : class
        {
            await _context.Set<T>().AddRangeAsync(entities);
        }

        public void RemoveRange<T>(IEnumerable<T> entities) where T : class
        {
            _context.Set<T>().RemoveRange(entities);
        }

        public void Update<T>(T entity) where T : class
        {
            _context.Set<T>().Update(entity);
        }

        public void Remove<T>(T entity) where T : class
        {
            _context.Set<T>().Remove(entity);
        }

        public bool SaveChanges()
        {
            return _context.SaveChanges() > 0;
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
