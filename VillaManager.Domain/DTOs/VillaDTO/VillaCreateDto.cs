using Microsoft.AspNetCore.Http;

namespace VillaManager.Domain.DTOs.VillaDTO
{
    public class VillaCreateDto
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        // Files 
        public List<IFormFile> Files { get; set; } = new List<IFormFile>();
    }
}