using System;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    /// <summary>
    /// Loại giấy tờ khách hàng
    /// </summary>
    public enum CustomerDocumentType
    {
        /// <summary>
        /// Giấy phép kinh doanh
        /// </summary>
        BusinessLicense = 0,
        
        /// <summary>
        /// Giấy chứng nhận đăng ký kinh doanh
        /// </summary>
        BusinessRegistrationCertificate = 1,
        
        /// <summary>
        /// Giấy phép bán lẻ thuốc
        /// </summary>
        PharmacyRetailLicense = 2,
        
        /// <summary>
        /// Giấy phép bán buôn thuốc
        /// </summary>
        PharmacyWholesaleLicense = 3,
        
        /// <summary>
        /// Chứng chỉ hành nghề dược
        /// </summary>
        PharmacistCertificate = 4,
        
        /// <summary>
        /// Giấy chứng nhận đủ điều kiện kinh doanh dược
        /// </summary>
        PharmacyComplianceCertificate = 5,
        
        /// <summary>
        /// CMND/CCCD người đại diện
        /// </summary>
        IdentificationCard = 6,
        
        /// <summary>
        /// Giấy ủy quyền (nếu có)
        /// </summary>
        PowerOfAttorney = 7,
        
        /// <summary>
        /// Giấy tờ khác
        /// </summary>
        Other = 99
    }
}
