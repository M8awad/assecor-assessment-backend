using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PersonsManager.Repository.Interface
{
    public interface IBaseRepository
    {
        // Get all
        List<T> GetAll<T>(bool withTracking = true) where T : class;
        Task<List<T>> GetAllAsync<T>(bool withTracking = true) where T : class;

        // Get all where
        List<T> GetAllWhere<T>(Expression<Func<T, bool>> predicate, bool withTracking = true) where T : class;
        Task<List<T>> GetAllWhereAsync<T>(Expression<Func<T, bool>> predicate, bool withTracking = true) where T : class;

        // First or default
        T FirstOrDefault<T>(Expression<Func<T, bool>> predicate, bool withTracking = true) where T : class;
        Task<T> FirstOrDefaultAsync<T>(Expression<Func<T, bool>> predicate, bool withTracking = true) where T : class;

        // Add
        void Add<T>(T entity) where T : class;
        Task AddAsync<T>(T entity) where T : class;

        // Update
        void Update<T>(T entity) where T : class;

        // Remove
        void Remove<T>(T entity) where T : class;

        // Save changes
        bool SaveChanges();
        Task<bool> SaveChangesAsync();
    }
}
