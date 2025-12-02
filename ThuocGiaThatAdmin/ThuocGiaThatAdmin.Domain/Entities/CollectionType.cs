namespace ThuocGiaThatAdmin.Domain.Entities
{
    public enum CollectionType
    {
        Manual = 0,      // Thêm sản phẩm thủ công (Dành riêng cho bạn)
        AutoBrand = 1,   // Tự động lấy hàng brand
        AutoHGSG = 2,    // Tự động lấy hàng HGSG tuyển chọn
        AutoBoth = 3     // Tự động lấy cả brand và HGSG
    }
}
