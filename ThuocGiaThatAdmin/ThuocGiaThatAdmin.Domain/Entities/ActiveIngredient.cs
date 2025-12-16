using System;
using System.Collections.Generic;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    /// <summary>
    /// Active Ingredient entity - Hoạt chất
    /// </summary>
    public class ActiveIngredient : AuditableEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ChemicalFormula { get; set; } // Công thức hóa học
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public ICollection<ProductActiveIngredient> ProductActiveIngredients { get; set; } = new List<ProductActiveIngredient>();
    }
}
