using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Moq;
using Xunit;
using Microsoft.Extensions.Logging;
// using Interface.Command; // თუ გჭირდება
// using Interface.IRepositories;
using Dto;
using Interface.IRepositories;
using Service.CommandService;

public class DiscountServiceTests
{
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly Mock<IDiscountRepository> _discountRepo = new();
    private readonly Mock<ILogger<DiscountService>> _logger = new();

    private readonly DiscountService _sut;

    public DiscountServiceTests()
    {
        _uow.SetupGet(x => x.DiscountRepository).Returns(_discountRepo.Object);
        _sut = new DiscountService(_uow.Object, _logger.Object);
    }

    // -------- GetByCode --------

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void GetByCode_ShouldThrow_WhenCodeIsEmpty(string code)
    {
        Assert.Throws<ArgumentException>(() => _sut.GetByCode(code));
    }

    [Fact]
    public void GetByCode_ShouldReturnDiscount_WhenExists()
    {
        var d = new Discount { Id = 1, Code = "SAVE10", DiscountPercentage = 10, ExpirationDate = DateTime.UtcNow.AddDays(1) };

        _discountRepo
            .Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Discount, bool>>>()))
            .Returns(new[] { d }.AsQueryable());

        var res = _sut.GetByCode("save10");

        Assert.NotNull(res);
        Assert.Equal(1, res.Id);
    }

    [Fact]
    public void GetByCode_ShouldReturnNull_WhenNotFound()
    {
        _discountRepo
            .Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Discount, bool>>>()))
            .Returns(Enumerable.Empty<Discount>().AsQueryable());

        var res = _sut.GetByCode("NOPE");

        Assert.Null(res);
    }

    // -------- IsValid --------

    [Fact]
    public void IsValid_ShouldReturnTrue_WhenNotExpired()
    {
        var d = new Discount { Code = "OK", DiscountPercentage = 15, ExpirationDate = DateTime.UtcNow.AddMinutes(5) };

        _discountRepo
            .Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Discount, bool>>>()))
            .Returns(new[] { d }.AsQueryable());

        var ok = _sut.IsValid("OK");

        Assert.True(ok);
    }

    [Fact]
    public void IsValid_ShouldReturnFalse_WhenExpired()
    {
        var d = new Discount { Code = "OLD", DiscountPercentage = 15, ExpirationDate = DateTime.UtcNow.AddMinutes(-1) };

        _discountRepo
            .Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Discount, bool>>>()))
            .Returns(new[] { d }.AsQueryable());

        var ok = _sut.IsValid("OLD");

        Assert.False(ok);
    }

    [Fact]
    public void IsValid_ShouldReturnFalse_WhenNotFound()
    {
        _discountRepo
            .Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Discount, bool>>>()))
            .Returns(Enumerable.Empty<Discount>().AsQueryable());

        var ok = _sut.IsValid("X");

        Assert.False(ok);
    }

    // -------- ApplyDiscount --------

    [Fact]
    public void ApplyDiscount_ShouldApplyPercentage_WhenValid()
    {
        // 20% of 100 = 20, final = 80
        var d = new Discount { Code = "SAVE20", DiscountPercentage = 20, ExpirationDate = DateTime.UtcNow.AddDays(1) };

        _discountRepo
            .Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Discount, bool>>>()))
            .Returns(new[] { d }.AsQueryable());

        var final = _sut.ApplyDiscount("SAVE20", 100m);

        Assert.Equal(80m, final);
    }

    [Fact]
    public void ApplyDiscount_ShouldReturnOriginal_WhenExpiredOrMissing()
    {
        var expired = new Discount { Code = "OLD", DiscountPercentage = 50, ExpirationDate = DateTime.UtcNow.AddDays(-1) };

        // სცენარი 1: ვადაგასული
        _discountRepo
            .SetupSequence(r => r.FindByCondition(It.IsAny<Expression<Func<Discount, bool>>>()))
            .Returns(new[] { expired }.AsQueryable())             // call #1
            .Returns(Enumerable.Empty<Discount>().AsQueryable()); // call #2

        var total1 = _sut.ApplyDiscount("OLD", 200m);
        Assert.Equal(200m, total1);

        // სცენარი 2: ვერ მოიძებნა
        var total2 = _sut.ApplyDiscount("NONE", 200m);
        Assert.Equal(200m, total2);
    }
}
