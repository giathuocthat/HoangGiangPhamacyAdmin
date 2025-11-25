using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThatAdmin.Contracts.DTOs
{
    public class FileUploadDto
    {
        [Required(ErrorMessage = "File is required")]
        public IFormFile File { get; set; } = null!;
        
        public UploadSource? UploadSource { get; set; }
        
        public int? RelatedEntityId { get; set; }
        
        public int? VendorId { get; set; }
        
        public string? Description { get; set; }
    }
    
    public class FileUploadResponseDto
    {
        public int Id { get; set; }
        public string OriginalFileName { get; set; } = string.Empty;
        public string StoredFileName { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string ContentType { get; set; } = string.Empty;
        public string FileExtension { get; set; } = string.Empty;
        public FileType FileType { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
