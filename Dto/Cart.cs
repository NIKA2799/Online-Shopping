namespace Dto
{
    public class Cart : IEntity
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public  User Customer { get; set; }
        public ICollection<CartItem> Items { get; set; }

    }
}