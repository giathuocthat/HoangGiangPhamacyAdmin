using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThuocGiaThatAdmin.Contract.Responses
{
    public class ValidationError
    {
        public string FieldName { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class ValidationResponse
    {
        public bool IsValid { get; set; } = true;
        public string Message { get; set; }
        public List<ValidationError> Errors { get; set; } = new List<ValidationError>();
    }
}
