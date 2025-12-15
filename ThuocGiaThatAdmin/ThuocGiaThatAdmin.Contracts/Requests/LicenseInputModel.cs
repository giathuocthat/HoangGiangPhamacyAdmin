using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThatAdmin.Contract.Requests
{
    public class LicenseInputModel
    {
        public string Id { get; set; }
        public string Number { get; set; }
        public DateTime? IssueDate { get; set; }
        public int? IssuePlace { get; set; }
        public IFormFile File { get; set; }
        public CustomerDocumentType Type { get; set; }        
    }
}
