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
            var customer = _unitOfWork.CustomerRepository.FindByCondition(c => c.Id == customerid).SingleOrDefault();
            if (customer == null) return false;

            var cart = GetCartByUserId(customerid);
            if (cart == null)
            {
                cart = new Cart { CustomerId = customerid, Customer = customer };
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

        public bool Checkout(int userId, CheckoutModel model)
        {
            var customer = _unitOfWork.CustomerRepository.FindByCondition(c => c.Id == userId).SingleOrDefault();
            if (customer == null) return false;

            var cart = _unitOfWork.CartRepository.FindByCondition(c => c.CustomerId == customer.Id).SingleOrDefault();
            if (cart == null) return false;

            var cartItems = _unitOfWork.CartItemRepository.FindByCondition(ci => ci.CartId == cart.Id).ToList();
            if (!cartItems.Any()) return false; // No items in cart to checkout

            // Create a new order
            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                TotalAmount = cartItems.Sum(ci => ci.Quantity * ci.Product.Price), // Assuming Product has a Price property
                ShippingAddress = model.ShippingAddress,
                BillingAddress = model.BillingAddress,
                PaymentMethod = model.PaymentMethod
            };

            _unitOfWork.OrderRepository.Insert(order);
            _unitOfWork.SaveChanges();

            // Move cart items to order details
            foreach (var item in cartItems)
            {
                var orderDetail = new OrderDetail
                {
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.Product.Price
                };

                _unitOfWork.OrderDetailRepository.Insert(orderDetail);
            }

            // Clear cart after checkout
            foreach (var item in cartItems)
            {
                _unitOfWork.CartItemRepository.Delete(item);
            }

            _unitOfWork.SaveChanges();

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
            return _mapper.Map<List<CartItemModel>>(cartItems);
        }

        public void DeletCart(int cartId)
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


