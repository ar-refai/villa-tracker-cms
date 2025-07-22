using VillaManager.Data.EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VillaManager.Data.Interfaces
{
    public interface IUnitOfWork
    {
        Task SaveAsync();
        Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action);
    }
}
