using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using VillaManager.Data.Models;

namespace VillaManager.Data.EntityModel
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        // public DbSet<Catering> Caterings { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Villa> Villas { get; set; }
        public DbSet<VillaFile> VillaFiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {


            base.OnModelCreating(modelBuilder);
        }
    }
}
