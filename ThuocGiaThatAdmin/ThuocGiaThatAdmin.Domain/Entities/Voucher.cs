using System;
using System.Collections.Generic;
using ThuocGiaThatAdmin.Domain.Enums;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    /// <summary>
    /// Entity chính cho voucher/mã giảm giá
    /// </summary>
    public class Voucher : AuditableEntity
    {
        // Id inherited from BaseEntity
        
        /// <summary>
        /// Mã voucher (unique)
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Tên voucher
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Mô tả voucher
        /// </summary>
        public string? Description { get; set; }

        // Loại giảm giá
        /// <summary>
        /// Loại giảm giá (Percentage, FixedAmount, FreeShipping, BuyXGetY)
        /// </summary>
        public DiscountType DiscountType { get; set; }

        /// <summary>
        /// Giá trị giảm giá (% hoặc số tiền)
        /// </summary>
        public decimal DiscountValue { get; set; }

        /// <summary>
        /// Số tiền giảm tối đa (cho loại Percentage)
        /// </summary>
        public decimal? MaxDiscountAmount { get; set; }

        // Điều kiện áp dụng
        /// <summary>
        /// Loại điều kiện số lượng (TotalQuantity, DistinctProducts, null = không yêu cầu)
        /// </summary>
        public MinimumQuantityType? MinimumQuantityType { get; set; }

        /// <summary>
        /// Giá trị số lượng tối thiểu
        /// </summary>
        public int? MinimumQuantityValue { get; set; }

        /// <summary>
        /// Giá trị đơn hàng tối thiểu
        /// </summary>
        public decimal? MinimumOrderValue { get; set; }

        // Phạm vi áp dụng
        /// <summary>
        /// Phạm vi áp dụng (All, Categories, ProductVariants, Mixed)
        /// </summary>
        public VoucherApplicableType ApplicableType { get; set; }

        // Giới hạn sử dụng
        /// <summary>
        /// Tổng số lần sử dụng tối đa (null = không giới hạn)
        /// </summary>
        public int? TotalUsageLimit { get; set; }

        /// <summary>
        /// Số lần sử dụng tối đa mỗi user (null = không giới hạn)
        /// </summary>
        public int? UsagePerUserLimit { get; set; }

        /// <summary>
        /// Số lần đã sử dụng hiện tại
        /// </summary>
        public int CurrentUsageCount { get; set; } = 0;

        // Stacking voucher
        /// <summary>
        /// Có thể stack với voucher khác không
        /// </summary>
        public bool CanStackWithOthers { get; set; } = false;

        /// <summary>
        /// Thứ tự ưu tiên khi stack (1 = cao nhất, null = không có priority)
        /// </summary>
        public int? StackPriority { get; set; }

        // Thời gian hiệu lực
        /// <summary>
        /// Ngày bắt đầu hiệu lực
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Ngày kết thúc hiệu lực
        /// </summary>
        public DateTime EndDate { get; set; }

        // Trạng thái
        /// <summary>
        /// Voucher có đang active không
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Người tạo voucher
        /// </summary>
        public string? CreatedBy { get; set; }

        /// <summary>
        /// Người cập nhật voucher
        /// </summary>
        public string? UpdatedBy { get; set; }

        // Navigation properties
        /// <summary>
        /// Danh sách danh mục áp dụng voucher
        /// </summary>
        public ICollection<VoucherCategory> VoucherCategories { get; set; } = new List<VoucherCategory>();

        /// <summary>
        /// Danh sách biến thể sản phẩm áp dụng voucher
        /// </summary>
        public ICollection<VoucherProductVariant> VoucherProductVariants { get; set; } = new List<VoucherProductVariant>();

        /// <summary>
        /// Lịch sử sử dụng voucher
        /// </summary>
        public ICollection<VoucherUsageHistory> UsageHistory { get; set; } = new List<VoucherUsageHistory>();

        /// <summary>
        /// Danh sách đơn hàng áp dụng voucher này
        /// </summary>
        public ICollection<OrderVoucher> OrderVouchers { get; set; } = new List<OrderVoucher>();
    }
}
