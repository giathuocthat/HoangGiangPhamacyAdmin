using System;

namespace ThuocGiaThatAdmin.Contracts.DTOs
{
    /// <summary>
    /// DTO for creating a new product review
    /// </summary>
    public class CreateProductReviewDto
    {
        public int CustomerId { get; set; }
        public int ProductId { get; set; }
        public string ReviewText { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO for updating an existing product review
    /// </summary>
    public class UpdateProductReviewDto
    {
        public string ReviewText { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO for product review response
    /// </summary>
    public class ProductReviewResponseDto
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ReviewText { get; set; } = string.Empty;
        public bool IsApproved { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
