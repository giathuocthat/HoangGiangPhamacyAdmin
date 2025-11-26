namespace ThuocGiaThatAdmin.Domain.Entities
{
    /// <summary>
    /// Loại cảnh báo tồn kho
    /// </summary>
    public enum AlertType
    {
        LowStock = 1,           // Tồn kho thấp
        OutOfStock = 2,         // Hết hàng
        OverStock = 3,          // Tồn kho quá cao
        NearExpiry = 4,         // Gần hết hạn (< 3 tháng)
        Expired = 5,            // Hết hạn
        CriticalExpiry = 6      // Hết hạn nghiêm trọng (< 1 tháng)
    }
}
