using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using Moq;
using Xunit;

using Service.CommandService;
using Interface.IRepositories;
using Interface.Model;
using Dto;
using Interface;
using Interface.Command; // IDiscountService, IAuditService

public class OrderCommandServiceTests
{
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly Mock<IDiscountService> _discount = new();
    private readonly Mock<IAuditService> _audit = new();

    private readonly Mock<IOrderRepository> _orderRepo = new();
    private readonly Mock<IOrderDetailRepository> _detailRepo = new();
    private readonly Mock<IProductRepository> _productRepo = new();
    private readonly Mock<ICartRepository> _cartRepo = new();
    private readonly Mock<ICartItemRepository> _cartItemRepo = new();

    private readonly OrderCommandService _sut;

    public OrderCommandServiceTests()
    {
        _uow.SetupGet(x => x.OrderRepository).Returns(_orderRepo.Object);
        _uow.SetupGet(x => x.OrderDetailRepository).Returns(_detailRepo.Object);
        _uow.SetupGet(x => x.ProductRepository).Returns(_productRepo.Object);
        _uow.SetupGet(x => x.CartRepository).Returns(_cartRepo.Object);
        _uow.SetupGet(x => x.CartItemRepository).Returns(_cartItemRepo.Object);

        // ტრანზაქციები
        _uow.Setup(x => x.BeginTransaction());
        _uow.Setup(x => x.Commit());
        _uow.Setup(x => x.Rollback());

        // SaveChanges თუ void-ია — Returns არ სჭირდება
        _uow.Setup(x => x.SaveChanges());

        _sut = new OrderCommandService(_uow.Object, _mapper.Object, _discount.Object, _audit.Object);
    }

    // ============== Insert ==============
    [Fact]
    public void Insert_Should_Map_Insert_Save_Log_And_Return_Id()
    {
        var model = new OrderModel { CustomerId = 1, TotalAmount = 0 };
        var mapped = new Order { Id = 0, CustomerId = 1 };

        _mapper.Setup(m => m.Map<Order>(model)).Returns(mapped);
        _orderRepo.Setup(r => r.Insert(It.IsAny<Order>()))
                  .Callback<Order>(o => o.Id = 123);

        var id = _sut.Insert(model);

        Assert.Equal(123, id);
        _orderRepo.Verify(r => r.Insert(It.Is<Order>(o => o.CustomerId == 1)), Times.Once);
        _uow.Verify(u => u.SaveChanges(), Times.Once);
        _audit.Verify(a => a.Log(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.AtLeastOnce);
    }

    // ============== Update ==============
    [Fact]
    public void Update_Should_ReturnFalse_When_NotFound()
    {
        _orderRepo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Order, bool>>>()))
                  .Returns(Enumerable.Empty<Order>().AsQueryable());

        var ok = _sut.Update(99, new OrderModel());

        Assert.False(ok);
    }

    [Fact]
    public void Update_Should_Update_Save_Log_When_Found()
    {
        var existing = new Order { Id = 10, CustomerId = 1, BillingAddress = "wrtertg", ShippingAddress = "megako" };
        var model = new OrderModel { CustomerId = 1, TotalAmount = 50 };
        var mapped = new Order { CustomerId = 1, TotalAmount = 50 };

        _orderRepo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Order, bool>>>()))
                  .Returns(new[] { existing }.AsQueryable());
        _mapper.Setup(m => m.Map<Order>(model)).Returns(mapped);

        var ok = _sut.Update(10, model);

        Assert.True(ok);
        _orderRepo.Verify(r => r.Update(It.Is<Order>(o => o.Id == 10 && o.TotalAmount == 50)), Times.Once);
        _uow.Verify(u => u.SaveChanges(), Times.Once);
    }

    // ============== Delete ==============
    [Fact]
    public void Delete_Should_ReturnFalse_When_NotFound()
    {
        _orderRepo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Order, bool>>>()))
                  .Returns(Enumerable.Empty<Order>().AsQueryable());

        var ok = _sut.Delete(5);

        Assert.False(ok);
    }

    [Fact]
    public void Delete_Should_Delete_Save_Log_When_Found()
    {
        var existing = new Order { Id = 5 };

        _orderRepo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Order, bool>>>()))
                  .Returns(new[] { existing }.AsQueryable());

        var ok = _sut.Delete(5);

        Assert.True(ok);
        _orderRepo.Verify(r => r.Delete(existing), Times.Once);
        _uow.Verify(u => u.SaveChanges(), Times.Once);
    }

    // ============== UpdateOrderStatus ==============
    [Fact]
    public void UpdateOrderStatus_Should_ReturnFalse_When_NotFound()
    {
        _orderRepo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Order, bool>>>()))
                  .Returns(Enumerable.Empty<Order>().AsQueryable());

        var ok = _sut.UpdateOrderStatus(1, OrderStatus.Shipped);

        Assert.False(ok);
    }

    [Fact]
    public void UpdateOrderStatus_Should_Update_Status_Save_Log()
    {
        var existing = new Order { Id = 2, Status = OrderStatus.Pending, BillingAddress = "niktyh", ShippingAddress = "nekora" };

        _orderRepo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Order, bool>>>()))
                  .Returns(new[] { existing }.AsQueryable());

        var ok = _sut.UpdateOrderStatus(2, OrderStatus.Processing);

        Assert.True(ok);
        _orderRepo.Verify(r => r.Update(It.Is<Order>(o => o.Id == 2 && o.Status == OrderStatus.Processing)), Times.Once);
        _uow.Verify(u => u.SaveChanges(), Times.Once);
    }

    // ============== CancelOrder ==============
    [Fact]
    public void CancelOrder_Should_ReturnFalse_When_NotFound()
    {
        _orderRepo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Order, bool>>>()))
                  .Returns(Enumerable.Empty<Order>().AsQueryable());

        var ok = _sut.CancelOrder(10, customerId: 1);

        Assert.False(ok);
    }

    [Fact]
    public void CancelOrder_Should_ReturnFalse_When_AlreadyCancelled()
    {
        var order = new Order { Id = 10, CustomerId = 1, Status = OrderStatus.Cancelled, BillingAddress = "wrtertg", ShippingAddress = "megako" };
        _orderRepo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Order, bool>>>()))
                  .Returns(new[] { order }.AsQueryable());

        var ok = _sut.CancelOrder(10, 1);

        Assert.False(ok);
    }

    [Fact]
    public void CancelOrder_Should_Restock_UpdateStatus_Save_Commit_And_Log()
    {
        var order = new Order { Id = 10, CustomerId = 1, Status = OrderStatus.Processing, BillingAddress = "wrtertg", ShippingAddress = "megako" };
        var details = new List<OrderDetail>
        {
            new OrderDetail { Id = 1, OrderId = 10, ProductId = 100, Quantity = 2 },
            new OrderDetail { Id = 2, OrderId = 10, ProductId = 200, Quantity = 1 },
        };
        var p100 = new Product { Id = 100, Name = "P100", Stock = 0, Price = 10, Description = "d", ImageUrl = "img" };
        var p200 = new Product { Id = 200, Name = "P200", Stock = 3, Price = 20, Description = "d", ImageUrl = "img" };

        _orderRepo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Order, bool>>>()))
                  .Returns(new[] { order }.AsQueryable());
        _uow.Setup(u => u.OrderDetailRepository.FindByCondition(It.IsAny<Expression<Func<OrderDetail, bool>>>()))
            .Returns(details.AsQueryable());

        _productRepo.Setup(r => r.GetById(100)).Returns(p100);
        _productRepo.Setup(r => r.GetById(200)).Returns(p200);

        var ok = _sut.CancelOrder(10, 1);

        Assert.True(ok);
        Assert.Equal(2, p100.Stock); // 0 + 2
        Assert.Equal(4, p200.Stock); // 3 + 1
        _orderRepo.Verify(r => r.Update(It.Is<Order>(o => o.Id == 10 && o.Status == OrderStatus.Cancelled)), Times.Once);
        _uow.Verify(u => u.SaveChanges(), Times.Once);
        _uow.Verify(u => u.Commit(), Times.Once);
    }

    [Fact]
    public void CancelOrder_Should_Rollback_And_Throw_On_Failure()
    {
        var order = new Order { Id = 10, CustomerId = 1, Status = OrderStatus.Processing, BillingAddress="wrtertg" , ShippingAddress="megako"};
        var details = new List<OrderDetail> { new OrderDetail { Id = 1, OrderId = 10, ProductId = 100, Quantity = 1 } };
        var p100 = new Product { Id = 100, Name = "P100", Stock = 0, Price = 10, Description = "d", ImageUrl = "img" };

        _orderRepo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Order, bool>>>()))
                  .Returns(new[] { order }.AsQueryable());
        _uow.Setup(u => u.OrderDetailRepository.FindByCondition(It.IsAny<Expression<Func<OrderDetail, bool>>>()))
            .Returns(details.AsQueryable());

        _productRepo.Setup(r => r.GetById(100)).Returns(p100);
        _productRepo.Setup(r => r.Update(It.IsAny<Product>())).Throws(new Exception("DB error"));

        Assert.Throws<Exception>(() => _sut.CancelOrder(10, 1));
        _uow.Verify(u => u.Rollback(), Times.Once);
    }

    // ============== Checkout ==============
    [Fact]
    public void Checkout_Should_Throw_When_Cart_NotFound()
    {
        _cartRepo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Cart, bool>>>()))
                 .Returns(Enumerable.Empty<Cart>().AsQueryable());

        Assert.Throws<InvalidOperationException>(() => _sut.Checkout(new CheckoutModel { Customerid = 1 }));
    }

    [Fact]
    public void Checkout_Should_Throw_When_Cart_Empty()
    {
        var cart = new Cart { Id = 1, CustomerId = 1, Items = new List<CartItem>() };

        _cartRepo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Cart, bool>>>()))
                 .Returns(new[] { cart }.AsQueryable());

        Assert.Throws<InvalidOperationException>(() => _sut.Checkout(new CheckoutModel { Customerid = 1 }));
    }

    [Fact]
    public void Checkout_Should_Throw_When_Insufficient_Stock()
    {
        var cart = new Cart
        {
            Id = 1,
            CustomerId = 1,
            Items = new List<CartItem>
            {
                new CartItem { ProductId = 100, Quantity = 5, Product = new Product { Id=100, Name="P", Stock=3, Price=10, Description="d", ImageUrl="img" } }
            }
        };

        _cartRepo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Cart, bool>>>()))
                 .Returns(new[] { cart }.AsQueryable());

        var ex = Assert.Throws<InvalidOperationException>(() => _sut.Checkout(new CheckoutModel { Customerid = 1 }));
        Assert.Contains("Insufficient stock", ex.Message);
    }

    [Fact]
    public void Checkout_Should_CreateOrder_Details_UpdateStock_ClearCart_ApplyDiscount_And_Log()
    {
        var p1 = new Product { Id = 100, Name = "A", Stock = 5, Price = 10, Description = "d", ImageUrl = "img" };
        var p2 = new Product { Id = 200, Name = "B", Stock = 3, Price = 20, Description = "d", ImageUrl = "img" };

        var cart = new Cart
        {
            Id = 1,
            CustomerId = 1,
            Items = new List<CartItem>
            {
                new CartItem { ProductId = 100, Quantity = 2, Product = p1 }, // 20
                new CartItem { ProductId = 200, Quantity = 1, Product = p2 }  // 20
            }
        };

        _cartRepo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Cart, bool>>>()))
                 .Returns(new[] { cart }.AsQueryable());

        // Discount: 10% ↓ => 40 -> 36
        _discount.Setup(d => d.IsValid("SAVE10")).Returns(true);
        _discount.Setup(d => d.ApplyDiscount("SAVE10", 40m)).Returns(36m);

        // Order insert → მიანიჭოს Id სანამ დეტალებს ვამატებთ
        _orderRepo.Setup(r => r.Insert(It.IsAny<Order>()))
                  .Callback<Order>(o => o.Id = 777);

        // Verify delete range
        _cartItemRepo.Setup(r => r.DeleteRange(It.IsAny<IEnumerable<CartItem>>()));

        var id = _sut.Checkout(new CheckoutModel
        {
            Customerid = 1,
            ShippingAddress = "Ship",
            BillingAddress = "Bill",
            PaymentMethod = "Card",
            DiscountCode = "SAVE10"
        });

        Assert.Equal(777, id);
        // დეტალები ჩაიწერა
        _detailRepo.Verify(r => r.Insert(It.Is<OrderDetail>(d =>
            d.OrderId == 777 && d.ProductId == 100 && d.Quantity == 2 && d.UnitPrice == 10m)), Times.Once);
        _detailRepo.Verify(r => r.Insert(It.Is<OrderDetail>(d =>
            d.OrderId == 777 && d.ProductId == 200 && d.Quantity == 1 && d.UnitPrice == 20m)), Times.Once);

        // მარაგი დაკლდა
        Assert.Equal(3, p1.Stock);
        Assert.Equal(2, p2.Stock);

        // კალათის items წაიშალა
        _cartItemRepo.Verify(r => r.DeleteRange(It.Is<IEnumerable<CartItem>>(list => list.Count() == 2)), Times.Once);

        _uow.Verify(u => u.SaveChanges(), Times.Once);
        _audit.Verify(a => a.Log(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.AtLeastOnce);
    }

    // ============== TrackOrderStatus ==============
    [Fact]
    public void TrackOrderStatus_Should_Return_Status_When_Found()
    {
        var order = new Order { Id = 5, CustomerId = 1, Status = OrderStatus.Shipped };
        _orderRepo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Order, bool>>>()))
                  .Returns(new[] { order }.AsQueryable());

        var st = _sut.TrackOrderStatus(5, 1);

        Assert.Equal(OrderStatus.Shipped, st);
    }

    [Fact]
    public void TrackOrderStatus_Should_Return_Null_When_NotFound()
    {
        _orderRepo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Order, bool>>>()))
                  .Returns(Enumerable.Empty<Order>().AsQueryable());

        var st = _sut.TrackOrderStatus(99, 1);

        Assert.Null(st);
    }
}
