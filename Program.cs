using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

// Validation
// Disable default ASP.NET Core model state validation so our filter takes over
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddValidatorsFromAssemblyContaining<CreateUserValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateUserValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<QueryUserValidator>();

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

// Global Exception Middleware (before MapControllers)
app.UseMiddleware<ExceptionMiddleware>();

app.MapControllers();

app.Run();
