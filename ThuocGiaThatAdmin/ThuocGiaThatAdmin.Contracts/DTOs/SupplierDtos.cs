using System;
using System.Collections.Generic;
using ThuocGiaThatAdmin.Domain.Enums;

namespace ThuocGiaThatAdmin.Contracts.DTOs
{
    // ============ Supplier DTOs ============

    public class SupplierDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public int? WardId { get; set; }
        public string? WardName { get; set; }
        public int? ProvinceId { get; set; }
        public string? ProvinceName { get; set; }
        public string? TaxCode { get; set; }
        public string? BankAccount { get; set; }
        public string? BankName { get; set; }
        public int PaymentTerms { get; set; }
        public decimal? CreditLimit { get; set; }
        public bool IsActive { get; set; }
        public int? Rating { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public List<SupplierContactDto> Contacts { get; set; } = new List<SupplierContactDto>();
    }

    public class CreateSupplierDto
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public int? WardId { get; set; }
        public int? ProvinceId { get; set; }
        public string? TaxCode { get; set; }
        public string? BankAccount { get; set; }
        public string? BankName { get; set; }
        public int PaymentTerms { get; set; } = 30;
        public decimal? CreditLimit { get; set; }
        public int? Rating { get; set; }
        public string? Notes { get; set; }
    }

    public class UpdateSupplierDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public int? WardId { get; set; }
        public int? ProvinceId { get; set; }
        public string? TaxCode { get; set; }
        public string? BankAccount { get; set; }
        public string? BankName { get; set; }
        public int PaymentTerms { get; set; }
        public decimal? CreditLimit { get; set; }
        public bool IsActive { get; set; }
        public int? Rating { get; set; }
        public string? Notes { get; set; }
    }

    public class SupplierListItemDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? TaxCode { get; set; }
        public bool IsActive { get; set; }
        public int? Rating { get; set; }
        public int TotalPurchaseOrders { get; set; }
        public decimal TotalPurchaseAmount { get; set; }
    }

    // ============ SupplierContact DTOs ============

    public class SupplierContactDto
    {
        public int Id { get; set; }
        public int SupplierId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Position { get; set; }
        public string? Department { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Mobile { get; set; }
        public SupplierContactType ContactType { get; set; }
        public bool IsActive { get; set; }
        public bool IsPrimary { get; set; }
        public string? Notes { get; set; }
    }

    public class CreateSupplierContactDto
    {
        public int SupplierId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Position { get; set; }
        public string? Department { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Mobile { get; set; }
        public SupplierContactType ContactType { get; set; }
        public bool IsPrimary { get; set; }
        public string? Notes { get; set; }
    }

    public class UpdateSupplierContactDto
    {
        public string FullName { get; set; } = string.Empty;
        public string? Position { get; set; }
        public string? Department { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Mobile { get; set; }
        public SupplierContactType ContactType { get; set; }
        public bool IsActive { get; set; }
        public bool IsPrimary { get; set; }
        public string? Notes { get; set; }
    }
}
