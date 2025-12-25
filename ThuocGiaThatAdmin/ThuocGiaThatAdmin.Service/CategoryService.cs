using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThuocGiaThat.Infrastucture;
using ThuocGiaThat.Infrastucture.Repositories;
using ThuocGiaThatAdmin.Contract.DTOs;
using ThuocGiaThatAdmin.Contracts.Responses;
using ThuocGiaThatAdmin.Domain.Entities;
using ThuocGiaThatAdmin.Service.Interfaces;

namespace ThuocGiaThatAdmin.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repo;
        private readonly TrueMecContext _context;

        public CategoryService(ICategoryRepository repo, TrueMecContext context)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _context = context;
        }

        public async Task<(IEnumerable<Category> Items, int TotalCount)> GetCategoriesAsync(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;
            pageSize = Math.Min(pageSize, 100);

            var all = (await _repo.GetAllAsync()).ToList();
            var total = all.Count;
            var items = all.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            return (items, total);
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            if (id <= 0) return null;
            return await _repo.GetByIdAsync(id);
        }

        public async Task<Category?> GetByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return null;
            return await _repo.GetByNameAsync(name);
        }

        public async Task<IEnumerable<Category>> SearchByNameAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm)) return Enumerable.Empty<Category>();
            return await _repo.SearchByNameAsync(searchTerm);
        }

        public async Task<IEnumerable<CategoryRootDto>> GetRootCategoriesAsync()
        {
            var result = _context.Categories.Where(c => c.ParentId == null && c.IsActive).OrderBy(c => c.DisplayOrder)
                .Select(x => new CategoryRootDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Slug = x.Slug,
                });

            var ids = string.Join(",", result.Select)
        }

        public async Task<IEnumerable<Category>> GetAllChildrenAsync()
        {
            var all = await _repo.GetAllAsync();
            return all.Where(c => c.ParentId != null && c.IsActive).OrderBy(c => c.DisplayOrder);
        }

        public async Task<IEnumerable<Category>> GetChildCategoriesAsync(int parentId)
        {
            if (parentId <= 0) return Enumerable.Empty<Category>();
            var all = await _repo.GetAllAsync();
            return all.Where(c => c.ParentId == parentId && c.IsActive).OrderBy(c => c.DisplayOrder);
        }

        /// <summary>
        /// Get categories organized in a hierarchical tree structure (parent-child)
        /// </summary>
        public async Task<IEnumerable<CategoryNodeResponse>> GetCategoryHierarchyAsync()
        {
            var all = (await _repo.GetAllAsync()).ToList();
            var activeCategories = all.Where(c => c.IsActive).ToList();

            // Build hierarchy recursively
            var rootCategories = activeCategories.Where(c => c.ParentId == null).OrderBy(c => c.DisplayOrder).ToList();
            return rootCategories.Select(root => BuildCategoryNode(root, activeCategories)).ToList();
        }

        /// <summary>
        /// Get all categories in flat view with parent info and child count
        /// </summary>
        public async Task<IEnumerable<CategoryFlatResponse>> GetCategoryFlatAsync()
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

        public async Task<Category> CreateAsync(Category entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (string.IsNullOrWhiteSpace(entity.Name)) throw new ArgumentException("Category name cannot be empty", nameof(entity.Name));
            if (string.IsNullOrWhiteSpace(entity.Slug)) throw new ArgumentException("Category slug cannot be empty", nameof(entity.Slug));

            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();
            return entity;
        }

        public async Task<int> UpdateAsync(Category entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (string.IsNullOrWhiteSpace(entity.Name)) throw new ArgumentException("Category name cannot be empty", nameof(entity.Name));
            if (string.IsNullOrWhiteSpace(entity.Slug)) throw new ArgumentException("Category slug cannot be empty", nameof(entity.Slug));

            var existing = await _repo.GetByIdAsync(entity.Id);
            if (existing == null) throw new InvalidOperationException($"Category with id {entity.Id} not found");

            // Prevent circular parent reference
            if (entity.ParentId == entity.Id)
                throw new InvalidOperationException("A category cannot be its own parent");

            // Map updatable fields
            existing.Name = entity.Name;
            existing.Description = entity.Description;
            existing.ParentId = entity.ParentId;
            existing.Slug = entity.Slug;
            existing.DisplayOrder = entity.DisplayOrder;
            existing.IsActive = entity.IsActive;
            existing.UpdatedDate = DateTime.UtcNow;

            _repo.Update(existing);
            return await _repo.SaveChangesAsync();
        }

        public async Task<int> DeleteAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("Invalid id", nameof(id));
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) throw new InvalidOperationException($"Category with id {id} not found");

            _repo.Delete(existing);
            return await _repo.SaveChangesAsync();
        }

        /// <summary>
        /// Build a single node in the hierarchy with its children
        /// </summary>
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
