using System.Collections.Generic;

namespace ThuocGiaThatAdmin.Contracts.DTOs
{
    /// <summary>
    /// Request DTO cho một movement đơn lẻ trong quá trình picking
    /// </summary>
    public class PickingMovementRequestDto
    {
        /// <summary>
        /// Mã vị trí nguồn (nơi lấy hàng)
        /// Ví dụ: "KA-R1-S2-B3"
        /// </summary>
        public string FromLocationCode { get; set; } = string.Empty;
        
        /// <summary>
        /// Số lô hàng
        /// Ví dụ: "LOT-2024-001"
        /// </summary>
        public string BatchNumber { get; set; } = string.Empty;
        
        /// <summary>
        /// Số lượng cần chuyển
        /// </summary>
        public int Quantity { get; set; }
    }
}
