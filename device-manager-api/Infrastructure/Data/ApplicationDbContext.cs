using device_manager_api.Domain.Entities;
using device_manager_api.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace device_manager_api.Infrastructure.Data;

/// <summary>
/// Database context for the Device Manager API
/// </summary>
public class ApplicationDbContext : DbContext
{
    private readonly IHttpContextAccessor? _httpContextAccessor;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor? httpContextAccessor = null)
        : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public DbSet<Device> Devices { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Device configuration
        modelBuilder.Entity<Device>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Brand).IsRequired().HasMaxLength(50);
            entity.Property(e => e.State).IsRequired();
            entity.Property(e => e.CreationTime).IsRequired();
            entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
            entity.Property(e => e.RowVersion).IsRowVersion();

            // Indexes for performance
            entity.HasIndex(e => e.Brand);
            entity.HasIndex(e => e.State);
            entity.HasIndex(e => new { e.Brand, e.State });
        });

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.Role).IsRequired();
            entity.Property(e => e.RefreshToken).HasMaxLength(500);

            // Unique index on username
            entity.HasIndex(e => e.Username).IsUnique();
        });

        // Seed default users
        SeedUsers(modelBuilder);
    }

    private void SeedUsers(ModelBuilder modelBuilder)
    {
        var users = new[]
        {
            new User
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Username = "admin1",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123", 12),
                Role = UserRole.Admin
            },
            new User
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111112"),
                Username = "admin2",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@456", 12),
                Role = UserRole.Admin
            },
            new User
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222221"),
                Username = "manager1",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Manager@123", 12),
                Role = UserRole.Manager
            },
            new User
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Username = "manager2",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Manager@456", 12),
                Role = UserRole.Manager
            },
            new User
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333331"),
                Username = "viewer1",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Viewer@123", 12),
                Role = UserRole.Viewer
            },
            new User
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333332"),
                Username = "viewer2",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Viewer@456", 12),
                Role = UserRole.Viewer
            }
        };

        modelBuilder.Entity<User>().HasData(users);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Auto-populate audit fields
        var username = _httpContextAccessor?.HttpContext?.User?.Identity?.Name;
        
        if (!string.IsNullOrEmpty(username))
        {
            var entries = ChangeTracker.Entries<Device>()
                .Where(e => e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                entry.Entity.ModifiedBy = username;
                entry.Entity.ModifiedAt = DateTime.UtcNow;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
