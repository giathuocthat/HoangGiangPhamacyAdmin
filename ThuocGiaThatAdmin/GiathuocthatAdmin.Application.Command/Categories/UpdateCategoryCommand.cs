using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Common;
using ThuocGiaThatAdmin.Common.Interfaces;
using ThuocGiaThatAdmin.Contract.Responses;

namespace ThuocGiaThatAdmin.Commands.Categories
{
    public class UpdateCategoryCommand : ICommand<Result<UpdateCategoryResponse>>
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? ParentId { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; } = true;
        public string? ImageUrl { get; set; }
    }
}
