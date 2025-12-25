using System.Collections.Generic;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Contract.Enums;
using ThuocGiaThatAdmin.Contracts.DTOs;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThatAdmin.Service.Services
{
    public interface IProductCollectionService
    {
        // Dynamic collections
        Task<List<ProductDto>> GetHighProfitProductsAsync(decimal minRevenue = 100000000);
        Task<List<ProductDto>> GetTopSellingProductsAsync(int minQuantity = 100);
        Task<List<ProductDto>> GetNewProductsAsync(int days = 30);
        Task<List<ProductDto>> GetLowStockProductsAsync(int maxStock = 100);
        
        // Manual collections
        Task<(List<ProductCollectionDto> Collections, int TotalCount)> GetAllCollectionsAsync(int pageNumber = 1, int pageSize = 10, string? searchName = null);
        Task<ProductCollectionDto?> GetCollectionByIdAsync(int id);
        Task<List<CollectionProductResponseDto>> GetCollectionProductsAsync(string slugOrName);
        Task<ProductCollectionDto> CreateCollectionAsync(CreateCollectionDto dto);
        Task<ProductCollectionDto> UpdateCollectionAsync(int id, UpdateProductCollectionDto dto);
        Task AddProductsToCollectionAsync(int collectionId, List<int> productIds);
        
        // Max order config
        Task<List<ProductDto>> GetProductsWithMaxOrderAsync();

        Task SetMaxOrderConfigAsync(int productId, SetMaxOrderDto dto);

        Task<List<CollectionProductResponseDto>> GetCollectionProductsByTypeAsync(ProductCollectionTypeEnum type, int pageSize);
    }
}
