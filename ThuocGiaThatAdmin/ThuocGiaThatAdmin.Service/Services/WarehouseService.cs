using ThuocGiaThatAdmin.Contracts.DTOs;
using ThuocGiaThatAdmin.Domain.Entities;
using ThuocGiaThat.Infrastucture.Repositories;

namespace ThuocGiaThatAdmin.Service.Services
{
    public class WarehouseService
    {
        private readonly IWarehouseRepository _warehouseRepository;

        public WarehouseService(IWarehouseRepository warehouseRepository)
        {
            _warehouseRepository = warehouseRepository;
        }

        public async Task<IEnumerable<WarehouseDto>> GetAllWarehousesAsync()
        {
            var warehouses = await _warehouseRepository.GetAllAsync();
            return warehouses.Select(MapToDto);
        }

        public async Task<WarehouseDto?> GetWarehouseByIdAsync(int id)
        {
            var warehouse = await _warehouseRepository.GetByIdAsync(id);
            return warehouse == null ? null : MapToDto(warehouse);
        }

        public async Task<IEnumerable<WarehouseDto>> GetActiveWarehousesAsync()
        {
            var warehouses = await _warehouseRepository.GetActiveWarehousesAsync();
            return warehouses.Select(MapToDto);
        }

        public async Task<WarehouseDto> CreateWarehouseAsync(CreateWarehouseDto dto)
        {
            // Validate unique code
            if (!await _warehouseRepository.IsCodeUniqueAsync(dto.Code))
            {
                throw new InvalidOperationException($"Warehouse code '{dto.Code}' already exists");
            }

            var warehouse = new Warehouse
            {
                Code = dto.Code,
                Name = dto.Name,
                Type = dto.Type,
                Address = dto.Address,
                Ward = dto.Ward,
                District = dto.District,
                City = dto.City,
                PhoneNumber = dto.PhoneNumber,
                Email = dto.Email,
                ManagerName = dto.ManagerName,
                IsActive = dto.IsActive,
                Notes = dto.Notes
            };

            await _warehouseRepository.AddAsync(warehouse);
            await _warehouseRepository.SaveChangesAsync();

            return MapToDto(warehouse);
        }

        public async Task<WarehouseDto> UpdateWarehouseAsync(int id, UpdateWarehouseDto dto)
        {
            var warehouse = await _warehouseRepository.GetByIdAsync(id);
            if (warehouse == null)
            {
                throw new KeyNotFoundException($"Warehouse with ID {id} not found");
            }

            // Validate unique code
            if (!await _warehouseRepository.IsCodeUniqueAsync(dto.Code, id))
            {
                throw new InvalidOperationException($"Warehouse code '{dto.Code}' already exists");
            }

            warehouse.Code = dto.Code;
            warehouse.Name = dto.Name;
            warehouse.Type = dto.Type;
            warehouse.Address = dto.Address;
            warehouse.Ward = dto.Ward;
            warehouse.District = dto.District;
            warehouse.City = dto.City;
            warehouse.PhoneNumber = dto.PhoneNumber;
            warehouse.Email = dto.Email;
            warehouse.ManagerName = dto.ManagerName;
            warehouse.IsActive = dto.IsActive;
            warehouse.Notes = dto.Notes;

            _warehouseRepository.Update(warehouse);
            await _warehouseRepository.SaveChangesAsync();

            return MapToDto(warehouse);
        }

        public async Task DeleteWarehouseAsync(int id)
        {
            var warehouse = await _warehouseRepository.GetByIdAsync(id);
            if (warehouse == null)
            {
                throw new KeyNotFoundException($"Warehouse with ID {id} not found");
            }

            // Soft delete by setting IsActive = false
            warehouse.IsActive = false;
            _warehouseRepository.Update(warehouse);
            await _warehouseRepository.SaveChangesAsync();
        }

        private static WarehouseDto MapToDto(Warehouse warehouse)
        {
            return new WarehouseDto
            {
                Id = warehouse.Id,
                Code = warehouse.Code,
                Name = warehouse.Name,
                Type = warehouse.Type,
                Address = warehouse.Address,
                Ward = warehouse.Ward,
                District = warehouse.District,
                City = warehouse.City,
                PhoneNumber = warehouse.PhoneNumber,
                Email = warehouse.Email,
                ManagerName = warehouse.ManagerName,
                IsActive = warehouse.IsActive,
                Notes = warehouse.Notes,
                CreatedDate = warehouse.CreatedDate
            };
        }
    }
}
