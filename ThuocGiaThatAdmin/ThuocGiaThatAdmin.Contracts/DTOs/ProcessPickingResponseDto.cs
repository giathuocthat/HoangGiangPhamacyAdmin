using System.Collections.Generic;

namespace ThuocGiaThatAdmin.Contracts.DTOs
{
    /// <summary>
    /// Response DTO cho kết quả xử lý picking
    /// </summary>
    public class ProcessPickingResponseDto
    {
        /// <summary>
        /// Tổng số movements được xử lý
        /// </summary>
        public int TotalMovements { get; set; }
        
        /// <summary>
        /// Số movements thành công
        /// </summary>
        public int SuccessfulMovements { get; set; }
        
        /// <summary>
        /// Số movements thất bại
        /// </summary>
        public int FailedMovements { get; set; }
        
        /// <summary>
        /// Chi tiết từng movement
        /// </summary>
        public List<PickingMovementResultDto> Movements { get; set; } = new();
    }
}
