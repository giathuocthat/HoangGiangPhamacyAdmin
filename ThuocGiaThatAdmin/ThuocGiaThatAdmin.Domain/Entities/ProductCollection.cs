using System;
using System.Collections.Generic;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    public class ProductCollection : AuditableEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Type { get; set; }
        public bool IsActive { get; set; } = true;
        public int DisplayOrder { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        
        // Navigation
        public ICollection<ProductCollectionItem> Items { get; set; } = new List<ProductCollectionItem>();
    }
}
