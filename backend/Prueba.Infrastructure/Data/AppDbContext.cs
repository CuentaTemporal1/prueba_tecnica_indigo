using Microsoft.EntityFrameworkCore;
using Prueba.Domain.Entities;
using System.Data;
using System.Reflection;

namespace Prueba.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is BaseEntity && (
                        e.State == EntityState.Added
                        || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                ((BaseEntity)entityEntry.Entity).UpdatedAt = DateTime.UtcNow;

                if (entityEntry.State == EntityState.Added)
                {
                    ((BaseEntity)entityEntry.Entity).CreatedAt = DateTime.UtcNow;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var seedDate = new DateTime(2025, 11, 1, 0, 0, 0, DateTimeKind.Utc);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

  
            var adminRole = new Role { Id = 1, Name = "Admin" };
            var userRole = new Role { Id = 2, Name = "User" };
            modelBuilder.Entity<Role>().HasData(adminRole, userRole);

            
            // NO AGREGUE REGISTRO POR TIEMPO DEJE ESTO AQUI PARA PROBAR att Kevin Bohorquez
            var adminUser = new User
            {
                Id = 1,
                Email = "admin@bohorquez.com", 
                FirstName = "Admin",
                LastName = "User",
                PasswordHash = "$2a$12$unsrbeaex4UoVADL9fRN7OFAWi23L397LFTwkj50GwrcBnFuLx2JC",
                ProfilePicturePath = null,
                CreatedAt = seedDate, 
                UpdatedAt = seedDate
            };
            modelBuilder.Entity<User>().HasData(adminUser);

            modelBuilder.Entity(
                "RoleUser", 
                j => j.HasData(new { RolesId = 1, UsersId = 1 }) 
            );
        }
    }
}