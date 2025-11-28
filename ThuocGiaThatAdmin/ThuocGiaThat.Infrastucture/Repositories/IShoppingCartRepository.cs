using System.Collections.Generic;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThat.Infrastucture.Repositories
{
    public interface IShoppingCartRepository
    {
        Task<ShoppingCart?> GetByIdAsync(int id);
        Task<ShoppingCart?> GetByCustomerIdAsync(int customerId);
        Task<ShoppingCart?> GetBySessionIdAsync(string sessionId);
        Task<ShoppingCart?> GetCartWithItemsAsync(int id);
        Task<ShoppingCart?> GetCartWithItemsByCustomerIdAsync(int customerId);
        Task<ShoppingCart?> GetCartWithItemsBySessionIdAsync(string sessionId);
        Task AddAsync(ShoppingCart cart);
        void Update(ShoppingCart cart);
        void Delete(ShoppingCart cart);
        Task<int> SaveChangesAsync();
    }
}
