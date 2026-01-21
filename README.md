# Device Manager API

A production-ready REST API for managing device resources with JWT authentication, role-based authorization, PostgreSQL persistence, Redis caching, and comprehensive observability.

## Features

- **CRUD Operations**: Full device lifecycle management with pagination and filtering
- **JWT Authentication**: Secure token-based authentication with refresh token support
- **Role-Based Authorization**: Three-tier access control (Admin, Manager, Viewer)
- **Redis Caching**: Multi-level caching strategy with automatic invalidation
- **Optimistic Concurrency**: Prevent lost updates with row versioning
- **Audit Logging**: Track who created and modified each device
- **Rate Limiting**: Protect endpoints from abuse
- **Structured Logging**: Serilog with Seq for advanced log querying
- **API Versioning**: URL-based versioning for API evolution
- **JSON Patch Support**: RFC 6902 compliant partial updates
- **Health Checks**: Kubernetes-ready health endpoints
- **Comprehensive Validation**: FluentValidation with business rules enforcement
- **Swagger Documentation**: Interactive API documentation with auth support

## Tech Stack

| Component | Technology |
|-----------|-----------|
| Framework | .NET 9.0 |
| Database | PostgreSQL 17 |
| Caching | Redis 7 |
| Logging | Serilog + Seq |
| Authentication | JWT Bearer |
| ORM | Entity Framework Core 9 |
| API Documentation | Swagger/OpenAPI |

## Quick Start

### Prerequisites
- Docker Desktop

### Running with Docker Compose

```bash
# Clone the repository
git clone https://github.com/narukamik/device-manager-api
cd device-manager-api

# Edit .env and set your JWT secret key (or use the default for testing)
# Generate a secure key with: openssl rand -base64 64

# Start all services
docker-compose up -d

# View logs
docker-compose logs -f api
```

**Access the application:**
- Swagger UI: http://localhost:5000/swagger
- Seq Logs: http://localhost:5341
- API: http://localhost:5000/api/v1

### Stopping Services

```bash
docker-compose down
```

## Default User Credentials

⚠️ **Security Warning**: Change these passwords in production!

| Username | Password | Role | Permissions |
|----------|----------|------|-------------|
| admin1 | Admin@123 | Admin | Full access (Create, Read, Update, Delete) |
| admin2 | Admin@456 | Admin | Full access (Create, Read, Update, Delete) |
| manager1 | Manager@123 | Manager | Create, Read, Update (no Delete) |
| manager2 | Manager@456 | Manager | Create, Read, Update (no Delete) |
| viewer1 | Viewer@123 | Viewer | Read-only access |
| viewer2 | Viewer@456 | Viewer | Read-only access |

## User Roles & Permissions

| Role | Create Device | View Devices | Update Device | Delete Device |
|------|--------------|--------------|---------------|---------------|
| **Admin** | ✅ | ✅ | ✅ | ✅ |
| **Manager** | ✅ | ✅ | ✅ | ❌ |
| **Viewer** | ❌ | ✅ | ❌ | ❌ |

## API Authentication

### 1. Login to Get Token

```bash
curl -X POST http://localhost:5000/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "username": "admin1",
    "password": "Admin@123"
  }'
```

**Response:**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "ABC123...",
  "expiresIn": 900
}
```

### 2. Use Token in Requests

```bash
curl -X GET http://localhost:5000/api/v1/devices \
  -H "Authorization: Bearer <your-access-token>"
```

### 3. Refresh Token When Expired

```bash
curl -X POST http://localhost:5000/api/v1/auth/refresh \
  -H "Content-Type: application/json" \
  -d '{
    "refreshToken": "<your-refresh-token>"
  }'
```

## API Examples

### Get All Devices (Paginated)

```bash
GET /api/v1/devices?pageNumber=1&pageSize=10
```

### Get Single Device

```bash
GET /api/v1/devices?id=<device-guid>
```

### Filter by Brand

```bash
GET /api/v1/devices?brand=Apple&pageNumber=1&pageSize=10
```

### Filter by State

```bash
GET /api/v1/devices?state=Available&pageNumber=1&pageSize=10
```

**Available States:** `Available`, `InUse`, `Inactive`

### Create Device (Admin/Manager)

```bash
POST /api/v1/devices
Content-Type: application/json
Authorization: Bearer <token>

{
  "name": "iPhone 15 Pro",
  "brand": "Apple",
  "state": "Available"
}
```

### Update Device (Admin/Manager)

```bash
PUT /api/v1/devices/{id}
Content-Type: application/json
Authorization: Bearer <token>

{
  "name": "iPhone 15 Pro Max",
  "brand": "Apple",
  "state": "InUse"
}
```

### Partial Update with JSON Patch (Admin/Manager)

```bash
PATCH /api/v1/devices/{id}
Content-Type: application/json-patch+json
Authorization: Bearer <token>

[
  { "op": "replace", "path": "/state", "value": "InUse" }
]
```

### Delete Device (Admin Only)

```bash
DELETE /api/v1/devices/{id}
Authorization: Bearer <token>
```

## Business Rules

1. **CreationTime is Immutable**: Cannot be updated after device creation
2. **Name/Brand Protection**: Cannot update Name or Brand when device State is `InUse`
3. **Delete Protection**: Cannot delete devices that are currently `InUse`
4. **Password Policy**: Minimum 8 characters with uppercase, lowercase, digit, and special character

## Caching Strategy

| Cache Type | TTL | Key Pattern |
|------------|-----|-------------|
| Single Device | 5 minutes | `device:{id}` |
| Filtered Collections | 2 minutes | `devices:brand:{brand}:{page}:{size}` |
| All Devices | 1 minute | `devices:all:{page}:{size}` |

Cache is automatically invalidated on Create, Update, Patch, and Delete operations.

## Project Highlights

### Clean Architecture
- **Domain Layer**: Entities and Enums
- **Application Layer**: DTOs, Interfaces, Services, Validators, Exceptions
- **Infrastructure Layer**: Data, Repositories, Caching
- **API Layer**: Controllers, Middleware, Mappings

### Security
- JWT with refresh token rotation
- BCrypt password hashing (work factor: 12)
- Role-based authorization on endpoints
- Rate limiting (100 req/min per IP, 5 req/min for login)
- Optimistic concurrency with RowVersion
- Audit logging (CreatedBy, ModifiedBy, ModifiedAt)

### Performance
- Redis distributed caching with TTL strategy
- Database indexes on Brand, State, and composite (Brand, State)
- EF Core AsNoTracking for read queries
- Pagination on all collection endpoints
- Connection pooling

### Code Quality
- FluentValidation for all inputs
- AutoMapper for clean DTO mappings
- Global exception handling with RFC 7807 ProblemDetails
- Comprehensive XML documentation
- Async/await throughout

### DevOps Ready
- Docker multi-stage builds
- docker-compose for local development
- Health checks for orchestration
- Structured logging with correlation IDs
- Environment variable configuration

## Running Tests

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

## Environment Variables

| Variable | Description | Default                           |
|----------|-------------|-----------------------------------|
| `POSTGRES_PASSWORD` | PostgreSQL password | postgres123                       |
| `JWT_SECRET_KEY` | Secret key for JWT signing | (see .env file)                   |
| `ConnectionStrings__DefaultConnection` | PostgreSQL connection string | Auto-configured in docker-compose |
| `ConnectionStrings__Redis` | Redis connection string | Auto-configured in docker-compose |

## Health Checks

- `/health` - Overall health status
- `/health/ready` - Readiness probe (for Kubernetes)
- `/health/live` - Liveness probe (for Kubernetes)

## Docker Services

| Service | Port | Purpose |
|---------|------|---------|
| api | 5000 | Device Manager API |
| postgres | 5432 | PostgreSQL database |
| redis | 6379 | Redis cache |
| seq | 5341 | Seq log viewer |

## Troubleshooting

### Connection refused errors
Ensure all Docker containers are healthy:
```bash
docker-compose ps
```

### Authentication fails
Check JWT secret key is set in .env file and matches between runs.

### Concurrency conflicts
If you get 409 Conflict errors, another user modified the resource. Fetch the latest version and retry.

### Rate limit exceeded
Wait 1 minute or whitelist your IP in appsettings.json `IpRateLimiting.IpWhitelist`.
