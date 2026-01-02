using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ThuocGiaThat.Infrastucture.Repositories;
using ThuocGiaThatAdmin.Contracts.DTOs;
using ThuocGiaThatAdmin.Domain.Entities;
using ThuocGiaThatAdmin.Service.Interfaces;

namespace ThuocGiaThatAdmin.Service.Services
{
    /// <summary>
    /// Service for CTA (Call To Action) business logic
    /// </summary>
    public class CTAService : ICTAService
    {
        private readonly IRepository<CTA> _ctaRepository;
        private readonly IRepository<Campaign> _campaignRepository;

        public CTAService(
            IRepository<CTA> ctaRepository,
            IRepository<Campaign> campaignRepository)
        {
            _ctaRepository = ctaRepository ?? throw new ArgumentNullException(nameof(ctaRepository));
            _campaignRepository = campaignRepository ?? throw new ArgumentNullException(nameof(campaignRepository));
        }

        #region CRUD Operations

        public async Task<IEnumerable<CTAResponseDto>> GetAllAsync()
        {
            var ctas = await _ctaRepository.GetAllAsync(c => c.Campaign);
            return ctas.Select(MapToDto);
        }

        public async Task<CTAResponseDto?> GetByIdAsync(int id)
        {
            var cta = await _ctaRepository.GetByIdAsync(id);
            if (cta == null) return null;

            // Load campaign separately if needed
            if (cta.CampaignId.HasValue)
            {
                var campaign = await _campaignRepository.GetByIdAsync(cta.CampaignId.Value);
                cta.Campaign = campaign;
            }

            return MapToDto(cta);
        }

        public async Task<CTAResponseDto?> GetByCodeAsync(string code)
        {
            var ctas = await _ctaRepository.FindAsync(c => c.Code == code);
            var cta = ctas.FirstOrDefault();

            if (cta == null) return null;

            // Load campaign separately if needed
            if (cta.CampaignId.HasValue)
            {
                var campaign = await _campaignRepository.GetByIdAsync(cta.CampaignId.Value);
                cta.Campaign = campaign;
            }

            return MapToDto(cta);
        }

        public async Task<CTAResponseDto> CreateAsync(CreateCTADto dto, string createdBy)
        {
            // Validate code uniqueness if provided
            if (!string.IsNullOrEmpty(dto.Code))
            {
                var existing = await _ctaRepository.AnyAsync(c => c.Code == dto.Code);
                if (existing)
                    throw new InvalidOperationException($"CTA code '{dto.Code}' already exists");
            }

            // Validate date range if provided
            if (dto.ValidFrom.HasValue && dto.ValidTo.HasValue && dto.ValidFrom >= dto.ValidTo)
                throw new InvalidOperationException("Valid from date must be before valid to date");

            var cta = new CTA
            {
                Code = dto.Code,
                Name = dto.Name,
                Title = dto.Title,
                Description = dto.Description,
                ButtonText = dto.ButtonText,
                Type = dto.Type,
                Position = dto.Position,
                Style = dto.Style,
                TargetUrl = dto.TargetUrl,
                OpenInNewTab = dto.OpenInNewTab,
                Icon = dto.Icon,
                ImageUrl = dto.ImageUrl,
                MobileImageUrl = dto.MobileImageUrl,
                BackgroundColor = dto.BackgroundColor,
                TextColor = dto.TextColor,
                DisplayOrder = dto.DisplayOrder,
                ValidFrom = dto.ValidFrom,
                ValidTo = dto.ValidTo,
                IsActive = dto.IsActive,
                Metadata = dto.Metadata,
                ViewCount = 0,
                ClickCount = 0,
                CreatedDate = DateTime.Now
            };

            await _ctaRepository.AddAsync(cta);
            await _ctaRepository.SaveChangesAsync();

            return MapToDto(cta);
        }

        public async Task<CTAResponseDto> UpdateAsync(int id, UpdateCTADto dto, string updatedBy)
        {
            var cta = await _ctaRepository.GetByIdAsync(id);
            if (cta == null)
                throw new InvalidOperationException($"CTA with ID {id} not found");

            // Validate date range if provided
            if (dto.ValidFrom.HasValue && dto.ValidTo.HasValue && dto.ValidFrom >= dto.ValidTo)
                throw new InvalidOperationException("Valid from date must be before valid to date");

            // Update properties
            cta.Name = dto.Name;
            cta.Title = dto.Title;
            cta.Description = dto.Description;
            cta.ButtonText = dto.ButtonText;
            cta.Type = dto.Type;
            cta.Position = dto.Position;
            cta.Style = dto.Style;
            cta.TargetUrl = dto.TargetUrl;
            cta.OpenInNewTab = dto.OpenInNewTab;
            cta.Icon = dto.Icon;
            cta.ImageUrl = dto.ImageUrl;
            cta.MobileImageUrl = dto.MobileImageUrl;
            cta.BackgroundColor = dto.BackgroundColor;
            cta.TextColor = dto.TextColor;
            cta.DisplayOrder = dto.DisplayOrder;
            cta.ValidFrom = dto.ValidFrom;
            cta.ValidTo = dto.ValidTo;
            cta.IsActive = dto.IsActive;
            cta.Metadata = dto.Metadata;
            cta.UpdatedDate = DateTime.Now;

            _ctaRepository.Update(cta);
            await _ctaRepository.SaveChangesAsync();

            // Reload with campaign info
            if (cta.CampaignId.HasValue)
            {
                var campaign = await _campaignRepository.GetByIdAsync(cta.CampaignId.Value);
                cta.Campaign = campaign;
            }

            return MapToDto(cta);
        }

        public async Task DeleteAsync(int id)
        {
            var cta = await _ctaRepository.GetByIdAsync(id);
            if (cta == null)
                throw new InvalidOperationException($"CTA with ID {id} not found");

            _ctaRepository.Delete(cta);
            await _ctaRepository.SaveChangesAsync();
        }

        #endregion

        #region Query Methods

        public async Task<IEnumerable<CTAResponseDto>> GetActiveAsync()
        {
            var now = DateTime.Now;
            var query = _ctaRepository.AsAsQueryable()
                .Include(c => c.Campaign)
                .Where(c => c.IsActive &&
                           (!c.ValidFrom.HasValue || c.ValidFrom <= now) &&
                           (!c.ValidTo.HasValue || c.ValidTo >= now))
                .OrderBy(c => c.DisplayOrder);

            var ctas = await query.ToListAsync();
            return ctas.Select(MapToDto);
        }

        public async Task<IEnumerable<CTAResponseDto>> GetByPositionAsync(CTAPosition position)
        {
            var query = _ctaRepository.AsAsQueryable()
                .Include(c => c.Campaign)
                .Where(c => c.Position == position && c.IsActive)
                .OrderBy(c => c.DisplayOrder);

            var ctas = await query.ToListAsync();
            return ctas.Select(MapToDto);
        }

        public async Task<IEnumerable<CTAResponseDto>> GetByTypeAsync(CTAType type)
        {
            var query = _ctaRepository.AsAsQueryable()
                .Include(c => c.Campaign)
                .Where(c => c.Type == type && c.IsActive)
                .OrderBy(c => c.DisplayOrder);

            var ctas = await query.ToListAsync();
            return ctas.Select(MapToDto);
        }

        public async Task<IEnumerable<CTAResponseDto>> GetByCampaignAsync(int campaignId)
        {
            var query = _ctaRepository.AsAsQueryable()
                .Include(c => c.Campaign)
                .Where(c => c.CampaignId == campaignId)
                .OrderBy(c => c.DisplayOrder);

            var ctas = await query.ToListAsync();
            return ctas.Select(MapToDto);
        }

        public async Task<(IEnumerable<CTAListItemDto>, int totalCount)> GetPagedAsync(int pageNumber = 1, int pageSize = 20)
        {
            var query = _ctaRepository.AsAsQueryable();
            var totalCount = await query.CountAsync();

            var ctas = await query
                .OrderByDescending(c => c.CreatedDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var dtos = ctas.Select(MapToListItemDto);
            return (dtos, totalCount);
        }

        #endregion

        #region Analytics

        public async Task TrackViewAsync(int ctaId)
        {
            var cta = await _ctaRepository.GetByIdAsync(ctaId);
            if (cta != null)
            {
                cta.ViewCount++;
                _ctaRepository.Update(cta);
                await _ctaRepository.SaveChangesAsync();
            }
        }

        public async Task TrackClickAsync(int ctaId)
        {
            var cta = await _ctaRepository.GetByIdAsync(ctaId);
            if (cta != null)
            {
                cta.ClickCount++;
                _ctaRepository.Update(cta);
                await _ctaRepository.SaveChangesAsync();
            }
        }

        #endregion

        #region Helper Methods

        private CTAResponseDto MapToDto(CTA cta)
        {
            return new CTAResponseDto
            {
                Id = cta.Id,
                Code = cta.Code,
                Name = cta.Name,
                Title = cta.Title,
                Description = cta.Description,
                ButtonText = cta.ButtonText,
                Type = cta.Type,
                Position = cta.Position,
                Style = cta.Style,
                TargetUrl = cta.TargetUrl,
                OpenInNewTab = cta.OpenInNewTab,
                Icon = cta.Icon,
                ImageUrl = cta.ImageUrl,
                MobileImageUrl = cta.MobileImageUrl,
                BackgroundColor = cta.BackgroundColor,
                TextColor = cta.TextColor,
                DisplayOrder = cta.DisplayOrder,
                ValidFrom = cta.ValidFrom,
                ValidTo = cta.ValidTo,
                IsActive = cta.IsActive,
                ViewCount = cta.ViewCount,
                ClickCount = cta.ClickCount,
                Metadata = cta.Metadata,
                CreatedDate = cta.CreatedDate,
                UpdatedDate = cta.UpdatedDate
            };
        }

        private CTAListItemDto MapToListItemDto(CTA cta)
        {
            return new CTAListItemDto
            {
                Id = cta.Id,
                Code = cta.Code,
                Name = cta.Name,
                ButtonText = cta.ButtonText,
                Type = cta.Type,
                Position = cta.Position,
                Style = cta.Style,
                IsActive = cta.IsActive,
                ViewCount = cta.ViewCount,
                ClickCount = cta.ClickCount,
                ValidFrom = cta.ValidFrom,
                ValidTo = cta.ValidTo
            };
        }

        #endregion
    }
}
