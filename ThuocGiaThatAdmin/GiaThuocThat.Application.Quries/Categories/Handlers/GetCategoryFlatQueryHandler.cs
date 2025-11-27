using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThuocGiaThat.Infrastucture.Repositories;
using ThuocGiaThatAdmin.Common.Interfaces;
using ThuocGiaThatAdmin.Contracts.Responses;

namespace ThuocGiaThatAdmin.Queries.Categories.Handlers
{
    public class GetCategoryFlatQueryHandler : IQueryHandler<GetCategoryFlatQuery, IEnumerable<CategoryFlatResponse>>
    {
        private readonly ICategoryRepository _repo;

        public GetCategoryFlatQueryHandler(ICategoryRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        public async Task<IEnumerable<CategoryFlatResponse>> HandleAsync(GetCategoryFlatQuery query)
        {
            var all = (await _repo.GetAllAsync()).ToList();
            var categoryDict = all.ToDictionary(c => c.Id, c => c);

            return all
                .Where(c => c.IsActive)
                .OrderBy(c => c.ParentId)
                .ThenBy(c => c.DisplayOrder)
                .Select(c => new CategoryFlatResponse(
                    c.Id,
                    c.Name,
                    c.Slug,
                    c.Description,
                    c.ParentId,
                    c.ParentId.HasValue && categoryDict.TryGetValue(c.ParentId.Value, out var parent)
                        ? parent.Name
                        : null,
                    c.DisplayOrder,
                    c.IsActive,
                    all.Count(child => child.ParentId == c.Id),
                    c.CreatedDate,
                    c.UpdatedDate
                ))
                .ToList();
        }
    }
}
