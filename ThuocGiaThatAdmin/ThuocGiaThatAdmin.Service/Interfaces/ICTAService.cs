using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Contracts.DTOs;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThatAdmin.Service.Interfaces
{
    /// <summary>
    /// Service interface for CTA (Call To Action) business logic
    /// </summary>
    public interface ICTAService
    {
        #region CRUD Operations

        Task<IEnumerable<CTAResponseDto>> GetAllAsync();
        Task<CTAResponseDto?> GetByIdAsync(int id);
        Task<CTAResponseDto?> GetByCodeAsync(string code);
        Task<CTAResponseDto> CreateAsync(CreateCTADto dto, string createdBy);
        Task<CTAResponseDto> UpdateAsync(int id, UpdateCTADto dto, string updatedBy);
        Task DeleteAsync(int id);

        #endregion

        #region Query Methods

        Task<IEnumerable<CTAResponseDto>> GetActiveAsync();
        Task<IEnumerable<CTAResponseDto>> GetByPositionAsync(CTAPosition position);
        Task<IEnumerable<CTAResponseDto>> GetByTypeAsync(CTAType type);
        Task<IEnumerable<CTAResponseDto>> GetByCampaignAsync(int campaignId);
        Task<(IEnumerable<CTAListItemDto>, int totalCount)> GetPagedAsync(int pageNumber = 1, int pageSize = 20);

        #endregion

        #region Analytics

        Task TrackViewAsync(int ctaId);
        Task TrackClickAsync(int ctaId);

        #endregion
    }
}
