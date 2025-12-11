using System;
using System.Collections.Generic;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    /// <summary>
    /// Tồn kho của ProductVariant tại một vị trí cụ thể trong kho
    /// Cho phép một variant có nhiều vị trí lưu trữ khác nhau
    /// </summary>
    public class VariantLocationStock : AuditableEntity
    {
        public int ProductVariantId { get; set; }
        public int WarehouseId { get; set; }
        
        /// <summary>
        /// Mã vị trí đầy đủ theo format: Zone-Rack-Shelf-Bin
        /// Ví dụ: "KA-R1-S2-B3"
        /// </summary>
        public string LocationCode { get; set; } = string.Empty;
        
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
        /// Số lượng tại vị trí này
        /// </summary>
        public int Quantity { get; set; } = 0;
        
        /// <summary>
        /// Số lượng đang được reserved tại vị trí này
        /// </summary>
        public int QuantityReserved { get; set; } = 0;
        
        /// <summary>
        /// Số lượng có thể dùng = Quantity - QuantityReserved
        /// </summary>
        public int QuantityAvailable => Quantity - QuantityReserved;
        
        /// <summary>
        /// Có phải vị trí chính (primary) không
        /// Mỗi variant nên có 1 primary location trong mỗi kho
        /// </summary>
        public bool IsPrimaryLocation { get; set; } = false;
        
        public string? Notes { get; set; }
        
        // Navigation properties
        public ProductVariant ProductVariant { get; set; } = null!;
        public Warehouse Warehouse { get; set; } = null!;
        public ICollection<LocationStockMovement> Movements { get; set; } = new List<LocationStockMovement>();
    }
}
