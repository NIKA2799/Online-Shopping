using AutoMapper;
using Dto;
using Interface.IRepositories;
using Interface.Model;
using Moq;
using Service.CommandService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Unit_Test.test
{
    public class OrderCommandTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly OrderCommand _orderCommand;

        public OrderCommandTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _orderCommand = new OrderCommand(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        [Fact]
        public void CancelOrder_ShouldSetStatusToCancelled_AndRestoreStock()
        {
            var orderId = 1;
            var order = new Order
            {
                Id = orderId,
                Status = OrderStatus.Pending,
                ShippingAddress = "123 Test St",
                BillingAddress = "123 Test St",
                PaymentMethod = "Credit Card"
            };
            var orderDetails = new List<OrderDetail>
            {
                new OrderDetail { OrderId = orderId, ProductId = 2, Quantity = 3 }
            };
            var product = new Product
            {
                Id = 2,
                Stock = 5,
                IsOutOfStock = false,
                Name = "Test Product", // Fix for CS9035: Required member 'Product.Name' must be set
                Description = "Test Description", // Fix for CS9035: Required member 'Product.Description' must be set
                Price = 10.99m, // Fix for CS9035: Required member 'Product.Price' must be set
                ImageUrl = "http://example.com/image.jpg" // Fix for CS9035: Required member 'Product.ImageUrl' must be set
            };

            var orderRepoMock = new Mock<IOrderRepository>();
            orderRepoMock.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Order, bool>>>()))
                .Returns(new List<Order> { order }.AsQueryable());

            var orderDetailRepoMock = new Mock<IOrderDetailRepository>();
            orderDetailRepoMock.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<OrderDetail, bool>>>()))
                .Returns(orderDetails.AsQueryable());

            var productRepoMock = new Mock<IProductRepository>();
            productRepoMock.Setup(r => r.GetById(2)).Returns(product);

            _unitOfWorkMock.SetupGet(u => u.OrderRepository).Returns(orderRepoMock.Object);
            _unitOfWorkMock.SetupGet(u => u.OrderDetailRepository).Returns(orderDetailRepoMock.Object);
            _unitOfWorkMock.SetupGet(u => u.ProductRepository).Returns(productRepoMock.Object);

            _orderCommand.CancelOrder(orderId);

            Assert.Equal(OrderStatus.Cancelled, order.Status);
            Assert.Equal(8, product.Stock);
            productRepoMock.Verify(r => r.Update(product), Times.Once);
            orderRepoMock.Verify(r => r.Update(order), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChanges(), Times.Once);
        }

        [Fact]
        public void Insert_ShouldMapAndInsertOrder()
        {
            var model = new OrderModel { CustomerId = 1 };
            var order = new Order
            {
                Id = 10,
                ShippingAddress = "123 Test St", // Fix for CS9035: Required member 'Order.ShippingAddress' must be set
                BillingAddress = "123 Test St", // Fix for CS9035: Required member 'Order.BillingAddress' must be set
                PaymentMethod = "Credit Card"   // Fix for CS9035: Required member 'Order.PaymentMethod' must be set
            };

            _mapperMock.Setup(m => m.Map<Order>(model)).Returns(order);
            var orderRepoMock = new Mock<IOrderRepository>();
            _unitOfWorkMock.SetupGet(u => u.OrderRepository).Returns(orderRepoMock.Object);

            var result = _orderCommand.Insert(model);

            orderRepoMock.Verify(r => r.Insert(order), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChanges(), Times.Once);
            Assert.Equal(order.Id, result);
        }

        [Fact]
        public void Update_ShouldUpdateOrder_WhenOrderExists()
        {
            var id = 1;
            var model = new OrderModel { Id = id };
            var existingOrder = new Order
            {
                Id = id,
                ShippingAddress = "123 Test St", // Fix for CS9035: Required member 'Order.ShippingAddress' must be set
                BillingAddress = "123 Test St", // Fix for CS9035: Required member 'Order.BillingAddress' must be set
                PaymentMethod = "Credit Card"   // Fix for CS9035: Required member 'Order.PaymentMethod' must be set
            };
            var updatedOrder = new Order
            {
                Id = id,
                ShippingAddress = "123 Test St", // Fix for CS9035: Required member 'Order.ShippingAddress' must be set
                BillingAddress = "123 Test St", // Fix for CS9035: Required member 'Order.BillingAddress' must be set
                PaymentMethod = "Credit Card"   // Fix for CS9035: Required member 'Order.PaymentMethod' must be set
            };

            var orderRepoMock = new Mock<IOrderRepository>();
            orderRepoMock.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Order, bool>>>()))
                .Returns(new List<Order> { existingOrder }.AsQueryable());

            _unitOfWorkMock.SetupGet(u => u.OrderRepository).Returns(orderRepoMock.Object);
            _mapperMock.Setup(m => m.Map<Order>(model)).Returns(updatedOrder);

            _orderCommand.Update(id, model);

            orderRepoMock.Verify(r => r.Update(updatedOrder), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChanges(), Times.Once);
        }

        [Fact]
        public void UpdateOrderStatus_ShouldUpdateStatus_WhenOrderExists()
        {
            var id = 1;
            var order = new Order
            {
                Id = id,
                Status = OrderStatus.Pending,
                ShippingAddress = "123 Test St", // Fix for CS9035: Required member 'Order.ShippingAddress' must be set
                BillingAddress = "123 Test St", // Fix for CS9035: Required member 'Order.BillingAddress' must be set
                PaymentMethod = "Credit Card"   // Fix for CS9035: Required member 'Order.PaymentMethod' must be set
            };

            var orderRepoMock = new Mock<IOrderRepository>();
            orderRepoMock.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Order, bool>>>()))
                .Returns(new List<Order> { order }.AsQueryable());

            _unitOfWorkMock.SetupGet(u => u.OrderRepository).Returns(orderRepoMock.Object);

            _orderCommand.UpdateOrderStatus(id, OrderStatus.Shipped);

            Assert.Equal(OrderStatus.Shipped, order.Status);
            orderRepoMock.Verify(r => r.Update(order), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChanges(), Times.Once);
        }

        [Fact]
        public void Checkout_ShouldThrow_WhenCartIsEmpty()
        {
            var model = new CheckoutModel { Customerid = 1 };
            var cartRepoMock = new Mock<ICartRepository>();
            cartRepoMock.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Cart, bool>>>()))
                .Returns(new List<Cart>().AsQueryable());

            _unitOfWorkMock.SetupGet(u => u.CartRepository).Returns(cartRepoMock.Object);

            Assert.Throws<InvalidOperationException>(() => _orderCommand.Checkout(model));
        }

        [Fact]
        public void TrackOrderStatus_ShouldReturnStatus_WhenOrderExists()
        {
            var orderId = 1;
            var userId = 2;
            var order = new Order
            {
                Id = orderId,
                UserId = userId,
                Status = OrderStatus.Processing,
                ShippingAddress = "123 Test St", // Fix for CS9035: Required member 'Order.ShippingAddress' must be set  
                BillingAddress = "123 Test St", // Fix for CS9035: Required member 'Order.BillingAddress' must be set  
                PaymentMethod = "Credit Card"   // Fix for CS9035: Required member 'Order.PaymentMethod' must be set  
            };

            var orderRepoMock = new Mock<IOrderRepository>();
            orderRepoMock.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Order, bool>>>()))
                .Returns(new List<Order> { order }.AsQueryable());

            _unitOfWorkMock.SetupGet(u => u.OrderRepository).Returns(orderRepoMock.Object);

            var status = _orderCommand.TrackOrderStatus(orderId, userId);

            Assert.Equal(OrderStatus.Processing, status);
        }
    }
}
