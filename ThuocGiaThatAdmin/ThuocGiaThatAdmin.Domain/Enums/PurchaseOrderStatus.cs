namespace ThuocGiaThatAdmin.Domain.Enums
{
    /// <summary>
    /// Trạng thái đơn đặt hàng nhập kho
    /// </summary>
    public enum PurchaseOrderStatus
    {
        /// <summary>
        /// Nháp
        /// </summary>
        Draft,

        /// <summary>
        /// Chờ duyệt
        /// </summary>
        Pending,

        /// <summary>
        /// Đã duyệt
        /// </summary>
        Approved,

        /// <summary>
        /// Đã nhận một phần
        /// </summary>
        PartiallyReceived,

        /// <summary>
        /// Hoàn thành
        /// </summary>
        Completed,

        /// <summary>
        /// Đã hủy
        /// </summary>
        Cancelled
    }
}
