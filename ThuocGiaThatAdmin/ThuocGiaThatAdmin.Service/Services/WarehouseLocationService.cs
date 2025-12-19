using Microsoft.EntityFrameworkCore;
using ThuocGiaThatAdmin.Contracts.DTOs;
using ThuocGiaThatAdmin.Domain.Entities;
using ThuocGiaThat.Infrastucture.Repositories;
using ThuocGiaThat.Infrastucture.Common;

namespace ThuocGiaThatAdmin.Service.Services
{
    public class WarehouseLocationService
    {
        private readonly IRepository<WarehouseLocation> _locationRepository;
        private readonly IRepository<BatchLocationStock> _batchLocationStockRepository;
        private readonly IWarehouseRepository _warehouseRepository;

        public WarehouseLocationService(
            IRepository<WarehouseLocation> locationRepository,
            IRepository<BatchLocationStock> batchLocationStockRepository,
            IWarehouseRepository warehouseRepository)
        {
            _locationRepository = locationRepository;
            _batchLocationStockRepository = batchLocationStockRepository;
            _warehouseRepository = warehouseRepository;
        }

        /// <summary>
        /// Get all warehouse locations with pagination and filtering
        /// </summary>
        public async Task<PagedResult<WarehouseLocationDto>> GetAllLocationsAsync(
            int pageNumber = 1,
            int pageSize = 10,
            string? searchText = null,
            int? warehouseId = null,
            bool? isActive = null)
        {
            var query = await _locationRepository.GetPagedAsync(
                pageNumber,
                pageSize,
                x => (string.IsNullOrEmpty(searchText) || 
                      x.LocationCode.Contains(searchText) || 
                      (x.ZoneName != null && x.ZoneName.Contains(searchText)) ||
                      (x.Description != null && x.Description.Contains(searchText))) &&
                     (!warehouseId.HasValue || x.WarehouseId == warehouseId) &&
                     (!isActive.HasValue || x.IsActive == isActive),
                string.Empty,
                includes: [x => x.Warehouse]
            );

            var listDto = new List<WarehouseLocationDto>();
            foreach (var location in query.Items)
            {
                var occupancy = await GetLocationOccupancyAsync(location.Id);
                listDto.Add(MapToDto(location, occupancy));
            }

            return PagedResult<WarehouseLocationDto>.Create(listDto, query.TotalCount, pageNumber, pageSize);
        }

        /// <summary>
        /// Get location by ID
        /// </summary>
        public async Task<WarehouseLocationDto?> GetLocationByIdAsync(int id)
        {
            var location = await _locationRepository.GetByIdAsync(id);
            if (location == null)
                return null;

            var occupancy = await GetLocationOccupancyAsync(id);
            return MapToDto(location, occupancy);
        }

        /// <summary>
        /// Get location by code
        /// </summary>
        public async Task<WarehouseLocationDto?> GetLocationByCodeAsync(string locationCode)
        {
            var location = await _locationRepository.FirstOrDefaultAsync(x => x.LocationCode == locationCode);
            if (location == null)
                return null;

            var occupancy = await GetLocationOccupancyAsync(location.Id);
            return MapToDto(location, occupancy);
        }

        /// <summary>
        /// Get all locations for a specific warehouse
        /// </summary>
        public async Task<IEnumerable<WarehouseLocationDto>> GetLocationsByWarehouseAsync(int warehouseId)
        {
            var locations = await _locationRepository.FindAsync(x => x.WarehouseId == warehouseId);
            var result = new List<WarehouseLocationDto>();

            foreach (var location in locations)
            {
                var occupancy = await GetLocationOccupancyAsync(location.Id);
                result.Add(MapToDto(location, occupancy));
            }

            return result;
        }

        /// <summary>
        /// Create new warehouse location
        /// </summary>
        public async Task<WarehouseLocationDto> CreateLocationAsync(CreateWarehouseLocationDto dto)
        {
            // Check if location code already exists
            var existing = await _locationRepository.FirstOrDefaultAsync(x => x.LocationCode == dto.LocationCode);
            if (existing != null)
            {
                throw new InvalidOperationException($"Location code '{dto.LocationCode}' already exists. Location codes must be unique across the system.");
            }

            // Validate warehouse if provided
            if (dto.WarehouseId.HasValue)
            {
                var warehouse = await _warehouseRepository.GetByIdAsync(dto.WarehouseId.Value);
                if (warehouse == null)
                {
                    throw new KeyNotFoundException($"Warehouse with ID {dto.WarehouseId} not found");
                }
            }

            var location = new WarehouseLocation
            {
                LocationCode = dto.LocationCode.Trim().ToUpper(),
                WarehouseId = dto.WarehouseId,
                ZoneName = dto.ZoneName?.Trim(),
                RackName = dto.RackName?.Trim(),
                ShelfName = dto.ShelfName?.Trim(),
                BinName = dto.BinName?.Trim(),
                Description = dto.Description?.Trim(),
                IsActive = dto.IsActive,
                MaxCapacity = dto.MaxCapacity,
                Notes = dto.Notes
            };

            await _locationRepository.AddAsync(location);
            await _locationRepository.SaveChangesAsync();

            return MapToDto(location, 0);
        }

        /// <summary>
        /// Update warehouse location
        /// </summary>
        public async Task<WarehouseLocationDto> UpdateLocationAsync(int id, UpdateWarehouseLocationDto dto)
        {
            var location = await _locationRepository.GetByIdAsync(id);
            if (location == null)
            {
                throw new KeyNotFoundException($"Location with ID {id} not found");
            }

            // Validate warehouse if provided
            if (dto.WarehouseId.HasValue)
            {
                var warehouse = await _warehouseRepository.GetByIdAsync(dto.WarehouseId.Value);
                if (warehouse == null)
                {
                    throw new KeyNotFoundException($"Warehouse with ID {dto.WarehouseId} not found");
                }
            }

            // Update properties
            location.WarehouseId = dto.WarehouseId;
            location.ZoneName = dto.ZoneName?.Trim();
            location.RackName = dto.RackName?.Trim();
            location.ShelfName = dto.ShelfName?.Trim();
            location.BinName = dto.BinName?.Trim();
            location.Description = dto.Description?.Trim();
            location.MaxCapacity = dto.MaxCapacity;
            location.Notes = dto.Notes;

            _locationRepository.Update(location);
            await _locationRepository.SaveChangesAsync();

            var occupancy = await GetLocationOccupancyAsync(id);
            return MapToDto(location, occupancy);
        }

        /// <summary>
        /// Set location active status
        /// </summary>
        public async Task<WarehouseLocationDto> SetLocationActiveStatusAsync(int id, bool isActive)
        {
            var location = await _locationRepository.GetByIdAsync(id);
            if (location == null)
            {
                throw new KeyNotFoundException($"Location with ID {id} not found");
            }

            // If deactivating, check if there's stock at this location
            if (!isActive)
            {
                var occupancy = await GetLocationOccupancyAsync(id);
                if (occupancy > 0)
                {
                    throw new InvalidOperationException($"Cannot deactivate location '{location.LocationCode}' because it currently has {occupancy} units of stock. Please move the stock first.");
                }
            }

            location.IsActive = isActive;
            _locationRepository.Update(location);
            await _locationRepository.SaveChangesAsync();

            return MapToDto(location, 0);
        }

        // ========== Helper Methods ==========

        private async Task<int> GetLocationOccupancyAsync(int locationId)
        {
            var stocks = await _batchLocationStockRepository.FindAsync(x => x.WarehouseLocationId == locationId);
            return stocks.Sum(x => x.Quantity);
        }

        private WarehouseLocationDto MapToDto(WarehouseLocation location, int currentOccupancy)
        {
            return new WarehouseLocationDto
            {
                Id = location.Id,
                LocationCode = location.LocationCode,
                WarehouseId = location.WarehouseId,
                WarehouseName = location.Warehouse?.Name,
                ZoneName = location.ZoneName,
                RackName = location.RackName,
                ShelfName = location.ShelfName,
                BinName = location.BinName,
                Description = location.Description,
                IsActive = location.IsActive,
                MaxCapacity = location.MaxCapacity,
                CurrentOccupancy = currentOccupancy,
                Notes = location.Notes,
                CreatedDate = location.CreatedDate
            };
        }
    }
}
