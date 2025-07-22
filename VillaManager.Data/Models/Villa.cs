using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VillaManager.Data.Models
{
    public class Villa
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<VillaFile> Files { get; set; }

    }
}