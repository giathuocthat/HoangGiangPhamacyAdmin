using System;
using System.Collections.Generic;

namespace ThuocGiaThatAdmin.Contract.DTOs
{
    /// <summary>
    /// DTO for Department basic information
    /// </summary>
    public class DepartmentDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? ManagerId { get; set; }
        public string? ManagerName { get; set; }
        public int TotalUsers { get; set; }
    }

    /// <summary>
    /// DTO for Department detail with statistics
    /// </summary>
    public class DepartmentDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? ManagerId { get; set; }
        public string? ManagerName { get; set; }
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public List<DepartmentUserDto> Users { get; set; } = new();
    }

    /// <summary>
    /// DTO for user in department
    /// </summary>
    public class DepartmentUserDto
    {
        public string Id { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// DTO for creating new department
    /// </summary>
    public class CreateDepartmentDto
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ManagerId { get; set; }
    }

    /// <summary>
    /// DTO for updating department
    /// </summary>
    public class UpdateDepartmentDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public string? ManagerId { get; set; }
    }
}
