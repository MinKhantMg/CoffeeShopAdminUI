namespace CoffeeShopAdmin.Models.OrderM
{
    public class OrderModel
    {
        public string Id { get; set; }  // Order ID
        public string OrderStatus { get; set; }  // Status of the order (e.g., Pending, Confirmed, etc.)
        public string OrderType { get; set; }  // Type of the order (e.g., Delivery, Pickup)
        public string PaymentType { get; set; }  // Payment method (e.g., Credit Card, Cash)
        public DateTime CreatedOn { get; set; }  // The date the order was created
        public int TotalAmount { get; set; }
    }
}
