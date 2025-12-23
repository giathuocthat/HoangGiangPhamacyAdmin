using System.Collections.Generic;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Contract.DTOs;
using ThuocGiaThatAdmin.Contract.Requests;
using ThuocGiaThatAdmin.Contract.Responses;
using ThuocGiaThatAdmin.Contracts.DTOs;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThatAdmin.Service.Interfaces
{
    public interface ICustomerService
    {
        /// <summary>
        /// Create a new customer with validation
        /// </summary>
        Task<(bool Success, string Message, CustomerResponseDto? Customer)> CreateCustomerAsync(CreateCustomerDto dto);

        /// <summary>
        /// Update existing customer with validation
        /// </summary>
        Task<(bool Success, string Message, CustomerResponseDto? Customer)> UpdateCustomerAsync(int id, UpdateCustomerDto dto);

        /// <summary>
        /// Get customer by ID
        /// </summary>
        Task<CustomerResponseDto?> GetCustomerByIdAsync(int id);

        /// <summary>
        /// Get all customers with pagination
        /// </summary>
        Task<(IEnumerable<CustomerResponseDto> Customers, int TotalCount)> GetCustomersAsync(int pageNumber, int pageSize);

        /// <summary>
        /// Search customers by phone number
        /// </summary>
        Task<IEnumerable<CustomerResponseDto>> SearchByPhoneAsync(string phoneNumber);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="documents"></param>
        /// <returns></returns>
        Task UpdateLicenses(IList<CustomerDocumentDto> documents);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        Task<IList<CustomerLicenseResponse>> GetLicenses(int customerId);

        /// <summary>
        /// Get all documents for a customer
        /// </summary>
        /// <param name="customerId">Customer ID</param>
        /// <returns>List of customer documents</returns>
        Task<IList<CustomerDocumentDto>> GetCustomerDocumentsAsync(int customerId);

        /// <summary>
        /// Upload a new document for a customer
        /// </summary>
        /// <param name="customerId">Customer ID</param>
        /// <param name="dto">Upload document DTO</param>
        /// <param name="uploadedByUserId">User ID who uploaded the document</param>
        /// <returns>Success status, message, and the created document</returns>
        Task<(bool Success, string Message, CustomerDocumentDto? Document)> UploadCustomerDocumentAsync(int customerId, UploadCustomerDocumentDto dto, string? uploadedByUserId = null);

        /// <summary>
        /// Verify or reject a customer document
        /// </summary>
        /// <param name="documentId">Document ID</param>
        /// <param name="dto">Verify document DTO</param>
        /// <param name="verifiedByUserId">User ID who verified the document</param>
        /// <returns>Success status, message, and the updated document</returns>
        Task<(bool Success, string Message, CustomerDocumentDto? Document)> VerifyCustomerDocumentAsync(int documentId, VerifyDocumentDto dto, string? verifiedByUserId = null);

        /// <summary>
        /// Verify or reject a customer based on their documents
        /// </summary>
        /// <param name="customerId">Customer ID</param>
        /// <param name="dto">Verify customer DTO</param>
        /// <param name="verifiedByUserId">User ID who verified the customer</param>
        /// <returns>Success status, message, and the customer status</returns>
        Task<(bool Success, string Message, CustomerStatusDto? CustomerStatus)> VerifyCustomerAsync(int customerId, VerifyCustomerDto dto, string? verifiedByUserId = null);

        // ========== Sales Hierarchy Methods ==========

        /// <summary>
        /// Lấy danh sách customers được assign cho một sale user
        /// </summary>
        Task<IEnumerable<CustomerResponseDto>> GetCustomersBySaleUserAsync(string saleUserId);

        /// <summary>
        /// Lấy danh sách customers của toàn bộ sales team (cho Sale Manager)
        /// </summary>
        Task<IEnumerable<CustomerResponseDto>> GetCustomersBySalesTeamAsync(string managerId);

        /// <summary>
        /// Assign customer cho sale user
        /// </summary>
        Task<bool> AssignSaleUserAsync(int customerId, string? saleUserId);
    }
}
