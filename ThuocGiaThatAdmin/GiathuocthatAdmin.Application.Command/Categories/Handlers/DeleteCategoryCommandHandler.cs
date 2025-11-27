using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThuocGiaThat.Infrastucture.Repositories;
using ThuocGiaThatAdmin.Common.Interfaces;

namespace ThuocGiaThatAdmin.Commands.Categories.Handlers
{
    /// <summary>
    /// Handler for DeleteCategoryCommand
    /// </summary>
    public class DeleteCategoryCommandHandler : ICommandHandler<DeleteCategoryCommand, DeleteCategoryResponse>
    {
        private readonly ICategoryRepository _categoryRepository;

        public DeleteCategoryCommandHandler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
        }

        public async Task<DeleteCategoryResponse> HandleAsync(DeleteCategoryCommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            if (command.Id <= 0)
                throw new ArgumentException("Invalid category id", nameof(command.Id));

            var category = await _categoryRepository.GetByIdAsync(command.Id);
            if (category == null)
                return new DeleteCategoryResponse
                {
                    Success = false,
                    Message = $"Category with id {command.Id} not found",
                    AffectedRows = 0
                };

            _categoryRepository.Delete(category);
            var affectedRows = await _categoryRepository.SaveChangesAsync();

            return new DeleteCategoryResponse
            {
                Success = true,
                Message = "Category deleted successfully",
                AffectedRows = affectedRows
            };
        }
    }
}
