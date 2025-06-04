using Org.BouncyCastle.Crypto.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto
{
    public class User : IEntity
    {

        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ShippingAddress { get; set; }
        public string? BillingAddress { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public ICollection<Order>? Orders { get; set; }
        public ICollection<Cart>? Carts { get; set; }
        public ICollection<Wishlist>? Wishlists { get; set; }
    }
}