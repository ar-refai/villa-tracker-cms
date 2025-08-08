using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace VillaManager.Domain.DTOs.VillaDTO
{
    public class VillaUpdateDto
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public List<int> ExistingFileIds { get; set; } // IDs of files to keep
        public List<IFormFile> NewFiles { get; set; } // New uploads
    }
}