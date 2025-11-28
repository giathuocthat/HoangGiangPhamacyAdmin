using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThat.Infrastucture.Repositories
{
    public class ShoppingCartItemRepository : IShoppingCartItemRepository
    {
        private readonly TrueMecContext _context;

        public ShoppingCartItemRepository(TrueMecContext context)
        {
            _context = context;
        }

        public async Task<ShoppingCartItem?> GetByIdAsync(int id)
        {
            return await _context.ShoppingCartItems.FindAsync(id);
        }

        public async Task<IEnumerable<ShoppingCartItem>> GetByCartIdAsync(int cartId)
        {
            return await _context.ShoppingCartItems
                .Where(i => i.ShoppingCartId == cartId)
                .ToListAsync();
        }

        public async Task<ShoppingCartItem?> GetByCartAndVariantAsync(int cartId, int variantId)
        {
            return await _context.ShoppingCartItems
                .FirstOrDefaultAsync(i => i.ShoppingCartId == cartId && i.ProductVariantId == variantId);
        }

        public async Task AddAsync(ShoppingCartItem item)
        {
            await _context.ShoppingCartItems.AddAsync(item);
        }

        public void Update(ShoppingCartItem item)
        {
            _context.ShoppingCartItems.Update(item);
        }

        public void Delete(ShoppingCartItem item)
        {
            _context.ShoppingCartItems.Remove(item);
        }

        public void DeleteRange(IEnumerable<ShoppingCartItem> items)
        {
            _context.ShoppingCartItems.RemoveRange(items);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
