using System.Collections.Generic;

namespace ThuocGiaThatAdmin.Contracts.DTOs
{
    /// <summary>
    /// Request DTO cho việc xử lý batch picking movements
    /// </summary>
    public class ProcessPickingRequestDto
    {
        /// <summary>
        /// ID đơn hàng (optional, để tracking)
        /// </summary>
        public int? OrderId { get; set; }
        
        /// <summary>
        /// ID kho
        /// </summary>
        public int WarehouseId { get; set; }
        
        /// <summary>
        /// Mã vị trí đích (thùng hàng/container)
        /// Ví dụ: "SHIP-CONTAINER-001"
        /// </summary>
        public string DestinationLocationCode { get; set; } = string.Empty;
        
        /// <summary>
        /// Danh sách các movements cần thực hiện
        /// </summary>
        public List<PickingMovementRequestDto> Movements { get; set; } = new();
        
        /// <summary>
        /// Ghi chú (optional)
        /// </summary>
        public string? Notes { get; set; }
    }
}
