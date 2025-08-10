using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using Moq;
using Xunit;
using Microsoft.Extensions.Logging;

// მოარგე namespaces შენს პროქტს
using Service.CommandService;
using Dto;
using Webdemo.Models;
using Interface.IRepositories;

public class ProductCommandServiceTests
{
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly Mock<IProductRepository> _productRepo = new();
    private readonly Mock<IReviewRepository> _reviewRepo = new();
    private readonly ProductCommandService _sut;

    public ProductCommandServiceTests()
    {
        _uow.SetupGet(x => x.ProductRepository).Returns(_productRepo.Object);
        _uow.SetupGet(x => x.ReviewRepository).Returns(_reviewRepo.Object);

        // თუ SaveChanges() არის void:
        _uow.Setup(x => x.SaveChanges());

        _sut = new ProductCommandService(_uow.Object, _mapper.Object);
    }

    // ---------------- Insert ----------------
    [Fact]
    public void Insert_Should_Map_Insert_Save_And_Return_Id_When_NoImage()
    {
        // REQUIRED ველებით ProductModel
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        var model = new ProductModel
        {
            Name = "Phone",
            Description = "Nice",
            Price = 999m,
            Stock = 5,
            ImageUrl = "img/phone.jpg", // REQUIRED
            // საჭიროების შემთხვევაში აქვე ჩასვი CategoryId/UserId და სხვ.
            ImageFile = null
        };
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

        // mapper -> REQUIRED ველებით Product entity
        var mapped = new Product
        {
            Id = 0, // insert-ის წინ
            Name = "Phone",
            Description = "Nice",
            Price = 999m,
            Stock = 5,
            ImageUrl = "img/phone.jpg" // REQUIRED
        };

        _mapper.Setup(m => m.Map<Product>(model)).Returns(mapped);

        _productRepo.Setup(r => r.Insert(It.IsAny<Product>()))
                    .Callback<Product>(p => p.Id = 123);

        var id = _sut.Insert(model);

        Assert.Equal(123, id);
        _productRepo.Verify(r => r.Insert(It.Is<Product>(p => p.Name == "Phone")), Times.Once);
        _uow.Verify(x => x.SaveChanges(), Times.Once);
    }

    // ---------------- Update ----------------
    [Fact]
    public void Update_Should_Update_And_Save_When_Found()
    {
        // არსებული entity — REQUIRED ველებით
        var db = new Product
        {
            Id = 10,
            Name = "Old",
            Description = "Desc",
            Price = 100m,
            Stock = 2,
            ImageUrl = "img/old.jpg" // REQUIRED
        };

        _productRepo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Product, bool>>>()))
                    .Returns(new[] { db }.AsQueryable());

        // incoming model — REQUIRED ველებით
        var incoming = new ProductModel
        {
            Name = "New",
            Description = "D2",
            Price = 200m,
            Stock = 3,
            ImageUrl = "img/new.jpg" // REQUIRED
        };

        // mapper-სგან დაბრუნებული entity — REQUIRED ველებით
        var mapped = new Product
        {
            Id = 0, // სერვისი გადააწერს Id-ს db.Id-ზე
            Name = "New",
            Description = "D2",
            Price = 200m,
            Stock = 3,
            ImageUrl = "img/new.jpg" // REQUIRED
        };

        _mapper.Setup(m => m.Map<Product>(incoming)).Returns(mapped);

        _sut.Update(10, incoming);

        _productRepo.Verify(r => r.Update(It.Is<Product>(p =>
            p.Id == 10 && p.Name == "New" && p.Price == 200m && p.ImageUrl == "img/new.jpg")), Times.Once);
        _uow.Verify(x => x.SaveChanges(), Times.Once);
    }

    [Fact]
    public void Update_Should_DoNothing_When_NotFound()
    {
        _productRepo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Product, bool>>>()))
                    .Returns(Enumerable.Empty<Product>().AsQueryable());

        _sut.Update(999, new ProductModel
        {
            Name = "X",
            Description = "Y",
            Price = 1m,
            Stock = 1,
            ImageUrl = "img/x.jpg" // REQUIRED
        });

        _productRepo.Verify(r => r.Update(It.IsAny<Product>()), Times.Never);
        _uow.Verify(x => x.SaveChanges(), Times.Never);
    }

    // ---------------- Delete ----------------
    [Fact]
    public void Delete_Should_Delete_And_Save_When_Found()
    {
        var db = new Product
        {
            Id = 7,
            Name = "P",
            Description = "D",
            Price = 10m,
            Stock = 1,
            ImageUrl = "img/p.jpg" // REQUIRED
        };

        _productRepo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Product, bool>>>()))
                    .Returns(new[] { db }.AsQueryable());

        _sut.Delete(7);

        _productRepo.Verify(r => r.Delete(It.Is<Product>(p => p.Id == 7)), Times.Once);
        _uow.Verify(x => x.SaveChanges(), Times.Once);
    }

    [Fact]
    public void Delete_Should_DoNothing_When_NotFound()
    {
        _productRepo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Product, bool>>>()))
                    .Returns(Enumerable.Empty<Product>().AsQueryable());

        _sut.Delete(7);

        _productRepo.Verify(r => r.Delete(It.IsAny<Product>()), Times.Never);
        _uow.Verify(x => x.SaveChanges(), Times.Never);
    }

    // ---------------- UpdateStock ----------------
    [Fact]
    public void UpdateStock_Should_Update_And_Save_When_Found()
    {
        var db = new Product
        {
            Id = 3,
            Name = "P",
            Description = "D",
            Price = 10m,
            Stock = 1,
            ImageUrl = "img/p.jpg" // REQUIRED
        };

        _productRepo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Product, bool>>>()))
                    .Returns(new[] { db }.AsQueryable());

        _sut.UpdateStock(3, 50);

        _productRepo.Verify(r => r.Update(It.Is<Product>(p => p.Id == 3 && p.Stock == 50)), Times.Once);
        _uow.Verify(x => x.SaveChanges(), Times.Once);
    }

    [Fact]
    public void UpdateStock_Should_DoNothing_When_NotFound()
    {
        _productRepo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Product, bool>>>()))
                    .Returns(Enumerable.Empty<Product>().AsQueryable());

        _sut.UpdateStock(3, 50);

        _productRepo.Verify(r => r.Update(It.IsAny<Product>()), Times.Never);
        _uow.Verify(x => x.SaveChanges(), Times.Never);
    }

    // ---------------- AddReview ----------------
    [Fact]
    public void AddReview_Should_Insert_And_Save_When_ProductExists()
    {
        var db = new Product
        {
            Id = 11,
            Name = "P",
            Description = "D",
            Price = 10m,
            Stock = 1,
            ImageUrl = "img/p.jpg" // REQUIRED
        };

        _productRepo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Product, bool>>>()))
                    .Returns(new[] { db }.AsQueryable());

        var reviewModel = new ReviewModel { Comment = "Great", Rating = 5 };

        var mappedReview = new Review
        {
            Comment = "Great",
            Rating = 5
        };

        _mapper.Setup(m => m.Map<Review>(reviewModel)).Returns(mappedReview);

        _sut.AddReview(11, reviewModel);

        _reviewRepo.Verify(r => r.Insert(It.Is<Review>(rv =>
            rv.ProductId == 11 && rv.Comment == "Great" && rv.Rating == 5)), Times.Once);
        _uow.Verify(x => x.SaveChanges(), Times.Once);
    }

    [Fact]
    public void AddReview_Should_DoNothing_When_ProductNotFound()
    {
        _productRepo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Product, bool>>>()))
                    .Returns(Enumerable.Empty<Product>().AsQueryable());

        _sut.AddReview(11, new ReviewModel { Comment = "X", Rating = 1 });

        _reviewRepo.Verify(r => r.Insert(It.IsAny<Review>()), Times.Never);
        _uow.Verify(x => x.SaveChanges(), Times.Never);
    }

    // ---------------- ToggleAvailability / ToggleFeatured ----------------
    [Fact]
    public void ToggleAvailability_Should_Toggle_And_Save()
    {
        var db = new Product
        {
            Id = 9,
            Name = "P",
            Description = "D",
            Price = 10m,
            Stock = 1,
            ImageUrl = "img/p.jpg", // REQUIRED
            IsOutOfStock = false
        };

        _productRepo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Product, bool>>>()))
                    .Returns(new[] { db }.AsQueryable());

        _sut.ToggleAvailability(9);

        _productRepo.Verify(r => r.Update(It.Is<Product>(p => p.Id == 9 && p.IsOutOfStock == true)), Times.Once);
        _uow.Verify(x => x.SaveChanges(), Times.Once);
    }

    [Fact]
    public void ToggleFeatured_Should_Toggle_And_Save()
    {
        var db = new Product
        {
            Id = 9,
            Name = "P",
            Description = "D",
            Price = 10m,
            Stock = 1,
            ImageUrl = "img/p.jpg", // REQUIRED
            IsFeatured = false
        };

        _productRepo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Product, bool>>>()))
                    .Returns(new[] { db }.AsQueryable());

        _sut.ToggleFeatured(9);

        _productRepo.Verify(r => r.Update(It.Is<Product>(p => p.Id == 9 && p.IsFeatured == true)), Times.Once);
        _uow.Verify(x => x.SaveChanges(), Times.Once);
    }

    // ---------------- GetLowStockProducts ----------------
    [Fact]
    public void GetLowStockProducts_Should_Map_And_Return_List()
    {
        var list = new List<Product>
        {
            new Product { Id = 1, Name = "A", Description = "D", Price = 5m, Stock = 1, ImageUrl = "img/a.jpg" }, // REQUIRED
            new Product { Id = 2, Name = "B", Description = "D", Price = 6m, Stock = 2, ImageUrl = "img/b.jpg" }  // REQUIRED
        };

        _productRepo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Product, bool>>>()))
                    .Returns(list.AsQueryable());

        _mapper.Setup(m => m.Map<List<ProductModel>>(list))
               .Returns(new List<ProductModel>
               {
                   new ProductModel { Id = 1, Name = "A", Description = "D", Price = 5m, Stock = 1, ImageUrl = "img/a.jpg" },
                   new ProductModel { Id = 2, Name = "B", Description = "D", Price = 6m, Stock = 2, ImageUrl = "img/b.jpg" }
               });

        var res = _sut.GetLowStockProducts(5).ToList();

        Assert.Equal(2, res.Count);
        Assert.Contains(res, p => p.Name == "A");
        Assert.Contains(res, p => p.Name == "B");
    }
}
