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
    public class CartServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly CartService _cartService;

        public CartServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _cartService = new CartService(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        [Fact]
        public void AddItemToCart_ShouldCreateCart_WhenCartDoesNotExist()
        {
            var customerId = 1;
            var productId = 2;
            var quantity = 3;

            _unitOfWorkMock.Setup(u => u.CartRepository.FindByCondition(It.IsAny<Expression<Func<Cart, bool>>>()))
                           .Returns(Enumerable.Empty<Cart>().AsQueryable());

            _unitOfWorkMock.Setup(u => u.CartRepository.Insert(It.IsAny<Cart>()));
            _unitOfWorkMock.Setup(u => u.CartItemRepository.Insert(It.IsAny<CartItem>()));

            var result = _cartService.AddItemToCart(customerId, productId, quantity);

            Assert.True(result);
            _unitOfWorkMock.Verify(u => u.CartRepository.Insert(It.IsAny<Cart>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChanges(), Times.Exactly(2));
        }

        [Fact]
        public void RemoveItemFromCart_ShouldReturnFalse_WhenCartNotFound()
        {
            _unitOfWorkMock.Setup(u => u.CartRepository.FindByCondition(It.IsAny<Expression<Func<Cart, bool>>>()))
                           .Returns(Enumerable.Empty<Cart>().AsQueryable());

            var result = _cartService.RemoveItemFromCart(1, 1);

            Assert.False(result);
        }

        [Fact]
        public void ClearCart_ShouldDeleteAllCartItems()
        {
            var cart = new Cart { Id = 1, CustomerId = 1 };
            var items = new List<CartItem> { new CartItem { Id = 1, CartId = 1 } };

            _unitOfWorkMock.Setup(u => u.CartRepository.FindByCondition(It.IsAny<Expression<Func<Cart, bool>>>()))
                           .Returns(new List<Cart> { cart }.AsQueryable());

            _unitOfWorkMock.Setup(u => u.CartItemRepository.FindByCondition(It.IsAny<Expression<Func<CartItem, bool>>>()))
                           .Returns(items.AsQueryable());

            var result = _cartService.ClearCart(1);

            Assert.True(result);
            _unitOfWorkMock.Verify(u => u.CartItemRepository.Delete(It.IsAny<CartItem>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChanges(), Times.Once);
        }

        [Fact]
        public void Checkout_ShouldReturnFalse_WhenCustomerNotFound()
        {
            _unitOfWorkMock.Setup(u => u.CustomerRepository.FindByCondition(It.IsAny<Expression<Func<User, bool>>>()))
                           .Returns(Enumerable.Empty<User>().AsQueryable());

            var result = _cartService.Checkout(1, new CheckoutModel());

            Assert.False(result);
        }

        [Fact]
        public void Checkout_ShouldReturnFalse_WhenCartIsEmpty()
        {
            var customer = new User { Id = 1, Name = "Test Customer" }; // Fixed CS9035 by setting the required 'Name' property.  

            _unitOfWorkMock.Setup(u => u.CustomerRepository.FindByCondition(It.IsAny<Expression<Func<User, bool>>>()))
                           .Returns(new List<User> { customer }.AsQueryable());

            _unitOfWorkMock.Setup(u => u.CartRepository.FindByCondition(It.IsAny<Expression<Func<Cart, bool>>>()))
                           .Returns(new List<Cart> { new Cart { Id = 1, CustomerId = 1 } }.AsQueryable());

            _unitOfWorkMock.Setup(u => u.CartItemRepository.FindByCondition(It.IsAny<Expression<Func<CartItem, bool>>>()))
                           .Returns(Enumerable.Empty<CartItem>().AsQueryable());

            var result = _cartService.Checkout(1, new CheckoutModel());

            Assert.False(result);
        }
    }
}