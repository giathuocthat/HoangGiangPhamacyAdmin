using System;
using System.Collections.Generic;

namespace ThuocGiaThatAdmin.Contracts.Responses
{
    /// <summary>
    /// Category with hierarchical structure (parent-child relationships)
    /// </summary>
    public sealed record CategoryNodeResponse(
        int Id,
        string Name,
        string Slug,
        string? Description,
        int DisplayOrder,
        bool IsActive,
        DateTime CreatedDate,
        DateTime? UpdatedDate,
        IReadOnlyList<CategoryNodeResponse> Children
    );

    /// <summary>
    /// Flat category with parent info
    /// </summary>
    public sealed record CategoryFlatResponse(
        int Id,
        string Name,
        string Slug,
        string? Description,
        int? ParentId,
        string? ParentName,
        int DisplayOrder,
        bool IsActive,
        int ChildCount,
        DateTime CreatedDate,
        DateTime? UpdatedDate
    );
}