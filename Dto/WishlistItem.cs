using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto
{
    public class WishlistItem :IEntity
    {
        public int Id { get; set; }
        public int WishlistId { get; set; }
        public Wishlist Wishlist { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
