namespace Dto
{
    public class Order : IEntity
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; }

        public OrderStatus Status { get; set; }

        public required string ShippingAddress { get; set; }
        public required string BillingAddress { get; set; }
        public required string PaymentMethod { get; set; }

    }
}