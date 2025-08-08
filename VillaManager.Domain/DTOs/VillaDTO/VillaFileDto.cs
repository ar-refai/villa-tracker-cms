using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VillaManager.Domain.DTOs.VillaDTO
{
    public class VillaFileDto
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FileType { get; set; }
    }
}