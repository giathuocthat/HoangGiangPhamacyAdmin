namespace ThuocGiaThatAdmin.Domain.Entities
{
    /// <summary>
    /// Loại giao dịch kho
    /// </summary>
    public enum TransactionType
    {
        Purchase = 1,           // Nhập hàng từ nhà cung cấp
        Sale = 2,               // Bán hàng
        Return = 3,             // Trả hàng (khách trả)
        ReturnToSupplier = 4,   // Trả hàng cho nhà cung cấp
        Transfer = 5,           // Chuyển kho
        Adjustment = 6,         // Điều chỉnh (kiểm kê)
        Damaged = 7,            // Hư hỏng
        Expired = 8,            // Hết hạn
        Promotion = 9,          // Khuyến mãi/Tặng
        Sample = 10             // Mẫu thử
    }
}
