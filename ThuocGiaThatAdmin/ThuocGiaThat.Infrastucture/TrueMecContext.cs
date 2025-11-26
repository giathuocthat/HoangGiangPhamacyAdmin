using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThat.Infrastucture
{
    public class TrueMecContext : IdentityDbContext<ApplicationUser>
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
        public DbSet<InventoryBatch> InventoryBatches { get; set; }
        public DbSet<InventoryTransaction> InventoryTransactions { get; set; }
        public DbSet<StockAlert> StockAlerts { get; set; }
        
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Province> Provinces { get; set; }
        public DbSet<Ward>  Wards { get; set; }
        public DbSet<UploadedFile> UploadedFiles { get; set; }
        
        public DbSet<BusinessType>  BusinessTypes { get; set; }

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

            // ============ Customer Configuration ============
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedDate);
                entity.HasIndex(e => e.Email).IsUnique();
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
                entity.HasIndex(e => e.OrderNumber).IsUnique();
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

            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable(name: "Users");
                entity.Property(u => u.FullName).HasMaxLength(200);
            });

            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable(name: "Users");
                entity.Property(u => u.FullName).HasMaxLength(200);
            });

            modelBuilder.Entity<IdentityRole>().ToTable("Roles");
            modelBuilder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
            modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
            modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
            modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
            modelBuilder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");

            modelBuilder.Entity<Country>().ToTable("Countries");
            modelBuilder.Entity<Province>().ToTable("Provinces");
            modelBuilder.Entity<Ward>().ToTable("Wards");
        }
    }
}
