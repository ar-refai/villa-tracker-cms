using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;

namespace VillaManager.Client.Models
{
    public class VillaCreateDto
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public List<VillaFileUploadDto> Files { get; set; } = new();
    }
    public class VillaFileUploadDto
    {
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] Data { get; set; }
    }
}