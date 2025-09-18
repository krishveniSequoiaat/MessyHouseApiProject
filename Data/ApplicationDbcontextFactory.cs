using Microsoft.EntityFrameworkCore;
using MessyHouseAPIProject.Models;
using Microsoft.EntityFrameworkCore.Design;

namespace MessyHouseAPIProject.Data
{
    public class ApplicationDbcontextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlite("Data Source=MessyHouseDb.db");

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}