using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThuocGiaThatAdmin.Contract.DTOs
{
    public class OrderSummaryDto
    {
        public int Id { get; set; }
        public string? OrderNumber { get; set; }
        public decimal Total { get; set; }
    }
}
