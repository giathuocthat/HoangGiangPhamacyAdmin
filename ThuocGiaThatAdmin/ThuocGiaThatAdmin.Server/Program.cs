using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using ThuocGiaThat.Infrastucture;
using ThuocGiaThat.Infrastucture.Data;
using ThuocGiaThat.Infrastucture.Repositories;
using ThuocGiaThatAdmin.Domain.Entities;
using ThuocGiaThatAdmin.Service;
using ThuocGiaThatAdmin.Service.Interfaces;
using ThuocGiaThatAdmin.Service.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure DbContext
// Update the connection string as needed
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ThuocGiaThat.Infrastucture.AppContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ThuocGiaThat.Infrastucture.AppContext>()
    .AddDefaultTokenProviders();

// Register repositories and services
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IBrandRepository, BrandRepository>();
builder.Services.AddScoped<IProductOptionRepository, ProductOptionRepository>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<BrandService>();
builder.Services.AddScoped<ProductOptionService>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserService, UserService>();

// Add CORS to allow frontend to call this API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost", builder =>
    {
        builder.WithOrigins("http://localhost:5173", "https://localhost:5173", "http://localhost:3000", "https://localhost:3000")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

var app = builder.Build();

// Apply migrations automatically on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ThuocGiaThat.Infrastucture.AppContext>();
    db.Database.Migrate();
    // seed admin user and roles (reads AdminUser:* from configuration)
    SeedData.InitializeAsync(scope.ServiceProvider, builder.Configuration).GetAwaiter().GetResult();
}

app.UseCors("AllowLocalhost");

// Add global exception handler middleware
app.UseMiddleware<ThuocGiaThatAdmin.Server.Middleware.GlobalExceptionHandlerMiddleware>();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Local")
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
