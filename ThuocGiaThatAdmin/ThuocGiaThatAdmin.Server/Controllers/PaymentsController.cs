//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using ThuocGiaThatAdmin.Contract.DTOs;
//using ThuocGiaThatAdmin.Contract.Models;
//using ThuocGiaThatAdmin.Domain.Entities;
//using ThuocGiaThatAdmin.Service.Services;

//namespace ThuocGiaThatAdmin.Server.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class PaymentsController : ControllerBase
//    {
//        private readonly VNPayService _vnPayService;

//        [HttpPost("verify-vnpay-return")]
//        [AllowAnonymous] // Có thể cần anonymous vì được gọi từ frontend
//        public async Task<IActionResult> VerifyVNPayReturn([FromBody] VNPayReturnDto vnpayDto)
//        {
//            try
//            {
//                // 2. Chuyển đổi DTO thành SortedList để xác minh chữ ký
//                var vnpParams = new SortedList<string, string>();

//                // Sử dụng reflection để lấy tất cả properties
//                var properties = vnpayDto.GetType().GetProperties();
//                foreach (var prop in properties)
//                {
//                    var value = prop.GetValue(vnpayDto)?.ToString();
//                    if (!string.IsNullOrEmpty(value) && prop.Name.StartsWith("vnp_"))
//                    {
//                        vnpParams.Add(prop.Name, value);
//                    }
//                }

//                // 3. Xác minh chữ ký (loại bỏ vnp_SecureHash khỏi params trước khi kiểm tra)
//                var secureHash = vnpParams["vnp_SecureHash"];
//                vnpParams.Remove("vnp_SecureHash");

//                bool isValid = _vnPayService.ValidateSignature(vnpParams, secureHash);

//                if (!isValid)
//                {
//                    return BadRequest(new
//                    {
//                        success = false,
//                        message = "Chữ ký không hợp lệ. Giao dịch có thể đã bị can thiệp."
//                    });
//                }

//                // 4. Kiểm tra mã phản hồi
//                if (vnpayDto.vnp_ResponseCode != "00")
//                {
//                    return Ok(new
//                    {
//                        success = false,
//                        message = GetResponseMessage(vnpayDto.vnp_ResponseCode),
//                        responseCode = vnpayDto.vnp_ResponseCode
//                    });
//                }

//                // 5. Tìm giao dịch trong database
//                var transaction = await _context.PaymentTransactions
//                    .Include(t => t.Order)
//                    .FirstOrDefaultAsync(t => t.TransactionCode == vnpayDto.vnp_TxnRef);

//                if (transaction == null)
//                {
//                    return NotFound(new
//                    {
//                        success = false,
//                        message = "Không tìm thấy thông tin giao dịch"
//                    });
//                }

//                // 6. Kiểm tra số tiền
//                decimal amount = Convert.ToDecimal(vnpayDto.vnp_Amount) / 100;
//                if (transaction.Amount != amount)
//                {
//                    return BadRequest(new
//                    {
//                        success = false,
//                        message = "Số tiền thanh toán không khớp",
//                        expectedAmount = transaction.Amount,
//                        receivedAmount = amount
//                    });
//                }

//                // 7. Kiểm tra trạng thái thanh toán đã được IPN xử lý chưa
//                if (transaction.PaymentStatus == 1) // Đã thành công
//                {
//                    return Ok(new
//                    {
//                        success = true,
//                        orderId = transaction.OrderId,
//                        orderCode = transaction.Order?.OrderCode,
//                        amount = transaction.Amount,
//                        paymentDate = transaction.PayDate?.ToString("dd/MM/yyyy HH:mm:ss"),
//                        message = "Thanh toán thành công",
//                        transactionNo = vnpayDto.vnp_TransactionNo,
//                        bankCode = vnpayDto.vnp_BankCode
//                    });
//                }
//                else if (transaction.PaymentStatus == 0) // Đang chờ (IPN có thể chưa xử lý)
//                {
//                    // Có thể đợi một chút và kiểm tra lại, hoặc yêu cầu người dùng đợi
//                    // Trong thực tế, nên có cơ chế polling hoặc webhook để cập nhật

//                    // Tạm thời thông báo đang xử lý
//                    return Ok(new
//                    {
//                        success = false,
//                        message = "Giao dịch đang được xử lý. Vui lòng đợi vài phút.",
//                        transactionNo = vnpayDto.vnp_TransactionNo
//                    });
//                }
//                else // Thất bại
//                {
//                    return Ok(new
//                    {
//                        success = false,
//                        message = transaction.Message ?? "Thanh toán thất bại",
//                        responseCode = transaction.ResponseCode
//                    });
//                }
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new
//                {
//                    success = false,
//                    message = "Đã có lỗi xảy ra khi xác minh thanh toán"
//                });
//            }
//        }

//        // Hàm lấy thông báo từ mã lỗi VNPay
//        private string GetResponseMessage(string responseCode)
//        {
//            return responseCode switch
//            {
//                "00" => "Giao dịch thành công",
//                "01" => "Giao dịch chưa hoàn tất",
//                "02" => "Giao dịch bị lỗi",
//                "04" => "Giao dịch đảo (Khách hàng đã bị trừ tiền tại Ngân hàng nhưng GD chưa thành công ở VNPAY)",
//                "05" => "VNPAY đang xử lý giao dịch này (GD hoàn tiền)",
//                "06" => "VNPAY đã gửi yêu cầu hoàn tiền sang Ngân hàng (GD hoàn tiền)",
//                "07" => "Giao dịch bị nghi ngờ gian lận",
//                "09" => "GD Hoàn trả bị từ chối",
//                "10" => "Đã hết hạn chờ thanh toán. Xin quý khách vui lòng thực hiện lại giao dịch.",
//                _ => "Mã lỗi không xác định"
//            };
//        }
//    }
//}
