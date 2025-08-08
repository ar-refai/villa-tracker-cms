using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VillaManager.Domain.DTOs.VillaDTO;

namespace VillaManager.Services.Interfaces
{
    public interface IVillaService
    {
        Task<IEnumerable<VillaDto>> GetAllAsync();
        Task<VillaDto> GetByIdAsync(int id);
        Task<VillaDto> CreateAsync(VillaCreateDto dto);
        Task<bool> UpdateAsync(int id, VillaUpdateDto dto);
        Task<bool> DeleteAsync(int id);

    }
}