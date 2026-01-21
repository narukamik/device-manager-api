using System.Reflection;
using System.Text;
using Asp.Versioning;
using AspNetCoreRateLimit;
using device_manager_api.API.Middleware;
using device_manager_api.Application.Interfaces;
using device_manager_api.Application.Services;
using device_manager_api.Application.Validators;
using device_manager_api.Infrastructure.Caching;
using device_manager_api.Infrastructure.Data;
using device_manager_api.Infrastructure.Repositories;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithEnvironmentName()
    .Enrich.WithThreadId()
    .Enrich.WithProperty("Application", "DeviceManagerAPI")
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container

// Database - PostgresSQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Redis Cache
var redisConnectionString = builder.Configuration.GetConnectionString("Redis");
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(redisConnectionString!));
builder.Services.AddSingleton<ICacheService, RedisCacheService>();

// HTTP Context Accessor
builder.Services.AddHttpContextAccessor();

// Repositories
builder.Services.AddScoped<IDeviceRepository, DeviceRepository>();

// Services
builder.Services.AddScoped<IDeviceService, DeviceService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();

// AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateDeviceRequestValidator>();

// JWT Authentication
var jwtSecret = builder.Configuration["JwtSettings:SecretKey"];
var jwtIssuer = builder.Configuration["JwtSettings:Issuer"];
var jwtAudience = builder.Configuration["JwtSettings:Audience"];

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret!)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// API Versioning
builder.Services.AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;
        options.ApiVersionReader = new UrlSegmentApiVersionReader();
    })
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'V";
        options.SubstituteApiVersionInUrl = true;
    });

// Rate Limiting
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.Configure<ClientRateLimitOptions>(builder.Configuration.GetSection("ClientRateLimiting"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

// CORS
var corsOrigins = builder.Configuration.GetSection("CorsOrigins").Get<string[]>() ?? Array.Empty<string>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevPolicy", policy =>
    {
        policy.WithOrigins(corsOrigins)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// Health Checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>("database")
    .AddCheck("redis", () =>
    {
        var redis = builder.Services.BuildServiceProvider().GetRequiredService<IConnectionMultiplexer>();
        return redis.IsConnected
            ? Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("Redis is connected")
            : Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Unhealthy("Redis is not connected");
    });

// Controllers
builder.Services.AddControllers()
    .AddNewtonsoftJson(); // Required for JSON Patch

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Device Manager API",
        Version = "v1",
        Description =
            "REST API for managing device resources with JWT authentication, Redis caching, and comprehensive validation",
        Contact = new OpenApiContact
        {
            Name = "Device Manager Team"
        }
    });

    // JWT Authentication in Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Enter your JWT token in the format: Bearer {your token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // Include XML comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

// Configure the HTTP request pipeline

// Global Exception Handler
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

// Serilog Request Logging
app.UseSerilogRequestLogging();

// Swagger (Development)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => { options.SwaggerEndpoint("/swagger/v1/swagger.json", "Device Manager API v1"); });
}

app.UseHttpsRedirection();

// CORS
app.UseCors("DevPolicy");

// Rate Limiting
app.UseIpRateLimiting();

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Health Checks
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready");
app.MapHealthChecks("/health/live");

// Controllers
app.MapControllers();

// Apply migrations and seed data on startup
using var scope = app.Services.CreateScope();

var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

    var autoMigrate = configuration.GetValue("DatabaseSettings:AutoMigrateOnStartup", false);

if (!autoMigrate)
{
    logger.LogInformation(
        "Automatic database migration is DISABLED (DatabaseSettings:AutoMigrateOnStartup = false)");
    logger.LogInformation(
        "To enable auto-migration: Set DatabaseSettings__AutoMigrateOnStartup=true in environment or appsettings");
    logger.LogInformation("For manual migration: Run 'dotnet ef database update' from the project directory");
}
else
{
    logger.LogInformation("Automatic database migration is ENABLED (DatabaseSettings:AutoMigrateOnStartup = true)");

    if (!app.Environment.IsDevelopment())
    {
        logger.LogWarning(
            "⚠️  Auto-migration is running in {Environment} environment. Consider using manual migrations in production!",
            app.Environment.EnvironmentName);
    }

    try
    {
        logger.LogInformation("Attempting to apply database migrations");

        dbContext.Database.Migrate();
        logger.LogInformation("✅ Database migrations applied successfully");
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "Failed to apply migrations");
    }
}

Log.Information("Device Manager API starting...");

app.Run();

Log.CloseAndFlush();