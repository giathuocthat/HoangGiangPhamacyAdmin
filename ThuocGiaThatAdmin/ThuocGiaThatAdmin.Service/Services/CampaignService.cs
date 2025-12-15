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
    /// Service for Campaign business logic
    /// </summary>
    public class CampaignService : ICampaignService
    {
        private readonly ICampaignRepository _campaignRepository;

        public CampaignService(ICampaignRepository campaignRepository)
        {
            _campaignRepository = campaignRepository ?? throw new ArgumentNullException(nameof(campaignRepository));
        }

        #region CRUD Operations

        public async Task<IEnumerable<CampaignResponseDto>> GetAllAsync()
        {
            var campaigns = await _campaignRepository.GetAllAsync();
            return campaigns.Select(MapToDto);
        }

        public async Task<CampaignResponseDto?> GetByIdAsync(int id)
        {
            var campaign = await _campaignRepository.GetByIdAsync(id);
            return campaign == null ? null : MapToDto(campaign);
        }

        public async Task<CampaignResponseDto?> GetByCodeAsync(string code)
        {
            var campaign = await _campaignRepository.GetByCodeAsync(code);
            return campaign == null ? null : MapToDto(campaign);
        }

        public async Task<IEnumerable<CampaignResponseDto>> GetActiveCampaignsAsync()
        {
            var campaigns = await _campaignRepository.GetActiveCampaignsAsync();
            return campaigns.Select(MapToDto);
        }

        public async Task<CampaignResponseDto> CreateAsync(CreateCampaignDto dto, string createdBy)
        {
            // Validate code uniqueness
            if (await _campaignRepository.CodeExistsAsync(dto.CampaignCode))
                throw new InvalidOperationException($"Campaign code '{dto.CampaignCode}' already exists");

            // Validate date range
            if (dto.StartDate >= dto.EndDate)
                throw new InvalidOperationException("Start date must be before end date");

            var campaign = new Campaign
            {
                CampaignCode = dto.CampaignCode,
                CampaignName = dto.CampaignName,
                Description = dto.Description,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Budget = dto.Budget,
                IsActive = dto.IsActive,
                CreatedDate = DateTime.Now
            };

            await _campaignRepository.AddAsync(campaign);
            await _campaignRepository.SaveChangesAsync();

            return MapToDto(campaign);
        }

        public async Task<CampaignResponseDto> UpdateAsync(int id, UpdateCampaignDto dto, string updatedBy)
        {
            var campaign = await _campaignRepository.GetByIdAsync(id);
            if (campaign == null)
                throw new InvalidOperationException($"Campaign with ID {id} not found");

            // Validate date range
            if (dto.StartDate >= dto.EndDate)
                throw new InvalidOperationException("Start date must be before end date");

            // Update properties
            campaign.CampaignName = dto.CampaignName;
            campaign.Description = dto.Description;
            campaign.StartDate = dto.StartDate;
            campaign.EndDate = dto.EndDate;
            campaign.Budget = dto.Budget;
            campaign.IsActive = dto.IsActive;
            campaign.UpdatedDate = DateTime.Now;

            _campaignRepository.Update(campaign);
            await _campaignRepository.SaveChangesAsync();

            return MapToDto(campaign);
        }

        public async Task DeleteAsync(int id)
        {
            var campaign = await _campaignRepository.GetByIdAsync(id);
            if (campaign == null)
                throw new InvalidOperationException($"Campaign with ID {id} not found");

            _campaignRepository.Delete(campaign);
            await _campaignRepository.SaveChangesAsync();
        }

        #endregion

        #region Query Methods

        public async Task<CampaignResponseDto?> GetWithBannersAsync(int id)
        {
            var campaign = await _campaignRepository.GetWithBannersAsync(id);
            return campaign == null ? null : MapToDto(campaign);
        }

        public async Task<(IEnumerable<CampaignListItemDto>, int totalCount)> GetPagedCampaignsAsync(int pageNumber = 1, int pageSize = 20)
        {
            var (campaigns, totalCount) = await _campaignRepository.GetPagedCampaignsAsync(pageNumber, pageSize);
            var dtos = campaigns.Select(MapToListItemDto);
            return (dtos, totalCount);
        }

        #endregion

        #region Helper Methods

        private CampaignResponseDto MapToDto(Campaign campaign)
        {
            return new CampaignResponseDto
            {
                Id = campaign.Id,
                CampaignCode = campaign.CampaignCode ?? string.Empty,
                CampaignName = campaign.CampaignName ?? string.Empty,
                Description = campaign.Description,
                StartDate = campaign.StartDate,
                EndDate = campaign.EndDate,
                Budget = campaign.Budget,
                IsActive = campaign.IsActive,
                CreatedDate = campaign.CreatedDate,
                UpdatedDate = campaign.UpdatedDate,
                Banners = campaign.Banners?.Select(MapBannerToListItem).ToList() ?? new List<BannerListItemDto>()
            };
        }

        private CampaignListItemDto MapToListItemDto(Campaign campaign)
        {
            return new CampaignListItemDto
            {
                Id = campaign.Id,
                CampaignCode = campaign.CampaignCode ?? string.Empty,
                CampaignName = campaign.CampaignName ?? string.Empty,
                StartDate = campaign.StartDate,
                EndDate = campaign.EndDate,
                IsActive = campaign.IsActive,
                BannerCount = campaign.Banners?.Count ?? 0
            };
        }

        private BannerListItemDto MapBannerToListItem(Banner banner)
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

        #endregion
    }
}
