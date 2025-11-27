using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThuocGiaThat.Infrastucture.Repositories;
using ThuocGiaThatAdmin.Common.Interfaces;
using ThuocGiaThatAdmin.Contracts.Responses;
using ThuocGiaThatAdmin.Queries.IQueries;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThatAdmin.Queries.Categories.Handlers
{
    public class GetCategoryHierarchyQueryHandler : IQueryHandler<GetCategoryHierarchyQuery, IEnumerable<CategoryNodeResponse>>
    {
        private readonly ICategoryRepository _repo;

        public GetCategoryHierarchyQueryHandler(ICategoryRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        public async Task<IEnumerable<CategoryNodeResponse>> HandleAsync(GetCategoryHierarchyQuery query)
        {
            var all = (await _repo.GetAllAsync()).ToList();
            var activeCategories = all.Where(c => c.IsActive).ToList();

            var rootCategories = activeCategories.Where(c => c.ParentId == null).OrderBy(c => c.DisplayOrder).ToList();
            return rootCategories.Select(root => BuildCategoryNode(root, activeCategories)).ToList();
        }

        private CategoryNodeResponse BuildCategoryNode(Category category, List<Category> allCategories)
        {
            var children = allCategories
                .Where(c => c.ParentId == category.Id && c.IsActive)
                .OrderBy(c => c.DisplayOrder)
                .Select(child => BuildCategoryNode(child, allCategories))
                .ToList();

            return new CategoryNodeResponse(
                category.Id,
                category.Name,
                category.Slug,
                category.Description,
                category.DisplayOrder,
                category.IsActive,
                category.CreatedDate,
                category.UpdatedDate,
                children
            );
        }
    }
}
