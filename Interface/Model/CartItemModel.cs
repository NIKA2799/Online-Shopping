
namespace Interface.Model
{
    public class CartItemModel
    {
        public int Id { get; set; } // Primary key
        public int CartId { get; set; } // Foreign key to the Cart
        public int ProductId { get; set; } // Foreign key to the Product
        public int Quantity { get; set; }

    }
}

