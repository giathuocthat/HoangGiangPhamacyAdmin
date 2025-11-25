using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using ThuocGiaThatAdmin.Domain.Entities;
using ThuocGiaThatAdmin.Contracts.Models;
using ThuocGiaThat.Infrastucture.Repositories;

namespace ThuocGiaThatAdmin.Service.Services
{
    public class FileUploadService
    {
        private readonly FileUploadSettings _settings;
        private readonly IUploadedFileRepository _fileRepository;
        
        public FileUploadService(
            IOptions<FileUploadSettings> settings,
            IUploadedFileRepository fileRepository)
        {
            _settings = settings.Value;
            _fileRepository = fileRepository;
        }
        
        /// <summary>
        /// Upload file và lưu metadata vào database
        /// </summary>
        public async Task<UploadedFile> UploadFileAsync(
            IFormFile file,
            UploadSource uploadSource,
            int? relatedEntityId = null,
            int? vendorId = null,
            string? description = null,
            string? uploadedByUserId = null)
        {
            // Validate file
            ValidateFile(file);
            
            // Determine file type
            var fileType = DetermineFileType(file.FileName);
            
            // Generate unique file name
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var storedFileName = $"{Guid.NewGuid()}{fileExtension}";
            
            // Determine storage folder based on upload source
            var storageFolder = GetStorageFolder(uploadSource);
            var fullFolderPath = Path.Combine(_settings.UploadPath, storageFolder);
            
            // Ensure directory exists
            Directory.CreateDirectory(fullFolderPath);
            
            // Full file path
            var filePath = Path.Combine(fullFolderPath, storedFileName);
            
            // Save file to disk
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            
            // Generate public URL
            var fileUrl = $"{_settings.BaseUrl}/uploads/{storageFolder}/{storedFileName}";
            
            // Create entity
            var uploadedFile = new UploadedFile
            {
                OriginalFileName = file.FileName,
                StoredFileName = storedFileName,
                FilePath = filePath,
                FileUrl = fileUrl,
                FileSize = file.Length,
                ContentType = file.ContentType,
                FileExtension = fileExtension,
                FileType = fileType,
                StorageFolder = storageFolder,
                UploadSource = uploadSource,
                RelatedEntityId = relatedEntityId,
                VendorId = vendorId,
                Description = description,
                UploadedByUserId = uploadedByUserId,
                IsInUse = false,
                IsDeleted = false
            };
            
            // Save to database
            await _fileRepository.AddAsync(uploadedFile);
            await _fileRepository.SaveChangesAsync();
            
            return uploadedFile;
        }
        
        /// <summary>
        /// Validate file size và extension
        /// </summary>
        private void ValidateFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty");
            
            // Check file size
            var maxSizeInBytes = _settings.MaxFileSizeInMB * 1024 * 1024;
            if (file.Length > maxSizeInBytes)
                throw new ArgumentException($"File size exceeds maximum allowed size of {_settings.MaxFileSizeInMB}MB");
            
            // Check extension
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var allowedExtensions = _settings.GetAllAllowedExtensions();
            
            if (!allowedExtensions.Contains(extension))
                throw new ArgumentException($"File type '{extension}' is not allowed. Allowed types: {string.Join(", ", allowedExtensions)}");
        }
        
        /// <summary>
        /// Xác định loại file dựa trên extension
        /// </summary>
        private FileType DetermineFileType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            
            if (_settings.AllowedImageExtensions.Contains(extension))
                return FileType.Image;
            if (_settings.AllowedDocumentExtensions.Contains(extension))
                return FileType.Document;
            if (_settings.AllowedVideoExtensions.Contains(extension))
                return FileType.Video;
            if (_settings.AllowedAudioExtensions.Contains(extension))
                return FileType.Audio;
            
            return FileType.Other;
        }
        
        /// <summary>
        /// Lấy tên thư mục lưu trữ dựa trên upload source
        /// </summary>
        private string GetStorageFolder(UploadSource uploadSource)
        {
            return uploadSource switch
            {
                UploadSource.Product => "products",
                UploadSource.ProductVariant => "product-variants",
                UploadSource.Brand => "brands",
                UploadSource.Category => "categories",
                UploadSource.Vendor => "vendors",
                UploadSource.Comment => "comments",
                UploadSource.Order => "orders",
                UploadSource.Customer => "customers",
                UploadSource.Blog => "blogs",
                UploadSource.Banner => "banners",
                _ => "general"
            };
        }
        
        /// <summary>
        /// Xóa file (soft delete)
        /// </summary>
        public async Task<bool> DeleteFileAsync(int fileId)
        {
            var file = await _fileRepository.GetByIdAsync(fileId);
            if (file == null)
                throw new InvalidOperationException($"File with ID {fileId} not found");
            
            file.IsDeleted = true;
            file.UpdatedDate = DateTime.UtcNow;
            
            _fileRepository.Update(file);
            await _fileRepository.SaveChangesAsync();
            
            return true;
        }
        
        /// <summary>
        /// Xóa file vật lý khỏi disk
        /// </summary>
        public async Task<bool> PermanentDeleteFileAsync(int fileId)
        {
            var file = await _fileRepository.GetByIdAsync(fileId);
            if (file == null)
                throw new InvalidOperationException($"File with ID {fileId} not found");
            
            // Delete physical file
            if (File.Exists(file.FilePath))
            {
                File.Delete(file.FilePath);
            }
            
            // Delete from database
            _fileRepository.Delete(file);
            await _fileRepository.SaveChangesAsync();
            
            return true;
        }
    }
}
