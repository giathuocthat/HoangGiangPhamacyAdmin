using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThuocGiaThatAdmin.Contract.Responses.Mobile
{
    public class MobileErrorResponse
    {
        public string ErrorCode { get; set; } // Machine-readable
        public string Message { get; set; } // Human-readable
        public Dictionary<string, string[]>? ValidationErrors { get; set; }
        public bool IsRetryable { get; set; } // Mobile can auto-retry
    }
}
