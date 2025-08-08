using VillaManager.Data.EntityModel;
using VillaManager.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VillaManager.Data.Models;

namespace VillaManager.Data.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }
        public ApplicationDbContext Context => _context; 
        public IGenericRepository<Villa> Villas => new GenericRepository<Villa>(_context);
        public IGenericRepository<VillaFile> VillaFiles => new GenericRepository<VillaFile>(_context);


        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                T result = await action();
                await transaction.CommitAsync();
                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
