using System;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThatAdmin.Contract.DTOs
{
    /// <summary>
    /// DTO cho thông tin cơ bản của Sales Region
    /// </summary>
    public class SalesRegionDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }

        // Sale Manager Information
        public string? SaleManagerId { get; set; }
        public string? SaleManagerName { get; set; }
        public List<SaleUsersDto> SalesUsers { get; set; } = new List<SaleUsersDto>();
    }

    public class SaleUsersDto
    {
        public string? Id { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// DTO chi tiết của Sales Region với thống kê
    /// </summary>
    public class SalesRegionDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }

        // Sale Manager Information
        public string? SaleManagerId { get; set; }
        public string? SaleManagerName { get; set; }

        // Statistics
        public int TotalSalesUsers { get; set; }
        public int TotalCustomers { get; set; }
        public int ActiveSalesUsers { get; set; }
        public int ActiveCustomers { get; set; }
        public List<SaleUsersDto> SalesUsers { get; set; } = new List<SaleUsersDto>();
    }

    /// <summary>
    /// Request để tạo Sales Region mới
    /// </summary>
    public class CreateSalesRegionDto
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? SalesManagerId { get; set; }
    }

    /// <summary>
    /// Request để cập nhật Sales Region
    /// </summary>
    public class UpdateSalesRegionDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public string? SalesManagerId { get; set; }
    }

    /// <summary>
    /// Request để assign region
    /// </summary>
    public class AssignRegionRequest
    {
        public int? RegionId { get; set; }
    }
}
