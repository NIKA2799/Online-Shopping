using AutoMapper;
using Dto;
using Interface;
using Interface.Command;
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
    public class OrderCommandServiceTests
    {
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IDiscountService> _discountMock;
        private readonly Mock<IAuditService> _auditMock;
        private readonly OrderCommandService _svc;

        public OrderCommandServiceTests()
        {
            _uowMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _discountMock = new Mock<IDiscountService>();
            _auditMock = new Mock<IAuditService>();

            _svc = new OrderCommandService(
                _uowMock.Object,
                _mapperMock.Object,
                _discountMock.Object,
                _auditMock.Object
            );
        }

        [Fact]
        public void Insert_ShouldMapPersistAndLog()
        {
            var model = new OrderModel();
            var order = new Order
            {
                Id = 99,
                ShippingAddress = "Default Shipping Address",
                BillingAddress = "Default Billing Address",
                PaymentMethod = "Default Payment Method"
            };
            _mapperMock.Setup(m => m.Map<Order>(model)).Returns(order);
            var repo = new Mock<IOrderRepository>();
            _uowMock.SetupGet(u => u.OrderRepository).Returns(repo.Object);

            var result = _svc.Insert(model);

            Assert.Equal(99, result);
            repo.Verify(r => r.Insert(order), Times.Once);
            _uowMock.Verify(u => u.SaveChanges(), Times.Once);
            _auditMock.Verify(a => a.Log(
                It.IsAny<string>(),
                nameof(Order),
                "99",
                "Created order"
            ), Times.Once);
        }

        [Fact]
        public void Update_ShouldReturnTrue_WhenOrderExists()
        {
            var existing = new Order
            {
                Id = 5,
                ShippingAddress = "Default Shipping Address",
                BillingAddress = "Default Billing Address",
                PaymentMethod = "Default Payment Method"
            };
            var repo = new Mock<IOrderRepository>();
            repo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Order, bool>>>()))
                .Returns(new[] { existing }.AsQueryable());
            _uowMock.SetupGet(u => u.OrderRepository).Returns(repo.Object);

            var ok = _svc.Update(5, new OrderModel());

            Assert.True(ok);
            _mapperMock.Verify(m => m.Map(It.IsAny<OrderModel>(), existing), Times.Once);
            repo.Verify(r => r.Update(existing), Times.Once);
            _uowMock.Verify(u => u.SaveChanges(), Times.Once);
            _auditMock.Verify(a => a.Log(
                It.IsAny<string>(),
                nameof(Order),
                "5",
                "Updated order"
            ), Times.Once);
        }

        [Fact]
        public void Update_ShouldReturnFalse_WhenOrderMissing()
        {
            var repo = new Mock<IOrderRepository>();
            repo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Order, bool>>>()))
                .Returns(Enumerable.Empty<Order>().AsQueryable());
            _uowMock.SetupGet(u => u.OrderRepository).Returns(repo.Object);

            var ok = _svc.Update(123, new OrderModel());

            Assert.False(ok);
            repo.Verify(r => r.Update(It.IsAny<Order>()), Times.Never);
            _uowMock.Verify(u => u.SaveChanges(), Times.Never);
        }

        [Fact]
        public void Delete_ShouldReturnTrue_WhenOrderExists()
        {
            var existing = new Order
            {
                Id = 7,
                ShippingAddress = "Default Shipping Address",
                BillingAddress = "Default Billing Address",
                PaymentMethod = "Default Payment Method"
            };
            var repo = new Mock<IOrderRepository>();
            repo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Order, bool>>>()))
                .Returns(new[] { existing }.AsQueryable());
            _uowMock.SetupGet(u => u.OrderRepository).Returns(repo.Object);

            var ok = _svc.Delete(7);

            Assert.True(ok);
            repo.Verify(r => r.Delete(existing), Times.Once);
            _uowMock.Verify(u => u.SaveChanges(), Times.Once);
            _auditMock.Verify(a => a.Log(
                It.IsAny<string>(),
                nameof(Order),
                "7",
                "Deleted order"
            ), Times.Once);
        }

        [Fact]
        public void Delete_ShouldReturnFalse_WhenMissing()
        {
            var repo = new Mock<IOrderRepository>();
            repo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Order, bool>>>()))
                .Returns(Enumerable.Empty<Order>().AsQueryable());
            _uowMock.SetupGet(u => u.OrderRepository).Returns(repo.Object);

            var ok = _svc.Delete(55);

            Assert.False(ok);
        }

        [Fact]
        public void UpdateOrderStatus_ShouldReturnTrue_WhenOrderExists()
        {
            var existing = new Order
            {
                Id = 3,
                Status = OrderStatus.Pending,
                ShippingAddress = "Default Shipping Address",
                BillingAddress = "Default Billing Address",
                PaymentMethod = "Default Payment Method"
            };
            var repo = new Mock<IOrderRepository>();
            repo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Order, bool>>>()))
                .Returns(new[] { existing }.AsQueryable());
            _uowMock.SetupGet(u => u.OrderRepository).Returns(repo.Object);

            var ok = _svc.UpdateOrderStatus(3, OrderStatus.Shipped);

            Assert.True(ok);
            Assert.Equal(OrderStatus.Shipped, existing.Status);
            repo.Verify(r => r.Update(existing), Times.Once);
            _uowMock.Verify(u => u.SaveChanges(), Times.Once);
            _auditMock.Verify(a => a.Log(
                It.IsAny<string>(),
                nameof(Order),
                "3",
                "Order status changed to Shipped"
            ), Times.Once);
        }

        [Fact]
        public void UpdateOrderStatus_ShouldReturnFalse_WhenMissing()
        {
            var repo = new Mock<IOrderRepository>();
            repo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Order, bool>>>()))
                .Returns(Enumerable.Empty<Order>().AsQueryable());
            _uowMock.SetupGet(u => u.OrderRepository).Returns(repo.Object);

            var ok = _svc.UpdateOrderStatus(999, OrderStatus.Cancelled);

            Assert.False(ok);
        }

        [Fact]
        public void TrackOrderStatus_ShouldReturnStatus_WhenOwned()
        {
            var existing = new Order
            {
                Id = 8,
                CustomerId = 2,
                Status = OrderStatus.Processing,
                ShippingAddress = "Default Shipping Address",
                BillingAddress = "Default Billing Address",
                PaymentMethod = "Default Payment Method"
            };
            var repo = new Mock<IOrderRepository>();
            repo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Order, bool>>>()))
                .Returns(new[] { existing }.AsQueryable());
            _uowMock.SetupGet(u => u.OrderRepository).Returns(repo.Object);

            var status = _svc.TrackOrderStatus(8, 2);

            Assert.Equal(OrderStatus.Processing, status);
        }

        [Fact]
        public void TrackOrderStatus_ShouldReturnNull_WhenMissingOrWrongOwner()
        {
            var repo = new Mock<IOrderRepository>();
            repo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Order, bool>>>()))
                .Returns(Enumerable.Empty<Order>().AsQueryable());
            _uowMock.SetupGet(u => u.OrderRepository).Returns(repo.Object);

            Assert.Null(_svc.TrackOrderStatus(1, 99));
        }

        [Fact]
        public void CancelOrder_ShouldReturnFalse_WhenNotFoundOrAlreadyCancelled()
        {
            var repo = new Mock<IOrderRepository>();
            repo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Order, bool>>>()))
                .Returns(Enumerable.Empty<Order>().AsQueryable());
            _uowMock.SetupGet(u => u.OrderRepository).Returns(repo.Object);

            var ok = _svc.CancelOrder(5, 5);
            Assert.False(ok);
        }

        [Fact]
        public void CancelOrder_ShouldRestockAndLog_WhenValid()
        {
            var orderId = 15;
            var custId = 42;
            var order = new Order
            {
                Id = orderId,
                CustomerId = custId,
                Status = OrderStatus.Pending,
                ShippingAddress = "Default Shipping Address",
                BillingAddress = "Default Billing Address",
                PaymentMethod = "Default Payment Method"
            };
            var detail = new OrderDetail { OrderId = orderId, ProductId = 1, Quantity = 2 };
            var prod = new Product
            {
                Id = 1,
                Name = "Sample Product", // Fix for CS9035: Required member 'Product.Name'
                Description = "Sample Description", // Fix for CS9035: Required member 'Product.Description'
                Price = 10.0m, // Fix for CS9035: Required member 'Product.Price'
                Stock = 5,
                ImageUrl = "http://example.com/image.jpg" // Fix for CS9035: Required member 'Product.ImageUrl'
            };

            var oRepo = new Mock<IOrderRepository>();
            oRepo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Order, bool>>>()))
                 .Returns(new[] { order }.AsQueryable());
            var odRepo = new Mock<IOrderDetailRepository>();
            odRepo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<OrderDetail, bool>>>()))
                  .Returns(new[] { detail }.AsQueryable());
            var pRepo = new Mock<IProductRepository>();
            pRepo.Setup(r => r.GetById(1)).Returns(prod);

            _uowMock.SetupGet(u => u.OrderRepository).Returns(oRepo.Object);
            _uowMock.SetupGet(u => u.OrderDetailRepository).Returns(odRepo.Object);
            _uowMock.SetupGet(u => u.ProductRepository).Returns(pRepo.Object);

            var ok = _svc.CancelOrder(orderId, custId);

            Assert.True(ok);
            Assert.Equal(7, prod.Stock);
            oRepo.Verify(r => r.Update(order), Times.Once);
            pRepo.Verify(r => r.Update(prod), Times.Once);
            _auditMock.Verify(a => a.Log(
                It.IsAny<string>(),
                nameof(Order),
                orderId.ToString(),
                "Cancelled order"
            ), Times.Once);
        }

        [Fact]
        public void Checkout_ShouldThrow_WhenCartEmpty()
        {
            var cartRepo = new Mock<ICartRepository>();
            cartRepo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Cart, bool>>>()))
                    .Returns(Enumerable.Empty<Cart>().AsQueryable());
            _uowMock.SetupGet(u => u.CartRepository).Returns(cartRepo.Object);

            var model = new CheckoutModel
            {
                Customerid = 1,
                ShippingAddress = "S",
                BillingAddress = "B",
                PaymentMethod = "P",
                DiscountCode = null
            };

            Assert.Throws<InvalidOperationException>(() => _svc.Checkout(model));
        }

        [Fact]
        public void Checkout_ShouldProcessOrder_WithoutDiscount()
        {
            var customerId = 3;
            var prod = new Product
            {
                Id = 2,
                Name = "Sample Product", // Fix for CS9035: Required member 'Product.Name'  
                Description = "Sample Description", // Fix for CS9035: Required member 'Product.Description'  
                Price = 20m,
                Stock = 4,
                ImageUrl = "http://example.com/image.jpg" // Fix for CS9035: Required member 'Product.ImageUrl'  
            };
            var item = new CartItem { CartId = 9, ProductId = 2, Quantity = 2, Product = prod };
            var cart = new Cart { Id = 9, CustomerId = customerId, Items = new List<CartItem> { item } };

            var cartRepo = new Mock<ICartRepository>();
            cartRepo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Cart, bool>>>()))
                    .Returns(new[] { cart }.AsQueryable());
            _uowMock.SetupGet(u => u.CartRepository).Returns(cartRepo.Object);

            var orderRepo = new Mock<IOrderRepository>();
            var detailRepo = new Mock<IOrderDetailRepository>();
            var cartItemRepo = new Mock<ICartItemRepository>();
            var pRepo = new Mock<IProductRepository>();

            _uowMock.SetupGet(u => u.OrderRepository).Returns(orderRepo.Object);
            _uowMock.SetupGet(u => u.OrderDetailRepository).Returns(detailRepo.Object);
            _uowMock.SetupGet(u => u.CartItemRepository).Returns(cartItemRepo.Object);
            _uowMock.SetupGet(u => u.ProductRepository).Returns(pRepo.Object);

            _discountMock.Setup(d => d.IsValid(It.IsAny<string>())).Returns(false);

            var model = new CheckoutModel
            {
                Customerid = customerId,
                ShippingAddress = "S",
                BillingAddress = "B",
                PaymentMethod = "P",
                DiscountCode = null
            };

            var newId = _svc.Checkout(model);

            orderRepo.Verify(r => r.Insert(It.Is<Order>(o =>
                o.CustomerId == customerId && o.TotalAmount == 40m
            )), Times.Once);

            detailRepo.Verify(r => r.Insert(It.IsAny<OrderDetail>()), Times.Once);
            cartItemRepo.Verify(r => r.DeleteRange(cart.Items), Times.Once);
            Assert.Equal(2, prod.Stock);
            _uowMock.Verify(u => u.SaveChanges(), Times.Once);
            _auditMock.Verify(a => a.Log(
                It.IsAny<string>(),
                nameof(Order),
                newId.ToString(),
                It.Is<string>(s => s.Contains("checked out"))
            ), Times.Once);
        }

        [Fact]
        public void Checkout_ShouldProcessOrder_WithDiscount()
        {
            var customerId = 4;
            var prod = new Product
            {
                Id = 5,
                Name = "Sample Product", // Fix for CS9035: Required member 'Product.Name'  
                Description = "Sample Description", // Fix for CS9035: Required member 'Product.Description'  
                Price = 100m,
                Stock = 3,
                ImageUrl = "http://example.com/image.jpg" // Fix for CS9035: Required member 'Product.ImageUrl'  
            };
            var item = new CartItem { CartId = 11, ProductId = 5, Quantity = 1, Product = prod };
            var cart = new Cart { Id = 11, CustomerId = customerId, Items = new List<CartItem> { item } };

            var cartRepo = new Mock<ICartRepository>();
            cartRepo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Cart, bool>>>()))
                    .Returns(new[] { cart }.AsQueryable());
            _uowMock.SetupGet(u => u.CartRepository).Returns(cartRepo.Object);

            var orderRepo = new Mock<IOrderRepository>();
            var detailRepo = new Mock<IOrderDetailRepository>();
            var cartItemRepo = new Mock<ICartItemRepository>();
            var pRepo = new Mock<IProductRepository>();

            _uowMock.SetupGet(u => u.OrderRepository).Returns(orderRepo.Object);
            _uowMock.SetupGet(u => u.OrderDetailRepository).Returns(detailRepo.Object);
            _uowMock.SetupGet(u => u.CartItemRepository).Returns(cartItemRepo.Object);
            _uowMock.SetupGet(u => u.ProductRepository).Returns(pRepo.Object);

            _discountMock.Setup(d => d.IsValid("SAVE10")).Returns(true);
            _discountMock.Setup(d => d.ApplyDiscount("SAVE10", 100m)).Returns(90m);

            var model = new CheckoutModel
            {
                Customerid = customerId,
                ShippingAddress = "S",
                BillingAddress = "B",
                PaymentMethod = "P",
                DiscountCode = "SAVE10"
            };

            var newId = _svc.Checkout(model);

            orderRepo.Verify(r => r.Insert(It.Is<Order>(o => o.TotalAmount == 90m)), Times.Once);
            detailRepo.Verify(r => r.Insert(It.IsAny<OrderDetail>()), Times.Once);
            cartItemRepo.Verify(r => r.DeleteRange(cart.Items), Times.Once);
            Assert.Equal(2, prod.Stock);
            _uowMock.Verify(u => u.SaveChanges(), Times.Once);
            _auditMock.Verify(a => a.Log(
                It.IsAny<string>(),
                nameof(Order),
                newId.ToString(),
                It.Is<string>(s => s.Contains("Applied discount 'SAVE10'"))
            ), Times.Once);
        }
    }
}