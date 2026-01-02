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
    /// Service for Banner business logic
    /// </summary>
    public class BannerService : IBannerService
    {
        private readonly IBannerRepository _bannerRepository;
        private readonly ICampaignRepository _campaignRepository;

        public BannerService(
            IBannerRepository bannerRepository,
            ICampaignRepository campaignRepository)
        {
            _bannerRepository = bannerRepository ?? throw new ArgumentNullException(nameof(bannerRepository));
            _campaignRepository = campaignRepository ?? throw new ArgumentNullException(nameof(campaignRepository));
        }

        #region CRUD Operations

        public async Task<IEnumerable<BannerResponseDto>> GetAllAsync()
        {
            var banners = await _bannerRepository.GetAllAsync(x => x.BannerSections);
            return banners.Select(MapToDto);
        }

        public async Task<BannerResponseDto?> GetByIdAsync(int id)
        {
            var banner = await _bannerRepository.GetWithDetailsAsync(id);
            return banner == null ? null : MapToDto(banner);
        }

        public async Task<BannerResponseDto?> GetByCodeAsync(string code)
        {
            var banner = await _bannerRepository.GetByCodeAsync(code);
            return banner == null ? null : MapToDto(banner);
        }

        public async Task<IEnumerable<BannerResponseDto>> GetActiveBannersAsync()
        {
            var banners = await _bannerRepository.GetActiveBannersAsync();
            return banners.Select(MapToDto);
        }

        public async Task<BannerResponseDto> CreateAsync(CreateBannerDto dto, string createdBy)
        {
            // Validate code uniqueness
            if (await _bannerRepository.CodeExistsAsync(dto.BannerCode))
                throw new InvalidOperationException($"Banner code '{dto.BannerCode}' already exists");

            // Validate campaign exists
            var campaign = await _campaignRepository.GetByIdAsync(dto.CampaignId);
            if (campaign == null)
                throw new InvalidOperationException($"Campaign with ID {dto.CampaignId} not found");

            // Validate date range
            if (dto.ValidFrom >= dto.ValidTo)
                throw new InvalidOperationException("Valid from date must be before valid to date");

            var banner = new Banner
            {
                CampaignId = dto.CampaignId,
                BannerCode = dto.BannerCode,
                Title = dto.Title,
                Subtitle = dto.Subtitle,
                Description = dto.Description,
                ImageUrl = dto.ImageUrl,
                MobileImageUrl = dto.MobileImageUrl,
                BackgroundColor = dto.BackgroundColor,
                BannerType = (int)dto.BannerType,
                LinkType = (int)dto.LinkType,
                LinkUrl = dto.LinkUrl,
                DisplayOrder = dto.DisplayOrder,
                ValidFrom = dto.ValidFrom,
                ValidTo = dto.ValidTo,
                IsActive = dto.IsActive,
                ViewCount = 0,
                ClickCount = 0,
                CreatedDate = DateTime.Now
            };

            // Add sections
            foreach (var sectionDto in dto.Sections)
            {
                banner.BannerSections.Add(new BannerSection
                {
                    SectionCode = sectionDto.SectionCode,
                    SectionName = sectionDto.SectionName,
                    Content = sectionDto.Content,
                    ImageUrl = sectionDto.ImageUrl,
                    DisplayOrder = sectionDto.DisplayOrder,
                    IsActive = sectionDto.IsActive,
                    CreatedDate = DateTime.Now
                });
            }

            await _bannerRepository.AddAsync(banner);
            await _bannerRepository.SaveChangesAsync();

            return MapToDto(banner);
        }

        public async Task<BannerResponseDto> UpdateAsync(int id, UpdateBannerDto dto, string updatedBy)
        {
            var banner = await _bannerRepository.GetWithDetailsAsync(id);
            if (banner == null)
                throw new InvalidOperationException($"Banner with ID {id} not found");

            // Validate date range
            if (dto.ValidFrom >= dto.ValidTo)
                throw new InvalidOperationException("Valid from date must be before valid to date");

            // Update properties
            banner.CampaignId = dto.CampaignId;
            banner.Title = dto.Title;
            banner.Subtitle = dto.Subtitle;
            banner.Description = dto.Description;
            banner.ImageUrl = dto.ImageUrl;
            banner.MobileImageUrl = dto.MobileImageUrl;
            banner.BackgroundColor = dto.BackgroundColor;
            banner.LinkType = (int)dto.LinkType;
            banner.LinkUrl = dto.LinkUrl;
            banner.DisplayOrder = dto.DisplayOrder;
            banner.ValidFrom = dto.ValidFrom;
            banner.ValidTo = dto.ValidTo;
            banner.IsActive = dto.IsActive;
            banner.UpdatedDate = DateTime.Now;

            // Update sections (remove old, add new)
            banner.BannerSections.Clear();
            foreach (var sectionDto in dto.Sections)
            {
                banner.BannerSections.Add(new BannerSection
                {
                    BannerId = banner.Id,
                    SectionCode = sectionDto.SectionCode,
                    SectionName = sectionDto.SectionName,
                    Content = sectionDto.Content,
                    ImageUrl = sectionDto.ImageUrl,
                    DisplayOrder = sectionDto.DisplayOrder,
                    IsActive = sectionDto.IsActive,
                    CreatedDate = DateTime.Now
                });
            }

            _bannerRepository.Update(banner);
            await _bannerRepository.SaveChangesAsync();

            return MapToDto(banner);
        }

        public async Task DeleteAsync(int id)
        {
            var banner = await _bannerRepository.GetByIdAsync(id);
            if (banner == null)
                throw new InvalidOperationException($"Banner with ID {id} not found");

            _bannerRepository.Delete(banner);
            await _bannerRepository.SaveChangesAsync();
        }

        #endregion

        #region Query Methods

        public async Task<IEnumerable<BannerSliderDto>> GetBannerSliderAsync(int? campaignId = null, int maxCount = 8)
        {
            var banners = await _bannerRepository.GetBannerSliderAsync(campaignId, maxCount);
            return banners.Select(MapToSliderDto);
        }

        public async Task<IEnumerable<BannerResponseDto>> GetByCampaignAsync(int campaignId)
        {
            var banners = await _bannerRepository.GetByCampaignAsync(campaignId);
            return banners.Select(MapToDto);
        }

        public async Task<(IEnumerable<BannerListItemDto>, int totalCount)> GetPagedBannersAsync(int pageNumber = 1, int pageSize = 20)
        {
            var (banners, totalCount) = await _bannerRepository.GetPagedBannersAsync(pageNumber, pageSize);
            var dtos = banners.Select(MapToListItemDto);
            return (dtos, totalCount);
        }

        #endregion

        #region Analytics

        public async Task TrackViewAsync(int bannerId)
        {
            await _bannerRepository.IncrementViewCountAsync(bannerId);
            await _bannerRepository.SaveChangesAsync();
        }

        public async Task TrackClickAsync(int bannerId)
        {
            await _bannerRepository.IncrementClickCountAsync(bannerId);
            await _bannerRepository.SaveChangesAsync();
        }

        #endregion

        #region Helper Methods

        private BannerResponseDto MapToDto(Banner banner)
        {
            return new BannerResponseDto
            {
                Id = banner.Id,
                CampaignId = banner.CampaignId,
                CampaignName = banner.Campaign?.CampaignName ?? string.Empty,
                BannerCode = banner.BannerCode ?? string.Empty,
                Title = banner.Title ?? string.Empty,
                Subtitle = banner.Subtitle,
                Description = banner.Description,
                ImageUrl = banner.ImageUrl,
                MobileImageUrl = banner.MobileImageUrl,
                BackgroundColor = banner.BackgroundColor,
                BannerType = (ThuocGiaThatAdmin.Domain.Enums.BannerType)banner.BannerType,
                LinkType = (ThuocGiaThatAdmin.Domain.Enums.LinkType)banner.LinkType,
                LinkUrl = banner.LinkUrl,
                DisplayOrder = banner.DisplayOrder,
                ValidFrom = banner.ValidFrom,
                ValidTo = banner.ValidTo,
                IsActive = banner.IsActive,
                ViewCount = banner.ViewCount,
                ClickCount = banner.ClickCount,
                CreatedDate = banner.CreatedDate,
                UpdatedDate = banner.UpdatedDate,
                Sections = banner.BannerSections?.Select(MapSectionToDto).ToList() ?? new List<BannerSectionDto>(),
                Combos = banner.Combos?.Select(MapComboToListItem).ToList() ?? new List<ComboListItemDto>()
            };
        }

        private BannerSliderDto MapToSliderDto(Banner banner)
        {
            return new BannerSliderDto
            {
                Id = banner.Id,
                Title = banner.Title ?? string.Empty,
                Subtitle = banner.Subtitle,
                Description = banner.Description,
                ImageUrl = banner.ImageUrl,
                MobileImageUrl = banner.MobileImageUrl,
                BackgroundColor = banner.BackgroundColor,
                BannerType = (ThuocGiaThatAdmin.Domain.Enums.BannerType)banner.BannerType,
                LinkType = (ThuocGiaThatAdmin.Domain.Enums.LinkType)banner.LinkType,
                LinkUrl = banner.LinkUrl,
                DisplayOrder = banner.DisplayOrder,
                Products = new List<ProductVariantSliderDto>() // Placeholder
            };
        }

        private BannerListItemDto MapToListItemDto(Banner banner)
        {
            return new BannerListItemDto
            {
                Id = banner.Id,
                BannerCode = banner.BannerCode ?? string.Empty,
                Title = banner.Title ?? string.Empty,
                BannerType = (ThuocGiaThatAdmin.Domain.Enums.BannerType)banner.BannerType,
                IsActive = banner.IsActive,
                ViewCount = banner.ViewCount,
                ClickCount = banner.ClickCount,
                ValidFrom = banner.ValidFrom,
                ValidTo = banner.ValidTo
            };
        }

        private BannerSectionDto MapSectionToDto(BannerSection section)
        {
            return new BannerSectionDto
            {
                Id = section.Id,
                BannerId = section.BannerId,
                SectionCode = section.SectionCode ?? string.Empty,
                SectionName = section.SectionName ?? string.Empty,
                Content = section.Content,
                ImageUrl = section.ImageUrl,
                DisplayOrder = section.DisplayOrder,
                IsActive = section.IsActive
            };
        }

        private ComboListItemDto MapComboToListItem(Combo combo)
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

        #endregion
    }
}
