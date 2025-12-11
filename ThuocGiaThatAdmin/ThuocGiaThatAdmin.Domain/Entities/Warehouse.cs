using System;
using System.Collections.Generic;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    /// <summary>
    /// Kho hàng hoặc chi nhánh
    /// </summary>
    public class Warehouse : AuditableEntity
    {
        /// <summary>
        /// Mã kho (unique)
        /// </summary>
        public string Code { get; set; } = string.Empty;
        
        /// <summary>
        /// Tên kho/chi nhánh
        /// </summary>
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// Loại: MainWarehouse, Branch, Storage
        /// </summary>
        public WarehouseType Type { get; set; }
        
        /// <summary>
        /// Địa chỉ
        /// </summary>
        public string? Address { get; set; }
        
        /// <summary>
        /// Phường/Xã
        /// </summary>
        public string? Ward { get; set; }
        
        /// <summary>
        /// Quận/Huyện
        /// </summary>
        public string? District { get; set; }
        
        /// <summary>
        /// Tỉnh/Thành phố
        /// </summary>
        public string? City { get; set; }
        
        /// <summary>
        /// Số điện thoại
        /// </summary>
        public string? PhoneNumber { get; set; }
        
        /// <summary>
        /// Email
        /// </summary>
        public string? Email { get; set; }
        
        /// <summary>
        /// Người quản lý kho
        /// </summary>
        public string? ManagerName { get; set; }
        
        /// <summary>
        /// Kho có đang hoạt động không
        /// </summary>
        public bool IsActive { get; set; } = true;
        
        /// <summary>
        /// Ghi chú
        /// </summary>
        public string? Notes { get; set; }
        
        // Navigation properties
        public ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
        public ICollection<InventoryTransaction> Transactions { get; set; } = new List<InventoryTransaction>();
        public ICollection<VariantLocationStock> VariantLocationStocks { get; set; } = new List<VariantLocationStock>();
    }
}
