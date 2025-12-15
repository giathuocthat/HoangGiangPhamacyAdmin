using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThuocGiaThatAdmin.Service.Interfaces
{
    public interface IZaloService
    {
        Task<bool> SendOtpMessageAsync(string phone);
        Task<bool> VerifyOtpAsync(string phone, string otp);
    }
}
