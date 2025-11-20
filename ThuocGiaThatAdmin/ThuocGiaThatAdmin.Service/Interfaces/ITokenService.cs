using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThatAdmin.Service.Interfaces
{
    public interface ITokenService
    {
        Task<(string AccessToken, string RefreshToken, DateTime ExpiresUtc)> GenerateTokenAsync(ApplicationUser user);
    }
}
