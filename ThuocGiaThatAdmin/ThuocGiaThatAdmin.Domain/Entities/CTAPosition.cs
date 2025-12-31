namespace ThuocGiaThatAdmin.Domain.Entities
{
    /// <summary>
    /// Vị trí hiển thị CTA
    /// </summary>
    public enum CTAPosition
    {
        /// <summary>
        /// Trang chủ - Hero section
        /// </summary>
        HomeHero = 1,

        /// <summary>
        /// Trang chủ - Giữa trang
        /// </summary>
        HomeMiddle = 2,

        /// <summary>
        /// Trang chủ - Cuối trang
        /// </summary>
        HomeBottom = 3,

        /// <summary>
        /// Trang sản phẩm
        /// </summary>
        ProductPage = 4,

        /// <summary>
        /// Trang danh mục
        /// </summary>
        CategoryPage = 5,

        /// <summary>
        /// Giỏ hàng
        /// </summary>
        Cart = 6,

        /// <summary>
        /// Thanh toán
        /// </summary>
        Checkout = 7,

        /// <summary>
        /// Sidebar
        /// </summary>
        Sidebar = 8,

        /// <summary>
        /// Header
        /// </summary>
        Header = 9,

        /// <summary>
        /// Footer
        /// </summary>
        Footer = 10,

        /// <summary>
        /// Floating (nổi trên màn hình)
        /// </summary>
        Floating = 11,

        /// <summary>
        /// Popup/Modal
        /// </summary>
        Popup = 12
    }
}
