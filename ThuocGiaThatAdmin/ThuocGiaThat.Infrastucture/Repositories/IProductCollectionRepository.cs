using System.Collections.Generic;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThat.Infrastucture.Repositories
{
    public interface IProductCollectionRepository
    {
        Task<ProductCollection?> GetBySlugAsync(string slug);
        Task<List<ProductCollection>> GetActiveCollectionsAsync();
        Task<ProductCollection?> GetWithProductsAsync(int id);
        Task AddProductToCollectionAsync(int collectionId, int productId, int displayOrder);
        Task RemoveProductFromCollectionAsync(int collectionId, int productId);
        Task<ProductCollection> AddAsync(ProductCollection collection);
        Task UpdateAsync(ProductCollection collection);
    }
}
