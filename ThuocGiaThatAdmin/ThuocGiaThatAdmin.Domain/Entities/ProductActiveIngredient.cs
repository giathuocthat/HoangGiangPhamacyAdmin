using System;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    /// <summary>
    /// Product Active Ingredient - Bảng trung gian giữa sản phẩm và hoạt chất
    /// </summary>
    public class ProductActiveIngredient : BaseEntity
    {
        public int ProductId { get; set; }
        public int ActiveIngredientId { get; set; }
        public int DisplayOrder { get; set; } // Thứ tự hiển thị
        public bool IsMainIngredient { get; set; } = false; // Hoạt chất chính
        public string? Quantity { get; set; } // Hàm lượng (vd: "500mg", "10ml")

        // Navigation properties
        public Product Product { get; set; } = null!;
        public ActiveIngredient ActiveIngredient { get; set; } = null!;
    }
}
