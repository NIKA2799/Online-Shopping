namespace Dto
{
    public class Order : IEntity
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public User Customer { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; }

        public OrderStatus Status { get; set; }

        public  string? ShippingAddress { get; set; }
        public  string? BillingAddress { get; set; }
        public  string? PaymentMethod { get; set; }
        public int UserId { get; set; }
    }
}