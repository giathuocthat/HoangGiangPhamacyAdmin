namespace ThuocGiaThatAdmin.Contracts.DTOs
{
    /// <summary>
    /// Request DTO for filtering and searching product variants with pagination
    /// </summary>
    public class GetProductVariantsRequestDto
    {
        /// <summary>
        /// Page number (1-based)
        /// </summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// Number of items per page
        /// </summary>
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// Search keyword for product name or SKU
        /// </summary>
        public string? SearchKeyword { get; set; }

        /// <summary>
        /// Filter by product active status (null = all, true = active only, false = inactive only)
        /// </summary>
        public bool? IsProductActive { get; set; }

        /// <summary>
        /// Filter by variant active status (null = all, true = active only, false = inactive only)
        /// </summary>
        public bool? IsVariantActive { get; set; }
    }
}
