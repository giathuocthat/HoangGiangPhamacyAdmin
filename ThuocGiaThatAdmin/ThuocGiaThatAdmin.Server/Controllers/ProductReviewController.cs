using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Contracts.DTOs;
using ThuocGiaThatAdmin.Service.Services;

namespace ThuocGiaThatAdmin.Server.Controllers
{
    /// <summary>
    /// Controller for managing product reviews
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ProductReviewController : BaseApiController
    {
        private readonly ProductReviewService _service;

        public ProductReviewController(ProductReviewService service, ILogger<ProductReviewController> logger)
            : base(logger)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        /// <summary>
        /// Get reviews by product ID with pagination
        /// </summary>
        /// <param name="productId">Product ID</param>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10)</param>
        /// <returns>Paginated list of reviews for the product</returns>
        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetReviewsByProduct(int productId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            return await ExecuteActionAsync(async () =>
            {
                var result = await _service.GetReviewsByProductAsync(productId, page, pageSize);
                return SuccessPaginated(result.Items, result.PageNumber, result.PageSize, result.TotalCount);
            }, "Get Reviews By Product");
        }

        /// <summary>
        /// Get reviews by customer ID with pagination
        /// </summary>
        /// <param name="customerId">Customer ID</param>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10)</param>
        /// <returns>Paginated list of reviews by the customer</returns>
        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetReviewsByCustomer(int customerId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            return await ExecuteActionAsync(async () =>
            {
                var result = await _service.GetReviewsByCustomerAsync(customerId, page, pageSize);
                return SuccessPaginated(result.Items, result.PageNumber, result.PageSize, result.TotalCount);
            }, "Get Reviews By Customer");
        }

        /// <summary>
        /// Add a new review
        /// </summary>
        /// <param name="dto">Review creation data</param>
        /// <returns>Created review</returns>
        [HttpPost]
        public async Task<IActionResult> AddReview([FromBody] CreateProductReviewDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                var review = await _service.AddReviewAsync(dto);
                return Created(review, "Review added successfully");
            }, "Add Review");
        }

        /// <summary>
        /// Update an existing review
        /// </summary>
        /// <param name="reviewId">Review ID</param>
        /// <param name="customerId">Customer ID (for authorization)</param>
        /// <param name="dto">Review update data</param>
        /// <returns>Updated review</returns>
        [HttpPut("{reviewId}")]
        public async Task<IActionResult> UpdateReview(int reviewId, [FromQuery] int customerId, [FromBody] UpdateProductReviewDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                var review = await _service.UpdateReviewAsync(reviewId, customerId, dto);
                return Success(review, "Review updated successfully");
            }, "Update Review");
        }

        /// <summary>
        /// Delete a review
        /// </summary>
        /// <param name="reviewId">Review ID</param>
        /// <param name="customerId">Customer ID (for authorization)</param>
        /// <returns>Deletion confirmation</returns>
        [HttpDelete("{reviewId}")]
        public async Task<IActionResult> DeleteReview(int reviewId, [FromQuery] int customerId)
        {
            return await ExecuteActionAsync(async () =>
            {
                await _service.DeleteReviewAsync(reviewId, customerId);
                return Success("Review deleted successfully");
            }, "Delete Review");
        }
    }
}
