using System.Collections.Generic;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Contracts.DTOs;

namespace ThuocGiaThatAdmin.Service.Interfaces
{
    /// <summary>
    /// Service interface for Banner business logic
    /// </summary>
    public interface IBannerService
    {
        // CRUD Operations
        Task<IEnumerable<BannerResponseDto>> GetAllAsync();
        Task<BannerResponseDto?> GetByIdAsync(int id);
        Task<BannerResponseDto?> GetByCodeAsync(string code);
        Task<IEnumerable<BannerResponseDto>> GetActiveBannersAsync();
        Task<BannerResponseDto> CreateAsync(CreateBannerDto dto, string createdBy);
        Task<BannerResponseDto> UpdateAsync(int id, UpdateBannerDto dto, string updatedBy);
        Task DeleteAsync(int id);

        // Query
        Task<IEnumerable<BannerSliderDto>> GetBannerSliderAsync(int? campaignId = null, int maxCount = 8);
        Task<IEnumerable<BannerResponseDto>> GetByCampaignAsync(int campaignId);
        Task<(IEnumerable<BannerListItemDto>, int totalCount)> GetPagedBannersAsync(int pageNumber = 1, int pageSize = 20);

        // Analytics
        Task TrackViewAsync(int bannerId);
        Task TrackClickAsync(int bannerId);
    }
}
