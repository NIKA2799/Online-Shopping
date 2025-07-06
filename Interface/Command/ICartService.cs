using Interface.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Command
{
   public interface ICartService
    {
        bool Checkout(int userId, CheckoutModel model);
        bool ClearCart(int userId);
        bool UpdateItemQuantity(int userId, int cartItemId, int quantity);
        bool RemoveItemFromCart(int userId, int cartItemId);
        bool AddItemToCart(int customerid, int productId, int quantity);
    }
}
