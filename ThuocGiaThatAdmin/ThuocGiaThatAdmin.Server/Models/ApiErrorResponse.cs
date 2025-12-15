namespace ThuocGiaThatAdmin.Server.Models
{
    public class ApiErrorResponse
    {     
        public string Detail { get; set; }
        public Dictionary<string, string> Errors { get; set; }
    }
}
