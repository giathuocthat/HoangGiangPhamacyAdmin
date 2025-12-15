using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ThuocGiaThatAdmin.Contract.Responses
{
    public class ZaloZNSResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("shop_id")]
        public string ShopId { get; set; } = string.Empty;

        [JsonPropertyName("user_id")]
        public string UserId { get; set; } = string.Empty;

        [JsonPropertyName("oa_id")]
        public string OaId { get; set; } = string.Empty;

        [JsonPropertyName("zl_user_id")]
        public string ZlUserId { get; set; } = string.Empty;

        [JsonPropertyName("phone")]
        public string Phone { get; set; } = string.Empty;

        [JsonPropertyName("msg_id")]
        public string MsgId { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("campaign_id")]
        public string CampaignId { get; set; } = string.Empty;

        [JsonPropertyName("template_id")]
        public int TemplateId { get; set; }

        [JsonPropertyName("template_data")]
        public ZaloTemplateData TemplateData { get; set; } = new();

        [JsonPropertyName("journey_id")]
        public string JourneyId { get; set; } = string.Empty;

        [JsonPropertyName("tracking_id")]
        public string TrackingId { get; set; } = string.Empty;

        [JsonPropertyName("fee_main")]
        public int FeeMain { get; set; }

        [JsonPropertyName("fee_token")]
        public int FeeToken { get; set; }

        [JsonPropertyName("timeout")]
        public int Timeout { get; set; }

        [JsonPropertyName("request_time")]
        public DateTime RequestTime { get; set; }

        [JsonPropertyName("sent_time")]
        public DateTime SentTime { get; set; }

        [JsonPropertyName("delivery_time")]
        public DateTime DeliveryTime { get; set; }

        [JsonPropertyName("delivery_status")]
        public string DeliveryStatus { get; set; } = string.Empty;

        [JsonPropertyName("is_charged")]
        public bool IsCharged { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("rate")]
        public decimal Rate { get; set; }

        [JsonPropertyName("note")]
        public string Note { get; set; } = string.Empty;

        [JsonPropertyName("feedback")]
        public List<string> Feedback { get; set; } = new();

        [JsonPropertyName("response_data")]
        public string ResponseData { get; set; } = string.Empty;

        [JsonPropertyName("submit_time")]
        public DateTime SubmitTime { get; set; }

        [JsonPropertyName("error_code")]
        public int ErrorCode { get; set; }

        [JsonPropertyName("error_message")]
        public string ErrorMessage { get; set; } = string.Empty;

        [JsonPropertyName("enable_sms_on_zns_failure")]
        public bool EnableSmsOnZnsFailure { get; set; }

        [JsonPropertyName("is_development")]
        public bool IsDevelopment { get; set; }

        [JsonPropertyName("sending_mode")]
        public string SendingMode { get; set; } = string.Empty;

        [JsonPropertyName("sms")]
        public object? Sms { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        // Helper properties
        [JsonIgnore]
        public bool IsSuccess => ErrorCode == 0;

        [JsonIgnore]
        public bool IsPending => Status == "P";

        [JsonIgnore]
        public bool IsDelivered => DeliveryStatus == "delivered";
    }

    public class ZaloTemplateData
    {
        [JsonPropertyName("otp")]
        public string Otp { get; set; } = string.Empty;
    }
}
