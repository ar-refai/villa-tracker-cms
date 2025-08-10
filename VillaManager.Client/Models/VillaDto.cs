using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VillaManager.Client.Models
{
    public class VillaDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public List<VillaFileDto> Files { get; set; } = new();
    }

    public class VillaFileDto
    {
        public int Id { get; set; }   // <-- this
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FileType { get; set; }
    }
}