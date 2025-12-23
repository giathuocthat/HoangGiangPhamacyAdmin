using System.Collections.Generic;

namespace ThuocGiaThatAdmin.Contracts.DTOs
{
    /// <summary>
    /// DTO for Active Ingredient
    /// </summary>
    public class ActiveIngredientDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO for Product Active Ingredient relationship
    /// </summary>
    public class ProductActiveIngredientDto
    {
        public int? ActiveIngredientId { get; set; }
        public string? ActiveIngredientName { get; set; } // For creating new ingredient
        public string? Quantity { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsMainIngredient { get; set; }
    }
}
