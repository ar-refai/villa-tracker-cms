using VillaManager.Data.EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VillaManager.Data.Models;

namespace VillaManager.Data.Interfaces
{
    public interface IUnitOfWork
    {
        IGenericRepository<Villa> Villas { get; }
        IGenericRepository<VillaFile> VillaFiles { get; }


        Task SaveAsync();
        Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action);
    }
}
