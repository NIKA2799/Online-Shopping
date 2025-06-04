namespace Dto
{
    public class CartItem : IEntity
    {
        public int Id { get; set; }
        public int CartId { get; set; }

        public Cart Cart { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public string? Items { get; set; }

        public int Quantity { get; set; }
    }
}