using System.Collections.Generic;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Contracts.DTOs;
using ThuocGiaThatAdmin.Contracts.Responses;
using ThuocGiaThatAdmin.Domain.Enums;

namespace ThuocGiaThatAdmin.Service.Interfaces
{
    /// <summary>
    /// Service interface for GoodsReceiptItem business logic
    /// </summary>
    public interface IGoodsReceiptItemService
    {
        // CRUD Operations
        Task<IEnumerable<GoodsReceiptItemResponse>> GetAllAsync();
        Task<GoodsReceiptItemResponse?> GetByIdAsync(int id);
        Task<GoodsReceiptItemResponse> CreateAsync(int goodsReceiptId, CreateGoodsReceiptItemDto dto);
        Task<GoodsReceiptItemResponse> UpdateAsync(int id, UpdateGoodsReceiptItemDto dto);
        Task DeleteAsync(int id);

        // Query Operations
        Task<IEnumerable<GoodsReceiptItemResponse>> GetByGoodsReceiptIdAsync(int goodsReceiptId);
        Task<IEnumerable<GoodsReceiptItemResponse>> GetByPurchaseOrderItemIdAsync(int purchaseOrderItemId);
        Task<GoodsReceiptItemResponse?> GetWithDetailsAsync(int id);

        // Business Operations
        Task<GoodsReceiptItemResponse> UpdateQualityInspectionAsync(int id, UpdateQualityInspectionDto dto);
        Task<GoodsReceiptItemResponse> UpdateLocationAsync(int id, UpdateItemLocationDto dto);
        Task<GoodsReceiptItemSummaryResponse> GetSummaryByGoodsReceiptIdAsync(int goodsReceiptId);
        Task<IEnumerable<GoodsReceiptItemResponse>> GetItemsByQualityStatusAsync(int goodsReceiptId, QualityStatus qualityStatus);
        Task<IEnumerable<GoodsReceiptItemResponse>> GetExpiringSoonItemsAsync(int daysThreshold = 30);
        Task<IEnumerable<GoodsReceiptItemResponse>> GetExpiredItemsAsync();
    }
}

