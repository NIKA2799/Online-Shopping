using AutoMapper;
using Dto;
using Interface.IRepositories;
using Interface.Model;
using Moq;
using Service.QueriesService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Unit_Test.test
{
    public class OrderQueryServiceTests
    {
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly OrderQuery _svc;

        public OrderQueryServiceTests()
        {
            _uowMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _svc = new OrderQuery(_uowMock.Object, _mapperMock.Object);
        }

        [Fact]
        public void FindAll_ShouldReturnMappedList()
        {
            // Arrange
            var orders = new List<Order>
                {
                    new Order { Id = 1, CustomerId = 10, ShippingAddress = "Address1", BillingAddress = "Address1", PaymentMethod = "CreditCard" },
                    new Order { Id = 2, CustomerId = 20, ShippingAddress = "Address2", BillingAddress = "Address2", PaymentMethod = "PayPal" }
                }.AsQueryable();

            var orderModels = new List<OrderModel>
                {
                    new OrderModel { Id = 1 },
                    new OrderModel { Id = 2 }
                };

            var repo = new Mock<IOrderRepository>();
            repo.Setup(r => r.FindAll()).Returns(orders);
            _uowMock.Setup(u => u.OrderRepository).Returns(repo.Object);

            _mapperMock
               .Setup(m => m.Map<IEnumerable<OrderModel>>(It.IsAny<IEnumerable<Order>>()))
               .Returns(orderModels);

            // Act
            var result = _svc.FindAll().ToList();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal(1, result[0].Id);
            Assert.Equal(2, result[1].Id);
            repo.Verify(r => r.FindAll(), Times.Once);
            _mapperMock.Verify(m => m.Map<IEnumerable<OrderModel>>(orders), Times.Once);
        }

        [Fact]
        public void FindByCondition_ShouldFilterAndMap()
        {
            // Arrange
            var orders = new List<Order>
                {
                    new Order { Id = 1, CustomerId = 10, ShippingAddress = "Address1", BillingAddress = "Address1", PaymentMethod = "CreditCard" },
                    new Order { Id = 2, CustomerId = 20, ShippingAddress = "Address2", BillingAddress = "Address2", PaymentMethod = "PayPal" }
                }.AsQueryable();

            var filteredModels = new List<OrderModel>
                {
                    new OrderModel { Id = 2 }
                };

            var repo = new Mock<IOrderRepository>();
            repo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Order, bool>>>()))
                .Returns(orders.Where(o => o.CustomerId == 20).AsQueryable());
            _uowMock.Setup(u => u.OrderRepository).Returns(repo.Object);

            _mapperMock
               .Setup(m => m.Map<IEnumerable<OrderModel>>(It.IsAny<IEnumerable<Order>>()))
               .Returns(filteredModels);

            // Act
            var result = _svc.FindByCondition(o => o.CustomerId == 20).ToList();

            // Assert
            Assert.Single(result);
            Assert.Equal(2, result[0].Id);
            repo.Verify(r => r.FindByCondition(It.IsAny<Expression<Func<Order, bool>>>()), Times.Once);
            _mapperMock.Verify(m => m.Map<IEnumerable<OrderModel>>(It.IsAny<IEnumerable<Order>>()), Times.Once);
        }

        [Fact]
        public void Get_ShouldReturnSingleMappedModel()
        {
            // Arrange
            var order = new Order
            {
                Id = 5,
                ShippingAddress = "123 Test St",
                BillingAddress = "123 Test St",
                PaymentMethod = "CreditCard"
            };
            var repo = new Mock<IOrderRepository>();
            repo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Order, bool>>>()))
                .Returns(new[] { order }.AsQueryable());
            _uowMock.Setup(u => u.OrderRepository).Returns(repo.Object);

            var model = new OrderModel { Id = 5 };
            _mapperMock.Setup(m => m.Map<OrderModel>(order)).Returns(model);

            // Act
            var result = _svc.Get(5);

            // Assert
            Assert.Equal(5, result.Id);
            repo.Verify(r => r.FindByCondition(It.IsAny<Expression<Func<Order, bool>>>()), Times.Once);
            _mapperMock.Verify(m => m.Map<OrderModel>(order), Times.Once);
        }

        [Fact]
        public void GetOrdersByUser_ShouldReturnAllForThatUser()
        {
            // Arrange
            var orders = new List<Order>
                {
                    new Order { Id = 1, CustomerId = 99, ShippingAddress = "Address1", BillingAddress = "Address1", PaymentMethod = "CreditCard" },
                    new Order { Id = 2, CustomerId = 99, ShippingAddress = "Address2", BillingAddress = "Address2", PaymentMethod = "PayPal" },
                    new Order { Id = 3, CustomerId = 100, ShippingAddress = "Address3", BillingAddress = "Address3", PaymentMethod = "DebitCard" }
                }.AsQueryable();

            var userModels = new List<OrderModel>
                {
                    new OrderModel { Id = 1 },
                    new OrderModel { Id = 2 }
                };

            var repo = new Mock<IOrderRepository>();
            repo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Order, bool>>>()))
                .Returns((Expression<Func<Order, bool>> pred) => orders.Where(pred.Compile()).AsQueryable());
            _uowMock.Setup(u => u.OrderRepository).Returns(repo.Object);

            _mapperMock
               .Setup(m => m.Map<IEnumerable<OrderModel>>(It.IsAny<IEnumerable<Order>>()))
               .Returns(userModels);

            // Act
            var result = _svc.GetOrdersByUser(99).ToList();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.All(result, m => Assert.Contains(m.Id, new[] { 1, 2 }));
            repo.Verify(r => r.FindByCondition(It.IsAny<Expression<Func<Order, bool>>>()), Times.Once);
            _mapperMock.Verify(m => m.Map<IEnumerable<OrderModel>>(It.IsAny<IEnumerable<Order>>()), Times.Once);
        }
    }
}