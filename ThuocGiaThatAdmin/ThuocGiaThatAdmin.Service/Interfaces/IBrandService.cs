using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThatAdmin.Service.Interfaces
{
    /// <summary>
    /// Service interface for Brand business logic
    /// Defines contract for brand management operations
    /// </summary>
    public interface IBrandService
    {
        #region Read Operations

        /// <summary>
        /// Get brands with pagination
        /// </summary>
        /// <param name="pageNumber">Page number (1-based), defaults to 1</param>
        /// <param name="pageSize">Number of items per page (1-100), defaults to 10</param>
        /// <returns>Tuple containing list of brands and total count</returns>
        /// <exception cref="ArgumentException">Thrown when pageNumber is less than or equal to 0, or pageSize is not between 1 and 100</exception>
        Task<(IEnumerable<Brand> Brands, int TotalCount)> GetBrandsAsync(int pageNumber = 1, int pageSize = 10);

        /// <summary>
        /// Get brand by ID
        /// </summary>
        /// <param name="id">Brand ID</param>
        /// <returns>Brand if found, null otherwise</returns>
        /// <exception cref="ArgumentException">Thrown when ID is less than or equal to 0</exception>
        Task<Brand?> GetBrandByIdAsync(int id);

        #endregion

        #region Create Operation
        
        /// <summary>
        /// Create a new brand
        /// </summary>
        /// <param name="brand">Brand entity to create</param>
        /// <returns>Number of entities affected in the database</returns>
        /// <exception cref="ArgumentNullException">Thrown when brand is null</exception>
        /// <exception cref="ArgumentException">Thrown when brand name is null or empty</exception>
        Task<int> CreateAsync(Brand brand);

        #endregion

        #region Update Operation

        /// <summary>
        /// Update an existing brand
        /// </summary>
        /// <param name="brand">Brand entity with updated values</param>
        /// <returns>Number of entities affected in the database</returns>
        /// <exception cref="ArgumentNullException">Thrown when brand is null</exception>
        /// <exception cref="ArgumentException">Thrown when brand ID is less than or equal to 0, or name is null or empty</exception>
        /// <exception cref="InvalidOperationException">Thrown when brand with specified ID is not found</exception>
        Task<int> UpdateBrandAsync(Brand brand);

        #endregion

        #region Delete Operation

        /// <summary>
        /// Delete a brand by ID
        /// </summary>
        /// <param name="id">Brand ID to delete</param>
        /// <returns>Number of entities affected in the database</returns>
        /// <exception cref="ArgumentException">Thrown when ID is less than or equal to 0</exception>
        /// <exception cref="InvalidOperationException">Thrown when brand is not found or brand is being used by products</exception>
        Task<int> DeleteBrandAsync(int id);

        #endregion
    }
}