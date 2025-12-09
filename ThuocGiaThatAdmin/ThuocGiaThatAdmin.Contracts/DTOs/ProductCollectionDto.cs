using System;
using System.Collections.Generic;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThatAdmin.Contracts.DTOs
{
    public class ProductCollectionDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Description { get; set; }
        public CollectionType Type { get; set; }
        public bool IsActive { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<ProductDto>? Products { get; set; }
    }

    public class CreateCollectionDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public CollectionType Type { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<CreateCollectionItemDto>? Items { get; set; }
    }

    public class CreateCollectionItemDto
    {
        public int ProductId { get; set; }
        public int DisplayOrder { get; set; }
    }

    public class UpdateProductCollectionDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public CollectionType Type { get; set; }
        public bool IsActive { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<UpdateCollectionItemDto>? Items { get; set; }
    }

    public class UpdateCollectionItemDto
    {
        public int ProductId { get; set; }
        public int DisplayOrder { get; set; }
    }

    public class SetMaxOrderDto
    {
        public int MaxQuantityPerOrder { get; set; }
        public int? MaxQuantityPerDay { get; set; }
        public int? MaxQuantityPerMonth { get; set; }
        public string? Reason { get; set; }
    }
}
