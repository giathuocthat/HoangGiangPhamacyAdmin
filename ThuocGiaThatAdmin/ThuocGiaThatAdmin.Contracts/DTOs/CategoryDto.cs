using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThuocGiaThatAdmin.Contract.DTOs
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public int? ParentId { get; set; }

        public string Slug { get; set; } = string.Empty;

        public int DisplayOrder { get; set; } = 0;

        public string? ImageUrl { get; set; }
    }
}
