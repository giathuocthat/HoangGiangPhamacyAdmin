using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThuocGiaThat.Infrastucture.Repositories;
using ThuocGiaThatAdmin.Common.Interfaces;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThatAdmin.Commands.Categories.Handlers
{
    /// <summary>
    /// Handler for CreateCategoryCommand
    /// </summary>
    public class CreateCategoryCommandHandler : ICommandHandler<CreateCategoryCommand, CreateCategoryResponse>
    {
        private readonly ICategoryRepository _categoryRepository;

        public CreateCategoryCommandHandler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
        }

        public async Task<CreateCategoryResponse> HandleAsync(CreateCategoryCommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            if (string.IsNullOrWhiteSpace(command.Name))
                throw new ArgumentException("Category name cannot be empty", nameof(command.Name));

            if (string.IsNullOrWhiteSpace(command.Slug))
                throw new ArgumentException("Category slug cannot be empty", nameof(command.Slug));

            // Check if slug already exists
            var existing = (await _categoryRepository.FindAsync(c => c.Slug == command.Slug)).FirstOrDefault();
            if (existing != null)
                throw new InvalidOperationException($"A category with slug '{command.Slug}' already exists");

            var category = new Category
            {
                Name = command.Name,
                Slug = command.Slug,
                Description = command.Description,
                ParentId = command.ParentId,
                DisplayOrder = command.DisplayOrder,
                IsActive = command.IsActive,
                CreatedDate = DateTime.UtcNow
            };

            await _categoryRepository.AddAsync(category);
            await _categoryRepository.SaveChangesAsync();

            return new CreateCategoryResponse
            {
                Id = category.Id,
                Name = category.Name,
                Slug = category.Slug,
                Description = category.Description,
                ParentId = category.ParentId,
                DisplayOrder = category.DisplayOrder,
                IsActive = category.IsActive,
                CreatedDate = category.CreatedDate
            };
        }
    }
}
