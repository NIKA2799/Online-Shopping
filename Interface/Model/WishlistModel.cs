using Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Model
{
   public class WishlistModel
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public ICollection<WishlistItem> Items { get; set; }
    }
}
