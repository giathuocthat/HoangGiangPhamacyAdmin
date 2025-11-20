using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThat.Infrastucture
{
    public class AppContext : IdentityDbContext<ApplicationUser>
    {
        public AppContext(DbContextOptions<AppContext> options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ProductPrice> ProductPrices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ============ Category Configuration ============
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255);
                
                entity.Property(e => e.Description)
                    .HasMaxLength(1000);
                
                entity.Property(e => e.Slug)
                    .IsRequired()
                    .HasMaxLength(255);
                
                entity.Property(e => e.ImageUrl)
                    .HasMaxLength(500);
                
                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("GETUTCDATE()");
                
                // Parent-Child relationship
                entity.HasOne(e => e.ParentCategory)
                    .WithMany(e => e.ChildCategories)
                    .HasForeignKey(e => e.ParentCategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                // Index for performance
                entity.HasIndex(e => e.Slug).IsUnique();
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.DisplayOrder);
            });

            // ============ ProductType Configuration ============
            modelBuilder.Entity<ProductType>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255);
                
                entity.Property(e => e.Description)
                    .HasMaxLength(1000);
                
                entity.Property(e => e.Slug)
                    .IsRequired()
                    .HasMaxLength(255);
                
                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("GETUTCDATE()");
                
                // Index for performance
                entity.HasIndex(e => e.Slug).IsUnique();
                entity.HasIndex(e => e.IsActive);
            });

            // ============ Product Configuration ============
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255);
                
                entity.Property(e => e.ShortName)
                    .HasMaxLength(100);
                
                entity.Property(e => e.Description)
                    .HasMaxLength(5000);
                
                entity.Property(e => e.Ingredients)
                    .HasMaxLength(2000);
                
                entity.Property(e => e.UsageInstructions)
                    .HasMaxLength(2000);
                
                entity.Property(e => e.Warnings)
                    .HasMaxLength(1000);
                
                entity.Property(e => e.SKU)
                    .IsRequired()
                    .HasMaxLength(100);
                
                entity.Property(e => e.Barcode)
                    .HasMaxLength(50);
                
                entity.Property(e => e.Manufacturer)
                    .HasMaxLength(255);
                
                entity.Property(e => e.ManufacturingCountry)
                    .HasMaxLength(100);
                
                entity.Property(e => e.Strength)
                    .HasMaxLength(100);
                
                entity.Property(e => e.Unit)
                    .IsRequired()
                    .HasMaxLength(50);
                
                entity.Property(e => e.Slug)
                    .IsRequired()
                    .HasMaxLength(255);
                
                entity.Property(e => e.ThumbnailImage)
                    .HasMaxLength(500);
                
                entity.Property(e => e.CostPrice)
                    .HasColumnType("decimal(18,2)")
                    .HasDefaultValue(0);
                
                entity.Property(e => e.SalePrice)
                    .HasColumnType("decimal(18,2)")
                    .HasDefaultValue(0);
                
                entity.Property(e => e.DiscountPrice)
                    .HasColumnType("decimal(18,2)");
                
                entity.Property(e => e.Rating)
                    .HasColumnType("decimal(3,2)")
                    .HasDefaultValue(0);
                
                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("GETUTCDATE()");
                
                // Foreign Keys
                entity.HasOne(e => e.Category)
                    .WithMany(e => e.Products)
                    .HasForeignKey(e => e.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                entity.HasOne(e => e.ProductType)
                    .WithMany(e => e.Products)
                    .HasForeignKey(e => e.ProductTypeId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                // Indexes
                entity.HasIndex(e => e.SKU).IsUnique();
                entity.HasIndex(e => e.Slug).IsUnique();
                entity.HasIndex(e => e.CategoryId);
                entity.HasIndex(e => e.ProductTypeId);
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.IsFeatured);
                entity.HasIndex(e => new { e.IsActive, e.IsFeatured });
            });

            // ============ ProductImage Configuration ============
            modelBuilder.Entity<ProductImage>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.ImageUrl)
                    .IsRequired()
                    .HasMaxLength(500);
                
                entity.Property(e => e.AltText)
                    .HasMaxLength(255);
                
                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("GETUTCDATE()");
                
                // Foreign Key
                entity.HasOne(e => e.Product)
                    .WithMany(e => e.Images)
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                // Index
                entity.HasIndex(e => e.ProductId);
            });

            // ============ ProductPrice Configuration ============
            modelBuilder.Entity<ProductPrice>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Reason)
                    .HasMaxLength(500);
                
                entity.Property(e => e.CostPrice)
                    .HasColumnType("decimal(18,2)");
                
                entity.Property(e => e.SalePrice)
                    .HasColumnType("decimal(18,2)");
                
                entity.Property(e => e.DiscountPrice)
                    .HasColumnType("decimal(18,2)");
                
                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("GETUTCDATE()");
                
                // Foreign Key
                entity.HasOne(e => e.Product)
                    .WithMany(e => e.PriceHistory)
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                // Index
                entity.HasIndex(e => e.ProductId);
                entity.HasIndex(e => e.EffectiveDate);
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
        }
    }
}
