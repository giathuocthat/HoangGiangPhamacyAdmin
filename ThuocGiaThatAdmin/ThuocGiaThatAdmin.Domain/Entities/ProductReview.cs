using System;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    /// <summary>
    /// Product Review entity - Customer reviews for products
    /// </summary>
    public class ProductReview : AuditableEntity
    {
        // Id inherited from BaseEntity

        /// <summary>
        /// Customer who wrote the review
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// Product being reviewed
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Review text content
        /// </summary>
        public string ReviewText { get; set; } = string.Empty;

        /// <summary>
        /// Is the review approved by admin
        /// </summary>
        public bool IsApproved { get; set; } = false;

        // Navigation properties
        public Customer Customer { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}
