using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Contracts.DTOs;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThatAdmin.Service.Interfaces
{
    /// <summary>
    /// Service interface for Voucher business logic
    /// </summary>
    public interface IVoucherService
    {
        // CRUD Operations
        Task<VoucherResponseDto?> GetByIdAsync(int id);
        Task<VoucherResponseDto?> GetByCodeAsync(string code);
        Task<IEnumerable<VoucherResponseDto>> GetAllActiveAsync();
        Task<VoucherResponseDto> CreateAsync(CreateVoucherDto dto, string createdBy);
        Task<VoucherResponseDto> UpdateAsync(int id, UpdateVoucherDto dto, string updatedBy);
        Task DeleteAsync(int id);

        // Validation
        Task<VoucherValidationResultDto> ValidateVoucherAsync(ValidateVoucherDto dto);
        Task<MultipleVouchersValidationResultDto> ValidateMultipleVouchersAsync(ValidateMultipleVouchersDto dto);

        // Calculation
        Task<decimal> CalculateDiscountAsync(string voucherCode, List<CartItemForVoucherDto> cartItems, decimal orderSubTotal);
        Task<(decimal totalDiscount, List<VoucherApplicationDto> applications)> CalculateStackedDiscountsAsync(
            List<string> voucherCodes, 
            List<CartItemForVoucherDto> cartItems, 
            decimal orderSubTotal);

        // Application
        Task ApplyVoucherToOrderAsync(int orderId, string voucherCode, string userId);
        Task ApplyMultipleVouchersToOrderAsync(int orderId, List<string> voucherCodes, string userId);

        // Query
        Task<IEnumerable<VoucherResponseDto>> GetAvailableVouchersForUserAsync(string userId);
        Task<IEnumerable<VoucherResponseDto>> GetStackableVouchersAsync();
        Task<IEnumerable<VoucherUsageHistoryDto>> GetUsageHistoryAsync(int voucherId, int pageNumber = 1, int pageSize = 20);
        Task<IEnumerable<VoucherUsageHistoryDto>> GetUserUsageHistoryAsync(string userId, int pageNumber = 1, int pageSize = 20);
    }
}
