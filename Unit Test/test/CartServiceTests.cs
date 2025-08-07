using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using Moq;
using Xunit;
using Microsoft.Extensions.Logging;
using Dto;
using Interface.IRepositories;
using Interface.Model;
using Service.CommandService;

// namespaces -> შენი პროექტის შესაბამისი
// using Domain.Entities;
// using Domain.Enums;
// using Interface;
// using Interface.Repositories;
// using Interface.Model;

public class CartServiceTests
{
    private readonly Mock<IUnitOfWork> _uow;
    private readonly Mock<IMapper> _mapper;
    private readonly Mock<ILogger<CartService>> _logger;

    // Repo mocks
    private readonly Mock<ICartRepository> _cartRepo;
    private readonly Mock<ICartItemRepository> _cartItemRepo;
    private readonly Mock<IOrderRepository> _orderRepo;
    private readonly Mock<IOrderDetailRepository> _orderDetailRepo;
    private readonly Mock<IProductRepository> _productRepo; // საჭირო იქნება ფასებისთვის (Price)

    private readonly CartService _sut;

    public CartServiceTests()
    {
        _uow = new Mock<IUnitOfWork>();
        _mapper = new Mock<IMapper>();
        _logger = new Mock<ILogger<CartService>>();

        _cartRepo = new Mock<ICartRepository>();
        _cartItemRepo = new Mock<ICartItemRepository>();
        _orderRepo = new Mock<IOrderRepository>();
        _orderDetailRepo = new Mock<IOrderDetailRepository>();
        _productRepo = new Mock<IProductRepository>();

        // Bind repos to UoW
        _uow.SetupGet(x => x.CartRepository).Returns(_cartRepo.Object);
        _uow.SetupGet(x => x.CartItemRepository).Returns(_cartItemRepo.Object);
        _uow.SetupGet(x => x.OrderRepository).Returns(_orderRepo.Object);
        _uow.SetupGet(x => x.OrderDetailRepository).Returns(_orderDetailRepo.Object);
        _uow.SetupGet(x => x.ProductRepository).Returns(_productRepo.Object);

        _uow.Verify(x => x.SaveChanges(), Times.Once);

        _sut = new CartService(_uow.Object, _mapper.Object, _logger.Object);
    }

    // ========== GetCartByUserId ==========
    [Fact]
    public void GetCartByUserId_ShouldReturnNull_WhenCartNotFound()
    {
        _cartRepo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Cart, bool>>>()))
                 .Returns(Enumerable.Empty<Cart>().AsQueryable());

        var result = _sut.GetCartByUserId(1);

        Assert.Null(result);
    }

    [Fact]
    public void GetCartByUserId_ShouldMapAndOrderItems_ByProductName()
    {
        var cart = new Cart
        {
            Id = 10,
            CustomerId = 1,
            Items = new List<CartItem>
            {
                new CartItem { Id = 3, ProductId = 2, Quantity = 1, Product = new Product { Id = 2, Name = "Zzz", Price = 5 , Description="წეწეწე", ImageUrl="http://example.com/image.jpg", Stock=30} },
                new CartItem { Id = 2, ProductId = 1, Quantity = 1, Product = new Product { Id = 1, Name = "Aaa", Price = 3 , Description="შოკოლადი", ImageUrl="http://example.com/image.jpg", Stock=30} },
            }
        };

        _cartRepo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Cart, bool>>>()))
                 .Returns(new[] { cart }.AsQueryable());

        // Map Cart -> CartModel
        var mapped = new CartModel { Id = cart.Id, CustomerId = cart.CustomerId };
        _mapper.Setup(m => m.Map<CartModel>(cart)).Returns(mapped);

        // Map ordered items -> CartItemModel
        _mapper.Setup(m => m.Map<IEnumerable<CartItemModel>>(It.IsAny<IEnumerable<CartItem>>()))
               .Returns<IEnumerable<CartItem>>(cis => cis.Select(ci => new CartItemModel
               {
                   Id = ci.Id,
                   ProductId = ci.ProductId,
                   Quantity = ci.Quantity
               }));

        var result = _sut.GetCartByUserId(1);

        Assert.NotNull(result);
        Assert.Equal(10, result.Id);
        // ივენთუალური შემოწმება: პირველი უნდა იყოს Aaa-ს პროდუქტი (Id 2 CartItem-იდან)
        Assert.Equal(new[] { 2, 3 }, result.Items.Select(i => i.Id)); // expecting Id order: 2 (Aaa), 3 (Zzz)
    }

    // ========== AddItemToCart ==========
    [Fact]
    public void AddItemToCart_ShouldThrow_WhenQuantityIsZeroOrLess()
    {
        Assert.Throws<ArgumentException>(() => _sut.AddItemToCart(1, 2, 0));
        Assert.Throws<ArgumentException>(() => _sut.AddItemToCart(1, 2, -1));
    }

    [Fact]
    public void AddItemToCart_ShouldCreateCart_WhenCartDoesNotExist()
    {
        _cartRepo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Cart, bool>>>()))
                 .Returns(Enumerable.Empty<Cart>().AsQueryable());

        // ჩაიწეროს ახალი კალათა; შეგვიძლია დავაფიქსიროთ რომ შენახვის შემდეგ ეძლევა Id
        _cartRepo.Setup(r => r.Insert(It.IsAny<Cart>()))
                 .Callback<Cart>(c => c.Id = 99);

        _cartItemRepo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<CartItem, bool>>>()))
                     .Returns(Enumerable.Empty<CartItem>().AsQueryable());

        var ok = _sut.AddItemToCart(1, 2, 3);

        Assert.True(ok);
        _cartRepo.Verify(r => r.Insert(It.IsAny<Cart>()), Times.Once);
        _cartItemRepo.Verify(r => r.Insert(It.Is<CartItem>(ci =>
            ci.CartId == 99 && ci.ProductId == 2 && ci.Quantity == 3)), Times.Once);
        _uow.Verify(u => u.SaveChanges(), Times.Exactly(2)); // ერთხელ კალათაზე, ერთხელ item-ზე
    }

    [Fact]
    public void AddItemToCart_ShouldIncreaseQuantity_WhenItemExists()
    {
        var cart = new Cart { Id = 10, CustomerId = 1 };
        var existing = new CartItem { Id = 5, CartId = 10, ProductId = 2, Quantity = 4 };

        _cartRepo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Cart, bool>>>()))
                 .Returns(new[] { cart }.AsQueryable());

        _cartItemRepo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<CartItem, bool>>>()))
                     .Returns(new[] { existing }.AsQueryable());

        var ok = _sut.AddItemToCart(1, 2, 3);

        Assert.True(ok);
        _cartItemRepo.Verify(r => r.Update(It.Is<CartItem>(ci => ci.Id == 5 && ci.Quantity == 7)), Times.Once);
        _uow.Verify(u => u.SaveChanges(), Times.Once);
    }

    // ========== RemoveItemFromCart ==========
    [Fact]
    public void RemoveItemFromCart_ShouldReturnFalse_WhenCartNotFound()
    {
        _cartRepo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Cart, bool>>>()))
                 .Returns(Enumerable.Empty<Cart>().AsQueryable());

        var ok = _sut.RemoveItemFromCart(1, 123);

        Assert.False(ok);
    }

    [Fact]
    public void RemoveItemFromCart_ShouldReturnFalse_WhenItemNotFound()
    {
        var cart = new Cart { Id = 10, CustomerId = 1 };
        _cartRepo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Cart, bool>>>()))
                 .Returns(new[] { cart }.AsQueryable());

        _cartItemRepo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<CartItem, bool>>>()))
                     .Returns(Enumerable.Empty<CartItem>().AsQueryable());

        var ok = _sut.RemoveItemFromCart(1, 999);

        Assert.False(ok);
    }

    [Fact]
    public void RemoveItemFromCart_ShouldDeleteAndSave_WhenItemExists()
    {
        var cart = new Cart { Id = 10, CustomerId = 1 };
        var item = new CartItem { Id = 7, CartId = 10, ProductId = 2, Quantity = 1 };

        _cartRepo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Cart, bool>>>()))
                 .Returns(new[] { cart }.AsQueryable());

        _cartItemRepo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<CartItem, bool>>>()))
                     .Returns(new[] { item }.AsQueryable());

        var ok = _sut.RemoveItemFromCart(1, 7);

        Assert.True(ok);
        _cartItemRepo.Verify(r => r.Delete(It.Is<CartItem>(ci => ci.Id == 7)), Times.Once);
        _uow.Verify(u => u.SaveChanges(), Times.Once);
    }

    // ========== UpdateItemQuantity ==========
    [Fact]
    public void UpdateItemQuantity_ShouldThrow_WhenQuantityIsZeroOrLess()
    {
        Assert.Throws<ArgumentException>(() => _sut.UpdateItemQuantity(1, 55, 0));
    }

    [Fact]
    public void UpdateItemQuantity_ShouldReturnFalse_WhenCartNotFound()
    {
        _cartRepo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Cart, bool>>>()))
                 .Returns(Enumerable.Empty<Cart>().AsQueryable());

        var ok = _sut.UpdateItemQuantity(1, 5, 2);

        Assert.False(ok);
    }

    [Fact]
    public void UpdateItemQuantity_ShouldReturnFalse_WhenItemNotFound()
    {
        var cart = new Cart { Id = 10, CustomerId = 1 };
        _cartRepo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Cart, bool>>>()))
                 .Returns(new[] { cart }.AsQueryable());

        _cartItemRepo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<CartItem, bool>>>()))
                     .Returns(Enumerable.Empty<CartItem>().AsQueryable());

        var ok = _sut.UpdateItemQuantity(1, 55, 2);

        Assert.False(ok);
    }

    [Fact]
    public void UpdateItemQuantity_ShouldUpdateAndSave_WhenItemExists()
    {
        var cart = new Cart { Id = 10, CustomerId = 1 };
        var item = new CartItem { Id = 5, CartId = 10, ProductId = 3, Quantity = 1 };

        _cartRepo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Cart, bool>>>()))
                 .Returns(new[] { cart }.AsQueryable());

        _cartItemRepo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<CartItem, bool>>>()))
                     .Returns(new[] { item }.AsQueryable());

        var ok = _sut.UpdateItemQuantity(1, 5, 7);

        Assert.True(ok);
        _cartItemRepo.Verify(r => r.Update(It.Is<CartItem>(ci => ci.Id == 5 && ci.Quantity == 7)), Times.Once);
        _uow.Verify(u => u.SaveChanges(), Times.Once);
    }

    // ========== ClearCart ==========
    [Fact]
    public void ClearCart_ShouldReturnFalse_WhenCartNotFound()
    {
        _cartRepo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Cart, bool>>>()))
                 .Returns(Enumerable.Empty<Cart>().AsQueryable());

        var ok = _sut.ClearCart(1);

        Assert.False(ok);
    }

    [Fact]
    public void ClearCart_ShouldDeleteAllItems_AndSave()
    {
        var cart = new Cart { Id = 10, CustomerId = 1 };
        var items = new List<CartItem>
        {
            new CartItem { Id = 1, CartId = 10 },
            new CartItem { Id = 2, CartId = 10 },
        };

        _cartRepo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Cart, bool>>>()))
                 .Returns(new[] { cart }.AsQueryable());

        _cartItemRepo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<CartItem, bool>>>()))
                     .Returns(items.AsQueryable());

        var ok = _sut.ClearCart(1);

        Assert.True(ok);
        _cartItemRepo.Verify(r => r.Delete(It.Is<CartItem>(ci => ci.Id == 1)), Times.Once);
        _cartItemRepo.Verify(r => r.Delete(It.Is<CartItem>(ci => ci.Id == 2)), Times.Once);
        _uow.Verify(u => u.SaveChanges(), Times.Once);
    }

    // ========== Checkout ==========
    [Fact]
    public void Checkout_ShouldThrow_WhenModelIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => _sut.Checkout(1, null));
    }

    [Fact]
    public void Checkout_ShouldReturnFalse_WhenCartNotFound()
    {
        _cartRepo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Cart, bool>>>()))
                 .Returns(Enumerable.Empty<Cart>().AsQueryable());

        var ok = _sut.Checkout(1, new CheckoutModel());

        Assert.False(ok);
    }

    [Fact]
    public void Checkout_ShouldReturnFalse_WhenCartIsEmpty()
    {
        var cart = new Cart { Id = 10, CustomerId = 1 };

        _cartRepo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Cart, bool>>>()))
                 .Returns(new[] { cart }.AsQueryable());

        _cartItemRepo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<CartItem, bool>>>()))
                     .Returns(Enumerable.Empty<CartItem>().AsQueryable());

        var ok = _sut.Checkout(1, new CheckoutModel());

        Assert.False(ok);
    }

    [Fact]
    public void Checkout_ShouldCreateOrder_InsertDetails_ClearCart_AndReturnTrue()
    {
        var cart = new Cart { Id = 10, CustomerId = 1 };
        var items = new List<CartItem>
        {
            new CartItem { Id = 1, CartId = 10, ProductId = 5, Quantity = 2, Product = new Product { Id = 5, Price = 10m ,Name="XILI", Description="გგ", ImageUrl="http://example.com/image.jpg", Stock=10} },
            new CartItem { Id = 2, CartId = 10, ProductId = 6, Quantity = 1, Product = new Product { Id = 6, Price = 20m,Name="XILI", Description="Eგ", ImageUrl="http://example.com/image.jpg", Stock=10 } },
        };

        _cartRepo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Cart, bool>>>()))
                 .Returns(new[] { cart }.AsQueryable());

        _cartItemRepo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<CartItem, bool>>>()))
                     .Returns(items.AsQueryable());

        // Order insert -> მივანიჭოთ Id
        _orderRepo.Setup(r => r.Insert(It.IsAny<Order>()))
                  .Callback<Order>(o => o.Id = 777);

        // ClearCart შიგნით იძახებს FindByCondition + Delete + SaveChanges-ს, მოვამზადოთ
        _cartItemRepo.Setup(r => r.FindByCondition(It.Is<Expression<Func<CartItem, bool>>>(expr =>
                items.Where(ci => ci.CartId == 10).AsQueryable().Any())))
                     .Returns(items.AsQueryable());

        var model = new CheckoutModel
        {
            ShippingAddress = "Ship",
            BillingAddress = "Bill",
            PaymentMethod = "Card"
        };

        var ok = _sut.Checkout(1, model);

        Assert.True(ok);

        // ჯამი უნდა იყოს: 2*10 + 1*20 = 40
        _orderRepo.Verify(r => r.Insert(It.Is<Order>(o =>
            o.UserId == 1 && o.TotalAmount == 40m && o.ShippingAddress == "Ship" && o.BillingAddress == "Bill" && o.PaymentMethod == "Card")), Times.Once);

        // OrderDetail-ები
        _orderDetailRepo.Verify(r => r.Insert(It.Is<OrderDetail>(d =>
            d.OrderId == 777 && d.ProductId == 5 && d.Quantity == 2 && d.UnitPrice == 10m)), Times.Once);

        _orderDetailRepo.Verify(r => r.Insert(It.Is<OrderDetail>(d =>
            d.OrderId == 777 && d.ProductId == 6 && d.Quantity == 1 && d.UnitPrice == 20m)), Times.Once);

        // Checkout-ში გამოიძახება ClearCart, იქაც მოხდება SaveChanges
        _uow.Verify(u => u.SaveChanges(), Times.AtLeast(2));
        _cartItemRepo.Verify(r => r.Delete(It.IsAny<CartItem>()), Times.Exactly(items.Count));
    }

    // ========== GetCartTotal ==========
    [Fact]
    public void GetCartTotal_ShouldReturnZero_WhenCartNotFound()
    {
        _cartRepo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Cart, bool>>>()))
                 .Returns(Enumerable.Empty<Cart>().AsQueryable());

        var total = _sut.GetCartTotal(1);

        Assert.Equal(0m, total);
    }

    [Fact]
    public void GetCartTotal_ShouldSumItemPrices()
    {
        var cart = new Cart { Id = 10, CustomerId = 1 };
        var items = new List<CartItem>
        {
            new CartItem { Id = 1, CartId = 10, ProductId = 5, Quantity = 2, Product = new Product { Id = 5, Price = 10m,Name="FOODE", Description="გგ", ImageUrl="http://example.com/image.jpg", Stock=10 } },
            new CartItem { Id = 2, CartId = 10, ProductId = 6, Quantity = 1, Product = new Product { Id = 6, Price = 20m,Name="ICE CREAM", Description="CGOCOLATE", ImageUrl="http://example.com/image.jpg", Stock=10 } },
        };

        _cartRepo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Cart, bool>>>()))
                 .Returns(new[] { cart }.AsQueryable());

        _cartItemRepo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<CartItem, bool>>>()))
                     .Returns(items.AsQueryable());

        var total = _sut.GetCartTotal(1);

        Assert.Equal(40m, total);
    }
}
