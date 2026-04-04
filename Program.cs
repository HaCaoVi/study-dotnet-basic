using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using project_basic.Database;
using project_basic.Filters;
using project_basic.Mappings;
using project_basic.Middleware;
using project_basic.Models;
using project_basic.Repositories;
using project_basic.Repositories.Interfaces;
using project_basic.Services;
using project_basic.Services.Interfaces;
using project_basic.Validators;
using project_basic.Validators.AuthValidator;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options => 
    options.UseNpgsql(builder.Configuration.GetConnectionString("DbContext")));

// Dependency Injection
// Register password hasher
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

// Register your services
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();

// Validation
// Disable default ASP.NET Core model state validation so our filter takes over
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddValidatorsFromAssemblyContaining<CreateUserValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateUserValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<QueryUserValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<LoginValidator>();

// Controllers with global ValidationFilter
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
});

// Auto mapper
builder.Services.AddAutoMapper(typeof(UserMapping));

// Config JWT
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
    var signingKey = builder.Configuration["JWT:SigningKey"]
                     ?? throw new Exception("JWT:SigningKey is missing");

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:Audience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(signingKey)
        ),
        ValidateLifetime = true,
        RequireExpirationTime = true,
        ClockSkew = TimeSpan.Zero
    };
});

var app = builder.Build();

// Auto-migration or DB check (optional, but keep it if user had it)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        if (db.Database.CanConnect())
        {
            Console.WriteLine("✅ Connected to Postgres successfully");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("❌ Cannot connect to Postgres: " + ex.Message);
    }
}

// HTTP request pipeline order is important
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Config Auth
app.UseAuthentication();
app.UseAuthorization();

// Global Exception Middleware (before MapControllers)
app.UseMiddleware<ExceptionMiddleware>();

app.MapControllers();

app.Run();
