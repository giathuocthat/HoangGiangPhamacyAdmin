using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThuocGiaThat.Infrastucture;
using ThuocGiaThat.Infrastucture.Common;
using ThuocGiaThatAdmin.Contracts.DTOs;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThatAdmin.Service.Services
{
    public class ProductReviewService
    {
        private readonly TrueMecContext _context;

        public ProductReviewService(TrueMecContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get reviews by product ID with pagination
        /// </summary>
        public async Task<PagedResult<ProductReviewResponseDto>> GetReviewsByProductAsync(int productId, int pageNumber, int pageSize)
        {
            var query = _context.ProductReviews
                .Where(r => r.ProductId == productId)
                .Include(r => r.Customer)
                .Include(r => r.Product)
                .OrderByDescending(r => r.CreatedDate);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new ProductReviewResponseDto
                {
                    Id = r.Id,
                    CustomerId = r.CustomerId,
                    CustomerName = r.Customer.FullName,
                    ProductId = r.ProductId,
                    ProductName = r.Product.Name,
                    ReviewText = r.ReviewText,
                    IsApproved = r.IsApproved,
                    CreatedDate = r.CreatedDate,
                    UpdatedDate = r.UpdatedDate
                })
                .ToListAsync();

            return new PagedResult<ProductReviewResponseDto>(items, totalCount, pageNumber, pageSize);
        }

        /// <summary>
        /// Get reviews by customer ID with pagination
        /// </summary>
        public async Task<PagedResult<ProductReviewResponseDto>> GetReviewsByCustomerAsync(int customerId, int pageNumber, int pageSize)
        {
            var query = _context.ProductReviews
                .Where(r => r.CustomerId == customerId)
                .Include(r => r.Customer)
                .Include(r => r.Product)
                .OrderByDescending(r => r.CreatedDate);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new ProductReviewResponseDto
                {
                    Id = r.Id,
                    CustomerId = r.CustomerId,
                    CustomerName = r.Customer.FullName,
                    ProductId = r.ProductId,
                    ProductName = r.Product.Name,
                    ReviewText = r.ReviewText,
                    IsApproved = r.IsApproved,
                    CreatedDate = r.CreatedDate,
                    UpdatedDate = r.UpdatedDate
                })
                .ToListAsync();

            return new PagedResult<ProductReviewResponseDto>(items, totalCount, pageNumber, pageSize);
        }

        /// <summary>
        /// Add a new review by customer
        /// </summary>
        public async Task<ProductReviewResponseDto> AddReviewAsync(CreateProductReviewDto dto)
        {
            // Verify customer exists
            var customer = await _context.Customers.FindAsync(dto.CustomerId);
            if (customer == null)
            {
                throw new InvalidOperationException($"Customer with ID {dto.CustomerId} not found");
            }

            // Verify product exists
            var product = await _context.Products.FindAsync(dto.ProductId);
            if (product == null)
            {
                throw new InvalidOperationException($"Product with ID {dto.ProductId} not found");
            }

            var review = new ProductReview
            {
                CustomerId = dto.CustomerId,
                ProductId = dto.ProductId,
                ReviewText = dto.ReviewText,
                IsApproved = false // Reviews need approval by default
            };

            _context.ProductReviews.Add(review);
            await _context.SaveChangesAsync();

            // Return the created review
            return new ProductReviewResponseDto
            {
                Id = review.Id,
                CustomerId = review.CustomerId,
                CustomerName = customer.FullName,
                ProductId = review.ProductId,
                ProductName = product.Name,
                ReviewText = review.ReviewText,
                IsApproved = review.IsApproved,
                CreatedDate = review.CreatedDate,
                UpdatedDate = review.UpdatedDate
            };
        }

        /// <summary>
        /// Update an existing review by customer
        /// </summary>
        public async Task<ProductReviewResponseDto> UpdateReviewAsync(int reviewId, int customerId, UpdateProductReviewDto dto)
        {
            var review = await _context.ProductReviews
                .Include(r => r.Customer)
                .Include(r => r.Product)
                .FirstOrDefaultAsync(r => r.Id == reviewId);

            if (review == null)
            {
                throw new InvalidOperationException($"Review with ID {reviewId} not found");
            }

            // Verify the review belongs to the customer
            if (review.CustomerId != customerId)
            {
                throw new UnauthorizedAccessException("You can only edit your own reviews");
            }

            review.ReviewText = dto.ReviewText;
            review.UpdatedDate = DateTime.UtcNow;
            // Reset approval status when review is edited
            review.IsApproved = false;

            await _context.SaveChangesAsync();

            return new ProductReviewResponseDto
            {
                Id = review.Id,
                CustomerId = review.CustomerId,
                CustomerName = review.Customer.FullName,
                ProductId = review.ProductId,
                ProductName = review.Product.Name,
                ReviewText = review.ReviewText,
                IsApproved = review.IsApproved,
                CreatedDate = review.CreatedDate,
                UpdatedDate = review.UpdatedDate
            };
        }

        /// <summary>
        /// Delete a review by customer
        /// </summary>
        public async Task DeleteReviewAsync(int reviewId, int customerId)
        {
            var review = await _context.ProductReviews
                .FirstOrDefaultAsync(r => r.Id == reviewId);

            if (review == null)
            {
                throw new InvalidOperationException($"Review with ID {reviewId} not found");
            }

            // Verify the review belongs to the customer
            if (review.CustomerId != customerId)
            {
                throw new UnauthorizedAccessException("You can only delete your own reviews");
            }

            _context.ProductReviews.Remove(review);
            await _context.SaveChangesAsync();
        }
    }
}
