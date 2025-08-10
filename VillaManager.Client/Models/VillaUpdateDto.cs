using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;

namespace VillaManager.Client.Models
{
    public class VillaUpdateDto
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public List<int> ExistingFileIds { get; set; } = new();
        public List<IBrowserFile> NewFiles { get; set; } = new();
    }
}