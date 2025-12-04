using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Domain.Enums;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    public class ProductStatusMap
    {
        public int ProductVariantId { get; set; }

        public ProductStatusType StatusType { get; set; }

        public string StatusName { get; set; } = string.Empty;

        public ProductVariant ProductVariant { get; set; } = null;
    }
}
