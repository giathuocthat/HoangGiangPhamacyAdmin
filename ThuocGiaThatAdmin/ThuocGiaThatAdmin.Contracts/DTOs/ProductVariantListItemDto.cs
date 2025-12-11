namespace ThuocGiaThatAdmin.Contracts.DTOs
{
    /// <summary>
    /// Response DTO for product variant list item
    /// </summary>
    public class ProductVariantListItemDto
    {
        /// <summary>
        /// Product variant ID
        /// </summary>
        public int VariantId { get; set; }

        /// <summary>
        /// Product ID
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Product name
        /// </summary>
        public string ProductName { get; set; } = string.Empty;

        /// <summary>
        /// Product SKU
        /// </summary>
        public string SKU { get; set; } = string.Empty;

        /// <summary>
        /// Barcode
        /// </summary>
        public string? Barcode { get; set; }

        /// <summary>
        /// Selling price
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Original price
        /// </summary>
        public decimal? OriginalPrice { get; set; }

        /// <summary>
        /// Stock quantity
        /// </summary>
        public int StockQuantity { get; set; }

        /// <summary>
        /// Maximum sales quantity per order
        /// </summary>
        public int? MaxSalesQuantity { get; set; }

        /// <summary>
        /// Variant image URL
        /// </summary>
        public string? ImageUrl { get; set; }

        /// <summary>
        /// Product active status
        /// </summary>
        public bool IsProductActive { get; set; }

        /// <summary>
        /// Variant active status
        /// </summary>
        public bool IsVariantActive { get; set; }

        /// <summary>
        /// Product option values (e.g., "Màu: Đỏ, Size: L")
        /// </summary>
        public List<VariantOptionInfo> OptionValues { get; set; } = new();
    }

    /// <summary>
    /// Product variant option information
    /// </summary>
    public class VariantOptionInfo
    {
        /// <summary>
        /// Option ID
        /// </summary>
        public int OptionId { get; set; }

        /// <summary>
        /// Option name (e.g., "Màu sắc", "Kích thước")
        /// </summary>
        public string OptionName { get; set; } = string.Empty;

        /// <summary>
        /// Option value ID
        /// </summary>
        public int OptionValueId { get; set; }

        /// <summary>
        /// Option value (e.g., "Đỏ", "L")
        /// </summary>
        public string OptionValue { get; set; } = string.Empty;
    }
}
