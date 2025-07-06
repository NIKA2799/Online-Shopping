using Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Model
{
    public class CartModel : IEntityModel
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public IEnumerable<CartItemModel> Items { get; set; }
    }
}
