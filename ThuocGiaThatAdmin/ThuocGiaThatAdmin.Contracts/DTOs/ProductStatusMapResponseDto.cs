using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Domain.Enums;

namespace ThuocGiaThatAdmin.Contracts.DTOs
{
    public class ProductStatusMapResponseDto
    {
        public int ProductVariantId { get; set; }
        public ProductStatusType StatusType { get; set; }
        public string StatusName { get; set; } = string.Empty;
    }
}
