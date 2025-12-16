using System;

namespace ThuocGiaThatAdmin.Contracts.DTOs
{
    /// <summary>
    /// Response DTO cho validation của location + batch
    /// </summary>
    public class ValidatePickingResponseDto
    {
        /// <summary>
        /// Location code và batch number có hợp lệ không
        /// </summary>
        public bool IsValid { get; set; }
        
        /// <summary>
        /// Mã vị trí
        /// </summary>
        public string? LocationCode { get; set; }
        
        /// <summary>
        /// Số lô
        /// </summary>
        public string? BatchNumber { get; set; }
        
        /// <summary>
        /// ID của ProductVariant
        /// </summary>
        public int? ProductVariantId { get; set; }
        
        /// <summary>
        /// SKU của sản phẩm
        /// </summary>
        public string? SKU { get; set; }
        
        /// <summary>
        /// Tên sản phẩm
        /// </summary>
        public string? ProductName { get; set; }
        
        /// <summary>
        /// Số lượng available tại vị trí này
        /// </summary>
        public int? AvailableQuantity { get; set; }
        
        /// <summary>
        /// Hạn sử dụng của lô
        /// </summary>
        public DateTime? ExpiryDate { get; set; }
        
        /// <summary>
        /// Thông báo lỗi (nếu không hợp lệ)
        /// </summary>
        public string? ErrorMessage { get; set; }
    }
}
