using System.Collections.Generic;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThat.Infrastucture.Repositories
{
    public interface IProductMaxOrderConfigRepository
    {
        Task<ProductMaxOrderConfig?> GetByProductIdAsync(int productId);
        Task<List<ProductMaxOrderConfig>> GetActiveConfigsAsync();
        Task<ProductMaxOrderConfig> AddAsync(ProductMaxOrderConfig config);
        Task UpdateAsync(ProductMaxOrderConfig config);
    }
}
