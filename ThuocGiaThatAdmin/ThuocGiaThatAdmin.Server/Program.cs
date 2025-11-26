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
using ThuocGiaThatAdmin.Contracts.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure DbContext
// Update the connection string as needed
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ThuocGiaThat.Infrastucture.TrueMecContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ThuocGiaThat.Infrastucture.TrueMecContext>()
    .AddDefaultTokenProviders();

// Configure FileUploadSettings
builder.Services.Configure<FileUploadSettings>(
    builder.Configuration.GetSection("FileUploadSettings"));

// Register repositories and services
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IBrandRepository, BrandRepository>();
builder.Services.AddScoped<IProductOptionRepository, ProductOptionRepository>();
builder.Services.AddScoped<IUploadedFileRepository, UploadedFileRepository>();

// Inventory Management Repositories
builder.Services.AddScoped<IWarehouseRepository, WarehouseRepository>();
builder.Services.AddScoped<IInventoryBatchRepository, InventoryBatchRepository>();
builder.Services.AddScoped<IInventoryTransactionRepository, InventoryTransactionRepository>();
builder.Services.AddScoped<IStockAlertRepository, StockAlertRepository>();

// Services
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<BrandService>();
builder.Services.AddScoped<ProductOptionService>();
builder.Services.AddScoped<FileUploadService>();
builder.Services.AddScoped<OrderService>();

// Inventory Management Services
builder.Services.AddScoped<WarehouseService>();
builder.Services.AddScoped<InventoryService>();
builder.Services.AddScoped<StockAlertService>();

builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<IUserService, UserService>();

// Add CORS to allow frontend to call this API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        policy =>
        {
            policy.AllowAnyOrigin() // Allows requests from any origin
                  .AllowAnyMethod() // Allows any HTTP method (GET, POST, PUT, DELETE, etc.)
                  .AllowAnyHeader(); // Allows any header in the request
        });
});

//builder.WebHost.UseUrls("https://0.0.0.0:5000");

var app = builder.Build();

// Apply migrations automatically on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ThuocGiaThat.Infrastucture.TrueMecContext>();
    db.Database.Migrate();
    // seed admin user and roles (reads AdminUser:* from configuration)
    UserMigration.InitializeAsync(scope.ServiceProvider, builder.Configuration).GetAwaiter().GetResult();
    CountryMigration.InitializeAsync(scope.ServiceProvider, builder.Configuration).GetAwaiter().GetResult();
    ProvinceMigration.InitializeAsync(scope.ServiceProvider, builder.Configuration).GetAwaiter().GetResult();
    WardMigration.InitializeAsync(scope.ServiceProvider, builder.Configuration).GetAwaiter().GetResult();
}

app.UseCors("AllowAllOrigins");

// Add global exception handler middleware
app.UseMiddleware<ThuocGiaThatAdmin.Server.Middleware.GlobalExceptionHandlerMiddleware>();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Local")
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Serve static files from wwwroot/uploads
app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.Run();
