using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThuocGiaThatAdmin.Contract.Responses
{
    public class CustomerLicenseResponse
    {
        public int Id { get; set; }
        public int Type { get; set; }
        public string? Number { get; set; }
        public DateTime? IssueDate { get; set; }
        public string? FilePath { get; set; }
        public int? IssuePlace { get; set; }
    }
}
