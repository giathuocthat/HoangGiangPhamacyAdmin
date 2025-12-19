using System;
using System.Collections.Generic;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    /// <summary>
    /// Quản lý các vị trí trong kho
    /// LocationCode là duy nhất trong toàn hệ thống
    /// </summary>
    public class WarehouseLocation : AuditableEntity
    {
        /// <summary>
        /// Mã vị trí duy nhất trong hệ thống
        /// Format: Zone-Rack-Shelf-Bin (VD: "KA-R1-S2-B3")
        /// </summary>
        public string LocationCode { get; set; } = string.Empty;

        /// <summary>
        /// Kho mà vị trí này thuộc về (nullable - có thể chưa gán kho)
        /// </summary>
        public int? WarehouseId { get; set; }

        /// <summary>
        /// Tên khu vực (Zone)
        /// Ví dụ: "Khu A", "Khu lạnh"
        /// </summary>
        public string? ZoneName { get; set; }

        /// <summary>
        /// Tên/Mã giá (Rack)
        /// Ví dụ: "R1", "R2"
        /// </summary>
        public string? RackName { get; set; }

        /// <summary>
        /// Tên/Mã tầng (Shelf)
        /// Ví dụ: "S1", "S2"
        /// </summary>
        public string? ShelfName { get; set; }

        /// <summary>
        /// Tên/Mã ô (Bin)
        /// Ví dụ: "B1", "B2", "B3"
        /// </summary>
        public string? BinName { get; set; }

        /// <summary>
        /// Mô tả vị trí
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Trạng thái hoạt động
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Sức chứa tối đa (tùy chọn)
        /// </summary>
        public int? MaxCapacity { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string? Notes { get; set; }

        // Navigation properties
        public Warehouse? Warehouse { get; set; }
        public ICollection<BatchLocationStock> BatchLocationStocks { get; set; } = new List<BatchLocationStock>();
    }
}
