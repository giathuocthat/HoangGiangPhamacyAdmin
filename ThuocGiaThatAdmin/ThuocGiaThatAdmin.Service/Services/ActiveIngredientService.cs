using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThuocGiaThat.Infrastucture.Repositories;
using ThuocGiaThatAdmin.Contracts.DTOs;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThatAdmin.Service.Services
{
    /// <summary>
    /// Service for ActiveIngredient business logic
    /// </summary>
    public class ActiveIngredientService
    {
        private readonly IRepository<ActiveIngredient> _repository;

        public ActiveIngredientService(IRepository<ActiveIngredient> repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        /// <summary>
        /// Get all active ingredients
        /// </summary>
        public async Task<IEnumerable<ActiveIngredientDto>> GetAllAsync()
        {
            var ingredients = await _repository.GetAllAsync();
            return ingredients
                .Where(x => x.IsActive)
                .Select(x => new ActiveIngredientDto
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .OrderBy(x => x.Name)
                .ToList();
        }

        /// <summary>
        /// Search active ingredients by name
        /// </summary>
        public async Task<IEnumerable<ActiveIngredientDto>> SearchByNameAsync(string searchTerm)
        {
            var ingredients = await _repository.GetAllAsync();
            return ingredients
                .Where(x => x.IsActive && x.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .Select(x => new ActiveIngredientDto
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .OrderBy(x => x.Name)
                .ToList();
        }

        /// <summary>
        /// Create a new active ingredient
        /// </summary>
        public async Task<ActiveIngredientDto> CreateAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Ingredient name cannot be null or empty", nameof(name));

            // Check if ingredient with same name already exists
            var existing = (await _repository.GetAllAsync())
                .FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (existing != null)
            {
                return new ActiveIngredientDto
                {
                    Id = existing.Id,
                    Name = existing.Name
                };
            }

            var ingredient = new ActiveIngredient
            {
                Name = name,
                IsActive = true
            };

            await _repository.AddAsync(ingredient);
            await _repository.SaveChangesAsync();

            return new ActiveIngredientDto
            {
                Id = ingredient.Id,
                Name = ingredient.Name
            };
        }

        /// <summary>
        /// Get or create active ingredient by name
        /// </summary>
        public async Task<int> GetOrCreateIdByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Ingredient name cannot be null or empty", nameof(name));

            var existing = (await _repository.GetAllAsync())
                .FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (existing != null)
                return existing.Id;

            var ingredient = new ActiveIngredient
            {
                Name = name,
                IsActive = true
            };

            await _repository.AddAsync(ingredient);
            await _repository.SaveChangesAsync();

            return ingredient.Id;
        }
    }
}
