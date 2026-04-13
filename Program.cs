using System.Text.Json;
using System.Threading.RateLimiting;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi;
using project_basic.Common.Configuration;
using project_basic.Database;
using project_basic.Mappings;
using project_basic.Middleware;
using project_basic.Entities;
using project_basic.Repositories;
using project_basic.Repositories.Interfaces;
using project_basic.Services;
using project_basic.Services.Interfaces;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// ──────────────────────────────────────────────
// 1. Strongly-typed Configuration (Options pattern)
// ──────────────────────────────────────────────
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection(JwtSettings.SectionName));

builder.Services.Configure<CorsSettings>(
    builder.Configuration.GetSection(CorsSettings.SectionName));

// Validate JWT settings on startup
builder.Services.AddOptions<JwtSettings>()
    .Bind(builder.Configuration.GetSection(JwtSettings.SectionName))
    .Validate(settings =>
            !string.IsNullOrEmpty(settings.SigningKey)
            && !string.IsNullOrEmpty(settings.Issuer)
            && !string.IsNullOrEmpty(settings.Audience),
        "JWT configuration (SigningKey, Issuer, Audience) must be provided.")
    .ValidateOnStart();

// ──────────────────────────────────────────────
// 2. OpenAPI / Scalar
// ──────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

        document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Name = "Authorization"
        };

        document.Security ??= new List<OpenApiSecurityRequirement>();
        document.Security.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer")] = new List<string>()
        });

        return Task.CompletedTask;
    });
});

// ──────────────────────────────────────────────
// 3. Database
// ──────────────────────────────────────────────
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DbContext")));

// ──────────────────────────────────────────────
// 4. Dependency Injection
// ──────────────────────────────────────────────
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();

// Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();

// ──────────────────────────────────────────────
// 5. Validation (single call scans entire assembly)
// ──────────────────────────────────────────────
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// ──────────────────────────────────────────────
// 6. Controllers with global filters
// ──────────────────────────────────────────────
builder.Services.AddControllers(options =>
{
    options.Filters.Add(new AuthorizeFilter());
    options.Filters.Add<ValidationFilter>();
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.DefaultIgnoreCondition =
        System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
});

// ──────────────────────────────────────────────
// 7. AutoMapper
// ──────────────────────────────────────────────
builder.Services.AddAutoMapper(typeof(EntityMapping));

// ──────────────────────────────────────────────
// 8. JWT Authentication
// ──────────────────────────────────────────────
var jwtSettings = builder.Configuration
    .GetSection(JwtSettings.SectionName)
    .Get<JwtSettings>()!;

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme =
        options.DefaultChallengeScheme =
            options.DefaultForbidScheme =
                options.DefaultScheme =
                    options.DefaultSignInScheme =
                        options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidateAudience = true,
        ValidAudience = jwtSettings.Audience,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(jwtSettings.SigningKey)
        ),
        ValidateLifetime = true,
        RequireExpirationTime = true,
        ClockSkew = TimeSpan.Zero
    };
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            var logger = context.HttpContext.RequestServices
                .GetRequiredService<ILogger<Program>>();
            logger.LogWarning("JWT authentication failed: {Error}", context.Exception.Message);
            return Task.CompletedTask;
        },
        OnMessageReceived = context =>
        {
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();

            if (string.IsNullOrEmpty(authHeader))
            {
                context.NoResult();
                return Task.CompletedTask;
            }

            if (authHeader.StartsWith("Bearer "))
            {
                context.Token = authHeader["Bearer ".Length..];
            }

            return Task.CompletedTask;
        }
    };
});

// ──────────────────────────────────────────────
// 9. HttpContext Accessor
// ──────────────────────────────────────────────
builder.Services.AddHttpContextAccessor();

// ──────────────────────────────────────────────
// 10. CORS
// ──────────────────────────────────────────────
var corsSettings = builder.Configuration
    .GetSection(CorsSettings.SectionName)
    .Get<CorsSettings>() ?? new CorsSettings();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .WithOrigins(corsSettings.AllowedOrigins)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// ──────────────────────────────────────────────
// 11. Rate Limiting
// ──────────────────────────────────────────────
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1)
            }));
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

// ======================================================================
// BUILD APP
// ======================================================================
var app = builder.Build();

// Verify database connectivity on startup
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        if (db.Database.CanConnect())
        {
            logger.LogInformation("Connected to PostgreSQL successfully");
        }
    }
    catch (Exception ex)
    {
        logger.LogCritical(ex, "Cannot connect to PostgreSQL");
    }
}

// ──────────────────────────────────────────────
// HTTP Request Pipeline (order matters)
// ──────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi().AllowAnonymous();
    app.MapScalarApiReference().AllowAnonymous();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// 1. Request logging (outermost — captures everything)
app.UseMiddleware<RequestLoggingMiddleware>();

// 2. Global exception handler
app.UseMiddleware<ExceptionMiddleware>();

// 3. Rate limiting
app.UseRateLimiter();

// 4. CORS
app.UseCors("AllowAll");

// 5. Auth
app.UseAuthentication();
app.UseAuthorization();

// 6. Map controllers
app.MapControllers();

app.Run();
