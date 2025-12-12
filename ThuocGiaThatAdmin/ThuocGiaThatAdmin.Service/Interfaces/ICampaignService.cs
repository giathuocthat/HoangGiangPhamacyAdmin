using System.Collections.Generic;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Contracts.DTOs;

namespace ThuocGiaThatAdmin.Service.Interfaces
{
    /// <summary>
    /// Service interface for Campaign business logic
    /// </summary>
    public interface ICampaignService
    {
        // CRUD Operations
        Task<IEnumerable<CampaignResponseDto>> GetAllAsync();
        Task<CampaignResponseDto?> GetByIdAsync(int id);
        Task<CampaignResponseDto?> GetByCodeAsync(string code);
        Task<IEnumerable<CampaignResponseDto>> GetActiveCampaignsAsync();
        Task<CampaignResponseDto> CreateAsync(CreateCampaignDto dto, string createdBy);
        Task<CampaignResponseDto> UpdateAsync(int id, UpdateCampaignDto dto, string updatedBy);
        Task DeleteAsync(int id);

        // Query
        Task<CampaignResponseDto?> GetWithBannersAsync(int id);
        Task<(IEnumerable<CampaignListItemDto>, int totalCount)> GetPagedCampaignsAsync(int pageNumber = 1, int pageSize = 20);
    }
}
