namespace ThuocGiaThatAdmin.Domain.Enums
{
    /// <summary>
    /// Loại liên hệ của nhà cung cấp
    /// </summary>
    public enum SupplierContactType
    {
        /// <summary>
        /// Liên hệ chính
        /// </summary>
        Primary,

        /// <summary>
        /// Liên hệ phụ
        /// </summary>
        Secondary,

        /// <summary>
        /// Liên hệ kỹ thuật
        /// </summary>
        Technical,

        /// <summary>
        /// Liên hệ tài chính
        /// </summary>
        Financial,

        /// <summary>
        /// Liên hệ vận chuyển/logistics
        /// </summary>
        Logistics
    }
}
