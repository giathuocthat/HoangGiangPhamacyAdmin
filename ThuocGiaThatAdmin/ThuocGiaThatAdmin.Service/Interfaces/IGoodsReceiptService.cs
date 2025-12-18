using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Contract.DTOs;
using ThuocGiaThatAdmin.Contracts.DTOs;
using ThuocGiaThatAdmin.Domain.Entities;
using ThuocGiaThatAdmin.Domain.Enums;

namespace ThuocGiaThatAdmin.Service.Interfaces
{
    /// <summary>
    /// Service interface for GoodsReceipt business logic
    /// </summary>
    public interface IGoodsReceiptService
    {
        // CRUD Operations
        Task<IEnumerable<GoodsReceiptDto>> GetAllAsync();
        Task<GoodsReceiptDto?> GetByIdAsync(int id);
        Task<GoodsReceiptDto?> GetByReceiptNumberAsync(string receiptNumber);
        Task<GoodsReceiptDto> CreateAsync(CreateGoodsReceiptDto dto);
        Task<GoodsReceiptDto> UpdateAsync(int id, UpdateGoodsReceiptDto dto);
        Task DeleteAsync(int id);

        // Query
        Task<GoodsReceiptDto?> GetWithDetailsAsync(int id);
        Task<IEnumerable<GoodsReceiptDto>> GetByPurchaseOrderIdAsync(int purchaseOrderId);
        Task<IEnumerable<GoodsReceiptDto>> GetByWarehouseIdAsync(int warehouseId);
        Task<IEnumerable<GoodsReceiptDto>> GetByStatusAsync(GoodsReceiptStatus status);
        Task<(IEnumerable<GoodsReceiptListItemDto>, int totalCount)> GetPagedGoodsReceiptsAsync(
            int pageNumber = 1,
            int pageSize = 20,
            string? searchTerm = null,
            int? purchaseOrderId = null,
            int? warehouseId = null,
            GoodsReceiptStatus? status = null,
            DateTime? fromDate = null,
            DateTime? toDate = null);

        // Business Operations
        Task<GoodsReceiptDto> ReceiveGoodsAsync(int id, ReceiveGoodsDto dto);
        Task<GoodsReceiptDto> CompleteAsync(int id, CompleteGoodsReceiptDto dto);
        Task<GoodsReceiptDto> RejectAsync(int id, RejectGoodsReceiptDto dto);
        Task<string> GenerateReceiptNumberAsync();

        Task<(IEnumerable<GoodsReceiptListItemDto> receipts, int totalCount)> GetFilteredGoodsReceiptsAsync(FilterRequest request);
    }
}
