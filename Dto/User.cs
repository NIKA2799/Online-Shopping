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
        public  string Name { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ShippingAddress { get; set; }
        public string? BillingAddress { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        private string _passwordHash;
        public string Password
        {
            // Getter only returns the hash
            get => _passwordHash;
            // Setter hashes the incoming plaintext
            set => _passwordHash = BCrypt.Net.BCrypt.HashPassword(value);
        }
        public string ApplicationUserId { get; set; } // დარჩეს string
        public ApplicationUser ApplicationUser { get; set; }
        public ICollection<Product> Products { get; set; }
        public ICollection<Order>? Orders { get; set; }
        public ICollection<Cart>? Carts { get; set; }
        public ICollection<Wishlist>? Wishlists { get; set; }
    }
}