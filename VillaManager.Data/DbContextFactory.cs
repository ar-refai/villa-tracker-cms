using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace VillaManager.Data.EntityModel
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            // go up to the API project (adjust if folder structure is different)
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../VillaManager.Api");

            var config = new ConfigurationBuilder()
                .SetBasePath(basePath) // or hardcode your API path
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(config.GetConnectionString("DefaultConnection"));

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
