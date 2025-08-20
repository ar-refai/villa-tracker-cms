using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using VillaManager.Data.Interfaces;
using VillaManager.Data.Models;
using VillaManager.Domain.DTOs.VillaDTO;
using VillaManager.Services.Interfaces;

namespace VillaManager.Services.Services
{
    public class VillaService : IVillaService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly string _uploadsPath = Path.Combine("wwwroot", "uploads", "villas");
        private readonly IHttpContextAccessor _httpContextAccessor;
        public VillaService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<VillaDto>> GetAllAsync(string? location = null, string? name = null)
        {
            var query = _unitOfWork.Villas.GetAllQueryable().Where(v => !v.IsDeleted);

            if (!string.IsNullOrWhiteSpace(location))
                query = query.Where(v => v.Location.Contains(location));

            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(v => v.Name.Contains(name));

            var villas = await query
                .Include(v => v.Creator)
                .Include(v => v.Files)
                .ToListAsync();

            return _mapper.Map<IEnumerable<VillaDto>>(villas);
        }


        public async Task<VillaDto> GetByIdAsync(int id)
        {
            var villa = await _unitOfWork.Villas.FindAsync(
                v => v.Id == id && !v.IsDeleted,
                include: v => v.Include(v => v.Files)
                .Include(v => v.Creator)
            );
            if (villa == null || villa.IsDeleted)
                throw new KeyNotFoundException($"Villa with ID {id} not found.");

            return _mapper.Map<VillaDto>(villa);
        }

        public async Task<VillaDto> CreateAsync(VillaCreateDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            // map scalar fields from dto
            var villa = _mapper.Map<Villa>(dto);

            // get current user id from claims
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                throw new InvalidOperationException("Cannot determine the current user id from the request context.");

            villa.CreatorId = userId;
            villa.CreatedAt = DateTime.UtcNow;
            villa.UpdatedAt = DateTime.UtcNow;
            villa.IsDeleted = false;

            // make sure collection is ready
            villa.Files ??= new List<VillaFile>();

            await _unitOfWork.Villas.AddAsync(villa);
            await _unitOfWork.SaveAsync(); // ensure villa.Id

            // handle uploads
            if (dto.Files != null && dto.Files.Any())
            {
                var saved = await SaveVillaFilesAsync(dto.Files, villa.Id, villa.Name);
                foreach (var f in saved)
                    villa.Files.Add(f);

                await _unitOfWork.SaveAsync();
            }

            // reload including navs
            var created = await _unitOfWork.Villas.GetByIdAsync(
                villa.Id,
                include: q => q.Include(v => v.Creator).Include(v => v.Files)
            );

            return _mapper.Map<VillaDto>(created);
        }


        public async Task<bool> UpdateAsync(int id, VillaUpdateDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var villa = await _unitOfWork.Villas.GetByIdAsync(
                id,
                include: q => q.Include(v => v.Files));

            if (villa == null || villa.IsDeleted)
                throw new KeyNotFoundException($"Villa with ID {id} not found.");

            // map scalar fields only (Files handled manually by us)
            _mapper.Map(dto, villa);
            villa.UpdatedAt = DateTime.UtcNow;
            villa.Files ??= new List<VillaFile>();

            // 1) Remove files that are not in ExistingFileIds
            var keepIds = new HashSet<int>(dto.ExistingFileIds ?? new List<int>());
            var toDelete = villa.Files.Where(f => !keepIds.Contains(f.Id)).ToList();

            foreach (var f in toDelete)
            {
                DeletePhysicalFile(f.FilePath);
                await _unitOfWork.VillaFiles.HardDeleteAsync(f.Id);
            }

            if (toDelete.Count > 0)
            {
                // keep in-memory collection consistent
                villa.Files = villa.Files.Where(f => keepIds.Contains(f.Id)).ToList();
                await _unitOfWork.SaveAsync(); // commit deletions before inserting new ones
            }

            // 2) Add new files
            if (dto.NewFiles != null && dto.NewFiles.Any())
            {
                var uploaded = await SaveVillaFilesAsync(dto.NewFiles, villa.Id, villa.Name);
                foreach (var f in uploaded)
                    villa.Files.Add(f);

                await _unitOfWork.SaveAsync(); // commit new VillaFile rows
            }

            // 3) Persist scalar changes on the Villa row
            await _unitOfWork.Villas.UpdateAsync(villa);
            await _unitOfWork.SaveAsync();

            return true;
        }




        public async Task<bool> DeleteAsync(int id)
        {
            var villa = await _unitOfWork.Villas.GetByIdAsync(id,
                include: q => q.Include(v => v.Files));

            if (villa == null || villa.IsDeleted)
                throw new KeyNotFoundException($"Villa with ID {id} not found.");

            // Delete physical files
            foreach (var file in villa.Files)
            {
                DeletePhysicalFile(file.FilePath);
                await _unitOfWork.VillaFiles.HardDeleteAsync(file.Id);
            }

            await _unitOfWork.Villas.DeleteAsync(id); // soft delete
            return true;
        }


        private void DeletePhysicalFile(string filePath)
        {
            if (!string.IsNullOrWhiteSpace(filePath) && File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }



        private async Task<List<VillaFile>> SaveVillaFilesAsync(IEnumerable<IFormFile> files, int villaId, string villaName)
        {
            Directory.CreateDirectory(_uploadsPath);
            var villaFiles = new List<VillaFile>();

            foreach (var file in files)
            {
                var safeFileName = $"{villaName}-{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var filePath = Path.Combine(_uploadsPath, safeFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                    await file.CopyToAsync(stream);

                var villaFile = new VillaFile
                {
                    FileName = Path.GetFileName(file.FileName),
                    FilePath = $"/uploads/villas/{safeFileName}",
                    FileType = file.ContentType,
                    VillaId = villaId
                };

                villaFiles.Add(villaFile);
                await _unitOfWork.VillaFiles.AddAsync(villaFile);
            }

            return villaFiles;
        }
    }

}
