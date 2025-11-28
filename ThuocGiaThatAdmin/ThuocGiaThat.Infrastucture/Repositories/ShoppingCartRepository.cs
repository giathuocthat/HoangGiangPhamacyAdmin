using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThat.Infrastucture.Repositories
{
    public class ShoppingCartRepository : IShoppingCartRepository
    {
        private readonly TrueMecContext _context;

        public ShoppingCartRepository(TrueMecContext context)
        {
            _context = context;
        }

        public async Task<ShoppingCart?> GetByIdAsync(int id)
        {
            return await _context.ShoppingCarts.FindAsync(id);
        }

        public async Task<ShoppingCart?> GetByCustomerIdAsync(int customerId)
        {
            return await _context.ShoppingCarts
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);
        }

        public async Task<ShoppingCart?> GetBySessionIdAsync(string sessionId)
        {
            return await _context.ShoppingCarts
                .FirstOrDefaultAsync(c => c.SessionId == sessionId);
        }

        public async Task<ShoppingCart?> GetCartWithItemsAsync(int id)
        {
            return await _context.ShoppingCarts
                .Include(c => c.CartItems)
                    .ThenInclude(i => i.Product)
                .Include(c => c.CartItems)
                    .ThenInclude(i => i.ProductVariant)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<ShoppingCart?> GetCartWithItemsByCustomerIdAsync(int customerId)
        {
            return await _context.ShoppingCarts
                .Include(c => c.CartItems)
                    .ThenInclude(i => i.Product)
                .Include(c => c.CartItems)
                    .ThenInclude(i => i.ProductVariant)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);
        }

        public async Task<ShoppingCart?> GetCartWithItemsBySessionIdAsync(string sessionId)
        {
            return await _context.ShoppingCarts
                .Include(c => c.CartItems)
                    .ThenInclude(i => i.Product)
                .Include(c => c.CartItems)
                    .ThenInclude(i => i.ProductVariant)
                .FirstOrDefaultAsync(c => c.SessionId == sessionId);
        }

        public async Task AddAsync(ShoppingCart cart)
        {
            await _context.ShoppingCarts.AddAsync(cart);
        }

        public void Update(ShoppingCart cart)
        {
            _context.ShoppingCarts.Update(cart);
        }

        public void Delete(ShoppingCart cart)
        {
            _context.ShoppingCarts.Remove(cart);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
