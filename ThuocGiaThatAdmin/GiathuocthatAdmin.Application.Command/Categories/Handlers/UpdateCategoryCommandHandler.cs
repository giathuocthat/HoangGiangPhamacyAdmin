using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThuocGiaThat.Infrastucture.Repositories;
using ThuocGiaThatAdmin.Common;
using ThuocGiaThatAdmin.Common.Interfaces;
using ThuocGiaThatAdmin.Contract.Responses;

namespace ThuocGiaThatAdmin.Commands.Categories.Handlers
{
    public class UpdateCategoryCommandHandler : ICommandHandler<UpdateCategoryCommand, Result<UpdateCategoryResponse>>
    {
        private readonly ICategoryRepository _categoryRepository;

        public UpdateCategoryCommandHandler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
        }

        public async Task<Result<UpdateCategoryResponse>> HandleAsync(UpdateCategoryCommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            if (command.Id <= 0)
                throw new ArgumentException("Invalid category id", nameof(command.Id));

            if (string.IsNullOrWhiteSpace(command.Name))
                throw new ArgumentException("Category name cannot be empty", nameof(command.Name));

            if (string.IsNullOrWhiteSpace(command.Slug))
                throw new ArgumentException("Category slug cannot be empty", nameof(command.Slug));

            // Prevent circular parent reference
            if (command.ParentId == command.Id)
                throw new InvalidOperationException("A category cannot be its own parent");

            var category = await _categoryRepository.GetByIdAsync(command.Id);
            if (category == null)
                throw new InvalidOperationException($"Category with id {command.Id} not found");

            // Check if new slug is unique (excluding current category)
            var slugExists = (await _categoryRepository.FindAsync(c => c.Slug == command.Slug && c.Id != command.Id)).FirstOrDefault();
            if (slugExists != null)
                throw new InvalidOperationException($"A category with slug '{command.Slug}' already exists");

            category.Name = command.Name;
            category.Slug = command.Slug;
            category.Description = command.Description;
            category.ParentId = command.ParentId;
            category.DisplayOrder = command.DisplayOrder;
            category.IsActive = command.IsActive;
            category.ImageUrl = command.ImageUrl;
            category.UpdatedDate = DateTime.UtcNow;

            _categoryRepository.Update(category);
            var affectedRows = await _categoryRepository.SaveChangesAsync();

            return Result<UpdateCategoryResponse>.Success(new UpdateCategoryResponse
            {
                Success = true,
                Message = "Category updated successfully",
                AffectedRows = affectedRows
            });
        }
    }
}
