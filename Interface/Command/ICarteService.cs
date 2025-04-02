using Dto;
using Interface.Model;
using System.Collections.Generic;

namespace Interface.Command
{
    public interface ICarteService
    {
        bool AddItemToCart(int customerid, int productId, int quantity);
        public Cart GetCartByUserId(int cuctomerid);
        bool RemoveItemFromCart(int userId, int cartItemId);
        bool UpdateItemQuantity(int userId, int cartItemId, int quantity);
        bool ClearCart(int userId);
        bool Checkout(int userId, CheckoutModel model);
        void AddToCart(int cartId, CartItemModel cartItem);
        IEnumerable<CartItemModel> GetCartItems(int cartId);
        void DeletCart(int cartId);
    }
}
