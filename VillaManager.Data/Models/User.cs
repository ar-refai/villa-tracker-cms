using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using VillaManager.Data.Models;

namespace VillaManager.Data.EntityModel
{

    public partial class User : IdentityUser
    {
        public string? Name { get; set; }
        public bool IsDeleted { get; set; } = false;
        public bool isActive { get; set; } = true;
        public IList<Villa> Villas { get; set; } = new List<Villa>();   // Navigation property to Villas created by the user *** new 
    }

}


