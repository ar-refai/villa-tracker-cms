using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using VillaManager.Domain.DTOs.UsersDTO;

namespace VillaManager.Domain.DTOs.VillaDTO
{
    public class VillaDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;
        // one or more files can be associated with a villa
        public List<VillaFileDto> Files { get; set; } = new();
        // New properties added for creator
        public ShowUserDto? Creator { get; set; } // Navigation property to User
        public string CreatorId { get; set; } // Foreign key to User
    }
}