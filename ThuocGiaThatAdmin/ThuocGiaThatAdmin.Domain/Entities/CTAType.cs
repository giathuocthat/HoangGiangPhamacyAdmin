namespace ThuocGiaThatAdmin.Domain.Entities
{
    /// <summary>
    /// Loại CTA (Call To Action)
    /// </summary>
    public enum CTAType
    {
        /// <summary>
        /// Nút bấm thông thường
        /// </summary>
        Button = 1,

        /// <summary>
        /// Link văn bản
        /// </summary>
        TextLink = 2,

        /// <summary>
        /// Banner có thể click
        /// </summary>
        ClickableBanner = 3,

        /// <summary>
        /// Popup/Modal
        /// </summary>
        Popup = 4,

        /// <summary>
        /// Floating button (nút nổi)
        /// </summary>
        FloatingButton = 5,

        /// <summary>
        /// Card có thể click
        /// </summary>
        ClickableCard = 6
    }
}
