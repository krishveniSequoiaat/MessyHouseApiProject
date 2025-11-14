using Microsoft.EntityFrameworkCore;
using MessyHouseAPIProject.Models;

namespace MessyHouseAPIProject.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Item> Items { get; set; }
        public DbSet<StorageBox> StorageBoxes { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Example: Primary Key for StorageBox
            modelBuilder.Entity<StorageBox>()
                .HasKey(b => b.Id);

            // Example: Unique constraint on Barcode
            modelBuilder.Entity<StorageBox>()
                .HasIndex(b => b.Barcode)
                .IsUnique();

            // Example: Primary Key for Item
            modelBuilder.Entity<Item>()
                .HasKey(i => i.Id);

            modelBuilder.Entity<Item>()
                .HasOne<StorageBox>()
                .WithMany()
                .HasPrincipalKey(b => b.Barcode)
                .HasForeignKey(i => i.Barcode);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

        }
    }
}