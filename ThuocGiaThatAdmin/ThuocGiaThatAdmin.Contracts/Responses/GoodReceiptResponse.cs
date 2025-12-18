using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Contracts.DTOs;
using ThuocGiaThatAdmin.Domain.Enums;

namespace ThuocGiaThatAdmin.Contract.Responses
{
    public class GoodReceiptResponse
    {
        public int Id { get; set; }
        public string ReceiptNumber { get; set; } = string.Empty;
        public int PurchaseOrderId { get; set; }
        public string PurchaseOrderNumber { get; set; } = string.Empty;
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public GoodsReceiptStatus Status { get; set; }
        public DateTime? ScheduledDate { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public int? ReceivedByUserId { get; set; }
        public string? ReceivedByUserName { get; set; }
        public int? InspectedByUserId { get; set; }
        public string? InspectedByUserName { get; set; }
        public string? ShippingCarrier { get; set; }
        public string? TrackingNumber { get; set; }
        public string? VehicleNumber { get; set; }
        public string? Notes { get; set; }
        public string? RejectionReason { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public List<GoodsReceiptItemDto> Items { get; set; } = new List<GoodsReceiptItemDto>();

    }
}
