using System.ComponentModel.DataAnnotations;

namespace ThuocGiaThatAdmin.Contracts.DTOs
{
    /// <summary>
    /// DTO for Bank entity
    /// </summary>
    public class BankDto
    {
        public int Id { get; set; }
        public string BankCode { get; set; } = string.Empty;
        public string BankName { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public bool IsActive { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    /// <summary>
    /// DTO for creating a new Bank
    /// </summary>
    public class CreateBankDto
    {
        [Required(ErrorMessage = "Bank code is required")]
        [StringLength(20, ErrorMessage = "Bank code cannot exceed 20 characters")]
        public string BankCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Bank name is required")]
        [StringLength(200, ErrorMessage = "Bank name cannot exceed 200 characters")]
        public string BankName { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Full name cannot exceed 500 characters")]
        public string? FullName { get; set; }

        public bool IsActive { get; set; } = true;

        public int DisplayOrder { get; set; } = 0;
    }

    /// <summary>
    /// DTO for updating an existing Bank
    /// </summary>
    public class UpdateBankDto
    {
        [Required(ErrorMessage = "Bank code is required")]
        [StringLength(20, ErrorMessage = "Bank code cannot exceed 20 characters")]
        public string BankCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Bank name is required")]
        [StringLength(200, ErrorMessage = "Bank name cannot exceed 200 characters")]
        public string BankName { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Full name cannot exceed 500 characters")]
        public string? FullName { get; set; }

        public bool IsActive { get; set; }

        public int DisplayOrder { get; set; }
    }
}
