using System;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    /// <summary>
    /// Bank entity - Danh sách ngân hàng Việt Nam
    /// </summary>
    public class Bank : AuditableEntity
    {
        public string BankCode { get; set; } = string.Empty; // Mã ngân hàng (VD: VCB, TCB, MB)
        public string BankName { get; set; } = string.Empty; // Tên ngân hàng (VD: Vietcombank)
        public string? FullName { get; set; } // Tên đầy đủ (VD: Ngân hàng TMCP Ngoại thương Việt Nam)
        public bool IsActive { get; set; } = true;
        public int DisplayOrder { get; set; } = 0; // Thứ tự hiển thị
    }
}
