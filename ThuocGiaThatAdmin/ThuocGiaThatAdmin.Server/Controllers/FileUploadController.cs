using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ThuocGiaThatAdmin.Contract.Requests;
using ThuocGiaThatAdmin.Contracts.DTOs;
using ThuocGiaThatAdmin.Domain.Entities;
using ThuocGiaThatAdmin.Service.Interfaces;
using ThuocGiaThatAdmin.Service.Services;

namespace ThuocGiaThatAdmin.Server.Controllers
{
    public class FileUploadController : BaseApiController
    {
        private readonly FileUploadService _fileUploadService;
        private readonly ICustomerService _customerService;
        
        public FileUploadController(
            FileUploadService fileUploadService,
            ICustomerService customerService,
            ILogger<FileUploadController> logger) : base(logger)
        {
            _fileUploadService = fileUploadService;
            _customerService = customerService;
        }
        
        /// <summary>
        /// Upload single file
        /// </summary>
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile([FromForm] FileUploadDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                var uploadedFile = await _fileUploadService.UploadFileAsync(
                    dto.File,
                    dto.UploadSource ?? UploadSource.General,
                    dto.RelatedEntityId,
                    dto.VendorId,
                    dto.Description,
                    uploadedByUserId: null // TODO: Get from authentication
                );
                
                var response = new FileUploadResponseDto
                {
                    Id = uploadedFile.Id,
                    OriginalFileName = uploadedFile.OriginalFileName,
                    StoredFileName = uploadedFile.StoredFileName,
                    FileUrl = uploadedFile.FileUrl ?? string.Empty,
                    FileSize = uploadedFile.FileSize,
                    ContentType = uploadedFile.ContentType,
                    FileExtension = uploadedFile.FileExtension,
                    FileType = uploadedFile.FileType,
                    CreatedDate = uploadedFile.CreatedDate
                };
                
                return Created(response, "File uploaded successfully");
            }, "Upload File");
        }
        
        /// <summary>
        /// Upload multiple files
        /// </summary>
        [HttpPost("upload-multiple")]
        public async Task<IActionResult> UploadMultipleFiles(
            [FromForm] List<IFormFile> files,
            [FromForm] UploadSource? uploadSource,
            [FromForm] int? relatedEntityId,
            [FromForm] int? vendorId)
        {
            return await ExecuteActionAsync(async () =>
            {
                var uploadedFiles = new List<FileUploadResponseDto>();
                
                foreach (var file in files)
                {
                    var uploadedFile = await _fileUploadService.UploadFileAsync(
                        file,
                        uploadSource ?? UploadSource.General,
                        relatedEntityId,
                        vendorId,
                        description: null,
                        uploadedByUserId: null
                    );
                    
                    uploadedFiles.Add(new FileUploadResponseDto
                    {
                        Id = uploadedFile.Id,
                        OriginalFileName = uploadedFile.OriginalFileName,
                        StoredFileName = uploadedFile.StoredFileName,
                        FileUrl = uploadedFile.FileUrl ?? string.Empty,
                        FileSize = uploadedFile.FileSize,
                        ContentType = uploadedFile.ContentType,
                        FileExtension = uploadedFile.FileExtension,
                        FileType = uploadedFile.FileType,
                        CreatedDate = uploadedFile.CreatedDate
                    });
                }
                
                return Created(uploadedFiles, $"{uploadedFiles.Count} files uploaded successfully");
            }, "Upload Multiple Files");
        }
        
        /// <summary>
        /// Delete file (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFile(int id)
        {
            return await ExecuteActionAsync(async () =>
            {
                await _fileUploadService.DeleteFileAsync(id);
                return Success("File deleted successfully");
            }, "Delete File");
        }
        
        /// <summary>
        /// Permanently delete file
        /// </summary>
        [HttpDelete("{id}/permanent")]
        public async Task<IActionResult> PermanentDeleteFile(int id)
        {
            return await ExecuteActionAsync(async () =>
            {
                await _fileUploadService.PermanentDeleteFileAsync(id);
                return Success("File permanently deleted");
            }, "Permanent Delete File");
        }

        [HttpPost("updateLicenses")]
        [Authorize]
        public async Task<IActionResult> UpdateLicenses([FromForm] List<LicenseInputModel> licenses)
        {
            // 1. Validation: Kiểm tra số lượng giấy phép có file
            if (licenses == null || licenses.Count < 1)
            {
                return BadRequest(new { message = "Vui lòng cung cấp ít nhất 2 loại giấy tờ." });
            }

            var customerId = int.Parse(User.FindFirst("customer_id")?.Value ?? "0");

            IList<CustomerDocumentDto> customerDocuments = new List<CustomerDocumentDto>();

            foreach (var license in licenses)
            {
                var uploadedFile = await _fileUploadService.UploadFileAsync(
                    license.File,
                    UploadSource.Customer,
                    null,
                    customerId,
                    description: null,
                    uploadedByUserId: customerId.ToString()
                );
                customerDocuments.Add(new CustomerDocumentDto
                {
                    DocumentNumber = license.Number,
                    CustomerId = customerId,
                    DocumentType = license.Type,
                    UploadedFileId = uploadedFile.Id,
                    IssueDate = license.IssueDate,
                    ProvinceId = license.IssuePlace,
                });
            }

            await _customerService.UpdateLicenses(customerDocuments);

            var response = await _customerService.GetLicenses(customerId);

            return Success(response);

        }

        [HttpGet("getLicenses")]
        [Authorize]
        public async Task<IActionResult> GetLicenses([FromForm] List<LicenseInputModel> licenses)
        {
            var customerId = int.Parse(User.FindFirst("customer_id")?.Value ?? "0");
            var response = await _customerService.GetLicenses(customerId);

            return Ok(response);
        }
    }
}
