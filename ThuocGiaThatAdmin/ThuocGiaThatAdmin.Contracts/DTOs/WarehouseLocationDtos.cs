using System;
using System.ComponentModel.DataAnnotations;

namespace ThuocGiaThatAdmin.Contracts.DTOs
{
    // ========== Warehouse Location DTOs ==========

    public class WarehouseLocationDto
    {
        public int Id { get; set; }
        public string LocationCode { get; set; } = string.Empty;
        public int? WarehouseId { get; set; }
        public string? WarehouseName { get; set; }
        public string? ZoneName { get; set; }
        public string? RackName { get; set; }
        public string? ShelfName { get; set; }
        public string? BinName { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public int? MaxCapacity { get; set; }
        public int CurrentOccupancy { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class CreateWarehouseLocationDto
    {
        [Required]
        [MaxLength(50)]
        public string LocationCode { get; set; } = string.Empty;

        public int? WarehouseId { get; set; }

        [MaxLength(100)]
        public string? ZoneName { get; set; }

        [MaxLength(50)]
        public string? RackName { get; set; }

        [MaxLength(50)]
        public string? ShelfName { get; set; }

        [MaxLength(50)]
        public string? BinName { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        [Range(1, int.MaxValue)]
        public int? MaxCapacity { get; set; }

        public string? Notes { get; set; }
    }

    public class UpdateWarehouseLocationDto
    {
        public int? WarehouseId { get; set; }

        [MaxLength(100)]
        public string? ZoneName { get; set; }

        [MaxLength(50)]
        public string? RackName { get; set; }

        [MaxLength(50)]
        public string? ShelfName { get; set; }

        [MaxLength(50)]
        public string? BinName { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [Range(1, int.MaxValue)]
        public int? MaxCapacity { get; set; }

        public string? Notes { get; set; }
    }
}
