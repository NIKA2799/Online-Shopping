using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using Moq;
using Xunit;

// მოარგე namespaces შენს 솔უშენზე
using Service.QueriesService;
using Interface.IRepositories;
using Webdemo.Models;
using Dto; // Review, ReviewModel

public class ReviewQueryServiceTests
{
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly Mock<IReviewRepository> _reviewRepo = new();
    private readonly ReviewQueryService _sut;

    public ReviewQueryServiceTests()
    {
        _uow.SetupGet(x => x.ReviewRepository).Returns(_reviewRepo.Object);

        // Generic mapping stubs
        _mapper
            .Setup(m => m.Map<List<ReviewModel>>(It.IsAny<IEnumerable<Review>>()))
            .Returns((IEnumerable<Review> src) => src.Select(ToModel).ToList());

#pragma warning disable CS8603 // Possible null reference return.
        _mapper
            .Setup(m => m.Map<ReviewModel>(It.IsAny<Review>()))
            .Returns((Review r) => r == null ? null : ToModel(r));
#pragma warning restore CS8603 // Possible null reference return.

        _sut = new ReviewQueryService(_uow.Object, _mapper.Object);
    }

    // ---------- GetReviewsByProduct ----------
    [Fact]
    public void GetReviewsByProduct_Should_ReturnMappedList_When_Exists()
    {
        var data = new List<Review>
        {
            R(1, productId: 7, customerId: 10, rating: 5, comment: "Great"),
            R(2, productId: 7, customerId: 11, rating: 4, comment: "Good")
        };

        _reviewRepo
            .Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Review, bool>>>()))
            .Returns((Expression<Func<Review, bool>> pred) => data.AsQueryable().Where(pred));

        var res = _sut.GetReviewsByProduct(7).ToList();

        Assert.Equal(2, res.Count);
        Assert.Contains(res, x => x.Comment == "Great");
        Assert.Contains(res, x => x.Comment == "Good");
    }

    [Fact]
    public void GetReviewsByProduct_Should_ReturnEmpty_When_None()
    {
        _reviewRepo
            .Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Review, bool>>>()))
            .Returns(Enumerable.Empty<Review>().AsQueryable());

        var res = _sut.GetReviewsByProduct(99);

        Assert.Empty(res);
    }

    // ---------- GetReviewByUser ----------
    [Fact]
    public void GetReviewByUser_Should_ReturnMapped_When_Found()
    {
        var item = R(5, productId: 7, customerId: 42, rating: 3, comment: "Ok");

        _reviewRepo
            .Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Review, bool>>>()))
            .Returns((Expression<Func<Review, bool>> pred) => new[] { item }.AsQueryable().Where(pred));

        var res = _sut.GetReviewByUser(7, 42);

        Assert.NotNull(res);
        Assert.Equal(5, res.Id);
        Assert.Equal(3, res.Rating);
    }

    [Fact]
    public void GetReviewByUser_Should_ReturnNull_When_NotFound()
    {
        _reviewRepo
            .Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Review, bool>>>()))
            .Returns(Enumerable.Empty<Review>().AsQueryable());

        var res = _sut.GetReviewByUser(7, 42);

        Assert.Null(res);
    }

    // ---------- FindByCondition ----------
    [Fact]
    public void FindByCondition_Should_Filter_And_Map_To_List()
    {
        var data = new List<Review>
        {
            R(1, productId: 7, customerId: 10, rating: 5, comment: "Great"),
            R(2, productId: 7, customerId: 11, rating: 2, comment: "Bad"),
            R(3, productId: 8, customerId: 12, rating: 4, comment: "Good")
        };

        _reviewRepo
            .Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Review, bool>>>()))
            .Returns((Expression<Func<Review, bool>> pred) => data.AsQueryable().Where(pred));

        var res = _sut.FindByCondition(r => r.ProductId == 7 && r.Rating >= 4).ToList();

        Assert.Single(res);
        Assert.Equal("Great", res[0].Comment);
    }

    // ------------ helpers ------------
    private static Review R(int id, int productId, int customerId, int rating, string comment) =>
        new Review
        {
            Id = id,
            ProductId = productId,
            CustomerId = customerId,
            Rating = rating,
            Comment = comment,
            DatePosted = DateTime.UtcNow.AddDays(-id)
        };

    private static ReviewModel ToModel(Review r) => new ReviewModel
    {
        Id = r.Id,
        ProductId = r.ProductId,
        CustomerId = r.CustomerId,
        Rating = r.Rating,
        Comment = r.Comment,
        DatePosted = r.DatePosted
    };
}
