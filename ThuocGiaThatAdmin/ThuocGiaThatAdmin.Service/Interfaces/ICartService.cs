using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Contract.DTOs;
using ThuocGiaThatAdmin.Contract.Requests;

namespace ThuocGiaThatAdmin.Service.Interfaces
{
    public interface ICartService
    {
        Task<IList<CartProductDto>> GetCartProductsAsync(IList<CartItem> cartItems);
    }
}
