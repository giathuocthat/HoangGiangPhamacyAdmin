using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThuocGiaThat.Infrastucture.Repositories;
using ThuocGiaThatAdmin.Contracts.DTOs;
using ThuocGiaThatAdmin.Domain.Entities;
using ThuocGiaThatAdmin.Service.Interfaces;

namespace ThuocGiaThatAdmin.Service.Services
{
    /// <summary>
    /// Service for Combo business logic
    /// </summary>
    public class ComboService : IComboService
    {
        private readonly IComboRepository _comboRepository;
        private readonly IBannerRepository _bannerRepository;

        public ComboService(
            IComboRepository comboRepository,
            IBannerRepository bannerRepository)
        {
            _comboRepository = comboRepository ?? throw new ArgumentNullException(nameof(comboRepository));
            _bannerRepository = bannerRepository ?? throw new ArgumentNullException(nameof(bannerRepository));
        }

        #region CRUD Operations

        public async Task<IEnumerable<ComboResponseDto>> GetAllAsync()
        {
            var combos = await _comboRepository.GetAllAsync();
            return combos.Select(MapToDto);
        }

        public async Task<ComboResponseDto?> GetByIdAsync(int id)
        {
            var combo = await _comboRepository.GetWithItemsAsync(id);
            return combo == null ? null : MapToDto(combo);
        }

        public async Task<ComboResponseDto?> GetByCodeAsync(string code)
        {
            var combo = await _comboRepository.GetByCodeAsync(code);
            return combo == null ? null : MapToDto(combo);
        }

        public async Task<IEnumerable<ComboResponseDto>> GetActiveCombosAsync()
        {
            var combos = await _comboRepository.GetActiveCombosAsync();
            return combos.Select(MapToDto);
        }

        public async Task<ComboResponseDto> CreateAsync(CreateComboDto dto, string createdBy)
        {
            // Validate code uniqueness
            if (await _comboRepository.CodeExistsAsync(dto.ComboCode))
                throw new InvalidOperationException($"Combo code '{dto.ComboCode}' already exists");

            // Validate banner if specified
            if (dto.BannerId.HasValue)
            {
                var banner = await _bannerRepository.GetByIdAsync(dto.BannerId.Value);
                if (banner == null)
                    throw new InvalidOperationException($"Banner with ID {dto.BannerId.Value} not found");
            }

            // Validate combo
            await ValidateComboAsync(dto);

            var combo = new Combo
            {
                BannerId = dto.BannerId,
                ComboCode = dto.ComboCode,
                ComboName = dto.ComboName,
                Description = dto.Description,
                OriginalPrice = dto.OriginalPrice,
                ComboPrice = dto.ComboPrice,
                DiscountAmount = dto.DiscountAmount,
                DiscountPercentage = dto.DiscountPercentage,
                ImageUrl = dto.ImageUrl,
                ValidFrom = dto.ValidFrom,
                ValidTo = dto.ValidTo,
                IsActive = dto.IsActive,
                DisplayOrder = dto.DisplayOrder,
                CreatedDate = DateTime.Now
            };

            // Add items
            foreach (var itemDto in dto.Items)
            {
                combo.ComboItems.Add(new ComboItem
                {
                    ProductVariantId = itemDto.ProductVariantId,
                    Quantity = itemDto.Quantity,
                    UnitPrice = itemDto.UnitPrice,
                    BadgeText = itemDto.BadgeText,
                    DisplayOrder = itemDto.DisplayOrder,
                    IsRequired = itemDto.IsRequired,
                    CreatedDate = DateTime.Now
                });
            }

            await _comboRepository.AddAsync(combo);
            await _comboRepository.SaveChangesAsync();

            return MapToDto(combo);
        }

        public async Task<ComboResponseDto> UpdateAsync(int id, UpdateComboDto dto, string updatedBy)
        {
            var combo = await _comboRepository.GetWithItemsAsync(id);
            if (combo == null)
                throw new InvalidOperationException($"Combo with ID {id} not found");

            // Validate combo
            var validateDto = new CreateComboDto
            {
                ComboCode = combo.ComboCode ?? string.Empty,
                ComboName = dto.ComboName,
                OriginalPrice = dto.OriginalPrice,
                ComboPrice = dto.ComboPrice,
                DiscountAmount = dto.DiscountAmount,
                ValidFrom = dto.ValidFrom,
                ValidTo = dto.ValidTo,
                Items = dto.Items
            };
            await ValidateComboAsync(validateDto);

            // Update properties
            combo.ComboName = dto.ComboName;
            combo.Description = dto.Description;
            combo.OriginalPrice = dto.OriginalPrice;
            combo.ComboPrice = dto.ComboPrice;
            combo.DiscountAmount = dto.DiscountAmount;
            combo.DiscountPercentage = dto.DiscountPercentage;
            combo.ImageUrl = dto.ImageUrl;
            combo.ValidFrom = dto.ValidFrom;
            combo.ValidTo = dto.ValidTo;
            combo.IsActive = dto.IsActive;
            combo.DisplayOrder = dto.DisplayOrder;
            combo.UpdatedDate = DateTime.Now;

            // Update items (remove old, add new)
            combo.ComboItems.Clear();
            foreach (var itemDto in dto.Items)
            {
                combo.ComboItems.Add(new ComboItem
                {
                    ComboId = combo.Id,
                    ProductVariantId = itemDto.ProductVariantId,
                    Quantity = itemDto.Quantity,
                    UnitPrice = itemDto.UnitPrice,
                    BadgeText = itemDto.BadgeText,
                    DisplayOrder = itemDto.DisplayOrder,
                    IsRequired = itemDto.IsRequired,
                    CreatedDate = DateTime.Now
                });
            }

            _comboRepository.Update(combo);
            await _comboRepository.SaveChangesAsync();

            return MapToDto(combo);
        }

        public async Task DeleteAsync(int id)
        {
            var combo = await _comboRepository.GetByIdAsync(id);
            if (combo == null)
                throw new InvalidOperationException($"Combo with ID {id} not found");

            _comboRepository.Delete(combo);
            await _comboRepository.SaveChangesAsync();
        }

        #endregion

        #region Query Methods

        public async Task<IEnumerable<ComboResponseDto>> GetByBannerAsync(int bannerId)
        {
            var combos = await _comboRepository.GetByBannerAsync(bannerId);
            return combos.Select(MapToDto);
        }

        public async Task<(IEnumerable<ComboListItemDto>, int totalCount)> GetPagedCombosAsync(int pageNumber = 1, int pageSize = 20)
        {
            var (combos, totalCount) = await _comboRepository.GetPagedCombosAsync(pageNumber, pageSize);
            var dtos = combos.Select(MapToListItemDto);
            return (dtos, totalCount);
        }

        #endregion

        #region Validation

        public async Task<bool> CheckStockAvailabilityAsync(int comboId, int quantity)
        {
            return await _comboRepository.CheckStockAvailabilityAsync(comboId, quantity);
        }

        public async Task ValidateComboAsync(CreateComboDto dto)
        {
            // Validate date range
            if (dto.ValidFrom >= dto.ValidTo)
                throw new InvalidOperationException("Valid from date must be before valid to date");

            // Validate pricing
            if (dto.ComboPrice >= dto.OriginalPrice)
                throw new InvalidOperationException("Combo price must be less than original price");

            if (dto.DiscountAmount != dto.OriginalPrice - dto.ComboPrice)
                throw new InvalidOperationException("Discount amount must equal original price minus combo price");

            // Validate items
            if (dto.Items == null || !dto.Items.Any())
                throw new InvalidOperationException("Combo must have at least one item");

            // Validate item prices sum
            var itemsTotal = dto.Items.Sum(i => i.UnitPrice * i.Quantity);
            if (Math.Abs(itemsTotal - dto.OriginalPrice) > 0.01m)
                throw new InvalidOperationException("Sum of item prices must equal original price");

            await Task.CompletedTask;
        }

        #endregion

        #region Helper Methods

        private ComboResponseDto MapToDto(Combo combo)
        {
            return new ComboResponseDto
            {
                Id = combo.Id,
                BannerId = combo.BannerId,
                BannerTitle = combo.Banner?.Title,
                ComboCode = combo.ComboCode ?? string.Empty,
                ComboName = combo.ComboName ?? string.Empty,
                Description = combo.Description,
                OriginalPrice = combo.OriginalPrice,
                ComboPrice = combo.ComboPrice,
                DiscountAmount = combo.DiscountAmount,
                DiscountPercentage = combo.DiscountPercentage,
                ImageUrl = combo.ImageUrl,
                ValidFrom = combo.ValidFrom,
                ValidTo = combo.ValidTo,
                IsActive = combo.IsActive,
                DisplayOrder = combo.DisplayOrder,
                CreatedDate = combo.CreatedDate,
                UpdatedDate = combo.UpdatedDate,
                Items = combo.ComboItems?.Select(MapItemToDto).ToList() ?? new List<ComboItemDto>()
            };
        }

        private ComboListItemDto MapToListItemDto(Combo combo)
        {
            return new ComboListItemDto
            {
                Id = combo.Id,
                ComboCode = combo.ComboCode ?? string.Empty,
                ComboName = combo.ComboName ?? string.Empty,
                OriginalPrice = combo.OriginalPrice,
                ComboPrice = combo.ComboPrice,
                DiscountAmount = combo.DiscountAmount,
                ImageUrl = combo.ImageUrl,
                IsActive = combo.IsActive,
                ValidFrom = combo.ValidFrom,
                ValidTo = combo.ValidTo,
                ItemCount = combo.ComboItems?.Count ?? 0
            };
        }

        private ComboItemDto MapItemToDto(ComboItem item)
        {
            return new ComboItemDto
            {
                Id = item.Id,
                ComboId = item.ComboId,
                ProductVariantId = item.ProductVariantId,
                ProductName = item.ProductVariant?.Product?.Name ?? string.Empty,
                VariantName = item.ProductVariant?.SKU ?? string.Empty,
                ImageUrl = item.ProductVariant?.ImageUrl,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                BadgeText = item.BadgeText,
                DisplayOrder = item.DisplayOrder,
                IsRequired = item.IsRequired
            };
        }

        #endregion
    }
}
