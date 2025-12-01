using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThuocGiaThat.Infrastucture.Repositories;
using ThuocGiaThatAdmin.Domain.Entities;
using ThuocGiaThatAdmin.Service.Interfaces;
    
namespace ThuocGiaThatAdmin.Service.Services
{
    /// <summary>
    /// Service for Brand business logic
    /// </summary>
    public class BrandService : IBrandService
    {
        private readonly IBrandRepository _brandRepository;

        public BrandService(IBrandRepository brandRepository)
        {
            _brandRepository = brandRepository ?? throw new ArgumentNullException(nameof(brandRepository));
        }

        #region Read Operations

        /// <summary>
        /// Get brands with pagination
        /// </summary>
        /// <param name="pageNumber">Page number (1-based)</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <returns>Tuple containing list of brands and total count</returns>
        public async Task<(IEnumerable<Brand> Brands, int TotalCount)> GetBrandsAsync(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber <= 0)
                throw new ArgumentException("Page number must be greater than 0", nameof(pageNumber));

            if (pageSize <= 0 || pageSize > 100)
                throw new ArgumentException("Page size must be between 1 and 100", nameof(pageSize));

            return await _brandRepository.GetBrandsWithPagingAsync(pageNumber, pageSize);
        }

        /// <summary>
        /// Get brand by ID
        /// </summary>
        public async Task<Brand?> GetBrandByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Brand ID must be greater than 0", nameof(id));

            return await _brandRepository.GetByIdAsync(id);
        }

        #endregion

        #region Update Operation

        /// <summary>
        /// Update an existing brand
        /// </summary>
        public async Task<int> UpdateBrandAsync(Brand brand)
        {
            if (brand == null)
                throw new ArgumentNullException(nameof(brand));

            if (brand.Id <= 0)
                throw new ArgumentException("Brand ID must be greater than 0", nameof(brand.Id));

            if (string.IsNullOrWhiteSpace(brand.Name))
                throw new ArgumentException("Brand name cannot be null or empty", nameof(brand.Name));

            // Check if brand exists
            var existingBrand = await _brandRepository.GetByIdAsync(brand.Id);
            if (existingBrand == null)
                throw new InvalidOperationException($"Brand with ID {brand.Id} not found");

            // Update only Brand's own properties, not navigation properties
            existingBrand.Name = brand.Name;
            existingBrand.Slug = brand.Slug;
            existingBrand.CountryOfOrigin = brand.CountryOfOrigin;
            existingBrand.Website = brand.Website;
            existingBrand.LogoUrl = brand.LogoUrl;
            existingBrand.IsActive = brand.IsActive;

            _brandRepository.Update(existingBrand);
            return await _brandRepository.SaveChangesAsync();
        }

        public async Task<int> CreateAsync(Brand brand)
        {
            if (brand == null)
                throw new ArgumentNullException(nameof(brand));

            if (string.IsNullOrWhiteSpace(brand.Name))
                throw new ArgumentException("Brand name cannot be null or empty", nameof(brand.Name));

            await _brandRepository.AddAsync(brand);
            return await _brandRepository.SaveChangesAsync();
        }

        #endregion

        #region Delete Operation

        /// <summary>
        /// Delete a brand by ID
        /// </summary>
        public async Task<int> DeleteBrandAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Brand ID must be greater than 0", nameof(id));

            var brand = await _brandRepository.GetByIdAsync(id);
            if (brand == null)
                throw new InvalidOperationException($"Brand with ID {id} not found");

            // Check if brand is being used by any products
            var hasProducts = await _brandRepository.HasProductsAsync(id);
            if (hasProducts)
                throw new InvalidOperationException($"Cannot delete brand with ID {id} because it is being used by one or more products");

            _brandRepository.Delete(brand);
            return await _brandRepository.SaveChangesAsync();
        }

        #endregion
    }
}
