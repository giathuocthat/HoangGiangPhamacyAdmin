using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThuocGiaThat.Infrastucture.Repositories;
using ThuocGiaThatAdmin.Contract.DTOs;
using ThuocGiaThatAdmin.Contract.Requests;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThatAdmin.Service.Services
{
    /// <summary>
    /// Service for Product business logic
    /// </summary>
    public class ProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        }

        #region Read Operations

        /// <summary>
        /// Get all products
        /// </summary>
        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _productRepository.GetAllWithBrandAndCategoryAsync();
        }

        /// <summary>
        /// Get product by ID
        /// </summary>
        public async Task<Product?> GetProductByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Product ID must be greater than 0", nameof(id));

            return await _productRepository.GetProductWithCategoryAsync(id);
        }

        /// <summary>
        /// Get products by category ID
        /// </summary>
        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            if (categoryId <= 0)
                throw new ArgumentException("Category ID must be greater than 0", nameof(categoryId));

            return await _productRepository.GetByCategoryIdAsync(categoryId);
        }

        /// <summary>
        /// Search products by name
        /// </summary>
        public async Task<IEnumerable<Product>> SearchProductsByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Product name cannot be null or empty", nameof(name));

            return await _productRepository.GetByNameAsync(name);
        }

        /// <summary>
        /// Get products with pagination
        /// </summary>
        /// <param name="pageNumber">Page number (1-based)</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <returns>Tuple containing list of products and total count</returns>
        public async Task<(IEnumerable<Product> products, int TotalCount)> GetPagedProductsAsync(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber <= 0)
                throw new ArgumentException("Page number must be greater than 0", nameof(pageNumber));

            if (pageSize <= 0 || pageSize > 100)
                throw new ArgumentException("Page size must be between 1 and 100", nameof(pageSize));

            return await _productRepository.GetPagedProductsAsync(pageNumber, pageSize);
        }

        #endregion

        #region Create Operation

        /// <summary>
        /// Create a new product
        /// </summary>
        public async Task<int> CreateProductAsync(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            if (string.IsNullOrWhiteSpace(product.Name))
                throw new ArgumentException("Product name cannot be null or empty", nameof(product.Name));

            if (product.CategoryId <= 0)
                throw new ArgumentException("Category ID must be greater than 0", nameof(product.CategoryId));

            await _productRepository.AddAsync(product);
            return await _productRepository.SaveChangesAsync();
        }

        /// <summary>
        /// Create multiple products
        /// </summary>
        public async Task<int> CreateProductsAsync(IEnumerable<Product> products)
        {
            if (products == null || !products.Any())
                throw new ArgumentException("Products list cannot be null or empty", nameof(products));

            foreach (var product in products)
            {
                if (string.IsNullOrWhiteSpace(product.Name))
                    throw new ArgumentException("Product name cannot be null or empty", nameof(product.Name));

                if (product.CategoryId <= 0)
                    throw new ArgumentException("Category ID must be greater than 0", nameof(product.CategoryId));
            }

            await _productRepository.AddRangeAsync(products);
            return await _productRepository.SaveChangesAsync();
        }

        #endregion

        #region Update Operation

        /// <summary>
        /// Update an existing product
        /// </summary>
        public async Task<int> UpdateProductAsync(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            if (product.Id <= 0)
                throw new ArgumentException("Product ID must be greater than 0", nameof(product.Id));

            if (string.IsNullOrWhiteSpace(product.Name))
                throw new ArgumentException("Product name cannot be null or empty", nameof(product.Name));

            if (product.CategoryId <= 0)
                throw new ArgumentException("Category ID must be greater than 0", nameof(product.CategoryId));

            // Check if product exists
            var existingProduct = await _productRepository.GetByIdAsync(product.Id);
            if (existingProduct == null)
                throw new InvalidOperationException($"Product with ID {product.Id} not found");

            _productRepository.Update(product);
            return await _productRepository.SaveChangesAsync();
        }

        #endregion

        #region Delete Operation

        /// <summary>
        /// Delete a product by ID
        /// </summary>
        public async Task<int> DeleteProductAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Product ID must be greater than 0", nameof(id));

            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                throw new InvalidOperationException($"Product with ID {id} not found");

            _productRepository.Delete(product);
            return await _productRepository.SaveChangesAsync();
        }

        /// <summary>
        /// Delete multiple products
        /// </summary>
        public async Task<int> DeleteProductsAsync(IEnumerable<int> ids)
        {
            if (ids == null || !ids.Any())
                throw new ArgumentException("Product IDs list cannot be null or empty", nameof(ids));

            var products = new List<Product>();
            foreach (var id in ids)
            {
                if (id <= 0)
                    throw new ArgumentException($"Product ID must be greater than 0, got: {id}", nameof(ids));

                var product = await _productRepository.GetByIdAsync(id);
                if (product == null)
                    throw new InvalidOperationException($"Product with ID {id} not found");

                products.Add(product);
            }

            _productRepository.DeleteRange(products);
            return await _productRepository.SaveChangesAsync();
        }

        #endregion

        
    }
}
