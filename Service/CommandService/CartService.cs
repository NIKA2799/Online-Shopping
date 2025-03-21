using AutoMapper;
using Dto;
using Interface.Command;
using Interface.IRepositories;
using Interface.Model;


namespace Service.CommandService
{
    public class CartService : ICarteService


    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CartService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        public Cart GetCartByUserId(int cuctomerid) => _unitOfWork.CartRepository.FindByCondition(c => c.CustomerId == cuctomerid).SingleOrDefault();

        public bool AddItemToCart(int customerid, int productId, int quantity)
        {
            var cart = GetCartByUserId(customerid);
            if (cart == null)
            {
                cart = new Cart { CustomerId = customerid };
                _unitOfWork.CartRepository.Insert(cart);
                _unitOfWork.SaveChanges();
            }

            var cartItem = new CartItem
            {
                CartId = cart.Id,
                ProductId = productId,
                Quantity = quantity

            };
            _unitOfWork.CartItemRepository.Insert(cartItem);
            _unitOfWork.SaveChanges();

            return true;
        }

        public bool RemoveItemFromCart(int userId, int cartItemId)
        {
            var cart = GetCartByUserId(userId);
            if (cart == null) return false;

            var cartItem = _unitOfWork.CartItemRepository.FindByCondition(ci => ci.CartId == cart.Id && ci.Id == cartItemId).SingleOrDefault();
            if (cartItem == null) return false;

            _unitOfWork.CartItemRepository.Delete(cartItem);
            _unitOfWork.SaveChanges();

            return true;
        }

        public bool UpdateItemQuantity(int userId, int cartItemId, int quantity)
        {
            var cart = GetCartByUserId(userId);
            if (cart == null) return false;

            var cartItem = _unitOfWork.CartItemRepository.FindByCondition(ci => ci.CartId == cart.Id && ci.Id == cartItemId).SingleOrDefault();
            if (cartItem == null) return false;

            cartItem.Quantity = quantity;
            _unitOfWork.CartItemRepository.Update(cartItem);
            _unitOfWork.SaveChanges();

            return true;
        }

        public bool ClearCart(int userId)
        {
            var cart = GetCartByUserId(userId);
            if (cart == null) return false;

            var cartItems = _unitOfWork.CartItemRepository.FindByCondition(ci => ci.CartId == cart.Id).ToList();
            foreach (var item in cartItems)
            {
                _unitOfWork.CartItemRepository.Delete(item);
            }
            _unitOfWork.SaveChanges();

            return true;
        }

        public bool Checkout(string userId, CheckoutModel model)
        {
            // Implement the checkout logic here.
            // For example, create an order and move items from the cart to the order.
            return true;
        }
    

        public void AddToCart(int cartId, CartItemModel cartItem)
        {
            var existingCart = _unitOfWork.CartRepository.FindByCondition(c => c.Id == cartId).SingleOrDefault();
            if (existingCart != null)
            {
                var cartItemEntity = _mapper.Map<CartItem>(cartItem);
                cartItemEntity.CartId = cartId;
                _unitOfWork.CartItemRepository.Insert(cartItemEntity);
                _unitOfWork.SaveChanges();
            }
        }

        public void UpdateCartItem(int cartId, CartItemModel cartItem)
        {
            var existingCartItem = _unitOfWork.CartItemRepository
                .FindByCondition(ci => ci.CartId == cartId && ci.ProductId == cartItem.ProductId)
                .SingleOrDefault();

            if (existingCartItem != null)
            {
                existingCartItem.Quantity = cartItem.Quantity;
                _unitOfWork.CartItemRepository.Update(existingCartItem);
                _unitOfWork.SaveChanges();
            }
        }

        public void RemoveFromCart(int cartId, int productId)
        {
            var cartItem = _unitOfWork.CartItemRepository
                .FindByCondition(ci => ci.CartId == cartId && ci.ProductId == productId)
                .SingleOrDefault();

            if (cartItem != null)
            {
                _unitOfWork.CartItemRepository.Delete(cartItem);
                _unitOfWork.SaveChanges();
            }
        }

        public IEnumerable<CartItemModel> GetCartItems(int cartId)
        {
            var cartItems = _unitOfWork.CartItemRepository.FindByCondition(ci => ci.CartId == cartId).ToList();
            return _mapper.Map<IEnumerable<CartItemModel>>(cartItems);
        }

        public void deletCart(int cartId)
        {
            var cartItems = _unitOfWork.CartItemRepository.FindByCondition(ci => ci.CartId == cartId).ToList();
            foreach (var cartItem in cartItems)
            {
                _unitOfWork.CartItemRepository.Delete(cartItem);
            }
            _unitOfWork.SaveChanges();
        }
    }
}
       

