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

            // *** relation: User to Villa
            modelBuilder.Entity<Villa>()
                .HasOne(v => v.Creator)
                .WithMany(u => u.Villas)
                .HasForeignKey(v => v.CreatorId)
                .OnDelete(DeleteBehavior.Restrict);

            // *** relation: Villa to VillaFile
            modelBuilder.Entity<Villa>()
                .HasMany(v => v.Files)
                .WithOne(f => f.Villa)
                .HasForeignKey(f => f.VillaId)
                .OnDelete(DeleteBehavior.Cascade);
                
        }
    }
}
