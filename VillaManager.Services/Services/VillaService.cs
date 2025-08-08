using System;
using System.Collections.Generic;
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

        public VillaService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<VillaDto>> GetAllAsync()
        {
            var villas = await _unitOfWork.Villas.GetAllAsync(v => !v.IsDeleted);
            return _mapper.Map<IEnumerable<VillaDto>>(villas);
        }

        public async Task<VillaDto> GetByIdAsync(int id)
        {
            var villa = await _unitOfWork.Villas.FindAsync(
                v => v.Id == id && !v.IsDeleted,
                include: v => v.Include(v => v.Files)
            );
            if (villa == null || villa.IsDeleted)
                throw new KeyNotFoundException($"Villa with ID {id} not found.");

            return _mapper.Map<VillaDto>(villa);
        }

        public async Task<VillaDto> CreateAsync(VillaCreateDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var villa = _mapper.Map<Villa>(dto);
            await _unitOfWork.Villas.AddAsync(villa);
            await _unitOfWork.SaveAsync(); // Save to get VillaId for file records

            if (dto.Files != null && dto.Files.Any())
                villa.Files = await SaveVillaFilesAsync(dto.Files, villa.Id, villa.Name);

            await _unitOfWork.SaveAsync();
            return _mapper.Map<VillaDto>(villa);
        }

        public async Task<bool> UpdateAsync(int id, VillaUpdateDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var villa = await _unitOfWork.Villas.GetByIdAsync(id, 
                include: q => q.Include(v => v.Files));

            if (villa == null || villa.IsDeleted)
                throw new KeyNotFoundException($"Villa with ID {id} not found.");

            // Map updated fields
            _mapper.Map(dto, villa);

            // Handle deleted files
            var existingFileIds = villa.Files.Select(f => f.Id).ToList();
            var keepFileIds = dto.ExistingFileIds ?? new List<int>();
            var filesToDelete = villa.Files.Where(f => !keepFileIds.Contains(f.Id)).ToList();

            foreach (var file in filesToDelete)
            {
                DeletePhysicalFile(file.FilePath);
                await _unitOfWork.VillaFiles.HardDeleteAsync(file.Id); // physical delete from DB
            }

            // Handle new file uploads
            if (dto.NewFiles != null && dto.NewFiles.Any())
            {
                var uploadedFiles = await SaveVillaFilesAsync(dto.NewFiles, villa.Id, villa.Name);
                foreach (var file in uploadedFiles)
                    villa.Files.Add(file);
            }

            await _unitOfWork.Villas.UpdateAsync(villa);
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



        private async Task<List<VillaFile>> SaveVillaFilesAsync(IEnumerable<IFormFile> files, int villaId,string villaName)
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
