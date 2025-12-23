using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThatAdmin.Contract.DTOs
{
    public class InvoiceInfoDto
    {
        public int? Id { get; set; }
        
        public int CustomerId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string TaxCode { get; set; } = string.Empty;
        public string InvoiceAddress { get; set; } = string.Empty;
        public bool IsDefault { get; set; } = false;
        public bool IsVerified { get; set; } = false;
        public string Note { get; set; } = string.Empty;
        public bool IsActive { get; set; } = false;
        public string BuyerName { get; set; } = string.Empty;
    }
}
