using System.Collections.Generic;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThatAdmin.Service.Interfaces
{
    public interface IBusinessTypeService
    {
        Task<(IEnumerable<BusinessType> Items, int TotalCount)> GetBusinessTypesAsync(int pageNumber = 1, int pageSize = 10);
        Task<BusinessType?> GetByIdAsync(int id);
        Task<BusinessType> CreateAsync(BusinessType entity);
        Task<int> UpdateAsync(BusinessType entity);
        Task<int> DeleteAsync(int id);
    }
}
