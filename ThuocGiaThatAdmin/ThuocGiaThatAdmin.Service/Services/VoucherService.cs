using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThuocGiaThat.Infrastucture.Repositories;
using ThuocGiaThatAdmin.Contracts.DTOs;
using ThuocGiaThatAdmin.Domain.Entities;
using ThuocGiaThatAdmin.Domain.Enums;
using ThuocGiaThatAdmin.Service.Interfaces;

namespace ThuocGiaThatAdmin.Service.Services
{
    /// <summary>
    /// Service for Voucher business logic
    /// </summary>
    public class VoucherService : IVoucherService
    {
        private readonly IVoucherRepository _voucherRepository;

        public VoucherService(IVoucherRepository voucherRepository)
        {
            _voucherRepository = voucherRepository ?? throw new ArgumentNullException(nameof(voucherRepository));
        }

        #region CRUD Operations


        public async Task<IEnumerable<VoucherResponseDto>> GetAllAsync()
        {
            var vouchers = await _voucherRepository.GetAllAsync();
            return vouchers.Select(MapToDto);
        }

        public async Task<VoucherResponseDto?> GetByIdAsync(int id)
        {
            var voucher = await _voucherRepository.GetByIdWithDetailsAsync(id);
            return voucher == null ? null : MapToDto(voucher);
        }

        public async Task<VoucherResponseDto?> GetByCodeAsync(string code)
        {
            var voucher = await _voucherRepository.GetByCodeWithDetailsAsync(code);
            return voucher == null ? null : MapToDto(voucher);
        }

        public async Task<IEnumerable<VoucherResponseDto>> GetAllActiveAsync()
        {
            var vouchers = await _voucherRepository.GetActiveVouchersAsync();
            return vouchers.Select(MapToDto);
        }

        public async Task<VoucherResponseDto> CreateAsync(CreateVoucherDto dto, string createdBy)
        {
            // Validate code uniqueness
            if (await _voucherRepository.CodeExistsAsync(dto.Code))
                throw new InvalidOperationException($"Voucher code '{dto.Code}' already exists");

            var voucher = new Voucher
            {
                Code = dto.Code,
                Name = dto.Name,
                Description = dto.Description,
                DiscountType = dto.DiscountType,
                DiscountValue = dto.DiscountValue,
                MaxDiscountAmount = dto.MaxDiscountAmount,
                MinimumQuantityType = dto.MinimumQuantityType,
                MinimumQuantityValue = dto.MinimumQuantityValue,
                MinimumOrderValue = dto.MinimumOrderValue,
                ApplicableType = dto.ApplicableType,
                TotalUsageLimit = dto.TotalUsageLimit,
                UsagePerUserLimit = dto.UsagePerUserLimit,
                CanStackWithOthers = dto.CanStackWithOthers,
                StackPriority = dto.StackPriority,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                IsActive = dto.IsActive,
                CreatedBy = createdBy,
                CreatedDate = DateTime.UtcNow
            };

            // Add category associations
            foreach (var categoryId in dto.CategoryIds)
            {
                voucher.VoucherCategories.Add(new VoucherCategory
                {
                    CategoryId = categoryId,
                    CreatedDate = DateTime.UtcNow
                });
            }

            // Add product variant associations
            foreach (var variantId in dto.ProductVariantIds)
            {
                voucher.VoucherProductVariants.Add(new VoucherProductVariant
                {
                    ProductVariantId = variantId,
                    CreatedDate = DateTime.UtcNow
                });
            }

            await _voucherRepository.AddAsync(voucher);
            await _voucherRepository.SaveChangesAsync();

            return MapToDto(voucher);
        }

        public async Task<VoucherResponseDto> UpdateAsync(int id, UpdateVoucherDto dto, string updatedBy)
        {
            var voucher = await _voucherRepository.GetByIdWithDetailsAsync(id);
            if (voucher == null)
                throw new InvalidOperationException($"Voucher with ID {id} not found");

            // Update properties
            voucher.Name = dto.Name;
            voucher.Description = dto.Description;
            voucher.DiscountValue = dto.DiscountValue;
            voucher.MaxDiscountAmount = dto.MaxDiscountAmount;
            voucher.MinimumQuantityType = dto.MinimumQuantityType;
            voucher.MinimumQuantityValue = dto.MinimumQuantityValue;
            voucher.MinimumOrderValue = dto.MinimumOrderValue;
            voucher.TotalUsageLimit = dto.TotalUsageLimit;
            voucher.UsagePerUserLimit = dto.UsagePerUserLimit;
            voucher.CanStackWithOthers = dto.CanStackWithOthers;
            voucher.StackPriority = dto.StackPriority;
            voucher.StartDate = dto.StartDate;
            voucher.EndDate = dto.EndDate;
            voucher.IsActive = dto.IsActive;
            voucher.UpdatedBy = updatedBy;
            voucher.UpdatedDate = DateTime.UtcNow;

            // Update categories (remove old, add new)
            voucher.VoucherCategories.Clear();
            foreach (var categoryId in dto.CategoryIds)
            {
                voucher.VoucherCategories.Add(new VoucherCategory
                {
                    VoucherId = voucher.Id,
                    CategoryId = categoryId,
                    CreatedDate = DateTime.UtcNow
                });
            }

            // Update product variants (remove old, add new)
            voucher.VoucherProductVariants.Clear();
            foreach (var variantId in dto.ProductVariantIds)
            {
                voucher.VoucherProductVariants.Add(new VoucherProductVariant
                {
                    VoucherId = voucher.Id,
                    ProductVariantId = variantId,
                    CreatedDate = DateTime.UtcNow
                });
            }

            _voucherRepository.Update(voucher);
            await _voucherRepository.SaveChangesAsync();

            return MapToDto(voucher);
        }

        public async Task DeleteAsync(int id)
        {
            var voucher = await _voucherRepository.GetByIdAsync(id);
            if (voucher == null)
                throw new InvalidOperationException($"Voucher with ID {id} not found");

            _voucherRepository.Delete(voucher);
            await _voucherRepository.SaveChangesAsync();
        }

        #endregion

        #region Validation

        public async Task<VoucherValidationResultDto> ValidateVoucherAsync(ValidateVoucherDto dto)
        {
            var voucher = await _voucherRepository.GetByCodeWithDetailsAsync(dto.VoucherCode);
            
            if (voucher == null)
            {
                return new VoucherValidationResultDto
                {
                    IsValid = false,
                    ErrorMessage = "Voucher not found"
                };
            }

            var validationError = await ValidateVoucherRules(voucher, dto.UserId, dto.CartItems, dto.OrderSubTotal);
            
            if (validationError != null)
            {
                return new VoucherValidationResultDto
                {
                    IsValid = false,
                    ErrorMessage = validationError,
                    Voucher = MapToDto(voucher)
                };
            }

            var discountAmount = CalculateDiscount(voucher, dto.CartItems, dto.OrderSubTotal);

            return new VoucherValidationResultDto
            {
                IsValid = true,
                Voucher = MapToDto(voucher),
                DiscountAmount = discountAmount,
                FinalAmount = dto.OrderSubTotal - discountAmount
            };
        }

        public async Task<MultipleVouchersValidationResultDto> ValidateMultipleVouchersAsync(ValidateMultipleVouchersDto dto)
        {
            var vouchers = await _voucherRepository.GetByCodesAsync(dto.VoucherCodes);
            
            if (!vouchers.Any())
            {
                return new MultipleVouchersValidationResultDto
                {
                    IsValid = false,
                    ErrorMessage = "No valid vouchers found"
                };
            }

            // Validate each voucher
            foreach (var voucher in vouchers)
            {
                var error = await ValidateVoucherRules(voucher, dto.UserId, dto.CartItems, dto.OrderSubTotal);
                if (error != null)
                {
                    return new MultipleVouchersValidationResultDto
                    {
                        IsValid = false,
                        ErrorMessage = $"Voucher '{voucher.Code}': {error}"
                    };
                }
            }

            // Check stacking compatibility
            var nonStackable = vouchers.Where(v => !v.CanStackWithOthers).ToList();
            if (nonStackable.Any() && vouchers.Count() > 1)
            {
                return new MultipleVouchersValidationResultDto
                {
                    IsValid = false,
                    ErrorMessage = $"Voucher '{nonStackable.First().Code}' cannot be stacked with other vouchers"
                };
            }

            // Calculate stacked discounts
            var (totalDiscount, applications) = await CalculateStackedDiscountsAsync(dto.VoucherCodes, dto.CartItems, dto.OrderSubTotal);

            return new MultipleVouchersValidationResultDto
            {
                IsValid = true,
                AppliedVouchers = applications,
                TotalDiscountAmount = totalDiscount,
                FinalAmount = dto.OrderSubTotal - totalDiscount
            };
        }

        #endregion

        #region Calculation

        public async Task<decimal> CalculateDiscountAsync(string voucherCode, List<CartItemForVoucherDto> cartItems, decimal orderSubTotal)
        {
            var voucher = await _voucherRepository.GetByCodeWithDetailsAsync(voucherCode);
            if (voucher == null)
                throw new InvalidOperationException("Voucher not found");

            return CalculateDiscount(voucher, cartItems, orderSubTotal);
        }

        public async Task<(decimal totalDiscount, List<VoucherApplicationDto> applications)> CalculateStackedDiscountsAsync(
            List<string> voucherCodes, 
            List<CartItemForVoucherDto> cartItems, 
            decimal orderSubTotal)
        {
            var vouchers = (await _voucherRepository.GetByCodesAsync(voucherCodes))
                .OrderBy(v => v.StackPriority ?? int.MaxValue)
                .ToList();

            var applications = new List<VoucherApplicationDto>();
            var currentPrice = orderSubTotal;
            var appliedOrder = 1;

            foreach (var voucher in vouchers)
            {
                var discount = CalculateDiscount(voucher, cartItems, currentPrice);
                var priceAfterDiscount = currentPrice - discount;

                applications.Add(new VoucherApplicationDto
                {
                    Voucher = MapToDto(voucher),
                    AppliedOrder = appliedOrder++,
                    DiscountAmount = discount,
                    PriceBeforeDiscount = currentPrice,
                    PriceAfterDiscount = priceAfterDiscount
                });

                currentPrice = priceAfterDiscount;
            }

            var totalDiscount = orderSubTotal - currentPrice;
            return (totalDiscount, applications);
        }

        #endregion

        #region Helper Methods

        private async Task<string?> ValidateVoucherRules(Voucher voucher, string? userId, List<CartItemForVoucherDto> cartItems, decimal orderSubTotal)
        {
            // Check if active
            if (!voucher.IsActive)
                return "Voucher is not active";

            // Check date range
            var now = DateTime.UtcNow;
            if (now < voucher.StartDate)
                return "Voucher is not yet valid";
            if (now > voucher.EndDate)
                return "Voucher has expired";

            // Check total usage limit
            if (voucher.TotalUsageLimit.HasValue && voucher.CurrentUsageCount >= voucher.TotalUsageLimit.Value)
                return "Voucher usage limit reached";

            // Check per-user usage limit
            if (!string.IsNullOrEmpty(userId) && voucher.UsagePerUserLimit.HasValue)
            {
                var userUsageCount = await _voucherRepository.GetUserVoucherUsageCountAsync(voucher.Id, userId);
                if (userUsageCount >= voucher.UsagePerUserLimit.Value)
                    return "You have reached the usage limit for this voucher";
            }

            // Check minimum order value
            if (voucher.MinimumOrderValue.HasValue && orderSubTotal < voucher.MinimumOrderValue.Value)
                return $"Minimum order value of {voucher.MinimumOrderValue.Value:C} required";

            // Check minimum quantity
            if (voucher.MinimumQuantityType.HasValue && voucher.MinimumQuantityValue.HasValue)
            {
                var meetsQuantityRequirement = voucher.MinimumQuantityType.Value switch
                {
                    MinimumQuantityType.TotalQuantity => cartItems.Sum(i => i.Quantity) >= voucher.MinimumQuantityValue.Value,
                    MinimumQuantityType.DistinctProducts => cartItems.Count >= voucher.MinimumQuantityValue.Value,
                    _ => true
                };

                if (!meetsQuantityRequirement)
                {
                    var requirementType = voucher.MinimumQuantityType.Value == MinimumQuantityType.TotalQuantity 
                        ? "total quantity" 
                        : "different products";
                    return $"Minimum {voucher.MinimumQuantityValue.Value} {requirementType} required";
                }
            }

            // Check applicable scope
            if (voucher.ApplicableType != VoucherApplicableType.All)
            {
                var hasApplicableItems = false;

                if (voucher.ApplicableType == VoucherApplicableType.Categories || voucher.ApplicableType == VoucherApplicableType.Mixed)
                {
                    var applicableCategoryIds = voucher.VoucherCategories.Select(vc => vc.CategoryId).ToList();
                    hasApplicableItems = cartItems.Any(item => applicableCategoryIds.Contains(item.CategoryId));
                }

                if (!hasApplicableItems && (voucher.ApplicableType == VoucherApplicableType.ProductVariants || voucher.ApplicableType == VoucherApplicableType.Mixed))
                {
                    var applicableVariantIds = voucher.VoucherProductVariants.Select(vpv => vpv.ProductVariantId).ToList();
                    hasApplicableItems = cartItems.Any(item => applicableVariantIds.Contains(item.ProductVariantId));
                }

                if (!hasApplicableItems)
                    return "No applicable products in cart";
            }

            return null; // Valid
        }

        private decimal CalculateDiscount(Voucher voucher, List<CartItemForVoucherDto> cartItems, decimal orderSubTotal)
        {
            decimal discount = 0;

            switch (voucher.DiscountType)
            {
                case DiscountType.Percentage:
                    discount = orderSubTotal * (voucher.DiscountValue / 100);
                    if (voucher.MaxDiscountAmount.HasValue && discount > voucher.MaxDiscountAmount.Value)
                        discount = voucher.MaxDiscountAmount.Value;
                    break;

                case DiscountType.FixedAmount:
                    discount = Math.Min(voucher.DiscountValue, orderSubTotal);
                    break;

                case DiscountType.FreeShipping:
                    // This would need shipping cost from order
                    discount = 0; // Placeholder
                    break;

                case DiscountType.BuyXGetY:
                    // Complex logic - placeholder
                    discount = 0;
                    break;
            }

            return discount;
        }

        private VoucherResponseDto MapToDto(Voucher voucher)
        {
            return new VoucherResponseDto
            {
                Id = voucher.Id,
                Code = voucher.Code,
                Name = voucher.Name,
                Description = voucher.Description,
                DiscountType = voucher.DiscountType,
                DiscountValue = voucher.DiscountValue,
                MaxDiscountAmount = voucher.MaxDiscountAmount,
                MinimumQuantityType = voucher.MinimumQuantityType,
                MinimumQuantityValue = voucher.MinimumQuantityValue,
                MinimumOrderValue = voucher.MinimumOrderValue,
                ApplicableType = voucher.ApplicableType,
                TotalUsageLimit = voucher.TotalUsageLimit,
                UsagePerUserLimit = voucher.UsagePerUserLimit,
                CurrentUsageCount = voucher.CurrentUsageCount,
                CanStackWithOthers = voucher.CanStackWithOthers,
                StackPriority = voucher.StackPriority,
                StartDate = voucher.StartDate,
                EndDate = voucher.EndDate,
                IsActive = voucher.IsActive,
                CreatedDate = voucher.CreatedDate,
                CreatedBy = voucher.CreatedBy,
                UpdatedDate = voucher.UpdatedDate,
                UpdatedBy = voucher.UpdatedBy,
                DiscountTypeName = Enum.GetName(typeof(DiscountType), voucher.DiscountType),
                CategoryIds = voucher.VoucherCategories?.Select(vc => vc.CategoryId).ToList() ?? [],
                ProductVariantIds = voucher.VoucherProductVariants?.Select(vpv => vpv.ProductVariantId).ToList() ?? []
            };
        }

        #endregion

        #region Application & Query Methods (Stub implementations)

        public async Task ApplyVoucherToOrderAsync(int orderId, string voucherCode, string userId)
        {
            // Implementation would create VoucherUsageHistory and OrderVoucher records
            await Task.CompletedTask;
            throw new NotImplementedException("Apply voucher to order - to be implemented with Order integration");
        }

        public async Task ApplyMultipleVouchersToOrderAsync(int orderId, List<string> voucherCodes, string userId)
        {
            await Task.CompletedTask;
            throw new NotImplementedException("Apply multiple vouchers - to be implemented with Order integration");
        }

        public async Task<IEnumerable<VoucherResponseDto>> GetAvailableVouchersForUserAsync(string userId)
        {
            var vouchers = await _voucherRepository.GetActiveVouchersAsync();
            return vouchers.Select(MapToDto);
        }

        public async Task<IEnumerable<VoucherResponseDto>> GetStackableVouchersAsync()
        {
            var vouchers = await _voucherRepository.GetStackableVouchersAsync();
            return vouchers.Select(MapToDto);
        }

        public async Task<IEnumerable<VoucherUsageHistoryDto>> GetUsageHistoryAsync(int voucherId, int pageNumber = 1, int pageSize = 20)
        {
            var history = await _voucherRepository.GetUsageHistoryAsync(voucherId, pageNumber, pageSize);
            return history.Select(h => new VoucherUsageHistoryDto
            {
                Id = h.Id,
                VoucherId = h.VoucherId,
                VoucherCode = h.Voucher?.Code ?? "",
                VoucherName = h.Voucher?.Name ?? "",
                UserId = h.UserId,
                UserName = h.User?.FullName ?? "",
                OrderId = h.OrderId,
                OrderNumber = h.Order?.OrderNumber ?? "",
                DiscountAmount = h.DiscountAmount,
                OrderTotalBeforeDiscount = h.OrderTotalBeforeDiscount,
                OrderTotalAfterDiscount = h.OrderTotalAfterDiscount,
                UsedAt = h.UsedAt
            });
        }

        public async Task<IEnumerable<VoucherUsageHistoryDto>> GetUserUsageHistoryAsync(string userId, int pageNumber = 1, int pageSize = 20)
        {
            var history = await _voucherRepository.GetUserUsageHistoryAsync(userId, pageNumber, pageSize);
            return history.Select(h => new VoucherUsageHistoryDto
            {
                Id = h.Id,
                VoucherId = h.VoucherId,
                VoucherCode = h.Voucher?.Code ?? "",
                VoucherName = h.Voucher?.Name ?? "",
                UserId = h.UserId,
                UserName = h.User?.FullName ?? "",
                OrderId = h.OrderId,
                OrderNumber = h.Order?.OrderNumber ?? "",
                DiscountAmount = h.DiscountAmount,
                OrderTotalBeforeDiscount = h.OrderTotalBeforeDiscount,
                OrderTotalAfterDiscount = h.OrderTotalAfterDiscount,
                UsedAt = h.UsedAt
            });
        }

        #endregion
    }
}
