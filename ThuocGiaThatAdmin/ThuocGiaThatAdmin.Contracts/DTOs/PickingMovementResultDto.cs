namespace ThuocGiaThatAdmin.Contracts.DTOs
{
    /// <summary>
    /// Response DTO cho kết quả của một movement
    /// </summary>
    public class PickingMovementResultDto
    {
        /// <summary>
        /// ID của LocationStockMovement record (nếu thành công)
        /// </summary>
        public int? MovementId { get; set; }
        
        /// <summary>
        /// Mã vị trí nguồn
        /// </summary>
        public string FromLocationCode { get; set; } = string.Empty;
        
        /// <summary>
        /// Mã vị trí đích
        /// </summary>
        public string ToLocationCode { get; set; } = string.Empty;
        
        /// <summary>
        /// Số lô
        /// </summary>
        public string BatchNumber { get; set; } = string.Empty;
        
        /// <summary>
        /// ID của ProductVariant
        /// </summary>
        public int ProductVariantId { get; set; }
        
        /// <summary>
        /// SKU của sản phẩm
        /// </summary>
        public string SKU { get; set; } = string.Empty;
        
        /// <summary>
        /// Tên sản phẩm
        /// </summary>
        public string ProductName { get; set; } = string.Empty;
        
        /// <summary>
        /// Số lượng đã chuyển
        /// </summary>
        public int Quantity { get; set; }
        
        /// <summary>
        /// Trạng thái: "Success" hoặc "Failed"
        /// </summary>
        public string Status { get; set; } = string.Empty;
        
        /// <summary>
        /// Thông báo lỗi (nếu failed)
        /// </summary>
        public string? ErrorMessage { get; set; }
    }
}
