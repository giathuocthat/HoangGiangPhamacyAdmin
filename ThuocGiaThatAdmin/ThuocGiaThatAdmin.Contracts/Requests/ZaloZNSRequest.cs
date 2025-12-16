using System.Text.Json.Serialization;

namespace ThuocGiaThatAdmin.Contract.Requests
{
    public class ZaloZNSRequest
    {
        [JsonPropertyName("oa_id")]
        public string OaId { get; set; } = string.Empty;

        [JsonPropertyName("template_id")]
        public int TemplateId { get; set; }

        [JsonPropertyName("phone")]
        public string Phone { get; set; } = string.Empty;

        [JsonPropertyName("template_data")]
        public TemplateData TemplateData { get; set; } = new();

        [JsonPropertyName("tracking_id")]
        public string TrackingId { get; set; } = string.Empty;

        [JsonPropertyName("sending_mode")]
        public string SendingMode { get; set; } = "default";
    }

    public class TemplateData
    {
        [JsonPropertyName("otp")]
        public string Otp { get; set; } = string.Empty;
    }
}
