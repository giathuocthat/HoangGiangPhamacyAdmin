using System;

namespace ThuocGiaThatAdmin.Contracts.DTOs
{
    /// <summary>
    /// DTO cho location có batch
    /// </summary>
    public class BatchLocationDto
    {
        public string LocationCode { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public bool IsPrimary { get; set; }
    }
}
