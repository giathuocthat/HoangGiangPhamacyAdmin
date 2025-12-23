using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThat.Infrastucture
{

    public class TrueMecContext : IdentityDbContext<ApplicationUser, ApplicationRole, string, IdentityUserClaim<string>, IdentityUserRole<string>, IdentityUserLogin<string>, ApplicationRoleClaim, IdentityUserToken<string>>
    {
        public TrueMecContext(DbContextOptions<TrueMecContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Bank> Banks { get; set; }
        // ProductType removed
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ProductOption> ProductOptions { get; set; }
        public DbSet<ProductOptionValue> ProductOptionValues { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<VariantOptionValue> VariantOptionValues { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<PriceHistory> PriceHistories { get; set; }

        // Inventory Management
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<WarehouseLocation> WarehouseLocations { get; set; }
        public DbSet<InventoryBatch> InventoryBatches { get; set; }
        public DbSet<InventoryTransaction> InventoryTransactions { get; set; }
        public DbSet<ProductBatch> ProductBatches { get; set; }
        public DbSet<StockAlert> StockAlerts { get; set; }
        public DbSet<BatchLocationStock> BatchLocationStocks { get; set; }
        public DbSet<LocationStockMovement> LocationStockMovements { get; set; }

        public virtual DbSet<Customer> Customers { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderItemSnapshot> OrderItemSnapshots { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Province> Provinces { get; set; }
        public DbSet<Ward> Wards { get; set; }
        public DbSet<UploadedFile> UploadedFiles { get; set; }

        public DbSet<BusinessType> BusinessTypes { get; set; }
        public DbSet<CustomerPaymentAccount> CustomerPaymentAccounts { get; set; }
        public DbSet<CustomerDocument> CustomerDocuments { get; set; }
        public DbSet<CustomerVerification> CustomerVerifications { get; set; }

        // Shopping Cart
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }

        // Product Collection
        public DbSet<ProductCollection> ProductCollections { get; set; }
        public DbSet<ProductCollectionItem> ProductCollectionItems { get; set; }
        public DbSet<ProductMaxOrderConfig> ProductMaxOrderConfigs { get; set; }

        // Product Status Mapping
        public DbSet<ProductStatusMap> ProductStatusMaps { get; set; }

        // Voucher System
        public DbSet<Voucher> Vouchers { get; set; }
        public DbSet<VoucherCategory> VoucherCategories { get; set; }
        public DbSet<VoucherProductVariant> VoucherProductVariants { get; set; }
        public DbSet<VoucherUsageHistory> VoucherUsageHistories { get; set; }
        public DbSet<OrderVoucher> OrderVouchers { get; set; }
        public DbSet<PaymentTransaction> PaymentTransactions { get; set; }
        public DbSet<OrderItemFulfillment> OrderItemFulfillments { get; set; }
        public DbSet<OtpCode> OtpCodes { get; set; }

        // Banner/Campaign/Combo System
        public DbSet<Campaign> Campaigns { get; set; }
        public DbSet<Banner> Banners { get; set; }
        public DbSet<BannerSection> BannerSections { get; set; }
        public DbSet<Combo> Combos { get; set; }
        public DbSet<ComboItem> ComboItems { get; set; }
        public DbSet<BannerAnalytics> BannerAnalytics { get; set; }

        // Purchase Order System
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<SupplierContact> SupplierContacts { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; }
        public DbSet<PurchaseOrderHistory> PurchaseOrderHistories { get; set; }
        public DbSet<GoodsReceipt> GoodsReceipts { get; set; }
        public DbSet<GoodsReceiptItem> GoodsReceiptItems { get; set; }

        // Sales Region System
        public DbSet<SalesRegion> SalesRegions { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ============ Category Configuration ============
            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(p => p.Id).UseIdentityColumn(seed: 1, increment: 1);
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.Slug).IsRequired().HasMaxLength(255);

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedDate);

                entity.HasOne(e => e.ParentCategory)
                    .WithMany(e => e.ChildCategories)
                    .HasForeignKey(e => e.ParentId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.Slug).IsUnique();
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.DisplayOrder);
            });

            // ============ Brand Configuration ============
            modelBuilder.Entity<Brand>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Slug).IsRequired().HasMaxLength(255);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedDate);
                entity.HasIndex(e => e.Slug).IsUnique();
            });

            // ============ Product Configuration ============
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Slug).IsRequired().HasMaxLength(255);
                entity.Property(e => e.CreatedDate).HasColumnName("CreatedDate").HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedDate).HasColumnName("UpdatedDate");

                entity.HasOne(e => e.Category).WithMany(e => e.Products).HasForeignKey(e => e.CategoryId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Brand).WithMany(e => e.Products).HasForeignKey(e => e.BrandId).OnDelete(DeleteBehavior.SetNull);

                entity.HasIndex(e => e.Slug).IsUnique();
            });

            // ============ ProductOption Configuration ============
            modelBuilder.Entity<ProductOption>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.HasOne(e => e.Product).WithMany(e => e.ProductOptions).HasForeignKey(e => e.ProductId).OnDelete(DeleteBehavior.Cascade);
            });

            // ============ ProductOptionValue Configuration ============
            modelBuilder.Entity<ProductOptionValue>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Value).IsRequired().HasMaxLength(255);
                entity.HasOne(e => e.ProductOption).WithMany(e => e.ProductOptionValues).HasForeignKey(e => e.ProductOptionId).OnDelete(DeleteBehavior.Cascade);
            });

            // ============ ProductVariant Configuration ============
            modelBuilder.Entity<ProductVariant>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.SKU).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
                entity.Property(e => e.OriginalPrice).HasColumnType("decimal(18,2)");
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedDate);

                entity.HasOne(e => e.Product).WithMany(e => e.ProductVariants).HasForeignKey(e => e.ProductId).OnDelete(DeleteBehavior.Cascade);
                entity.HasIndex(e => e.SKU).IsUnique();
            });

            // ============ VariantOptionValue Configuration ============
            modelBuilder.Entity<VariantOptionValue>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.ProductVariant).WithMany(e => e.VariantOptionValues).HasForeignKey(e => e.ProductVariantId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.ProductOptionValue).WithMany(e => e.VariantOptionValues).HasForeignKey(e => e.ProductOptionValueId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => new { e.ProductVariantId, e.ProductOptionValueId }).IsUnique();
            });

            // ============ Inventory Configuration ============
            modelBuilder.Entity<Inventory>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedDate);

                entity.HasOne(e => e.ProductVariant)
                    .WithMany(e => e.Inventories)
                    .HasForeignKey(e => e.ProductVariantId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Warehouse)
                    .WithMany(e => e.Inventories)
                    .HasForeignKey(e => e.WarehouseId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Unique constraint: one inventory record per product variant per warehouse
                entity.HasIndex(e => new { e.ProductVariantId, e.WarehouseId }).IsUnique();
                entity.HasIndex(e => e.WarehouseId);
                entity.HasIndex(e => e.QuantityOnHand);
            });

            // ============ Warehouse Configuration ============
            modelBuilder.Entity<Warehouse>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Address).HasMaxLength(500);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedDate);

                entity.HasIndex(e => e.Code).IsUnique();
                entity.HasIndex(e => e.IsActive);
            });

            // ============ InventoryBatch Configuration ============
            modelBuilder.Entity<InventoryBatch>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.BatchNumber).IsRequired().HasMaxLength(100);
                entity.Property(e => e.CostPrice).HasColumnType("decimal(18,2)");
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedDate);

                entity.HasOne(e => e.Inventory)
                    .WithMany(e => e.Batches)
                    .HasForeignKey(e => e.InventoryId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.BatchNumber);
                entity.HasIndex(e => e.ExpiryDate);
                entity.HasIndex(e => e.Status);
            });

            // ============ ProductBatch Configuration ============
            modelBuilder.Entity<ProductBatch>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.BatchNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.CostPrice).HasColumnType("decimal(18,2)");
                entity.Property(e => e.PurchaseOrderNumber).HasMaxLength(100);
                entity.Property(e => e.Supplier).HasMaxLength(255);
                entity.Property(e => e.QRCodePath).HasMaxLength(500);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedDate);

                entity.HasOne(e => e.ProductVariant)
                    .WithMany(v => v.ProductBatches)
                    .HasForeignKey(e => e.ProductVariantId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.BatchNumber).IsUnique();
                entity.HasIndex(e => e.ExpiryDate);
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.ProductVariantId);
            });

            // ============ InventoryTransaction Configuration ============
            modelBuilder.Entity<InventoryTransaction>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TotalValue).HasColumnType("decimal(18,2)");
                entity.Property(e => e.ReferenceNumber).HasMaxLength(100);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedDate);

                entity.HasOne(e => e.ProductVariant)
                    .WithMany(e => e.InventoryTransactions)
                    .HasForeignKey(e => e.ProductVariantId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Warehouse)
                    .WithMany(e => e.Transactions)
                    .HasForeignKey(e => e.WarehouseId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Batch)
                    .WithMany()
                    .HasForeignKey(e => e.BatchId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasIndex(e => e.ProductVariantId);
                entity.HasIndex(e => e.WarehouseId);
                entity.HasIndex(e => e.Type);
                entity.HasIndex(e => e.CreatedDate);
                entity.HasIndex(e => e.ReferenceNumber);
            });

            // ============ StockAlert Configuration ============
            modelBuilder.Entity<StockAlert>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Message).IsRequired().HasMaxLength(500);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedDate);

                entity.HasOne(e => e.ProductVariant)
                    .WithMany()
                    .HasForeignKey(e => e.ProductVariantId)
                    .OnDelete(DeleteBehavior.NoAction);  // Changed from Cascade to NoAction to prevent cycle

                entity.HasOne(e => e.Warehouse)
                    .WithMany()
                    .HasForeignKey(e => e.WarehouseId)
                    .OnDelete(DeleteBehavior.NoAction);  // Changed from Restrict to NoAction

                entity.HasOne(e => e.Batch)
                    .WithMany()
                    .HasForeignKey(e => e.BatchId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasIndex(e => e.Type);
                entity.HasIndex(e => e.Priority);
                entity.HasIndex(e => e.IsResolved);
                entity.HasIndex(e => e.CreatedDate);
            });

            // ============ PriceHistory Configuration ============
            modelBuilder.Entity<PriceHistory>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
                entity.HasOne(e => e.ProductVariant).WithMany(e => e.PriceHistories).HasForeignKey(e => e.ProductVariantId).OnDelete(DeleteBehavior.Cascade);
            });

            // ============ WarehouseLocation Configuration ============
            modelBuilder.Entity<WarehouseLocation>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.LocationCode).IsRequired().HasMaxLength(50);
                entity.Property(e => e.ZoneName).HasMaxLength(100);
                entity.Property(e => e.RackName).HasMaxLength(50);
                entity.Property(e => e.ShelfName).HasMaxLength(50);
                entity.Property(e => e.BinName).HasMaxLength(50);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Notes).HasMaxLength(500);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedDate);

                entity.HasOne(e => e.Warehouse)
                    .WithMany()
                    .HasForeignKey(e => e.WarehouseId)
                    .OnDelete(DeleteBehavior.SetNull);

                // Unique constraint: LocationCode must be unique across the entire system
                entity.HasIndex(e => e.LocationCode).IsUnique();
                entity.HasIndex(e => e.WarehouseId);
                entity.HasIndex(e => e.IsActive);
            });

            // ============ BatchLocationStock Configuration ============
            modelBuilder.Entity<BatchLocationStock>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.LocationCode).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Notes).HasMaxLength(500);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedDate);

                // Relationship with InventoryBatch
                entity.HasOne(e => e.InventoryBatch)
                    .WithMany()
                    .HasForeignKey(e => e.InventoryBatchId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.ProductVariant)
                    .WithMany()
                    .HasForeignKey(e => e.ProductVariantId)
                    .OnDelete(DeleteBehavior.Restrict);  // Changed from Cascade to Restrict to avoid multiple cascade paths

                entity.HasOne(e => e.Warehouse)
                    .WithMany(e => e.BatchLocationStocks)
                    .HasForeignKey(e => e.WarehouseId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relationship with WarehouseLocation
                entity.HasOne(e => e.WarehouseLocation)
                    .WithMany(e => e.BatchLocationStocks)
                    .HasForeignKey(e => e.WarehouseLocationId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Composite unique constraint: Mỗi batch chỉ có 1 record tại mỗi location trong 1 warehouse
                entity.HasIndex(e => new { e.InventoryBatchId, e.WarehouseLocationId }).IsUnique();

                // Indexes for queries
                entity.HasIndex(e => e.LocationCode);
                entity.HasIndex(e => e.WarehouseId);
                entity.HasIndex(e => e.ProductVariantId);
                entity.HasIndex(e => e.InventoryBatchId);
                entity.HasIndex(e => e.WarehouseLocationId);
                entity.HasIndex(e => e.IsPrimaryLocation);
            });

            // ============ LocationStockMovement Configuration ============
            modelBuilder.Entity<LocationStockMovement>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FromLocationCode).HasMaxLength(100);
                entity.Property(e => e.ToLocationCode).HasMaxLength(100);
                entity.Property(e => e.BatchNumber).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Reason).HasMaxLength(500);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedDate);

                entity.HasOne(e => e.ProductVariant)
                    .WithMany()
                    .HasForeignKey(e => e.ProductVariantId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Warehouse)
                    .WithMany()
                    .HasForeignKey(e => e.WarehouseId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Unique constraint: Same batch cannot be at same location in same warehouse
                // This ensures batch location consistency
                entity.HasIndex(e => new { e.BatchNumber, e.WarehouseId, e.ToLocationCode })
                    .IsUnique()
                    .HasFilter("[ToLocationCode] IS NOT NULL"); // Only apply when ToLocationCode is not null

                // Indexes for queries
                entity.HasIndex(e => e.ProductVariantId);
                entity.HasIndex(e => e.WarehouseId);
                entity.HasIndex(e => e.MovementDate);
                entity.HasIndex(e => e.BatchNumber);
                entity.HasIndex(e => e.FromLocationCode);
                entity.HasIndex(e => e.ToLocationCode);
            });

            // ============ Customer Configuration ============
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Email).HasMaxLength(255);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedDate);
                entity.Property(e => e.PhoneNumber).IsRequired(true);

                // Business Type relationship (nullable)
                entity.HasOne(e => e.BusinessType)
                      .WithMany()
                      .HasForeignKey(e => e.BusinessTypeId)
                      .OnDelete(DeleteBehavior.SetNull);

                // Enterprise Information fields
                entity.Property(e => e.CompanyName).HasMaxLength(200);
                entity.Property(e => e.TaxCode).HasMaxLength(20);
                entity.Property(e => e.BusinessRegistrationNumber).HasMaxLength(50);
                entity.Property(e => e.LegalRepresentative).HasMaxLength(100);
                entity.Property(e => e.BusinessLicenseUrl).HasMaxLength(500);
                entity.Property(e => e.BusinessAddress).HasMaxLength(500);
                entity.Property(e => e.BusinessPhone).HasMaxLength(20);
                entity.Property(e => e.BusinessEmail).HasMaxLength(100);

                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.TaxCode);
                entity.HasIndex(e => e.BusinessTypeId);

                // Approval Workflow Relationships
                entity.HasMany(e => e.Documents)
                    .WithOne(d => d.Customer)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.VerificationHistory)
                    .WithOne(v => v.Customer)
                    .HasForeignKey(v => v.CustomerId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.ApprovedByUser)
                    .WithMany()
                    .HasForeignKey(e => e.ApprovedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Sales Region relationship
                entity.HasOne(e => e.Region)
                    .WithMany(r => r.Customers)
                    .HasForeignKey(e => e.RegionId)
                    .OnDelete(DeleteBehavior.SetNull);

                // Index for region queries
                entity.HasIndex(e => e.RegionId);
            });

            // ============ CustomerPaymentAccount Configuration ============
            modelBuilder.Entity<CustomerPaymentAccount>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.Customer)
                      .WithMany(e => e.PaymentAccounts)
                      .HasForeignKey(e => e.CustomerId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(e => e.BankName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.AccountNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.AccountHolder).IsRequired().HasMaxLength(100);
                entity.Property(e => e.BankBranch).HasMaxLength(200);
                entity.Property(e => e.SwiftCode).HasMaxLength(20);
                entity.Property(e => e.Notes).HasMaxLength(500);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedDate);

                // Indexes for faster queries
                entity.HasIndex(e => e.CustomerId);
                entity.HasIndex(e => new { e.CustomerId, e.IsDefault });
            });

            // ============ SalesRegion Configuration ============
            modelBuilder.Entity<SalesRegion>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Code).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedDate);

                // Unique constraint on Code
                entity.HasIndex(e => e.Code).IsUnique();
                entity.HasIndex(e => e.IsActive);

                // Seed initial regions
                entity.HasData(
                    new SalesRegion
                    {
                        Id = 1,
                        Name = "Miền Bắc",
                        Code = "MB",
                        Description = "Khu vực miền Bắc Việt Nam",
                        IsActive = true,
                        CreatedDate = DateTime.UtcNow
                    },
                    new SalesRegion
                    {
                        Id = 2,
                        Name = "Miền Trung",
                        Code = "MT",
                        Description = "Khu vực miền Trung Việt Nam",
                        IsActive = true,
                        CreatedDate = DateTime.UtcNow
                    },
                    new SalesRegion
                    {
                        Id = 3,
                        Name = "Miền Nam",
                        Code = "MN",
                        Description = "Khu vực miền Nam Việt Nam",
                        IsActive = true,
                        CreatedDate = DateTime.UtcNow
                    }
                );
            });

            // ============ Address Configuration ============
            modelBuilder.Entity<Address>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedDate);
                entity.HasOne(e => e.Customer).WithMany(e => e.Addresses).HasForeignKey(e => e.CustomerId).OnDelete(DeleteBehavior.Cascade);
            });

            // ============ Order Configuration ============
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.OrderNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedDate);

                entity.HasOne(e => e.Customer).WithMany(e => e.Orders).HasForeignKey(e => e.CustomerId).OnDelete(DeleteBehavior.SetNull);

                // Location relationships
                entity.HasOne(e => e.Ward)
                    .WithMany()
                    .HasForeignKey(e => e.WardId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.Province)
                    .WithMany()
                    .HasForeignKey(e => e.ProvinceId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasIndex(e => e.OrderNumber).IsUnique();
                entity.HasIndex(e => e.WardId);
                entity.HasIndex(e => e.ProvinceId);
            });

            // ============ OrderItem Configuration ============
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TotalLineAmount).HasColumnType("decimal(18,2)");
                entity.HasOne(e => e.Order).WithMany(e => e.OrderItems).HasForeignKey(e => e.OrderId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.ProductVariant).WithMany().HasForeignKey(e => e.ProductVariantId).OnDelete(DeleteBehavior.Restrict);
            });

            // ============ OrderItemSnapshot Configuration ============
            modelBuilder.Entity<OrderItemSnapshot>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ProductName).IsRequired().HasMaxLength(255);
                entity.Property(e => e.SKU).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Barcode).HasMaxLength(100);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
                entity.Property(e => e.OriginalPrice).HasColumnType("decimal(18,2)");
                entity.Property(e => e.ThumbnailUrl).HasMaxLength(500);
                entity.Property(e => e.VariantImageUrl).HasMaxLength(500);
                entity.Property(e => e.CategoryName).HasMaxLength(255);
                entity.Property(e => e.BrandName).HasMaxLength(255);
                entity.Property(e => e.RegistrationNumber).HasMaxLength(100);

                entity.HasOne(e => e.OrderItem)
                    .WithOne()
                    .HasForeignKey<OrderItemSnapshot>(e => e.OrderItemId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.OrderItemId).IsUnique();
                entity.HasIndex(e => e.ProductVariantId);
            });

            // ============ OrderItemFulfillment Configuration ============
            modelBuilder.Entity<OrderItemFulfillment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedDate);
                entity.Property(e => e.Notes).HasMaxLength(500);

                entity.HasOne(e => e.OrderItem)
                    .WithMany(oi => oi.Fulfillments)
                    .HasForeignKey(e => e.OrderItemId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.InventoryBatch)
                    .WithMany()
                    .HasForeignKey(e => e.InventoryBatchId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.OrderItemId);
                entity.HasIndex(e => e.InventoryBatchId);
                entity.HasIndex(e => e.FulfilledDate);
            });

            // ============ ApplicationUser Configuration ============
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable(name: "Users");
                entity.Property(u => u.FullName).HasMaxLength(200);

                // Sales Hierarchy: Self-referencing relationship
                entity.HasOne(u => u.Manager)
                    .WithMany(u => u.SalesTeamMembers)
                    .HasForeignKey(u => u.ManagerId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Sales Hierarchy: Relationship with Customers
                entity.HasMany(u => u.AssignedCustomers)
                    .WithOne(c => c.SaleUser)
                    .HasForeignKey(c => c.SaleUserId)
                    .OnDelete(DeleteBehavior.SetNull);

                // Sales Region: Relationship with SalesRegion
                entity.HasOne(u => u.Region)
                    .WithMany(r => r.SalesUsers)
                    .HasForeignKey(u => u.RegionId)
                    .OnDelete(DeleteBehavior.SetNull);

                // Indexes for performance
                entity.HasIndex(u => u.ManagerId);
                entity.HasIndex(u => u.RegionId);
            });


            modelBuilder.Entity<ApplicationRole>().ToTable("Roles");
            modelBuilder.Entity<ApplicationRoleClaim>().ToTable("RoleClaims");

            // ============ ShoppingCart Configuration ============
            modelBuilder.Entity<ShoppingCart>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.SubTotal).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Note).HasMaxLength(500);
                entity.Property(e => e.SessionId).HasMaxLength(100);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedDate);

                entity.HasOne(e => e.Customer)
                      .WithMany()
                      .HasForeignKey(e => e.CustomerId)
                      .OnDelete(DeleteBehavior.SetNull);

                // Indexes
                entity.HasIndex(e => e.CustomerId);
                entity.HasIndex(e => e.SessionId);
                entity.HasIndex(e => e.CreatedDate);
            });

            // ============ ShoppingCartItem Configuration ============
            modelBuilder.Entity<ShoppingCartItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)");
                entity.Property(e => e.OriginalPrice).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TotalLineAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.ProductName).IsRequired().HasMaxLength(255);
                entity.Property(e => e.VariantSKU).IsRequired().HasMaxLength(100);
                entity.Property(e => e.ImageUrl).HasMaxLength(500);
                entity.Property(e => e.VariantAttributes).HasMaxLength(500);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedDate);

                entity.HasOne(e => e.ShoppingCart)
                      .WithMany(e => e.CartItems)
                      .HasForeignKey(e => e.ShoppingCartId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Product)
                      .WithMany()
                      .HasForeignKey(e => e.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.ProductVariant)
                      .WithMany()
                      .HasForeignKey(e => e.ProductVariantId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Indexes
                entity.HasIndex(e => e.ShoppingCartId);
                entity.HasIndex(e => e.ProductVariantId);
            });


            //modelBuilder.Entity<IdentityRole>().ToTable("Roles");
            modelBuilder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
            modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
            modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
            //modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
            modelBuilder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");

            modelBuilder.Entity<Country>().ToTable("Countries");
            modelBuilder.Entity<Province>().ToTable("Provinces");
            modelBuilder.Entity<Ward>().ToTable("Wards");


            // ============ ProductCollection Configuration ============
            modelBuilder.Entity<ProductCollection>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Slug).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedDate);

                entity.HasIndex(e => e.Slug).IsUnique();
                entity.HasIndex(e => e.Type);
                entity.HasIndex(e => e.IsActive);
            });

            // ============ ProductCollectionItem Configuration ============
            modelBuilder.Entity<ProductCollectionItem>(entity =>
            {
                entity.HasKey(e => new { e.ProductCollectionId, e.ProductId });

                entity.HasOne(e => e.ProductCollection)
                    .WithMany(c => c.Items)
                    .HasForeignKey(e => e.ProductCollectionId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Product)
                    .WithMany(p => p.CollectionItems)
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ============ ProductMaxOrderConfig Configuration ============
            modelBuilder.Entity<ProductMaxOrderConfig>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Reason).HasMaxLength(500);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedDate);

                entity.HasIndex(e => e.ProductId).IsUnique();
                entity.HasIndex(e => e.IsActive);

                entity.HasOne(e => e.Product)
                    .WithOne(p => p.MaxOrderConfig)
                    .HasForeignKey<ProductMaxOrderConfig>(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ProductStatusMap>(entity =>
            {
                entity.ToTable("ProductStatusMap");

                entity.HasKey(x => new { x.ProductVariantId, x.StatusType });

                entity.Property(x => x.StatusType)
                       .HasConversion<int>()
                       .IsRequired();

                entity.Property(x => x.StatusName)
                       .HasMaxLength(50)
                       .IsRequired();

                entity.HasOne(x => x.ProductVariant)
                       .WithMany()
                       .HasForeignKey(x => x.ProductVariantId)
                       .OnDelete(DeleteBehavior.Cascade);
            });

            // ============ CustomerDocument Configuration ============
            modelBuilder.Entity<CustomerDocument>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => e.CustomerId);
                entity.HasIndex(e => new { e.CustomerId, e.DocumentType });
                entity.HasIndex(e => e.UploadedFileId);
                entity.HasIndex(e => new { e.IsRequired, e.IsVerified });

                entity.HasOne(d => d.UploadedFile)
                    .WithMany()
                    .HasForeignKey(d => d.UploadedFileId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.VerifiedByUser)
                    .WithMany()
                    .HasForeignKey(d => d.VerifiedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ============ CustomerVerification Configuration ============
            modelBuilder.Entity<CustomerVerification>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => e.CustomerId);
                entity.HasIndex(e => e.ProcessedDate);
                entity.HasIndex(e => new { e.CustomerId, e.ProcessedDate });

                entity.HasOne(v => v.ProcessedByUser)
                    .WithMany()
                    .HasForeignKey(v => v.ProcessedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ============ Voucher Configuration ============
            modelBuilder.Entity<Voucher>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);

                entity.Property(e => e.DiscountType).HasConversion<int>().IsRequired();
                entity.Property(e => e.DiscountValue).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(e => e.MaxDiscountAmount).HasColumnType("decimal(18,2)");

                entity.Property(e => e.MinimumQuantityType).HasConversion<int?>();
                entity.Property(e => e.MinimumQuantityValue);
                entity.Property(e => e.MinimumOrderValue).HasColumnType("decimal(18,2)");

                entity.Property(e => e.ApplicableType).HasConversion<int>().IsRequired();

                entity.Property(e => e.TotalUsageLimit);
                entity.Property(e => e.UsagePerUserLimit);
                entity.Property(e => e.CurrentUsageCount).HasDefaultValue(0);

                entity.Property(e => e.CanStackWithOthers).HasDefaultValue(false);
                entity.Property(e => e.StackPriority);

                entity.Property(e => e.StartDate).IsRequired();
                entity.Property(e => e.EndDate).IsRequired();
                entity.Property(e => e.IsActive).HasDefaultValue(true);

                entity.Property(e => e.CreatedBy).HasMaxLength(100);
                entity.Property(e => e.UpdatedBy).HasMaxLength(100);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedDate);

                // Indexes
                entity.HasIndex(e => e.Code).IsUnique();
                entity.HasIndex(e => new { e.IsActive, e.StartDate, e.EndDate });
                entity.HasIndex(e => e.CanStackWithOthers);
            });

            // ============ VoucherCategory Configuration ============
            modelBuilder.Entity<VoucherCategory>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(e => e.Voucher)
                    .WithMany(v => v.VoucherCategories)
                    .HasForeignKey(e => e.VoucherId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Category)
                    .WithMany()
                    .HasForeignKey(e => e.CategoryId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Unique constraint: một voucher chỉ liên kết một lần với một category
                entity.HasIndex(e => new { e.VoucherId, e.CategoryId }).IsUnique();
                entity.HasIndex(e => e.VoucherId);
                entity.HasIndex(e => e.CategoryId);
            });

            // ============ VoucherProductVariant Configuration ============
            modelBuilder.Entity<VoucherProductVariant>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(e => e.Voucher)
                    .WithMany(v => v.VoucherProductVariants)
                    .HasForeignKey(e => e.VoucherId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.ProductVariant)
                    .WithMany()
                    .HasForeignKey(e => e.ProductVariantId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Unique constraint: một voucher chỉ liên kết một lần với một product variant
                entity.HasIndex(e => new { e.VoucherId, e.ProductVariantId }).IsUnique();
                entity.HasIndex(e => e.VoucherId);
                entity.HasIndex(e => e.ProductVariantId);
            });

            // ============ VoucherUsageHistory Configuration ============
            modelBuilder.Entity<VoucherUsageHistory>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.DiscountAmount).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(e => e.OrderTotalBeforeDiscount).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(e => e.OrderTotalAfterDiscount).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(e => e.UsedAt).HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(e => e.Voucher)
                    .WithMany(v => v.UsageHistory)
                    .HasForeignKey(e => e.VoucherId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Order)
                    .WithMany()
                    .HasForeignKey(e => e.OrderId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Indexes
                entity.HasIndex(e => e.VoucherId);
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.OrderId);
                entity.HasIndex(e => e.UsedAt);
            });

            // ============ OrderVoucher Configuration ============
            modelBuilder.Entity<OrderVoucher>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.AppliedOrder).IsRequired();
                entity.Property(e => e.DiscountAmount).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(e => e.Order)
                    .WithMany()
                    .HasForeignKey(e => e.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Voucher)
                    .WithMany(v => v.OrderVouchers)
                    .HasForeignKey(e => e.VoucherId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Unique constraint: một voucher chỉ áp dụng một lần cho một order
                entity.HasIndex(e => new { e.OrderId, e.VoucherId }).IsUnique();
                entity.HasIndex(e => e.OrderId);
                entity.HasIndex(e => e.VoucherId);
            });


            // ============ Payment Transaction ============
            modelBuilder.Entity<PaymentTransaction>(entity =>
            {
                entity.HasKey(e => e.Id);
                // Relationships
                entity.HasOne(x => x.Order)
                       .WithMany(o => o.PaymentTransactions)
                       .HasForeignKey(x => x.OrderId)
                       .OnDelete(DeleteBehavior.Restrict);

                // Fields
                entity.Property(x => x.TransactionCode)
                       .HasMaxLength(50);

                entity.HasIndex(x => x.TransactionCode)
                       .IsUnique();

                entity.Property(x => x.VNPAYTransactionNo)
                       .HasMaxLength(50);

                entity.Property(x => x.BankCode)
                       .HasMaxLength(20);

                entity.Property(x => x.Amount)
                       .HasColumnType("decimal(18,2)")
                       .IsRequired();

                entity.Property(x => x.PaymentGateway)
                       .HasMaxLength(50)
                       .HasDefaultValue("VNPAY");

                entity.Property(x => x.PaymentStatus)
                       .IsRequired();

                entity.Property(x => x.ResponseCode)
                       .HasMaxLength(10);

                entity.Property(x => x.Message)
                       .HasMaxLength(500);

                entity.Property(x => x.CardType)
                       .HasMaxLength(100);

                entity.Property(x => x.BankTranNo)
                       .HasMaxLength(50);
            });

            // ============ Campaign Configuration ============
            modelBuilder.Entity<Campaign>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CampaignCode).IsRequired().HasMaxLength(100);
                entity.Property(e => e.CampaignName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Budget).HasColumnType("decimal(18,2)");

                entity.HasIndex(e => e.CampaignCode).IsUnique();
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => new { e.StartDate, e.EndDate });
            });

            // ============ Banner Configuration ============
            modelBuilder.Entity<Banner>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.BannerCode).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Subtitle).HasMaxLength(200);
                entity.Property(e => e.ImageUrl).HasMaxLength(500);
                entity.Property(e => e.MobileImageUrl).HasMaxLength(500);
                entity.Property(e => e.BackgroundColor).HasMaxLength(50);
                entity.Property(e => e.LinkUrl).HasMaxLength(500);

                entity.HasOne(e => e.Campaign)
                    .WithMany(c => c.Banners)
                    .HasForeignKey(e => e.CampaignId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.BannerCode).IsUnique();
                entity.HasIndex(e => e.CampaignId);
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => new { e.ValidFrom, e.ValidTo });
                entity.HasIndex(e => e.DisplayOrder);
            });

            // ============ BannerSection Configuration ============
            modelBuilder.Entity<BannerSection>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.SectionCode).IsRequired().HasMaxLength(100);
                entity.Property(e => e.SectionName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.ImageUrl).HasMaxLength(500);

                entity.HasOne(e => e.Banner)
                    .WithMany(b => b.BannerSections)
                    .HasForeignKey(e => e.BannerId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.BannerId);
                entity.HasIndex(e => e.DisplayOrder);
            });

            // ============ Combo Configuration ============
            modelBuilder.Entity<Combo>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ComboCode).IsRequired().HasMaxLength(100);
                entity.Property(e => e.ComboName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.OriginalPrice).HasColumnType("decimal(18,2)");
                entity.Property(e => e.ComboPrice).HasColumnType("decimal(18,2)");
                entity.Property(e => e.DiscountAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.ImageUrl).HasMaxLength(500);

                entity.HasOne(e => e.Banner)
                    .WithMany(b => b.Combos)
                    .HasForeignKey(e => e.BannerId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasIndex(e => e.ComboCode).IsUnique();
                entity.HasIndex(e => e.BannerId);
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => new { e.ValidFrom, e.ValidTo });
            });

            // ============ ComboItem Configuration ============
            modelBuilder.Entity<ComboItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)");
                entity.Property(e => e.BadgeText).HasMaxLength(50);

                entity.HasOne(e => e.Combo)
                    .WithMany(c => c.ComboItems)
                    .HasForeignKey(e => e.ComboId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.ProductVariant)
                    .WithMany()
                    .HasForeignKey(e => e.ProductVariantId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.ComboId);
                entity.HasIndex(e => e.ProductVariantId);
                entity.HasIndex(e => e.DisplayOrder);
            });

            // ============ BannerAnalytics Configuration ============
            modelBuilder.Entity<BannerAnalytics>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.IpAddress).HasMaxLength(50);
                entity.Property(e => e.UserAgent).HasMaxLength(500);
                entity.Property(e => e.DeviceType).HasMaxLength(50);

                entity.HasOne(e => e.Banner)
                    .WithMany(b => b.BannerAnalytics)
                    .HasForeignKey(e => e.BannerId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.BannerId);
                entity.HasIndex(e => e.CustomerId);
                entity.HasIndex(e => e.EventType);
            });

            // ============ Purchase Order System Configuration ============

            // ============ Supplier Configuration ============
            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Email).HasMaxLength(255);
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.Property(e => e.Address).HasMaxLength(500);
                entity.Property(e => e.TaxCode).HasMaxLength(50);
                entity.Property(e => e.BankAccount).HasMaxLength(50);
                entity.Property(e => e.ImageUrl).HasMaxLength(500);
                entity.Property(e => e.CreditLimit).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Notes).HasMaxLength(1000);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedDate);

                entity.HasOne(e => e.Ward)
                    .WithMany()
                    .HasForeignKey(e => e.WardId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.Province)
                    .WithMany()
                    .HasForeignKey(e => e.ProvinceId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.Bank)
                    .WithMany()
                    .HasForeignKey(e => e.BankId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasIndex(e => e.Code).IsUnique();
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.TaxCode);
            });

            // ============ SupplierContact Configuration ============
            modelBuilder.Entity<SupplierContact>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Position).HasMaxLength(100);
                entity.Property(e => e.Department).HasMaxLength(100);
                entity.Property(e => e.Email).HasMaxLength(255);
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.Property(e => e.Mobile).HasMaxLength(20);
                entity.Property(e => e.ContactType).HasConversion<int>();
                entity.Property(e => e.Notes).HasMaxLength(1000);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedDate);

                entity.HasOne(e => e.Supplier)
                    .WithMany(s => s.SupplierContacts)
                    .HasForeignKey(e => e.SupplierId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.SupplierId);
                entity.HasIndex(e => new { e.SupplierId, e.IsPrimary });
                entity.HasIndex(e => e.IsActive);
            });

            // ============ PurchaseOrder Configuration ============
            modelBuilder.Entity<PurchaseOrder>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.OrderNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Status).HasConversion<int>();
                entity.Property(e => e.SubTotal).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TaxAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.DiscountAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.ShippingFee).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Notes).HasMaxLength(1000);
                entity.Property(e => e.Terms).HasMaxLength(2000);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedDate);

                entity.HasOne(e => e.Supplier)
                    .WithMany(s => s.PurchaseOrders)
                    .HasForeignKey(e => e.SupplierId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.SupplierContact)
                    .WithMany()
                    .HasForeignKey(e => e.SupplierContactId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.Warehouse)
                    .WithMany()
                    .HasForeignKey(e => e.WarehouseId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.OrderNumber).IsUnique();
                entity.HasIndex(e => e.SupplierId);
                entity.HasIndex(e => e.WarehouseId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.OrderDate);
            });

            // ============ PurchaseOrderItem Configuration ============
            modelBuilder.Entity<PurchaseOrderItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TaxRate).HasColumnType("decimal(18,2)");
                entity.Property(e => e.DiscountAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.ProductName).IsRequired().HasMaxLength(255);
                entity.Property(e => e.SKU).IsRequired().HasMaxLength(100);
                entity.Property(e => e.VariantOptions).HasMaxLength(1000);
                entity.Property(e => e.Notes).HasMaxLength(1000);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedDate);

                entity.HasOne(e => e.PurchaseOrder)
                    .WithMany(po => po.PurchaseOrderItems)
                    .HasForeignKey(e => e.PurchaseOrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.ProductVariant)
                    .WithMany()
                    .HasForeignKey(e => e.ProductVariantId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.PurchaseOrderId);
                entity.HasIndex(e => e.ProductVariantId);
            });

            // ============ PurchaseOrderHistory Configuration ============
            modelBuilder.Entity<PurchaseOrderHistory>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FromStatus).HasMaxLength(50);
                entity.Property(e => e.ToStatus).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Action).IsRequired().HasMaxLength(50);
                entity.Property(e => e.ChangeDetails).HasMaxLength(2000);
                entity.Property(e => e.Reason).HasMaxLength(500);
                entity.Property(e => e.Notes).HasMaxLength(1000);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedDate);

                entity.HasOne(e => e.PurchaseOrder)
                    .WithMany(po => po.PurchaseOrderHistories)
                    .HasForeignKey(e => e.PurchaseOrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.PurchaseOrderId);
                entity.HasIndex(e => e.ChangedDate);
                entity.HasIndex(e => e.Action);
            });

            // ============ GoodsReceipt Configuration ============
            modelBuilder.Entity<GoodsReceipt>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ReceiptNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Status).HasConversion<int>();
                entity.Property(e => e.ShippingCarrier).HasMaxLength(255);
                entity.Property(e => e.TrackingNumber).HasMaxLength(100);
                entity.Property(e => e.VehicleNumber).HasMaxLength(50);
                entity.Property(e => e.Notes).HasMaxLength(1000);
                entity.Property(e => e.RejectionReason).HasMaxLength(500);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedDate);

                entity.HasOne(e => e.PurchaseOrder)
                    .WithMany(po => po.GoodsReceipts)
                    .HasForeignKey(e => e.PurchaseOrderId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Warehouse)
                    .WithMany()
                    .HasForeignKey(e => e.WarehouseId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.ReceiptNumber).IsUnique();
                entity.HasIndex(e => e.PurchaseOrderId);
                entity.HasIndex(e => e.WarehouseId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.ReceivedDate);
            });

            // ============ GoodsReceiptItem Configuration ============
            modelBuilder.Entity<GoodsReceiptItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.QualityStatus).HasConversion<int>();
                entity.Property(e => e.BatchNumber).HasMaxLength(100);
                entity.Property(e => e.LocationCode).HasMaxLength(100);
                entity.Property(e => e.ShelfNumber).HasMaxLength(50);
                entity.Property(e => e.Notes).HasMaxLength(1000);
                entity.Property(e => e.InspectionNotes).HasMaxLength(1000);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedDate);

                entity.HasOne(e => e.GoodsReceipt)
                    .WithMany(gr => gr.GoodsReceiptItems)
                    .HasForeignKey(e => e.GoodsReceiptId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.PurchaseOrderItem)
                    .WithMany(poi => poi.GoodsReceiptItems)
                    .HasForeignKey(e => e.PurchaseOrderItemId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.GoodsReceiptId);
                entity.HasIndex(e => e.PurchaseOrderItemId);
                entity.HasIndex(e => e.BatchNumber);
                entity.HasIndex(e => e.ExpiryDate);
            });

            modelBuilder.Entity<OtpCode>(entity =>
            {
                entity.HasIndex(o => new { o.Phone, o.Code, o.IsUsed, o.Type });
            });
        }
    }
}
