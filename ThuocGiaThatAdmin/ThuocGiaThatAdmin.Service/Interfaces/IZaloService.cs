using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Contract.Enums;
using ThuocGiaThatAdmin.Contract.Requests;

namespace ThuocGiaThatAdmin.Service.Interfaces
{
    public interface IZaloService
    {
        Task<bool> SendOtpMessageAsync(OtpRequest request);
        Task<bool> VerifyOtpAsync(string phone, OtpCodeTypeEnum type, string? otp);
    }
}
