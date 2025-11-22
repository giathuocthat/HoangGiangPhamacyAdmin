using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThuocGiaThat.Infrastucture.Repositories;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThatAdmin.Service.Services
{
    /// <summary>
    /// Service for ProductOption business logic
    /// </summary>
    public class ProductOptionService
    {
        private readonly IProductOptionRepository _productOptionRepository;

        public ProductOptionService(IProductOptionRepository productOptionRepository)
        {
            _productOptionRepository = productOptionRepository ?? throw new ArgumentNullException(nameof(productOptionRepository));
        }

        #region Read Operations

        /// <summary>
        /// Get product options with pagination, including their values
        /// </summary>
        /// <param name="pageNumber">Page number (1-based)</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <returns>Tuple containing list of product options and total count</returns>
        public async Task<(IEnumerable<ProductOption> ProductOptions, int TotalCount)> GetProductOptionsAsync(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber <= 0)
                throw new ArgumentException("Page number must be greater than 0", nameof(pageNumber));

            if (pageSize <= 0 || pageSize > 100)
                throw new ArgumentException("Page size must be between 1 and 100", nameof(pageSize));

            return await _productOptionRepository.GetProductOptionsWithPagingAsync(pageNumber, pageSize);
        }

        /// <summary>
        /// Get product option by ID with its values
        /// </summary>
        public async Task<ProductOption?> GetProductOptionByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ProductOption ID must be greater than 0", nameof(id));

            return await _productOptionRepository.GetByIdWithValuesAsync(id);
        }

        #endregion

        #region Update Operation

        /// <summary>
        /// Update an existing product option with its values
        /// </summary>
        public async Task<int> UpdateProductOptionAsync(ProductOption productOption)
        {
            if (productOption == null)
                throw new ArgumentNullException(nameof(productOption));

            if (productOption.Id <= 0)
                throw new ArgumentException("ProductOption ID must be greater than 0", nameof(productOption.Id));

            if (string.IsNullOrWhiteSpace(productOption.Name))
                throw new ArgumentException("ProductOption name cannot be null or empty", nameof(productOption.Name));

            // Check if product option exists
            var existingProductOption = await _productOptionRepository.GetByIdWithValuesAsync(productOption.Id);
            if (existingProductOption == null)
                throw new InvalidOperationException($"ProductOption with ID {productOption.Id} not found");

            // Update only ProductOption's own properties
            existingProductOption.Name = productOption.Name;
            existingProductOption.DisplayOrder = productOption.DisplayOrder;
            existingProductOption.ProductId = productOption.ProductId;

            // Update ProductOptionValues
            // Remove old values that are not in the new list
            var newValueIds = productOption.ProductOptionValues?.Select(v => v.Id).ToList() ?? new List<int>();
            var valuesToRemove = existingProductOption.ProductOptionValues
                .Where(v => !newValueIds.Contains(v.Id))
                .ToList();

            foreach (var valueToRemove in valuesToRemove)
            {
                existingProductOption.ProductOptionValues.Remove(valueToRemove);
            }

            // Update existing values and add new ones
            if (productOption.ProductOptionValues != null)
            {
                foreach (var newValue in productOption.ProductOptionValues)
                {
                    var existingValue = existingProductOption.ProductOptionValues
                        .FirstOrDefault(v => v.Id == newValue.Id);

                    if (existingValue != null)
                    {
                        // Update existing value
                        existingValue.Value = newValue.Value;
                        existingValue.DisplayOrder = newValue.DisplayOrder;
                    }
                    else
                    {
                        // Add new value
                        newValue.ProductOptionId = existingProductOption.Id;
                        existingProductOption.ProductOptionValues.Add(newValue);
                    }
                }
            }

            _productOptionRepository.Update(existingProductOption);
            return await _productOptionRepository.SaveChangesAsync();
        }

        #endregion

        #region Delete Operation

        /// <summary>
        /// Delete a product option by ID
        /// </summary>
        public async Task<int> DeleteProductOptionAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ProductOption ID must be greater than 0", nameof(id));

            var productOption = await _productOptionRepository.GetByIdAsync(id);
            if (productOption == null)
                throw new InvalidOperationException($"ProductOption with ID {id} not found");

            // Check if product option is being used by any product
            var isUsedByProduct = await _productOptionRepository.IsUsedByProductAsync(id);
            if (isUsedByProduct)
                throw new InvalidOperationException($"Cannot delete ProductOption with ID {id} because it is being used by a product");

            _productOptionRepository.Delete(productOption);
            return await _productOptionRepository.SaveChangesAsync();
        }

        #endregion
    }
}
