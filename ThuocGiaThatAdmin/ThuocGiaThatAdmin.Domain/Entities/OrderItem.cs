using System;
using System.Collections.Generic;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    public class OrderItem : BaseEntity
    {
        // Id inherited
        public int OrderId { get; set; }
        public int ProductVariantId { get; set; }
        public int ProductId { get; set; }
        
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalLineAmount { get; set; }
        
        /// <summary>
        /// Số lượng đã được fulfill (book hàng từ kho)
        /// </summary>
        public int QuantityFulfilled { get; set; } = 0;
        
        /// <summary>
        /// Số lượng còn chưa fulfill = Quantity - QuantityFulfilled
        /// </summary>
        public int QuantityPending => Quantity - QuantityFulfilled;

        public Order Order { get; set; } = null!;
        public ProductVariant ProductVariant { get; set; } = null!;
        public ICollection<OrderItemFulfillment> Fulfillments { get; set; } = new List<OrderItemFulfillment>();
    }
}
