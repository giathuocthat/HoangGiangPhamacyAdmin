namespace ThuocGiaThatAdmin.Domain.Enums
{
    /// <summary>
    /// Trạng thái phiếu nhập kho
    /// </summary>
    public enum GoodsReceiptStatus
    {
        /// <summary>
        /// Chờ nhận hàng
        /// </summary>
        Pending,

        /// <summary>
        /// Đang vận chuyển
        /// </summary>
        InTransit,

        /// <summary>
        /// Đã nhận hàng
        /// </summary>
        Received,

        /// <summary>
        /// Từ chối
        /// </summary>
        Rejected,

        /// <summary>
        /// Hoàn thành
        /// </summary>
        Completed
    }
}
