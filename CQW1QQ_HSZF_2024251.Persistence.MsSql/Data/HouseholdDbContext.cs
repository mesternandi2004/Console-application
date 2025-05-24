using CQW1QQ_HSZF_2024251.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace CQW1QQ_HSZF_2024251.Persistence.MsSql.Data
{
    public class HouseholdDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<Pantry> Pantry { get; set; }
        public DbSet<Fridge> Fridge { get; set; }

        public HouseholdDbContext()
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string cnnStr = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=householdDb;Integrated Security=True;MultipleActiveResultSets=True";
            optionsBuilder.UseSqlServer(cnnStr).UseLazyLoadingProxies(true);

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .Property(p => p.Id)
                .ValueGeneratedNever();

            modelBuilder.Entity<Pantry>()
                .HasBaseType<Storage>()
                .HasMany(p => p.Products)
                .WithOne(p => (Pantry)p.Storage)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Fridge>()
                .HasBaseType<Storage>()
                .HasMany(p => p.Products)
                .WithOne(p => (Fridge)p.Storage)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Storage)
                .WithMany(s => s.Products)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Person>()
                .HasMany(p => p.FavoriteProducts);

            base.OnModelCreating(modelBuilder);
        }
    }
}
