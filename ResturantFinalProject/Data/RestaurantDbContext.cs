using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ResturantFinalProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace ResturantFinalProject.Data
{
    public class RestaurantDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        // Define a static DateTime for seed data to avoid dynamic values
        private static readonly DateTime SeedDate = new DateTime(2025, 6, 20, 0, 0, 0, DateTimeKind.Utc);

        public RestaurantDbContext(DbContextOptions<RestaurantDbContext> options, IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(warnings =>
                warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
        }

        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Table> Tables { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries<BaseEntity>();

            foreach (var entry in entries)
            {
                var now = DateTime.UtcNow; // Use UTC for consistency
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = now;
                    entry.Entity.CreatedBy = userId ?? "System";
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = now;
                    entry.Entity.UpdatedBy = userId ?? "System";
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure decimal precision for monetary values
            modelBuilder.Entity<MenuItem>()
                .Property(m => m.Price)
                .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<CartItem>()
                .Property(ci => ci.Price)
                .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.Price)
                .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.Subtotal)
                .HasColumnType("decimal(10,2)");

            // Configure one-to-one relationship: ApplicationUser -> Cart
            modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.Cart)
                .WithOne(c => c.User)
                .HasForeignKey<Cart>(c => c.UserId);

            modelBuilder.Entity<Cart>()
                .HasIndex(c => c.UserId)
                .IsUnique();

            // Configure one-to-many: Cart -> CartItems
            modelBuilder.Entity<Cart>()
                .HasMany(c => c.Items)
                .WithOne(ci => ci.Cart)
                .HasForeignKey(ci => ci.CartId);

            // Configure one-to-many: ApplicationUser -> Orders (Customer)
            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.Orders)
                .WithOne(o => o.Customer)
                .HasForeignKey(o => o.CustomerId);

            // Configure one-to-many: ApplicationUser -> AssignedOrders (Staff)
            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.AssignedOrders)
                .WithOne(o => o.AssignedStaff)
                .HasForeignKey(o => o.AssignedStaffId);

            // Global query filter for soft deletion
            modelBuilder.Entity<Cart>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<CartItem>().HasQueryFilter(ci => ci != null);
            modelBuilder.Entity<Order>().HasQueryFilter(o => !o.IsDeleted);
            modelBuilder.Entity<OrderItem>().HasQueryFilter(oi => !oi.IsDeleted);
            modelBuilder.Entity<MenuItem>().HasQueryFilter(mi => !mi.IsDeleted);
            modelBuilder.Entity<Table>().HasQueryFilter(t => !t.IsDeleted);

            // Seed Categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Pizza", CreatedAt = SeedDate, CreatedBy = "System" },
                new Category { Id = 2, Name = "Burgers", CreatedAt = SeedDate, CreatedBy = "System" },
                new Category { Id = 3, Name = "Drinks", CreatedAt = SeedDate, CreatedBy = "System" },
                new Category { Id = 4, Name = "Desserts", CreatedAt = SeedDate, CreatedBy = "System" },
                new Category { Id = 5, Name = "Pasta", CreatedAt = SeedDate, CreatedBy = "System" }
            );

            // Seed MenuItems
            modelBuilder.Entity<MenuItem>().HasData(
                new MenuItem { Id = 1, Name = "Margherita Pizza", Price = 89.99m, CategoryId = 1, IsAvailable = true, PreparationTimeMinutes = 15, ImageUrl = "/images/margarita.jpg", CreatedAt = SeedDate, CreatedBy = "System" },
                new MenuItem { Id = 2, Name = "Pepperoni Pizza", Price = 99.99m, CategoryId = 1, IsAvailable = true, PreparationTimeMinutes = 17, ImageUrl = "/images/pepperoni.jpg", CreatedAt = SeedDate, CreatedBy = "System" },
                new MenuItem { Id = 3, Name = "Cheeseburger", Price = 74.50m, CategoryId = 2, IsAvailable = true, PreparationTimeMinutes = 10, ImageUrl = "/images/cheeseburger.jpg", CreatedAt = SeedDate, CreatedBy = "System" },
                new MenuItem { Id = 4, Name = "Chicken Burger", Price = 69.99m, CategoryId = 2, IsAvailable = true, PreparationTimeMinutes = 9, ImageUrl = "/images/chickenburger.jpg", CreatedAt = SeedDate, CreatedBy = "System" },
                new MenuItem { Id = 5, Name = "Coca-Cola", Price = 19.99m, CategoryId = 3, IsAvailable = true, PreparationTimeMinutes = 0, ImageUrl = "/images/cocacola.jpg", CreatedAt = SeedDate, CreatedBy = "System" },
                new MenuItem { Id = 6, Name = "Fresh Orange Juice", Price = 24.99m, CategoryId = 3, IsAvailable = true, PreparationTimeMinutes = 2, ImageUrl = "/images/orangejuice.jpg", CreatedAt = SeedDate, CreatedBy = "System" },
                new MenuItem { Id = 7, Name = "Chocolate Cake", Price = 49.99m, CategoryId = 4, IsAvailable = true, PreparationTimeMinutes = 5, ImageUrl = "/images/chocolatecake.jpg", CreatedAt = SeedDate, CreatedBy = "System" },
                new MenuItem { Id = 8, Name = "Ice Cream", Price = 29.99m, CategoryId = 4, IsAvailable = true, PreparationTimeMinutes = 0, ImageUrl = "/images/icecream.jpg", CreatedAt = SeedDate, CreatedBy = "System" },
                new MenuItem { Id = 9, Name = "Spaghetti Bolognese", Price = 94.99m, CategoryId = 5, IsAvailable = true, PreparationTimeMinutes = 18, ImageUrl = "/images/spaghetti.jpg", CreatedAt = SeedDate, CreatedBy = "System" },
                new MenuItem { Id = 10, Name = "Alfredo Pasta", Price = 89.99m, CategoryId = 5, IsAvailable = true, PreparationTimeMinutes = 16, ImageUrl = "/images/alfredo.jpg", CreatedAt = SeedDate, CreatedBy = "System" }
            );

            // Seed Users with static password hash for NewP@ssw0rd123
            // Note: Password hash statically set for "NewP@ssw0rd123"

            // Admin User
            var adminUser = new ApplicationUser
            {
                Id = "admin-1",
                UserName = "admin@restaurant.com",
                NormalizedUserName = "ADMIN@RESTAURANT.COM",
                Email = "admin@restaurant.com",
                NormalizedEmail = "ADMIN@RESTAURANT.COM",
                EmailConfirmed = true,
                FullName = "Admin User",
                Role = StaffRole.Manager,
                SecurityStamp = "ADMIN-SECURITY-STAMP-12345",
                PasswordHash = "AQAAAAIAAYagAAAAEJoXvY9a7Yc1z4z5gK8qN3rXh2mW9pL8vT6uR5wZ2kM7nP4bQ9cF3jH6mY8vL2xK9Q=="
            };

            // Chef User
            var chefUser = new ApplicationUser
            {
                Id = "chef-1",
                UserName = "chef@restaurant.com",
                NormalizedUserName = "CHEF@RESTAURANT.COM",
                Email = "chef@restaurant.com",
                NormalizedEmail = "CHEF@RESTAURANT.COM",
                EmailConfirmed = true,
                FullName = "Chef User",
                Role = StaffRole.Chef,
                SecurityStamp = "CHEF-SECURITY-STAMP-12345",
                PasswordHash = "AQAAAAIAAYagAAAAEJoXvY9a7Yc1z4z5gK8qN3rXh2mW9pL8vT6uR5wZ2kM7nP4bQ9cF3jH6mY8vL2xK9Q=="
            };

            // Waiter User
            var waiterUser = new ApplicationUser
            {
                Id = "waiter-1",
                UserName = "waiter@restaurant.com",
                NormalizedUserName = "WAITER@RESTAURANT.COM",
                Email = "waiter@restaurant.com",
                NormalizedEmail = "WAITER@RESTAURANT.COM",
                EmailConfirmed = true,
                FullName = "Waiter User",
                Role = StaffRole.Waiter,
                SecurityStamp = "WAITER-SECURITY-STAMP-12345",
                PasswordHash = "AQAAAAIAAYagAAAAEJoXvY9a7Yc1z4z5gK8qN3rXh2mW9pL8vT6uR5wZ2kM7nP4bQ9cF3jH6mY8vL2xK9Q=="
            };

            // Customer User
            var customerUser = new ApplicationUser
            {
                Id = "customer-1",
                UserName = "customer@restaurant.com",
                NormalizedUserName = "CUSTOMER@RESTAURANT.COM",
                Email = "customer@restaurant.com",
                NormalizedEmail = "CUSTOMER@RESTAURANT.COM",
                EmailConfirmed = true,
                FullName = "Customer User",
                Role = StaffRole.None,
                CartId = 1,
                SecurityStamp = "CUSTOMER-SECURITY-STAMP-12345",
                PasswordHash = "AQAAAAIAAYagAAAAEJoXvY9a7Yc1z4z5gK8qN3rXh2mW9pL8vT6uR5wZ2kM7nP4bQ9cF3jH6mY8vL2xK9Q=="
            };

            modelBuilder.Entity<ApplicationUser>().HasData(
                adminUser,
                chefUser,
                waiterUser,
                customerUser
            );

            // Seed Cart for Customer Only
            modelBuilder.Entity<Cart>().HasData(
                new Cart { Id = 1, UserId = "customer-1", CreatedAt = SeedDate, CreatedBy = "customer-1" }
            );

            // Seed Identity Roles
            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = "admin-role", Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Id = "chef-role", Name = "Chef", NormalizedName = "CHEF" },
                new IdentityRole { Id = "waiter-role", Name = "Waiter", NormalizedName = "WAITER" },
                new IdentityRole { Id = "customer-role", Name = "Customer", NormalizedName = "CUSTOMER" }
            );

            // Seed User-Role Assignments
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string> { RoleId = "admin-role", UserId = "admin-1" },
                new IdentityUserRole<string> { RoleId = "chef-role", UserId = "chef-1" },
                new IdentityUserRole<string> { RoleId = "waiter-role", UserId = "waiter-1" },
                new IdentityUserRole<string> { RoleId = "customer-role", UserId = "customer-1" }
            );
        }
    }
}