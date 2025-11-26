namespace ThuocGiaThatAdmin.Contracts.Models
{
    public class FileUploadSettings
    {
        public int MaxFileSizeInMB { get; set; }
        public string UploadPath { get; set; } = string.Empty;
        public List<string> AllowedImageExtensions { get; set; } = [];
        public List<string> AllowedDocumentExtensions { get; set; } = [];
        public List<string> AllowedVideoExtensions { get; set; } = [];
        public List<string> AllowedAudioExtensions { get; set; } = [];
        public string BaseUrl { get; set; } = string.Empty;
        
        public List<string> GetAllAllowedExtensions()
        {
            var all = new List<string>();
            all.AddRange(AllowedImageExtensions);
            all.AddRange(AllowedDocumentExtensions);
            all.AddRange(AllowedVideoExtensions);
            all.AddRange(AllowedAudioExtensions);
            return all;
        }
    }
}
