using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using ThuocGiaThat.Infrastucture.Data;
using ThuocGiaThat.Infrastucture.Interfaces;
using ThuocGiaThat.Infrastucture.Repositories;
using ThuocGiaThat.Infrastucture.Utils;
using ThuocGiaThatAdmin.Commands;
using ThuocGiaThatAdmin.Common.Interfaces;
using ThuocGiaThatAdmin.Contract.Models;
using ThuocGiaThatAdmin.Contracts.Models;
using ThuocGiaThatAdmin.Domain.Entities;
using ThuocGiaThatAdmin.Queries;
using ThuocGiaThatAdmin.Server.Extensions;
using ThuocGiaThatAdmin.Service;
using ThuocGiaThatAdmin.Service.Interfaces;
using ThuocGiaThatAdmin.Service.Services;

using ThuocGiaThatAdmin.Common.Interfaces;
using ThuocGiaThatAdmin.Queries;
using ThuocGiaThatAdmin.Commands;
using ThuocGiaThatAdmin.Contract.Models;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Thuoc Gia That API",
        Version = "v1",
        Description = "API for Pharmacy Management System with Admin and Customer authentication"
    });

    // Add JWT Authentication to Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n" +
                      "Enter your token in the text input below.\r\n\r\n" +
                      "Example: '12345abcdef'"
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
            new string[] {}
        }
    });
});

// Configure DbContext
// Update the connection string as needed
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ThuocGiaThat.Infrastucture.TrueMecContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddEntityFrameworkStores<ThuocGiaThat.Infrastucture.TrueMecContext>()
    .AddDefaultTokenProviders();

// Configure JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

if (string.IsNullOrWhiteSpace(jwtKey))
    throw new InvalidOperationException("JWT Key is not configured in appsettings.json");

var keyBytes = Encoding.UTF8.GetBytes(jwtKey);
var signingKey = new SymmetricSecurityKey(keyBytes);

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
        IssuerSigningKey = signingKey
    };
    options.MapInboundClaims = false;
});

builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<GzipCompressionProvider>();
});

// Configure FileUploadSettings
builder.Services.Configure<FileUploadSettings>(
    builder.Configuration.GetSection("FileUploadSettings"));

// ============================================================
// Register Repositories
// ============================================================
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IBrandRepository, BrandRepository>();
builder.Services.AddScoped<IProductOptionRepository, ProductOptionRepository>();
builder.Services.AddScoped<IUploadedFileRepository, UploadedFileRepository>();
builder.Services.AddScoped<IProductVariantRepository, ProductVariantRepository>();
builder.Services.AddScoped<IAddressRepository, AddressRepository>();

// Inventory Management Repositories
builder.Services.AddScoped<IWarehouseRepository, WarehouseRepository>();
builder.Services.AddScoped<IInventoryBatchRepository, InventoryBatchRepository>();
builder.Services.AddScoped<IInventoryTransactionRepository, InventoryTransactionRepository>();
builder.Services.AddScoped<IStockAlertRepository, StockAlertRepository>();
builder.Services.AddScoped<IBusinessTypeRepository, BusinessTypeRepository>();

// Shopping Cart Repositories
builder.Services.AddScoped<IShoppingCartRepository, ShoppingCartRepository>();
builder.Services.AddScoped<IShoppingCartItemRepository, ShoppingCartItemRepository>();

// Customer Management Repository
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICustomerPaymentAccountRepository, CustomerPaymentAccountRepository>();
builder.Services.AddScoped<IProductCollectionRepository, ProductCollectionRepository>();
builder.Services.AddScoped<IProductMaxOrderConfigRepository, ProductMaxOrderConfigRepository>();

// Purchase Order Repositories
builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
builder.Services.AddScoped<ISupplierContactRepository, SupplierContactRepository>();
builder.Services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();
builder.Services.AddScoped<IPurchaseOrderItemRepository, PurchaseOrderItemRepository>();
builder.Services.AddScoped<IPurchaseOrderHistoryRepository, PurchaseOrderHistoryRepository>();
builder.Services.AddScoped<IGoodsReceiptRepository, GoodsReceiptRepository>();
builder.Services.AddScoped<IGoodsReceiptItemRepository, GoodsReceiptItemRepository>();
builder.Services.AddScoped<IBankRepository, BankRepository>();

// Generic Repository
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// ============================================================
// Register Services (Legacy - for backward compatibility)
// ============================================================
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<IBrandService, BrandService>();
builder.Services.AddScoped<ProductOptionService>();
builder.Services.AddScoped<FileUploadService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<CategoryService>();

// Inventory Management Services
builder.Services.AddScoped<WarehouseService>();
builder.Services.AddScoped<InventoryService>();
builder.Services.AddScoped<StockAlertService>();
builder.Services.AddScoped<ProductBatchService>();
builder.Services.AddScoped<WarehouseLocationService>();

builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<BusinessTypeService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IVoucherService, VoucherService>();

builder.Services.AddScoped<IVoucherRepository, VoucherRepository>();

// Banner/Campaign/Combo Repositories
builder.Services.AddScoped<ICampaignRepository, CampaignRepository>();
builder.Services.AddScoped<IBannerRepository, BannerRepository>();
builder.Services.AddScoped<IComboRepository, ComboRepository>();
builder.Services.AddScoped<IBannerAnalyticsRepository, BannerAnalyticsRepository>();

// Banner/Campaign/Combo Services
builder.Services.AddScoped<ICampaignService, CampaignService>();
builder.Services.AddScoped<IBannerService, BannerService>();
builder.Services.AddScoped<IComboService, ComboService>();
builder.Services.AddScoped<IBannerAnalyticsService, BannerAnalyticsService>();

// Shopping Cart Service
builder.Services.AddScoped<IShoppingCartService, ShoppingCartService>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<ICustomerPaymentAccountService, CustomerPaymentAccountService>();

// Purchase Order Services
builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<ISupplierContactService, SupplierContactService>();
builder.Services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();
builder.Services.AddScoped<IGoodsReceiptService, GoodsReceiptService>();
builder.Services.AddScoped<IGoodsReceiptItemService, GoodsReceiptItemService>();
builder.Services.AddScoped<IBankService, BankService>();


// other
builder.Services.AddScoped<IRoleClaimService, RoleClaimService>();
builder.Services.AddScoped<DynamicFilterService>();

// ============================================================
// Register CQRS - Dispatchers
// ============================================================
builder.Services.AddScoped<ICommandDispatcher, CommandDispatcher>();
builder.Services.AddScoped<IQueryDispatcher, QueryDispatcher>();

// ============================================================
// Register CQRS - Auto-discover Handlers
// ============================================================
builder.RegisterQueryHandlers();
builder.RegisterCommandHandlers();

// ============================================================
// Add CORS
// ============================================================
// Customer Authentication Service
builder.Services.AddScoped<ICustomerAuthService, CustomerAuthService>();

// Customer Management Service
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IProductCollectionService, ProductCollectionService>();
builder.Services.AddScoped<VNPayService>();
builder.Services.AddScoped<IZaloService, ZaloService>();
builder.Services.AddScoped<HttpClient>();



// Order Fulfillment Service
builder.Services.AddScoped<IOrderFulfillmentService, OrderFulfillmentService>();
builder.Services.AddScoped<IOrderFulfillmentRepository, OrderFulfillmentRepository>();

// Warehouse Picking Service
builder.Services.AddScoped<IWarehousePickingService, WarehousePickingService>();
builder.Services.AddScoped<IWarehousePickingRepository, WarehousePickingRepository>();

// Add CORS to allow frontend to call this API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000")
            //.AllowAnyOrigin() // Allows requests from any origin
                  .AllowAnyMethod() // Allows any HTTP method (GET, POST, PUT, DELETE, etc.)
                  .AllowAnyHeader()
                  .AllowCredentials(); // Allows any header in the request
        });
});

builder.Services.Configure<VNPaySetting>(builder.Configuration.GetSection("VNPAY"));
builder.Services.Configure<ZaloSetting>(builder.Configuration.GetSection("Zalo"));

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddPolicy("SendOtpPolicy", context =>
    {
        var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unkndown";

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: ip,
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 2,
                Window = TimeSpan.FromMinutes(3),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            });
    });
});


//builder.WebHost.UseUrls("https://0.0.0.0:5000");

var app = builder.Build();

// ============================================================
// Apply Migrations & Seeding
// ============================================================
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ThuocGiaThat.Infrastucture.TrueMecContext>();
    db.Database.Migrate();
    // seed admin user and roles (reads AdminUser:* from configuration)
    ChildrenRoleMigration.InitializeAsync(scope.ServiceProvider, builder.Configuration).GetAwaiter().GetResult();
    UserMigration.InitializeAsync(scope.ServiceProvider, builder.Configuration).GetAwaiter().GetResult();
    // seed role claims (permissions) for admin role
    RoleClaimsMigration.InitializeAsync(scope.ServiceProvider, builder.Configuration).GetAwaiter().GetResult();
    CountryMigration.InitializeAsync(scope.ServiceProvider, builder.Configuration).GetAwaiter().GetResult();
    ProvinceMigration.InitializeAsync(scope.ServiceProvider, builder.Configuration).GetAwaiter().GetResult();
    WardMigration.InitializeAsync(scope.ServiceProvider, builder.Configuration).GetAwaiter().GetResult();
    BusinessTypeMigration.InitializeAsync(scope.ServiceProvider, builder.Configuration).GetAwaiter().GetResult();
    CategoryMigration.InitializeAsync(scope.ServiceProvider, builder.Configuration).GetAwaiter().GetResult();
    InventoryPermissionMigration.InitializeAsync(scope.ServiceProvider, builder.Configuration).GetAwaiter().GetResult();
    CamPaignMigration.InitializeAsync(scope.ServiceProvider, builder.Configuration).GetAwaiter().GetResult();
    BankMigration.InitializeAsync(scope.ServiceProvider, builder.Configuration).GetAwaiter().GetResult();
}

// ============================================================
// Middleware Pipeline
// ============================================================
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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseRateLimiter();

app.Run();
