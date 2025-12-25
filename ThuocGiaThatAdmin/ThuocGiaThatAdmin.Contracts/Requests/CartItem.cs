namespace ThuocGiaThatAdmin.Contract.Requests
{
    public class CartItem
    {
        public int? Id { get; set; }
        public int ProductVariantId { get; set; }
        public int Quantity { get; set; }
        
    }
}
