using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VillaManager.Data.EntityModel;

namespace VillaManager.Data.Models
{
    public class Villa
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public bool IsDeleted { get; set; } = false;

        // New properties added for creator 
        public string CreatorId { get; set; }                                  //string to match PK :| Foreign key to User  *** new
        public User Creator { get; set; }                                   // Navigation property to User *** new

        // Navigation property to files associated with the villa
        public ICollection<VillaFile> Files { get; set; }

        // Timestamps for creation and updates
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;      // Optional UTC timestamp for updated at  *** new
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;


    }
}