using VillaManager.Data.EntityModel;
using VillaManager.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace VillaManager.Data.Repositories
{
    public class GenericRepository<T>: IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, Func<IQueryable<T>, IQueryable<T>> include = null)
        {
            IQueryable<T> query = _dbSet;


            // Apply filter
            if (filter != null)
            {
                query = query.Where(filter);
            }
            // Apply includes
            if (include != null)
            {
                query = include(query);

            }

            return await query.ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id, Func<IQueryable<T>, IQueryable<T>> include = null)
        {
            IQueryable<T> query = _dbSet;

            if (include != null)
            {
                query = include(query);
                
            }

            return await query.FirstOrDefaultAsync(entity => EF.Property<int>(entity, "Id") == id);
        }
        public async Task<T> GetElementByFilterAsync(Expression<Func<T, bool>>? filter = null, Func<IQueryable<T>, IQueryable<T>> include = null)
        {
            IQueryable<T> query = _dbSet;

            if (include != null)
            {
                query = include(query);

            }
            // Apply filter
            if (filter != null)
            {
                query = query.Where(filter);
            }
            return await query.FirstOrDefaultAsync();
        }

        public async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                //_dbSet.Remove(entity);
                //await _context.SaveChangesAsync();
                // Assuming the entity has an 'IsDeleted' property
                var isDeletedProperty = typeof(T).GetProperty("IsDeleted");

                if (isDeletedProperty != null )
                {
                    isDeletedProperty.SetValue(entity, true); // Set IsDeleted to true
                    _dbSet.Update(entity); // Mark entity as modified
                    await _context.SaveChangesAsync();
                }
                else
                {
                    throw new InvalidOperationException($"Entity {typeof(T).Name} does not support soft delete.");
                }
            }
        }
        public async Task HardDeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);  // Perform physical removal from DbSet
                await _context.SaveChangesAsync();  // Commit changes to the database
            }
            else
            {
                throw new InvalidOperationException($"Entity with id {id} not found for hard deletion.");
            }
        }
        public async Task<T> FindAsync(
            Expression<Func<T, bool>> filter,
            Func<IQueryable<T>, IQueryable<T>> include = null)
        {
            IQueryable<T> query = _dbSet;

            // Apply includes if any
            if (include != null)
            {
                query = include(query);
            }

            // Apply filter
            if (filter != null)
            {
                query = query.Where(filter);
            }

            // Return the first or default match
            return await query.FirstOrDefaultAsync();
        }


    public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities)
    {
        if (entities == null || !entities.Any())
        {
            throw new ArgumentException("Entities cannot be null or empty", nameof(entities));
        }

        await _dbSet.AddRangeAsync(entities);  // Add the entities in bulk
        await _context.SaveChangesAsync();  // Save the changes to the database
        return entities;  // Return the added entities
    }

    public IQueryable<T> GetAllQueryable()
    {
        return _dbSet.AsQueryable();
    }


    }
}
