using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ThuocGiaThat.Infrastucture;
using ThuocGiaThatAdmin.Common;
using ThuocGiaThatAdmin.Contract.Enums;
using ThuocGiaThatAdmin.Contract.Models;
using ThuocGiaThatAdmin.Contract.Requests;
using ThuocGiaThatAdmin.Domain.Entities;
using ThuocGiaThatAdmin.Service.Interfaces;

namespace ThuocGiaThatAdmin.Service.Services
{
    public class ZaloService : IZaloService
    {
        private readonly HttpClient _httpClient;
        private readonly ZaloSetting _zaloSetting;
        private readonly TrueMecContext _context;

        public ZaloService(HttpClient httpClient, IConfiguration configuration, IOptions<ZaloSetting> options, TrueMecContext context) 
        {
            _httpClient = httpClient;
            _zaloSetting = options.Value;
            _context = context;
        }

        public async Task<bool> SendOtpMessageAsync(OtpRequest otpRequest)
        {
            var otp = NumberGenerator.GenernateNumbersString(4);

            if (string.IsNullOrEmpty(otpRequest.PhoneNumber)) throw new ArgumentNullException("Phone number is required");

            var otpCode = new OtpCode
            {
                Phone = otpRequest.PhoneNumber,
                Code = otp,
                CreatedAt = DateTime.UtcNow,
                Type = otpRequest.Type.ToString()
            };
            _context.OtpCodes.Add(otpCode);
            await _context.SaveChangesAsync();

            var body = new ZaloZNSRequest
            {
                OaId = _zaloSetting.OAId,
                Phone = otpRequest.PhoneNumber,
                SendingMode = "default",
                TemplateData = new TemplateData
                {
                    Otp = otp,
                },
                TemplateId = _zaloSetting.ZNSTemplateId,
                TrackingId = ""
            };

            var json = JsonSerializer.Serialize(body);

            var request = new HttpRequestMessage(HttpMethod.Post, _zaloSetting.BaseUrl + "/SendZNS")
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("Authorization", $"Bearer {_zaloSetting.ApiKey}");
            
            var response = await _httpClient.SendAsync(request);            

            return response.StatusCode == System.Net.HttpStatusCode.OK;
        }

        public async Task<bool> VerifyOtpAsync(string phone, OtpCodeTypeEnum type, string? otp)
        {
            if (string.IsNullOrEmpty(otp)) return false;

            var optCode = await _context.OtpCodes.FirstOrDefaultAsync(x => x.Phone == phone && x.Code == otp && !x.IsUsed && x.Type == type.ToString());

            if (optCode == null) return false;

            optCode.IsUsed = true;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
