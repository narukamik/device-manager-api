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
            entity.ToTable("devices"); // PostgreSQL convention: lowercase table names
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Brand).IsRequired().HasMaxLength(50);
            entity.Property(e => e.State).IsRequired();
            entity.Property(e => e.CreationTime).IsRequired();
            entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
            entity.Property(e => e.RowVersion).IsRowVersion().HasDefaultValue(new byte[] { 0, 0, 0, 0, 0, 0, 0, 1 });

            // Indexes for performance
            entity.HasIndex(e => e.Brand);
            entity.HasIndex(e => e.State);
            entity.HasIndex(e => new { e.Brand, e.State });
        });

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users"); // PostgreSQL convention: lowercase table names
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
        
        // Seed devices for development testing
        SeedDevices(modelBuilder);
    }

    private void SeedDevices(ModelBuilder modelBuilder)
    {
        var deviceTypes = new[] { "Laptop", "Smartphone", "Tablet", "Desktop", "Monitor", "Printer", "Scanner", "Router", "Server", "Smartwatch" };
        var brands = new[] { "Apple", "Samsung", "Dell", "HP", "Lenovo", "Sony", "LG", "Microsoft", "Google", "Asus" };
        var creationTime = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        
        var devices = new List<Device>();
        
        for (int i = 1; i <= 50; i++)
        {
            // Determine state: ~60% Available (1-30), ~30% InUse (31-45), ~10% Inactive (46-50)
            DeviceState state;
            if (i <= 30)
                state = DeviceState.Available;
            else if (i <= 45)
                state = DeviceState.InUse;
            else
                state = DeviceState.Inactive;
            
            devices.Add(new Device
            {
                Id = Guid.Parse($"aaaaaaaa-aaaa-aaaa-aaaa-{i:D12}"),
                Name = $"{deviceTypes[(i - 1) % deviceTypes.Length]} {i:D3}",
                Brand = brands[(i - 1) % brands.Length],
                State = state,
                CreationTime = creationTime,
                CreatedBy = "system",
                RowVersion = new byte[] { 0, 0, 0, 0, 0, 0, 0, 1 }
            });
        }
        
        modelBuilder.Entity<Device>().HasData(devices);
    }

    private void SeedUsers(ModelBuilder modelBuilder)
    {
        // Pre-computed BCrypt hashes (work factor 12) to ensure consistent seed data
        // These hashes are static and won't change between model builds
        // Passwords: Admin@123, Admin@456, Manager@123, Manager@456, Viewer@123, Viewer@456
        var users = new[]
        {
            new User
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Username = "admin1",
                PasswordHash = "$2a$12$yc7jeLROf1Gv/PpQbqQkgOnnOKWbHJZt17fcNZrbM5TkDFOiVtJta", // Admin@123
                Role = UserRole.Admin
            },
            new User
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111112"),
                Username = "admin2",
                PasswordHash = "$2a$12$IgdRp/rI37ALjB7qsKAu8Oo70B8ZM7GX1g.r031nJ96fJazRuLhPO", // Admin@456
                Role = UserRole.Admin
            },
            new User
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222221"),
                Username = "manager1",
                PasswordHash = "$2a$12$9KedhTiOP/MuS/9OVjTXmeV8.4WjfiGT4JO3qhFHNBlI/JaoqBbwa", // Manager@123
                Role = UserRole.Manager
            },
            new User
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Username = "manager2",
                PasswordHash = "$$2a$12$I1fLhtp9vplrLTNKEADFOO85IaQFg0nLyeoUUqPAGHTELD5/96Iii", // Manager@456
                Role = UserRole.Manager
            },
            new User
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333331"),
                Username = "viewer1",
                PasswordHash = "$2a$12$iHTaa1AYo6r2hJVJY876JeUzGQTf8tjFf32jSybFbbf7S6vaayGui", // Viewer@123
                Role = UserRole.Viewer
            },
            new User
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333332"),
                Username = "viewer2",
                PasswordHash = "$2a$12$J.8.n1hf.KXKf8FcOGVSI.r5DFmD.ykbTSsqHMvTKGvGVEw.uI8Dy", // Viewer@456
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
