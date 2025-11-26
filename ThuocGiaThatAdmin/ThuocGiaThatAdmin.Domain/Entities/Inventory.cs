using System;
using System.Collections.Generic;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    /// <summary>
    /// Tồn kho theo ProductVariant và Warehouse
    /// </summary>
    public class Inventory : AuditableEntity
    {
        /// <summary>
        /// ID của ProductVariant
        /// </summary>
        public int ProductVariantId { get; set; }
        
        /// <summary>
        /// ID của Warehouse (kho) - REQUIRED
        /// </summary>
        public int WarehouseId { get; set; }
        
        /// <summary>
        /// Số lượng tồn kho hiện tại
        /// </summary>
        public int QuantityOnHand { get; set; } = 0;
        
        /// <summary>
        /// Số lượng đang được đặt hàng (reserved)
        /// </summary>
        public int QuantityReserved { get; set; } = 0;
        
        /// <summary>
        /// Số lượng có thể bán = QuantityOnHand - QuantityReserved
        /// </summary>
        public int QuantityAvailable => QuantityOnHand - QuantityReserved;
        
        /// <summary>
        /// Mức tồn kho tối thiểu (cảnh báo khi dưới mức này)
        /// </summary>
        public int ReorderLevel { get; set; } = 10;
        
        /// <summary>
        /// Mức tồn kho tối đa
        /// </summary>
        public int MaxStockLevel { get; set; } = 1000;
        
        /// <summary>
        /// Số lượng đặt hàng lại (khi tồn kho thấp)
        /// </summary>
        public int ReorderQuantity { get; set; } = 50;
        
        /// <summary>
        /// Vị trí trong kho (shelf, bin location)
        /// </summary>
        public string? Location { get; set; }
        
        /// <summary>
        /// Ghi chú
        /// </summary>
        public string? Notes { get; set; }
        
        // Navigation properties
        public ProductVariant ProductVariant { get; set; } = null!;
        public Warehouse Warehouse { get; set; } = null!;
        public ICollection<InventoryBatch> Batches { get; set; } = new List<InventoryBatch>();
    }
}
