using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ThuocGiaThat.Infrastucture;
using ThuocGiaThatAdmin.Contract.DTOs;
using ThuocGiaThatAdmin.Contract.Enums;
using ThuocGiaThatAdmin.Contract.Models;
using ThuocGiaThatAdmin.Domain.Entities;
using ThuocGiaThatAdmin.Domain.Enums;

namespace ThuocGiaThatAdmin.Service.Services
{
    public class VNPayService
    {
        private readonly VNPaySetting _settings;
        public readonly IConfiguration _configuration;
        private readonly TrueMecContext _context;

        public VNPayService(IConfiguration configuration, TrueMecContext context, IOptions<VNPaySetting> options)
        {
            _configuration = configuration;
            _context = context;
            _settings = options.Value;
        }

        public string CreatePaymentUrl(PaymentInformationModel model, string ipAddress)
        {
            var vnpParams = new SortedList<string, string>
            {
                { "vnp_Version", _settings.Version },
                { "vnp_Command", _settings.Command },
                { "vnp_TmnCode", _settings.TmnCode },
                { "vnp_Amount", (model.Amount * 100).ToString() }, // Nhân 100 vì VNPAY yêu cầu
                { "vnp_CreateDate", model.CreatedDate.ToString("yyyyMMddHHmmss") },
                { "vnp_CurrCode", _settings.CurrCode },
                { "vnp_IpAddr", ipAddress },
                { "vnp_Locale", _settings.Locale },
                { "vnp_OrderInfo", $"Thanh toan don hang: {model.OrderId}" },
                { "vnp_OrderType", "other" },
                { "vnp_ReturnUrl", _settings.ReturnUrl },
                { "vnp_TxnRef", model.OrderId.ToString() }, // Mã tham chiếu đơn hàng,
                { "vnp_BankCode", model.BankCode }
            };

            var queryString = BuildQueryString(vnpParams);
            var hash = HmacSHA512(_settings.HashSecret, queryString);
            var paymentUrl = $"{_settings.Url}?{queryString}&vnp_SecureHash={hash}";

            return paymentUrl;
        }

        private string BuildQueryString(SortedList<string, string> vnpParams)
        {
            StringBuilder query = new StringBuilder();
            foreach (var vnpParam in vnpParams)
            {
                if (!string.IsNullOrEmpty(vnpParam.Key) && !vnpParam.Key.Contains("vnp_SecureHash"))
                {
                    query.Append(WebUtility.UrlEncode(vnpParam.Key) + "=" + WebUtility.UrlEncode(vnpParam.Value) + "&");
                }
            }

            query.Length--;

            return query.ToString();
        }

        private string HmacSHA512(string key, String inputData)
        {
            var hash = new StringBuilder();
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputData);
            using (var hmac = new HMACSHA512(keyBytes))
            {
                byte[] hashValue = hmac.ComputeHash(inputBytes);
                foreach (var theByte in hashValue)
                {
                    hash.Append(theByte.ToString("x2"));
                }
            }

            return hash.ToString();
        }

        public bool ValidateSignature(SortedList<string, string> vnpParams, string vnpHash)
        {
            string hash = HmacSHA512(_settings.HashSecret, BuildQueryString(vnpParams));
            return hash.Equals(vnpHash, StringComparison.InvariantCultureIgnoreCase);
        }

        public async Task UpdatePayIPN(SortedList<string, string> vnpParams, string rawData)
        {
            var vnpHash = vnpParams["vnp_SecureHash"];
            vnpParams.Remove("vnp_SecureHash");
            bool isValid = ValidateSignature(vnpParams, vnpHash);

            if (!isValid) return;// throw Exception($"Invalid Signature {vnpParams} - rawData: {rawData}");

            string transactionCode = vnpParams["vnp_TxnRef"];
            string vnpResponseCode = vnpParams["vnp_ResponseCode"];
            decimal amount = Convert.ToDecimal(vnpParams["vnp_Amount"]) / 100; // Chia 100

            // 6. Tìm payment transaction trong DB
            var payment = await _context.PaymentTransactions
                .Include(p => p.Order)
                .FirstOrDefaultAsync(p => p.TransactionCode == transactionCode);

            if (payment == null || payment.Amount != amount) return;

            if (payment.PaymentStatus == (int)PaymentTransactionStatus.Pending)
            {
                if (vnpResponseCode == "00") // success
                {
                    payment.PaymentStatus = (int)PaymentTransactionStatus.Success;
                    payment.Order.PaymentStatus = OrderPaymentStatus.Paid.ToString();
                    payment.Order.OrderStatus = OrderStatus.Confirmed.ToString();
                    payment.VNPAYTransactionNo = vnpParams["vnp_TransactionNo"];
                    payment.BankCode = vnpParams["vnp_BankCode"];
                    payment.ResponseCode = vnpResponseCode;
                    payment.Message = vnpParams["vnp_Message"];
                    payment.PayDate = DateTimeOffset.ParseExact(vnpParams["vnp_PayDate"], "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                    payment.ModifiedAt = DateTimeOffset.Now;
                }
                else
                {
                    payment.PaymentStatus = (int)PaymentTransactionStatus.Failed;
                    payment.Order.PaymentStatus = OrderPaymentStatus.Failed.ToString();
                    payment.ResponseCode = vnpResponseCode;
                    payment.Message = vnpParams["vnp_Message"];
                    payment.ModifiedAt = DateTimeOffset.Now;
                }

                await _context.SaveChangesAsync();
            }

            return;
        }

        public async Task<dynamic> VerifyVNPayReturn(VNPayReturnDto vnpayDto)
        {
            var vnpParams = new SortedList<string, string>();

            var properties = vnpayDto.GetType().GetProperties();
            foreach (var prop in properties)
            {
                var value = prop.GetValue(vnpayDto)?.ToString();
                if (!string.IsNullOrEmpty(value) && prop.Name.StartsWith("vnp_"))
                {
                    vnpParams.Add(prop.Name, value);
                }
            }

            var secureHash = vnpParams["vnp_SecureHash"];
            vnpParams.Remove("vnp_SecureHash");

            bool isValid = ValidateSignature(vnpParams, secureHash);
            if (!isValid) return new { success = false, message = "Chữ ký không hợp lệ" };

            var payment = await _context.PaymentTransactions.Include(x => x.Order).FirstOrDefaultAsync(x => x.TransactionCode == vnpayDto.vnp_TxnRef);

            if (payment == null)
                return new { success = false, message = "Không Tìm thấy giao dịch" };

            decimal amount = Convert.ToDecimal(vnpayDto.vnp_Amount) / 100;
            if (payment.Amount != amount) return new { success = false, message = "Số tiền thanh toán không khớp" };

            if (payment.PaymentStatus == (int)PaymentTransactionStatus.Success)
            {
                return new { success = true, orderId = payment.OrderId };
            }

            if (payment.PaymentStatus == (int)PaymentTransactionStatus.Pending)
            {
                if (vnpayDto.vnp_ResponseCode == "00") // success
                {
                    payment.PaymentStatus = (int)PaymentTransactionStatus.Success;
                    payment.Order.PaymentStatus = OrderPaymentStatus.Paid.ToString();
                    payment.Order.OrderStatus = OrderStatus.Confirmed.ToString();
                    payment.VNPAYTransactionNo = vnpParams["vnp_TransactionNo"];
                    payment.BankCode = vnpParams["vnp_BankCode"];
                    payment.ResponseCode = vnpayDto.vnp_ResponseCode;
                    payment.PayDate = DateTimeOffset.ParseExact(vnpParams["vnp_PayDate"], "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                    payment.ModifiedAt = DateTimeOffset.Now;
                }
                else
                {
                    payment.PaymentStatus = (int)PaymentTransactionStatus.Failed;
                    payment.Order.PaymentStatus = OrderPaymentStatus.Failed.ToString();
                    payment.ResponseCode = vnpayDto.vnp_ResponseCode;
                    payment.Message = GetResponseMessage(vnpayDto.vnp_ResponseCode);
                    payment.ModifiedAt = DateTimeOffset.Now;
                }

                await _context.SaveChangesAsync();

                return new { success = true, orderId = payment.OrderId };
            }
            else // Thất bại
            {
                return new
                {
                    success = false,
                    message = payment.Message ?? "Thanh toán thất bại",
                    responseCode = payment.ResponseCode
                };
            }
        }

        private string GetResponseMessage(string responseCode)
        {
            return responseCode switch
            {
                "00" => "Giao dịch thành công",
                "01" => "Giao dịch chưa hoàn tất",
                "02" => "Giao dịch bị lỗi",
                "04" => "Giao dịch đảo (Khách hàng đã bị trừ tiền tại Ngân hàng nhưng GD chưa thành công ở VNPAY)",
                "05" => "VNPAY đang xử lý giao dịch này (GD hoàn tiền)",
                "06" => "VNPAY đã gửi yêu cầu hoàn tiền sang Ngân hàng (GD hoàn tiền)",
                "07" => "Giao dịch bị nghi ngờ gian lận",
                "09" => "GD Hoàn trả bị từ chối",
                "10" => "Đã hết hạn chờ thanh toán. Xin quý khách vui lòng thực hiện lại giao dịch.",
                _ => "Mã lỗi không xác định"
            };
        }
    }

    public enum ValidationVNPayIPNStatus
    {
        InvalidSignature,
        TransactionNotFound,
        InvalidAmount,
        ConfirmSuccess
    }
}
