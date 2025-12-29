using System.Collections.Generic;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Contract.DTOs;
using ThuocGiaThatAdmin.Contracts.Responses;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThatAdmin.Service.Interfaces
{
    public interface ICategoryService
    {
        Task<(IEnumerable<Category> Items, int TotalCount)> GetCategoriesAsync(int pageNumber = 1, int pageSize = 10);
        Task<Category?> GetByIdAsync(int id);
        Task<Category?> GetByNameAsync(string name);
        Task<IEnumerable<Category>> SearchByNameAsync(string searchTerm);
        //Task<IEnumerable<Category>> GetRootCategoriesAsync();
        Task<IEnumerable<CategoryRootCountProductsDto>> GetCategoryRootCountProductsAsync();
        Task<IEnumerable<Category>> GetChildCategoriesAsync(int parentId);
        Task<IEnumerable<CategoryNodeResponse>> GetCategoryHierarchyAsync();
        Task<IEnumerable<CategoryFlatResponse>> GetCategoryFlatAsync();
        Task<Category> CreateAsync(Category entity);
        Task<int> UpdateAsync(Category entity);
        Task<int> DeleteAsync(int id);
        Task<IEnumerable<Category>> GetAllChildrenAsync();
        Task<IList<CategoryDto>> GetRootCategoriesAsync();
    }
}
