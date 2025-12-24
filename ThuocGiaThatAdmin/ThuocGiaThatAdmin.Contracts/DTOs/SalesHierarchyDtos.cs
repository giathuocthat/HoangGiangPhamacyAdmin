using System;
using System.Collections.Generic;

namespace ThuocGiaThatAdmin.Contract.DTOs
{
    /// <summary>
    /// DTO cho thông tin cơ bản của Sale User
    /// </summary>
    public class SalesUserDto
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public string? ManagerId { get; set; }
        public string? ManagerName { get; set; }
        public int? RegionId { get; set; }
        public string? RegionName { get; set; }
        public DateTime CreatedDate { get; set; }
        public int AssignedCustomerCount { get; set; }
        public string RoleName { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO cho Sale Team Member với thông tin số lượng khách hàng được assign
    /// </summary>
    public class SalesTeamMemberDto
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public int AssignedCustomerCount { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? RegionId { get; set; }
        public string? RegionName { get; set; }
    }

    /// <summary>
    /// Request để assign manager cho user
    /// </summary>
    public class AssignManagerRequest
    {
        public string UserId { get; set; } = string.Empty;
        public string? ManagerId { get; set; }
    }

    /// <summary>
    /// Request để assign customer cho sale user
    /// </summary>
    public class AssignCustomerToSaleRequest
    {
        public int CustomerId { get; set; }
        public string? SaleUserId { get; set; }
    }

    /// <summary>
    /// Response cho sales hierarchy
    /// </summary>
    public class SalesHierarchyResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
