namespace ThuocGiaThatAdmin.Domain.Enums
{
    /// <summary>
    /// Trạng thái thanh toán
    /// </summary>
    public enum PaymentStatus
    {
        /// <summary>
        /// Chưa thanh toán
        /// </summary>
        Unpaid,

        /// <summary>
        /// Đã thanh toán
        /// </summary>
        Paid,

        /// <summary>
        /// Quá hạn thanh toán
        /// </summary>
        Overdue
    }
}
