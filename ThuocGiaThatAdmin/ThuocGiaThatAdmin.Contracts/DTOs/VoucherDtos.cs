using System;
using System.Collections.Generic;
using ThuocGiaThatAdmin.Domain.Enums;

namespace ThuocGiaThatAdmin.Contracts.DTOs
{
    /// <summary>
    /// DTO cho voucher response
    /// </summary>
    public class VoucherResponseDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        
        public DiscountType DiscountType { get; set; }
        public string DiscountTypeName { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal? MaxDiscountAmount { get; set; }
        
        public MinimumQuantityType? MinimumQuantityType { get; set; }
        public int? MinimumQuantityValue { get; set; }
        public decimal? MinimumOrderValue { get; set; }
        
        public VoucherApplicableType ApplicableType { get; set; }
        
        public int? TotalUsageLimit { get; set; }
        public int? UsagePerUserLimit { get; set; }
        public int CurrentUsageCount { get; set; }
        
        public bool CanStackWithOthers { get; set; }
        public int? StackPriority { get; set; }
        
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        
        public DateTime CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        
        // Related data
        public List<int> CategoryIds { get; set; } = new();
        public List<int> ProductVariantIds { get; set; } = new();
    }

    /// <summary>
    /// DTO cho tạo voucher mới
    /// </summary>
    public class CreateVoucherDto
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        
        public DiscountType DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal? MaxDiscountAmount { get; set; }
        
        public MinimumQuantityType? MinimumQuantityType { get; set; }
        public int? MinimumQuantityValue { get; set; }
        public decimal? MinimumOrderValue { get; set; }
        
        public VoucherApplicableType ApplicableType { get; set; }
        
        public int? TotalUsageLimit { get; set; }
        public int? UsagePerUserLimit { get; set; }
        
        public bool CanStackWithOthers { get; set; }
        public int? StackPriority { get; set; }
        
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; } = true;
        
        public List<int> CategoryIds { get; set; } = new();
        public List<int> ProductVariantIds { get; set; } = new();
    }

    /// <summary>
    /// DTO cho cập nhật voucher
    /// </summary>
    public class UpdateVoucherDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        
        public decimal DiscountValue { get; set; }
        public decimal? MaxDiscountAmount { get; set; }
        
        public MinimumQuantityType? MinimumQuantityType { get; set; }
        public int? MinimumQuantityValue { get; set; }
        public decimal? MinimumOrderValue { get; set; }
        
        public int? TotalUsageLimit { get; set; }
        public int? UsagePerUserLimit { get; set; }
        
        public bool CanStackWithOthers { get; set; }
        public int? StackPriority { get; set; }
        
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        
        public List<int> CategoryIds { get; set; } = new();
        public List<int> ProductVariantIds { get; set; } = new();
    }

    /// <summary>
    /// DTO cho validate voucher request
    /// </summary>
    public class ValidateVoucherDto
    {
        public string VoucherCode { get; set; } = string.Empty;
        public string? UserId { get; set; }
        public List<CartItemForVoucherDto> CartItems { get; set; } = new();
        public decimal OrderSubTotal { get; set; }
    }

    /// <summary>
    /// DTO cho validate nhiều voucher (stacking)
    /// </summary>
    public class ValidateMultipleVouchersDto
    {
        public List<string> VoucherCodes { get; set; } = new();
        public string? UserId { get; set; }
        public List<CartItemForVoucherDto> CartItems { get; set; } = new();
        public decimal OrderSubTotal { get; set; }
    }

    /// <summary>
    /// DTO cho cart item khi validate voucher
    /// </summary>
    public class CartItemForVoucherDto
    {
        public int ProductVariantId { get; set; }
        public int CategoryId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }

    /// <summary>
    /// DTO cho kết quả validate voucher
    /// </summary>
    public class VoucherValidationResultDto
    {
        public bool IsValid { get; set; }
        public string? ErrorMessage { get; set; }
        public VoucherResponseDto? Voucher { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalAmount { get; set; }
    }

    /// <summary>
    /// DTO cho kết quả validate nhiều voucher
    /// </summary>
    public class MultipleVouchersValidationResultDto
    {
        public bool IsValid { get; set; }
        public string? ErrorMessage { get; set; }
        public List<VoucherApplicationDto> AppliedVouchers { get; set; } = new();
        public decimal TotalDiscountAmount { get; set; }
        public decimal FinalAmount { get; set; }
    }

    /// <summary>
    /// DTO cho voucher đã áp dụng
    /// </summary>
    public class VoucherApplicationDto
    {
        public VoucherResponseDto Voucher { get; set; } = null!;
        public int AppliedOrder { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal PriceBeforeDiscount { get; set; }
        public decimal PriceAfterDiscount { get; set; }
    }

    /// <summary>
    /// DTO cho lịch sử sử dụng voucher
    /// </summary>
    public class VoucherUsageHistoryDto
    {
        public int Id { get; set; }
        public int VoucherId { get; set; }
        public string VoucherCode { get; set; } = string.Empty;
        public string VoucherName { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public int OrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public decimal DiscountAmount { get; set; }
        public decimal OrderTotalBeforeDiscount { get; set; }
        public decimal OrderTotalAfterDiscount { get; set; }
        public DateTime UsedAt { get; set; }
    }
}
