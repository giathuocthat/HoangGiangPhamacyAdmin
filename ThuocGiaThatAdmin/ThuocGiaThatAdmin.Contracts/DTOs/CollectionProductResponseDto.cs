using System;

namespace ThuocGiaThatAdmin.Contracts.DTOs
{
    public class CollectionProductResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ThumbnailUrl { get; set; }
        public int? ProductVariantId { get; set; }
        public string? Specification { get; set;}
        public decimal? OriginalPrice { get; set; }
        public decimal? Price { get; set; }
        public int? MaxQuantityPerOrder { get; set; }
        public string? ShortDescription { get; set; }
        public int DisplayOrder { get; set; }
        public string? Slug { get; set; }
        public bool? IsFavorite { get; set; }
        public string BrandName { get; set; }
        public decimal? SuggestedRetailPrice
        {
            get
            {
                return Price.HasValue ? Price.Value + 100000 : null;
            }
        }
        public int QuantityInCart { get; set; }
    }
}
