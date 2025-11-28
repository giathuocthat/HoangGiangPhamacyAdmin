using System.Collections.Generic;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThat.Infrastucture.Repositories
{
    public interface IShoppingCartItemRepository
    {
        Task<ShoppingCartItem?> GetByIdAsync(int id);
        Task<IEnumerable<ShoppingCartItem>> GetByCartIdAsync(int cartId);
        Task<ShoppingCartItem?> GetByCartAndVariantAsync(int cartId, int variantId);
        Task AddAsync(ShoppingCartItem item);
        void Update(ShoppingCartItem item);
        void Delete(ShoppingCartItem item);
        void DeleteRange(IEnumerable<ShoppingCartItem> items);
        Task<int> SaveChangesAsync();
    }
}
