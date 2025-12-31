namespace ThuocGiaThatAdmin.Domain.Entities
{
    /// <summary>
    /// Kiểu style của CTA button
    /// </summary>
    public enum CTAStyle
    {
        /// <summary>
        /// Primary - Nút chính (màu nổi bật)
        /// </summary>
        Primary = 1,

        /// <summary>
        /// Secondary - Nút phụ
        /// </summary>
        Secondary = 2,

        /// <summary>
        /// Outline - Viền ngoài
        /// </summary>
        Outline = 3,

        /// <summary>
        /// Ghost - Trong suốt
        /// </summary>
        Ghost = 4,

        /// <summary>
        /// Link - Dạng link
        /// </summary>
        Link = 5,

        /// <summary>
        /// Danger - Cảnh báo (màu đỏ)
        /// </summary>
        Danger = 6,

        /// <summary>
        /// Success - Thành công (màu xanh lá)
        /// </summary>
        Success = 7,

        /// <summary>
        /// Warning - Cảnh báo (màu vàng)
        /// </summary>
        Warning = 8
    }
}
