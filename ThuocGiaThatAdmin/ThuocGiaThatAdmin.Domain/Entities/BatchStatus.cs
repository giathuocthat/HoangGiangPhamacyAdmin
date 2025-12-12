namespace ThuocGiaThatAdmin.Domain.Entities
{
    /// <summary>
    /// Trạng thái lô hàng
    /// </summary>
    public enum BatchStatus
    {
        Active = 1,         // Đang sử dụng
        NearExpiry = 2,     // Gần hết hạn (< 3 tháng)
        Expired = 3,        // Hết hạn
        Recalled = 4,       // Thu hồi
        Damaged = 5,        // Hư hỏng
        OutOfStock = 6     // Hết hàng
    }
}
