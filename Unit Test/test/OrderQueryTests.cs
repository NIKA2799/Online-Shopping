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
    public class OrderQueryTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly OrderQuery _orderQuery;

        public OrderQueryTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _orderQuery = new OrderQuery(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        [Fact]
        public void FindAll_ShouldReturnMappedOrders()
        {
            var orders = new List<Order>
                {
                    new Order
                    {
                        Id = 1,
                        ShippingAddress = "123 Test St",
                        BillingAddress = "123 Test St",
                        PaymentMethod = "Credit Card"
                    }
                };
            var mapped = new List<OrderModel> { new OrderModel { Id = 1 } };

            _unitOfWorkMock.Setup(u => u.OrderRepository.FindAll()).Returns(orders.AsQueryable());
            _mapperMock.Setup(m => m.Map<List<OrderModel>>(orders)).Returns(mapped);

            var result = _orderQuery.FindAll();

            Assert.Single(result);
            Assert.Equal(1, result.First().Id);
        }

        [Fact]
        public void FindByCondition_ShouldReturnFilteredOrders()
        {
            var orders = new List<Order>
                {
                    new Order
                    {
                        Id = 2,
                        ShippingAddress = "456 Test Ave",
                        BillingAddress = "456 Test Ave",
                        PaymentMethod = "PayPal"
                    }
                };
            var mapped = new List<OrderModel> { new OrderModel { Id = 2 } };

            _unitOfWorkMock.Setup(u => u.OrderRepository.FindByCondition(It.IsAny<Expression<Func<Order, bool>>>()))
                .Returns(orders.AsQueryable());
            _mapperMock.Setup(m => m.Map<List<OrderModel>>(orders)).Returns(mapped);

            var result = _orderQuery.FindByCondition(o => o.Id == 2);

            Assert.Single(result);
            Assert.Equal(2, result.First().Id);
        }

        [Fact]
        public void Get_ShouldReturnSingleOrder_WhenExists()
        {
            var order = new Order
            {
                Id = 11,
                ShippingAddress = "789 Test Blvd",
                BillingAddress = "789 Test Blvd",
                PaymentMethod = "Debit Card"
            };
            var mapped = new OrderModel { Id = 3 };

            _unitOfWorkMock.Setup(u => u.OrderRepository.FindByCondition(It.IsAny<Expression<Func<Order, bool>>>()))
                .Returns(new List<Order> { order }.AsQueryable());
            _mapperMock.Setup(m => m.Map<OrderModel>(order)).Returns(mapped);

            var result = _orderQuery.Get(3);

            Assert.Equal(3, result.Id);
        }

        [Fact]
        public void GetOrdersByUser_ShouldReturnUserOrders()
        {
            var orders = new List<Order>
                {
                    new Order
                    {
                        CustomerId = 4,
                        ShippingAddress = "101 Test Lane",
                        BillingAddress = "101 Test Lane",
                        PaymentMethod = "Bank Transfer"
                    }
                };
            var mapped = new List<OrderModel> { new OrderModel { CustomerId = 4 } };

            _unitOfWorkMock.Setup(u => u.OrderRepository.FindByCondition(It.IsAny<Expression<Func<Order, bool>>>()))
                .Returns(orders.AsQueryable());
            _mapperMock.Setup(m => m.Map<List<OrderModel>>(orders)).Returns(mapped);

            var result = _orderQuery.GetOrdersByUser(4);

            Assert.Single(result);
            Assert.Equal(4, result.First().CustomerId);
        }
    }
}