using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace VillaManager.Data.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, Func<IQueryable<T>, IQueryable<T>> include = null);
        Task<T> GetByIdAsync(int id, Func<IQueryable<T>, IQueryable<T>> include = null);
        Task<T> GetElementByFilterAsync(Expression<Func<T, bool>>? filter = null, Func<IQueryable<T>, IQueryable<T>> include = null);
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
        Task HardDeleteAsync(int id);
        Task<T> FindAsync(
            Expression<Func<T, bool>> filter,
            Func<IQueryable<T>, IQueryable<T>> include = null
        );
        Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);
        IQueryable<T> GetAllQueryable();

    }
}
