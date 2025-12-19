using System;
using System.Collections.Generic;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    /// <summary>
    /// Tồn kho của một lô hàng (InventoryBatch) tại một vị trí cụ thể trong kho
    /// Cho phép tracking chính xác lô nào đang ở vị trí nào
    /// </summary>
    public class BatchLocationStock : AuditableEntity
    {
        /// <summary>
        /// ID của lô hàng (InventoryBatch)
        /// </summary>
        public int InventoryBatchId { get; set; }
        
        public int ProductVariantId { get; set; }
        public int WarehouseId { get; set; }
        
        /// <summary>
        /// ID của vị trí kho (tham chiếu đến WarehouseLocation)
        /// </summary>
        public int WarehouseLocationId { get; set; }
        
        /// <summary>
        /// Mã vị trí (denormalized từ WarehouseLocation để truy vấn nhanh)
        /// </summary>
        public string LocationCode { get; set; } = string.Empty;
        
        /// <summary>
        /// Số lượng của lô này tại vị trí này
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
        /// Mỗi batch nên có 1 primary location trong mỗi kho
        /// </summary>
        public bool IsPrimaryLocation { get; set; } = false;
        
        public string? Notes { get; set; }
        
        // Navigation properties
        public InventoryBatch InventoryBatch { get; set; } = null!;
        public ProductVariant ProductVariant { get; set; } = null!;
        public Warehouse Warehouse { get; set; } = null!;
        public WarehouseLocation WarehouseLocation { get; set; } = null!;
        public ICollection<LocationStockMovement> Movements { get; set; } = new List<LocationStockMovement>();
    }
}
