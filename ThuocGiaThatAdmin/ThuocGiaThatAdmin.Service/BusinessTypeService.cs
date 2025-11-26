using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThuocGiaThat.Infrastucture.Interfaces;
using ThuocGiaThatAdmin.Domain.Entities;
using ThuocGiaThatAdmin.Service.Interfaces;

namespace ThuocGiaThatAdmin.Service
{
    public class BusinessTypeService : IBusinessTypeService
    {
        private readonly IBusinessTypeRepository _repo;

        public BusinessTypeService(IBusinessTypeRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        public async Task<(IEnumerable<BusinessType> Items, int TotalCount)> GetBusinessTypesAsync(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;
            pageSize = Math.Min(pageSize, 100);

            var all = (await _repo.GetAllAsync()).ToList();
            var total = all.Count;
            var items = all.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            return (items, total);
        }

        public async Task<BusinessType?> GetByIdAsync(int id)
        {
            if (id <= 0) return null;
            return await _repo.GetByIdAsync(id);
        }

        public async Task<BusinessType> CreateAsync(BusinessType entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();
            return entity;
        }

        public async Task<int> UpdateAsync(BusinessType entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            var existing = await _repo.GetByIdAsync(entity.Id);
            if (existing == null) throw new InvalidOperationException($"BusinessType with id {entity.Id} not found");

            // Map updatable fields
            existing.Name = entity.Name;
            existing.UpdatedDate = DateTime.UtcNow;

            _repo.Update(existing);
            return await _repo.SaveChangesAsync();
        }

        public async Task<int> DeleteAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("Invalid id", nameof(id));
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) throw new InvalidOperationException($"BusinessType with id {id} not found");

            _repo.Delete(existing);
            return await _repo.SaveChangesAsync();
        }
    }
}
