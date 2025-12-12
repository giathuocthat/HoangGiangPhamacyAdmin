using System.Collections.Generic;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Contracts.DTOs;

namespace ThuocGiaThatAdmin.Service.Interfaces
{
    /// <summary>
    /// Service interface for Combo business logic
    /// </summary>
    public interface IComboService
    {
        // CRUD Operations
        Task<IEnumerable<ComboResponseDto>> GetAllAsync();
        Task<ComboResponseDto?> GetByIdAsync(int id);
        Task<ComboResponseDto?> GetByCodeAsync(string code);
        Task<IEnumerable<ComboResponseDto>> GetActiveCombosAsync();
        Task<ComboResponseDto> CreateAsync(CreateComboDto dto, string createdBy);
        Task<ComboResponseDto> UpdateAsync(int id, UpdateComboDto dto, string updatedBy);
        Task DeleteAsync(int id);

        // Query
        Task<IEnumerable<ComboResponseDto>> GetByBannerAsync(int bannerId);
        Task<(IEnumerable<ComboListItemDto>, int totalCount)> GetPagedCombosAsync(int pageNumber = 1, int pageSize = 20);

        // Validation
        Task<bool> CheckStockAvailabilityAsync(int comboId, int quantity);
        Task ValidateComboAsync(CreateComboDto dto);
    }
}
