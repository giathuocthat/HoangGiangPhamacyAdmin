using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThuocGiaThatAdmin.Contract.DTOs
{
    public class FilterRequest
    {
        public List<FilterCondition> Filters { get; set; } = new();
        public string Logic { get; set; } = "and"; // "and" hoặc "or"
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; }
        public string SortOrder { get; set; } = "asc"; // "asc" hoặc "desc"
    }

    public class FilterCondition
    {
        public string? Field { get; set; }
        public string? Operator { get; set; } // eq, ne, gt, lt, gte, lte, contains, startswith, endswith
        public dynamic? Value { get; set; }
    }
}
