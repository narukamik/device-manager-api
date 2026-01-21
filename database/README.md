# Database Directory

This directory contains database-related configurations for the Device Manager API.

## Schema Management

The database schema is managed through **Entity Framework Core Migrations**, not SQL scripts.

### Migration Files Location
All migrations are located in: `device-manager-api/Infrastructure/Data/Migrations/`

### Automatic Migrations (Development)

In development environments, migrations are automatically applied on application startup when:
- `DatabaseSettings:AutoMigrateOnStartup` is set to `true` in appsettings
- Or `DatabaseSettings__AutoMigrateOnStartup=true` environment variable is set

The Docker Compose setup enables auto-migration by default.

### Manual Migrations (Production)

For production deployments, it's recommended to apply migrations manually:

```bash
# Navigate to the API project directory
cd device-manager-api

# Apply all pending migrations
dotnet ef database update

# Rollback to a specific migration
dotnet ef database update <MigrationName>

# Generate SQL script for review before applying
dotnet ef migrations script > migration.sql
```

### Creating New Migrations

When you modify entity models or DbContext configuration:

```bash
# Create a new migration
dotnet ef migrations add <MigrationName>

# Remove the last migration (if not yet applied)
dotnet ef migrations remove
```

### Current Schema

The database includes:

**Tables:**
- `Devices` - Device inventory with optimistic concurrency control
- `Users` - User accounts with JWT authentication support
- `__EFMigrationsHistory` - EF Core migration tracking

**Seed Data:**
- 6 pre-configured users (2 admins, 2 managers, 2 viewers)
- See `ApplicationDbContext.cs` for default credentials

## PostgreSQL Configuration

- **Version:** PostgreSQL 17 Alpine
- **Database Name:** devicemanager
- **Default Credentials:** postgres/postgres123 (configurable via `POSTGRES_PASSWORD` env var)
- **Port:** 5432
- **Data Persistence:** Docker volume `postgres-data`
