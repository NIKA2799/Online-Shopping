using AutoMapper;
using Dto;
using Interface.Command;
using Interface.IRepositories;
using Interface.Model;
using Microsoft.Extensions.Logging;


namespace Service.CommandService
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CartService> _logger;

        public CartService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<CartService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Retrieves the user's cart (with items) by userId, or null if not found.
        /// </summary>
        public CartModel GetCartByUserId(int userId)
        {
            var cart = _unitOfWork.CartRepository
                .FindByCondition(c => c.CustomerId == userId)
                .SingleOrDefault();

            if (cart == null)
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.

            // Map entity to DTO; sort items for user-friendliness
            var cartModel = _mapper.Map<CartModel>(cart);
            cart = new Cart
            {
                CustomerId = userId,
                Customer = new User { Id = userId },
                
            };
            cartModel.Items = _mapper.Map<IEnumerable<CartItemModel>>(
                cart.Items.OrderBy(ci => ci.Product.Name)
            );
            return cartModel;
        }

        /// <summary>
        /// Adds an item to the user's cart or increases quantity if the item already exists.
        /// </summary>
        public bool AddItemToCart(int userId, int productId, int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));

            // Retrieve or create the cart for the user
            var cart = _unitOfWork.CartRepository
                .FindByCondition(c => c.CustomerId == userId)
                .SingleOrDefault();

            if (cart == null)
            {
                cart = new Cart { CustomerId = userId };
                _unitOfWork.CartRepository.Insert(cart);
                _unitOfWork.SaveChanges(); // Assigns cart.Id
            }

            // Find existing item in the cart
            var existingItem = _unitOfWork.CartItemRepository
                .FindByCondition(ci => ci.CartId == cart.Id && ci.ProductId == productId)
                .SingleOrDefault();

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                _unitOfWork.CartItemRepository.Update(existingItem);
            }
            else
            {
                // Add as new item
                var newItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = productId,
                    Quantity = quantity
                };
                _unitOfWork.CartItemRepository.Insert(newItem);
            }

            _unitOfWork.SaveChanges();
            _logger.LogInformation(
                "User {UserId} added product {ProductId} (x{Quantity}) to cart {CartId}",
                userId, productId, quantity, cart.Id);
            return true;
        }

        /// <summary>
        /// Removes an item from user's cart.
        /// </summary>
        public bool RemoveItemFromCart(int userId, int cartItemId)
        {
            var cart = _unitOfWork.CartRepository
                .FindByCondition(c => c.CustomerId == userId)
                .SingleOrDefault();

            if (cart == null)
                return false;

            var item = _unitOfWork.CartItemRepository
                .FindByCondition(ci => ci.CartId == cart.Id && ci.Id == cartItemId)
                .SingleOrDefault();

            if (item == null)
                return false;

            _unitOfWork.CartItemRepository.Delete(item);
            _unitOfWork.SaveChanges();
            _logger.LogInformation("Removed cart item {CartItemId} from cart {CartId}", cartItemId, cart.Id);
            return true;
        }

        /// <summary>
        /// Updates the quantity for a specific cart item.
        /// </summary>
        public bool UpdateItemQuantity(int userId, int cartItemId, int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));

            var cart = _unitOfWork.CartRepository
                .FindByCondition(c => c.CustomerId == userId)
                .SingleOrDefault();

            if (cart == null)
                return false;

            var item = _unitOfWork.CartItemRepository
                .FindByCondition(ci => ci.CartId == cart.Id && ci.Id == cartItemId)
                .SingleOrDefault();

            if (item == null)
                return false;

            item.Quantity = quantity;
            _unitOfWork.CartItemRepository.Update(item);
            _unitOfWork.SaveChanges();
            _logger.LogInformation("Updated cart item {CartItemId} quantity to {Quantity}", cartItemId, quantity);
            return true;
        }

        /// <summary>
        /// Removes all items from user's cart.
        /// </summary>
        public bool ClearCart(int userId)
        {
            var cart = _unitOfWork.CartRepository
                .FindByCondition(c => c.CustomerId == userId)
                .SingleOrDefault();
            if (cart == null) return false;

            var items = _unitOfWork.CartItemRepository
                .FindByCondition(ci => ci.CartId == cart.Id)
                .ToList();

            foreach (var i in items)
                _unitOfWork.CartItemRepository.Delete(i);

            _unitOfWork.SaveChanges();
            _logger.LogInformation("Cleared cart {CartId} for user {UserId}", cart.Id, userId);
            return true;
        }

        /// <summary>
        /// Finalizes the order: creates an order, moves cart items to order details, and clears the cart.
        /// </summary>
        public bool Checkout(int userId, CheckoutModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var cart = _unitOfWork.CartRepository
                .FindByCondition(c => c.CustomerId == userId)
                .SingleOrDefault();
            if (cart == null)
                return false;

            var items = _unitOfWork.CartItemRepository
                .FindByCondition(ci => ci.CartId == cart.Id)
                .ToList();
            if (!items.Any())
                return false;

            // Calculate total amount from cart items
            var total = items.Sum(ci => ci.Quantity * ci.Product.Price);

            // Create order
#pragma warning disable CS8601 // Possible null reference assignment.
            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                TotalAmount = total,
                ShippingAddress = model.ShippingAddress,
                BillingAddress = model.BillingAddress,
                PaymentMethod = model.PaymentMethod
            };
#pragma warning restore CS8601 // Possible null reference assignment.
            _unitOfWork.OrderRepository.Insert(order);
            _unitOfWork.SaveChanges(); // Required for order.Id

            // Copy cart items to order details
            foreach (var ci in items)
            {
                var detail = new OrderDetail
                {
                    OrderId = order.Id,
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    UnitPrice = ci.Product.Price
                };
                _unitOfWork.OrderDetailRepository.Insert(detail);
            }

            // Cart is emptied after checkout
            ClearCart(userId);
            _logger.LogInformation(
                "User {UserId} checked out cart {CartId} into order {OrderId}",
                userId, cart.Id, order.Id);
            return true;
        }

        /// <summary>
        /// Gets cart total price for a specific user (helper for displaying summary).
        /// </summary>
        public decimal GetCartTotal(int userId)
        {
            var cart = _unitOfWork.CartRepository
                .FindByCondition(c => c.CustomerId == userId)
                .SingleOrDefault();
            if (cart == null) return 0;

            var items = _unitOfWork.CartItemRepository
                .FindByCondition(ci => ci.CartId == cart.Id)
                .ToList();

            return items.Sum(i => i.Quantity * i.Product.Price);
        }
    }
}